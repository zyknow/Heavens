using Bing.Extensions;
using Furion;
using Furion.DatabaseAccessor;
using Furion.DataEncryption;
using Furion.DependencyInjection;
using Furion.FriendlyException;
using Heavens.Core;
using Heavens.Core.Authorizations;
using Heavens.Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Heavens.Application.AuthorizeApp.Services;

public class AuthorizeServices : IAuthorizeServices, IScoped
{
    public AuthorizeServices(IHttpContextAccessor httpContextAccessor, IRepository<User> userRepository)
    {
        _httpContextAccessor = httpContextAccessor;
        _userRepository = userRepository;
    }

    public IHttpContextAccessor _httpContextAccessor { get; }
    public IRepository<User> _userRepository { get; }

    /// <summary>
    /// 根据用户创建token
    /// </summary>
    /// <param name="user"></param>
    /// <param name="expiredTime">过期时间 /分钟</param>
    /// <returns></returns>
    public string CreateToken(User user, long? expiredTime = null)
    {
        JWTSettings jwtSettings = App.GetConfig<JWTSettings>("JWTSettings");

        List<Claim> claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sid,user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub,user.Account),
                //new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                //new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

        user.Roles?.Split("|")?.ForEach(role => claims.Add(new Claim(ClaimTypes.Role, role)));

        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.IssuerSigningKey));
        SigningCredentials creds = new SigningCredentials(key, jwtSettings.Algorithm);

        string token = new JwtSecurityTokenHandler()
            .WriteToken(new JwtSecurityToken(
                issuer: jwtSettings.ValidIssuer,
                audience: jwtSettings.ValidAudience,
                expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(expiredTime ?? jwtSettings.ExpiredTime)),
                claims: claims,
                signingCredentials: creds
                ));


        return token;
    }

    public async Task<string> RefreshToken(string oldToken)
    {
        Microsoft.IdentityModel.JsonWebTokens.JsonWebToken tokenInfo = JWTEncryption.ReadJwtToken(oldToken);
        int id = tokenInfo.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sid).Value.ToInt();

        User user = await _userRepository.FirstOrDefaultAsync();

        return user.IsNull() ? throw Oops.Oh(Excode.REFRESHTOKEN_NO_EXIST_OR_EXPIRE) : CreateToken(user);
    }

}
