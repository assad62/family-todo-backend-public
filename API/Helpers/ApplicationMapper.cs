using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Entity;
using Entity.DTOs;

namespace API.Helpers
{
    public class ApplicationMapper: Profile
    {
        public ApplicationMapper()
        {
            
            
             CreateMap<UserTask, UserTaskDTO>().ReverseMap();
            
        }
    }

}