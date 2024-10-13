using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Reklamna_agencija.Data;
using Reklamna_agencija.DTOs;
using Reklamna_agencija.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;


namespace Reklamna_agencija.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly DataContext dc;

        private readonly IConfiguration _configuration;

        public LoginController(DataContext dataContext, IConfiguration configuration)
        {
            this.dc = dataContext;
            this._configuration = configuration;
        }
        public static string EncryptPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Pretvori lozinku u bajtovni niz
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Konvertuj bajtove u string u heksadecimalan format
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        [HttpPost]
        [Route("PostLoginDetails")]
        public async Task<IActionResult> PostLoginDetails(UserData _userData)
        {
            if (_userData != null)
            {
                Korisnik user;

                var resultLoginCheck = dc.AdminiAgencije
                .Where(e => e.Email == _userData.Email && e.Password == EncryptPassword(_userData.Password) && e.Status == true)
                .FirstOrDefault();
                    var resultLoginCheck2 = dc.Klijenti
                        .Where(e => e.Email == _userData.Email && e.Password == EncryptPassword(_userData.Password) && e.Status == true)
                        .FirstOrDefault();
                var resultLoginCheck3 = dc.Posetioci
                        .Where(e => e.Email == _userData.Email && e.Password == EncryptPassword(_userData.Password) && e.Status == true)
                        .FirstOrDefault();

                switch (true)
                {
                    case object when resultLoginCheck != null:
                        user = resultLoginCheck;
                        break;
                    case object when resultLoginCheck2 != null:
                        user = resultLoginCheck2;
                        break;
                    case object when resultLoginCheck3 != null:
                        user = resultLoginCheck3;
                        break;
                    default:
                        return BadRequest("Invalid Credentials");
                }

                var refreshToken = GenerateRefreshToken();
                //user.RefreshToken = refreshToken;
                await dc.SaveChangesAsync();
                //_userData.UserMessage = "Login Success";

#pragma warning disable CS8604 // Possible null reference argument.
                var claims = new[] {
                        new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim("UserEmail", user.Email),
                        new Claim("UserRole", user.Uloga),
                        new Claim("UserPassword", user.Password),
                        new Claim("UserId", user.Id.ToString())
                    };
#pragma warning restore CS8604 // Possible null reference argument.


#pragma warning disable CS8604 // Possible null reference argument.
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
#pragma warning restore CS8604 // Possible null reference argument.
                var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    _configuration["Jwt:Issuer"],
                    _configuration["Jwt:Audience"],
                    claims,
                    expires: DateTime.UtcNow.AddMinutes(60),
                    signingCredentials: signIn);


                _userData.AccessToken = new JwtSecurityTokenHandler().WriteToken(token);


                KorisnikDto userDto = new KorisnikDto(new JwtSecurityTokenHandler().WriteToken(token), user.Email, user.Uloga, user.Id);

               return new JsonResult(userDto);

                //return new JsonResult(response);
            }
            else
            {
                return new JsonResult("Hej :)");
            }
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
    }
}
