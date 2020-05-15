using AutoMapper;
using ParkyAPI.Models;
using ParkyAPI.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkyAPI.Mappers
{
    public class ParkyMappings : Profile
    {
        public ParkyMappings()
        {
            CreateMap<NationalPark, NationalParkDTO>().ReverseMap();
            CreateMap<Trail, TrailDTO>().ReverseMap();
            CreateMap<Trail, TrailUpdateDTO>().ReverseMap();
            CreateMap<Trail, TrailCreateDTO>().ReverseMap();
        }
    }
}
