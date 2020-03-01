using ActiveDirectoryDemo.Infrastructure;
using ActiveDirectoryDemo.Infrastructure.ActiveDirectory;
using MediatR;
using Microsoft.Extensions.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace ActiveDirectoryDemo.Features.Users
{
    public class Create
    {
        public class Command : IRequest
        {
            public string AccountName { get; set; }
            public string DisplayName { get; set; }
            public string EmailAddress { get; set; }
        }

        public class CommandHandler : IRequestHandler<Command, Unit>
        {
            private IActiveDirectoryProxy _adProxy;
            private readonly INotifier _notifier;
            private IConfiguration _configuration;

            public CommandHandler(IActiveDirectoryProxy adProxy, INotifier notifier, IConfiguration configuration)
            {
                _adProxy = adProxy;
                _notifier = notifier;
                _configuration = configuration;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var ldapPath = _configuration.GetValue<string>("Users:LdapPath");
                var passwordLength = _configuration.GetValue<int>("Passwords:Length");
                var numberOfNonAlphanumericCharacters = _configuration.GetValue<int>("Passwords:NumberOfNonAlphanumericCharacters");
                var password = Password.Generate(passwordLength, numberOfNonAlphanumericCharacters);

                _adProxy.CreateUser(
                    request.AccountName,
                    request.EmailAddress,
                    password,
                    request.DisplayName,
                    ldapPath);

                await NotifyAdministratorsAsync();

                // This method is really a void, but mediator returns a Unit by default for voids.
                return Unit.Value;
            }

            private async Task NotifyAdministratorsAsync()
            {
                var emailSubject = "New user created in Active Directory";
                var emailBody = string.Empty; // Generate this from something like https://github.com/toddams/RazorLight
                var fromAddress = _configuration.GetValue<string>("Mail:FromAddress");
                var toAddress = _configuration.GetValue<string>("Mail:AdminAddress");

                await _notifier.SendAsync(emailSubject, emailBody, fromAddress, toAddress);
            }
        }
    }
}
