﻿using Microsoft.EntityFrameworkCore;
using ParkyAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkyAPI.Data
{
    public class ParkyAPIdbContext: DbContext
    {
        public ParkyAPIdbContext(DbContextOptions<ParkyAPIdbContext> options): 
            base(options)
        {

        }

        public DbSet<NationalPark> NationalParks { get; set; }
    }
}
