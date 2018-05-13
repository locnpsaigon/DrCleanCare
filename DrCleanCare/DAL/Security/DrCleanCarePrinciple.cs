using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Principal;

namespace DrCleanCare.DAL.Security
{
    public class DrCleanCarePrinciple : IPrincipal
    {
        public IIdentity Identity { get; private set; }

        public DrCleanCarePrinciple(String Username)
        {
            this.Identity = new GenericIdentity(Username);
        }

        public Boolean IsInRole(string userRoles)
        {
            foreach (var role in Roles)
            {
                if (userRoles.Contains(role))
                {
                    return true;
                }   
            }
            return false;
        }

        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string[] Roles { get; set; }
    }
}