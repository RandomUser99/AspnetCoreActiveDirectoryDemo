using System.DirectoryServices.AccountManagement;

namespace ActiveDirectoryDemo.Infrastructure.ActiveDirectory
{
    /// <summary>
    /// Extension of the ComputerPrincipal class to add missing properties
    /// https://blogs.msdn.microsoft.com/kaevans/2012/04/11/querying-active-directory-using-principal-extensions-in-system-directoryservices-accountmanagement/
    /// </summary>

    [DirectoryObjectClass("computer")]
    [DirectoryRdnPrefix("CN")] // Must be CN for Active Directory
    public class ComputerPrincipalEx : ComputerPrincipal
    {
        public ComputerPrincipalEx(PrincipalContext context) : base(context) { }

        public ComputerPrincipalEx(PrincipalContext context, string samAccountName, string password, bool enabled) : base(context, samAccountName, password, enabled) { }

        // Implement the overloaded search method FindByIdentity.
        public new static ComputerPrincipalEx FindByIdentity(PrincipalContext context, string identityValue)
        {
            return (ComputerPrincipalEx)FindByIdentityWithType(context, typeof(ComputerPrincipalEx), identityValue);
        }

        // Implement the overloaded search method FindByIdentity. 
        public new static ComputerPrincipalEx FindByIdentity(PrincipalContext context, IdentityType identityType, string identityValue)
        {
            return (ComputerPrincipalEx)FindByIdentityWithType(context, typeof(ComputerPrincipalEx), identityType, identityValue);
        }

        [DirectoryProperty("userAccountControl")]
        public int? UserAccountControl
        {
            get
            {
                var result = ExtensionGet("userAccountControl");

                if (result.HasData())
                    return (int)result[0];

                return null;
            }
            set => ExtensionSet("userAccountControl", value);
        }

        [DirectoryProperty("managedBy")]
        public string ManagedBy
        {
            get
            {
                var result = ExtensionGet("managedBy");

                if (result.HasData())
                    return (string)result[0];

                return null;
            }
            set => ExtensionSet("managedBy", value);
        }

        [DirectoryProperty("isSynchronized")]
        public bool? IsSynchronized
        {
            get
            {
                var result = ExtensionGet("isSynchronized");

                if (result.HasData())
                    return (bool)result[0];

                return null;
            }
            set => ExtensionSet("isSynchronized", value);
        }

        [DirectoryProperty("isGlobalCatalogReady")]
        public bool? IsGlobalCatalogReady
        {
            get
            {
                var result = ExtensionGet("isGlobalCatalogReady");

                if (result.HasData())
                    return (bool)result[0];

                return null;
            }
            set => ExtensionSet("isGlobalCatalogReady", value);
        }
    }
}
