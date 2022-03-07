using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TeTacApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Cors;

namespace JWTAuthenticationExample.Controllers
{
  [Route("[controller]")]
  [ApiController]
  public class LoginController : ControllerBase
  {
    private readonly IConfiguration _config;
    private readonly TeTacApiContext _context;
    private readonly ILogger _logger;


    public LoginController(IConfiguration config, TeTacApiContext context, ILogger<LoginController> logger)
    {
      _config = config;
      _context = context;
      _logger = logger;
    }
    private void Log(string _txt)
    {
      _logger.LogInformation("IP : " + HttpContext.Connection.RemoteIpAddress.ToString() + " | " + _txt);
    }

    [HttpPost]
    [AllowAnonymous]
    public IActionResult Login([FromBody]User login)
    {
      Log("AuthenticateUser with UserName: " + login.UserName + ", password:" + login.Password);
      IActionResult response = Unauthorized();
      Utilisateur user = AuthenticateUser(login);
      if (user != null)
      {
        var tokenString = GenerateJWTToken(user);
        response = Ok(new
        {
          token = tokenString,
          userDetails = user,
        });
        Log("AuthenticateUser OK with UserName: " + login.UserName+ " | password:" + login.Password + " | token : " + tokenString);
      }
      else Log("AuthenticateUser Unauthorized with UserName: " + login.UserName + ", password:" + login.Password);
      return response;
    }
    Utilisateur AuthenticateUser(User loginCredentials)
    {
     

      Utilisateur utilisateur = _context.Utilisateurs.SingleOrDefault(x => x.Login == loginCredentials.UserName && x.Password == loginCredentials.Password);
            
            if (utilisateur!=null)
      {

      }
      return utilisateur;
    }
    string GenerateJWTToken(Utilisateur userInfo)
    {
      var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
      var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
      Droit _droit = _context.Droits.SingleOrDefault(x => x.IdUtilisateur == userInfo.IdUtilisateur && x.IdApp == "TETACAPI");
      Salon _salon = _context.Salons.Find(_droit.IdSalon);
      var claims = new[]
        {
new Claim(JwtRegisteredClaimNames.Sub, "TeTacApi"),
new Claim("fullName", userInfo.Prenom.ToString() + " " +userInfo.Nom.ToString()),
new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
};
      var token = new JwtSecurityToken(
      issuer: _config["Jwt:Issuer"],
      audience: _config["Jwt:Audience"],
      claims: claims,
      expires: DateTime.Now.AddMinutes(30),
      signingCredentials: credentials
      );
      return new JwtSecurityTokenHandler().WriteToken(token);


    }
  }
}
