using AutoMapper;
using UserManagementReact.Entities;
using UserManagementReact.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagementReact.Helpers
{
	public class AutomapperProfiles
	{
		public class UserProfile : Profile
		{
			public UserProfile()
			{
				CreateMap<ApplicationUser, UserModel>()
					.ForMember(dest => dest.Password, opt => opt.Ignore())
					.ForMember(dest => dest.ConfirmPassword, opt => opt.Ignore());

				// Make sure to not ovewrite automatically created ApplicationUser Id when ApplicationUserViewModel Id is null
				CreateMap<UserModel, ApplicationUser>()
					.ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
					.ForMember(dest => dest.Id, opt => opt.Condition(cond => cond.Id != null));

				CreateMap<ApplicationUser, UpdateUserModel>();

				CreateMap<UpdateUserModel, ApplicationUser>()
					.ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));

				//CreateMap<ApplicationUser, RegisterModel>();

				//CreateMap<RegisterModel, ApplicationUser>()
				//	.ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));
			}
		}
	}
}
