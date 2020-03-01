using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.DirectoryServices.AccountManagement;

namespace ActiveDirectoryDemo.Infrastructure.ActiveDirectory
{
    public interface IActiveDirectoryProxy
    {
        UserPrincipalEx FindUserByCn(string userName);
        UserPrincipalEx FindUserByCn(string userName, string ldapPath);
        UserPrincipalEx FindUserByDn(string distinguishedName);
        UserPrincipalEx FindUserBySamAccountName(string samAccountName);
        UserPrincipalEx FindUserBySamAccountName(string samAccountName, string ldapPath);
        ComputerPrincipalEx FindComputer(string computerName);
        ComputerPrincipalEx FindComputer(string computerName, string ldapPath);
        GroupPrincipal FindGroup(string groupName);
        GroupPrincipal FindGroup(string groupName, string ldapPath);
        IEnumerable<Principal> Groups(Principal principal);
        void Update(Principal principal);
        void CreateUser(string accountName, string emailAddress, string password, string displayName, string ldapPath);
    }

    public class ActiveDirectoryProxy : IDisposable, IActiveDirectoryProxy
    {
        private PrincipalContext _ldapContext;
        private PrincipalContext _rootContext;

        private readonly string _domainName = string.Empty;
        private readonly string _userName = string.Empty;
        private readonly string _password = string.Empty;
        private string _ldapPath = string.Empty;

        public ActiveDirectoryProxy(string domainName)
        {
            if (string.IsNullOrEmpty(domainName))
                throw new ArgumentNullException(nameof(domainName));

            _domainName = domainName;
        }

        public ActiveDirectoryProxy(string domainName, string userName, string password)
        {
            if (string.IsNullOrEmpty(domainName))
                throw new ArgumentNullException(nameof(domainName));

            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException(nameof(userName));

            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException(nameof(password));

            _domainName = domainName;
            _userName = userName;
            _password = password;
        }

        private PrincipalContext RootContext
        {
            get
            {
                if (_rootContext != null)
                    return _rootContext;

                try
                {
                    if (string.IsNullOrEmpty(_userName) || string.IsNullOrEmpty(_password))
                    {
                        _rootContext = new PrincipalContext(ContextType.Domain, _domainName);
                    }
                    else
                    {
                        _rootContext = new PrincipalContext(ContextType.Domain, _domainName, _userName, _password);
                    }
                }
                catch (Exception ex)
                {
                    throw new ActiveDirectoryProxyException("Failed to create root PrincipleContext.", ex);
                }
                return _rootContext;
            }
        }

        private PrincipalContext LdapContext
        {
            get
            {
                if (_ldapContext != null && _ldapContext.Container.Equals(_ldapPath, StringComparison.OrdinalIgnoreCase))
                    return _ldapContext;

                try
                {
                    if (string.IsNullOrEmpty(_userName) || string.IsNullOrEmpty(_password))
                    {
                        _ldapContext = new PrincipalContext(ContextType.Domain, _domainName, _ldapPath);
                    }
                    else
                    {
                        _ldapContext = new PrincipalContext(ContextType.Domain, _domainName, _ldapPath, _userName, _password);
                    }
                }
                catch (Exception ex)
                {
                    throw new ActiveDirectoryProxyException("Failed to create PrincipleContext for the LDAP path '{_ldapPath}'.", ex);
                }

                return _ldapContext;
            }
        }

        public UserPrincipalEx FindUserBySamAccountName(string samAccountName)
        {
            return FindOne(new UserPrincipalEx(RootContext) { SamAccountName = samAccountName });
        }

        public UserPrincipalEx FindUserBySamAccountName(string samAccountName, string ldapPath)
        {
            _ldapPath = ldapPath;

            return FindOne(new UserPrincipalEx(LdapContext) { SamAccountName = samAccountName });
        }

        public UserPrincipalEx FindUserByCn(string userName)
        {
            return UserPrincipalEx.FindByIdentity(RootContext, userName);
        }

        public UserPrincipalEx FindUserByDn(string distinguishedName)
        {
            return FindUserByCn(distinguishedName.GetCnFromDn());
        }

        public UserPrincipalEx FindUserByCn(string userName, string ldapPath)
        {
            _ldapPath = ldapPath;
            return UserPrincipalEx.FindByIdentity(LdapContext, userName);
        }

        public GroupPrincipal FindGroup(string groupName)
        {
            return GroupPrincipal.FindByIdentity(RootContext, groupName);
        }

        public GroupPrincipal FindGroup(string groupName, string ldapPath)
        {
            _ldapPath = ldapPath;
            return GroupPrincipal.FindByIdentity(LdapContext, groupName);
        }

        public ComputerPrincipalEx FindComputer(string computerName)
        {
            return ComputerPrincipalEx.FindByIdentity(RootContext, IdentityType.Name, computerName);
        }

        public ComputerPrincipalEx FindComputer(string computerName, string ldapPath)
        {
            _ldapPath = ldapPath;
            return ComputerPrincipalEx.FindByIdentity(LdapContext, IdentityType.Name, computerName);
        }

        public IEnumerable<Principal> Groups(Principal principal)
        {
            var groupResults = principal.GetGroups();
            var groupPrincipals = new Collection<Principal>();

            foreach (var result in groupResults)
            {
                groupPrincipals.Add(result);
            }
            return groupPrincipals;
        }

        private static TPrincipal FindOne<TPrincipal>(TPrincipal queryFilter) where TPrincipal : Principal
        {
            using (var searcher = new PrincipalSearcher { QueryFilter = queryFilter })
            {
                return (TPrincipal)searcher.FindOne();
            }
        }

        public void CreateUser(string accountName, string emailAddress, string password, string displayName, string ldapPath)
        {
            _ldapPath = ldapPath;

            using (var userPrincipal = new UserPrincipal(LdapContext))
            {
                userPrincipal.SamAccountName = accountName;
                userPrincipal.EmailAddress = emailAddress;
                userPrincipal.DisplayName = displayName;
                userPrincipal.SetPassword(password);
                userPrincipal.Save();
            }

        }

        public void Update(Principal principal)
        {
            principal.Save();
        }

        public void Dispose()
        {
            _ldapContext?.Dispose();
            _rootContext?.Dispose();
        }
    }

    public class ActiveDirectoryProxyException : Exception
    {
        public ActiveDirectoryProxyException(string message, Exception exception) : base(message, exception) { }
    }
}
