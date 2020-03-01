using System;
using System.Linq;

namespace ActiveDirectoryDemo.Infrastructure.ActiveDirectory
{
    public static class Extensions
    {
        public static string GetCnFromDn(this string distinguishedName)
        {
            var orgUnitIndex = distinguishedName.IndexOf("OU=", StringComparison.OrdinalIgnoreCase);
            var cn = distinguishedName.Substring(3, orgUnitIndex - 4);

            return cn.Replace("\\", string.Empty);
        }

        public static bool HasData(this object[] result)
        {
            return result != null && result.Any();
        }
    }
}
