﻿using MeetMe.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetMe.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options) 
        {

        }
        public DbSet<AppUser> Users { get; set; }

        internal Task<bool> AnyAsync()
        {
            throw new NotImplementedException();
        }
    }
}
