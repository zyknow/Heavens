using Bing.Extensions;
using Heavens.Core.Entities;
using Mapster;
using System;
using System.Linq;

namespace Heavens.Application.UserApp.Dtos
{
    public class Mapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.ForType<User, UserDto>()
            .Map(dest => dest.Roles, src => src.Roles.Split("|", StringSplitOptions.None).ToList(), src => !src.Roles.IsEmpty())
            .Ignore(dest => dest.Passwd);

            config.ForType<UserDto, User>()
                .Map(dest => dest.Roles, src => string.Join("|", src.Roles), src => !src.Roles.IsEmpty());
        }
    }
}