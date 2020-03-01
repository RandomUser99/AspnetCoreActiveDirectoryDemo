using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ActiveDirectoryDemo.Features.Users
{
    public class UsersController : Controller
    {
        private IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Create.Command command)
        {
            await _mediator.Send(command);

            return RedirectToAction(nameof(Create));
            
        }
    }
}
