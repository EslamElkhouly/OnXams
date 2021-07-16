using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using OnlineExamPlatform.Models;

namespace OnlineExamPlatform.Authentication
{
    public class MyCustomRoles:RoleProvider
    {
       
        public override string[] GetRolesForUser(string username)
        {
            List<string> myRolesList = new List<string>();  
            using (var _context=new OnXamsEntities())
            {
                
               var user= _context.UserAuthentications.SingleOrDefault(u => u.Email.ToLower() == username.ToLower());
               if (user==null)
               {
                   return new string[] { };
               }

               var roles = _context.RolesMappingUsers
                   .FirstOrDefault(rmu => rmu.UserAuthenticationId == user.UserAuthenticationId);
               var rolename = _context.Roles.Where(r => r.RolesId == roles.RolesId);
               foreach (var role in rolename)
               {
                   myRolesList.Add(role.RoleName);
               }
               if (rolename==null)
               {
                   return new string[] { }; 
               }

               return myRolesList.ToArray();

            }
          
        }
        public override bool IsUserInRole(string username, string roleName)
        {
            using (OnXamsEntities _context=new OnXamsEntities())
            {
                var user = _context.UserAuthentications.FirstOrDefault(u => u.Email.ToLower() == username.ToLower());

                var roles = _context.RolesMappingUsers
                    .Where(rmu =>
                        rmu.UserAuthentication.UserAuthenticationId ==
                        user.UserAuthenticationId)
                    .Select(r => r.Role.RoleName);
                if (user != null)
                {
                    return roles.Any(r => r.Equals(roleName));
                }
                else
                {
                    return false;
                }
            }
        }

       

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            List<string> roles = new List<string>();
            using (OnXamsEntities _context=new OnXamsEntities())
            {
                try
                {
                    var rolesInDb = _context.Roles.ToList();
                    foreach (var role in rolesInDb)
                    {
                        roles.Add(role.RoleName);
                    }
                }
                catch 
                {
                }
            }

            return roles.ToArray();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string ApplicationName { get; set; }
    }
}