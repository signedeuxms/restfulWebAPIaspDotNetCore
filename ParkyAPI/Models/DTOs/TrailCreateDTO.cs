﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static ParkyAPI.Models.Trail;

namespace ParkyAPI.Models.DTOs
{
    // trailDTO for create and update queries
    public class TrailCreateDTO
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public double Distance { get; set; }

        [Required]
        public int NationalParkId { get; set; }

        [Required]
        public DifficultyType Difficulty { get; set; }

    }
}
