using Furion;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Heavens.Application.AuthorizeApp.Services;

public class AuthorizeService : IAuthorizeService, IScoped
{
    public AuthorizeService(IHttpContextAccessor httpContextAccessor, IRepository<User> userRepository)
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
                new Claim(ClaimTypes.Sid,user.Id.ToString()),
                new Claim(ClaimTypes.Name,user.Account),
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
