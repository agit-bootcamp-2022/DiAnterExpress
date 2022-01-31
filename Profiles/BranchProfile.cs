using System;
using AutoMapper;
using DiAnterExpress.Dtos;
using DiAnterExpress.Models;

namespace DiAnterExpress.Profiles
{
    public class BranchProfile : Profile
    {
        public BranchProfile()
        {
            CreateMap<Branch, BranchOutputDto>();

            CreateMap<BranchInsertDto, Branch>();
        }
    }
}