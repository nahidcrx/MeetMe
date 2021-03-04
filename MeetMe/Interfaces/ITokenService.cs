using MeetMe.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetMe.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(AppUser appUser);
    }
}
