using System.Collections;
using System.Security.AccessControl;
using System.Security.Principal;


#nullable enable
namespace Utility.FileSystem.Helper
{
    public static class AccessRightsChecker
    {
        public static bool ItemHasPermision(string itemPath, FileSystemRights accessRight)
        {
            if (string.IsNullOrEmpty(itemPath))
                return false;
            bool? nullable1 = itemPath.IsPathFile();
            if (!nullable1.HasValue)
                return false;
            try
            {
                AuthorizationRuleCollection authorizationRuleCollection = (AuthorizationRuleCollection)null;
                bool? nullable2 = nullable1;
                bool flag = true;
                if (!(nullable2.GetValueOrDefault() == flag & nullable2.HasValue))
                    ;
                WindowsIdentity current = WindowsIdentity.GetCurrent();
                string str = current.User.Value;
                foreach (FileSystemAccessRule systemAccessRule in (ReadOnlyCollectionBase)authorizationRuleCollection)
                {
                    if ((((object)systemAccessRule.IdentityReference).ToString() == str || current.Groups.Contains(systemAccessRule.IdentityReference)) && (accessRight & systemAccessRule.FileSystemRights) == accessRight)
                    {
                        if (systemAccessRule.AccessControlType == AccessControlType.Deny)
                            return false;
                        if (systemAccessRule.AccessControlType == AccessControlType.Allow)
                            return true;
                    }
                }
            }
            catch
            {
            }
            return false;
        }
    }
}