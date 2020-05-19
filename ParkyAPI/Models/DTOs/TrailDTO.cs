using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
// to access the enum DifficultyType
using static ParkyAPI.Models.Trail;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace ParkyAPI.Models.DTOs
{
    public class TrailDTO
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public double Distance { get; set; }

        [Required]
        public double Elevation { get; set; }

        [Required]
        public int NationalParkId { get; set; }

        [Required]
        public DifficultyType Difficulty { get; set; }

        public NationalParkDTO NationalParkDTO { get; set; }
    }
}
