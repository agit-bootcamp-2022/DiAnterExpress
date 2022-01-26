using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiAnterExpress.Dtos;
using AutoMapper;

namespace DiAnterExpress.Profiles
{
    public class ProfileUser : Profile
    {
        public ProfileUser()
        {
            CreateMap<DtoUserInputBranch, DtoUserRegister>()
                .ForMember(dst => dst.Role, opt => opt.MapFrom(src => role.branch));
    
        }
    }
}