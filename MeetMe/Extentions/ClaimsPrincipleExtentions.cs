﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MeetMe.Extentions
{
    public static class ClaimsPrincipleExtentions
    {
        public static string GetUserName(this ClaimsPrincipal user) 
        {
            return user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
