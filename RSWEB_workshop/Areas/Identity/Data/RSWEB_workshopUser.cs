using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace RSWEB_workshop.Areas.Identity.Data;

// Add profile data for application users by adding properties to the RSWEB_workshopUser class
public class RSWEB_workshopUser : IdentityUser
{
    public static implicit operator RSWEB_workshopUser(List<RSWEB_workshopUser> v)
    {
        throw new NotImplementedException();
    }
}

