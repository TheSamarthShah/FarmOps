using FarmOps.Models;
using FarmOps.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FarmOps.Controllers
{
    [Route("api/")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        private readonly ILoginService _loginService;
        private readonly IConfiguration _configuration;
        public class LoginRequest
        {
            public string AccountType { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
        }
        public ApiController(ILoginService loginService, IConfiguration configuration)
        {
            _loginService = loginService;
            _configuration = configuration;
        }
        [HttpPost("loginapi")]
        public string Post([FromBody] string value)
        {
            try
            {
                LoginRequest data = JsonConvert.DeserializeObject<LoginRequest>(value);
                BasicUserDetailModel user = null;
                if (data != null && data.AccountType != null && data.Email != null && data.Password != null)
                {
                    if (data.AccountType == "S")
                    {
                        data.AccountType = "Supervisor";
                    }
                    else if (data.AccountType == "W")
                    {
                        data.AccountType = "Worker";
                    }
                    else if (data.AccountType == "C")
                    {
                        data.AccountType = "Contractor";
                    }
                    else
                    {
                        data.AccountType = "Monitor";
                    }
                    user = _loginService.verifyLogin(data.AccountType, data.Email, data.Password);
                }
                if (user.Email != null)
                {
                    var claims = new[]
                    {
                        new Claim(ClaimTypes.Name, user.Id)
                    };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]));
                    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                    var token = new JwtSecurityToken(
                        issuer: _configuration["Jwt:Issuer"],
                        audience: _configuration["Jwt:Audience"],
                        claims: claims,
                        expires: DateTime.Now.AddMinutes(30),
                        signingCredentials: creds
                    );

                    var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                    string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/AppData/ProfilePics", "luffy_pp.jpg");
                    string base64String = "";
                    if (System.IO.File.Exists(imagePath))
                    {
                        byte[] imageBytes = System.IO.File.ReadAllBytes(imagePath);
                        base64String = Convert.ToBase64String(imageBytes);
                    }
                        
                    return JsonConvert.SerializeObject(new { type = "success", statuscode = 200, data = user, profilePic_bas64Str = base64String, token = tokenString });
                }
                else
                {
                    return JsonConvert.SerializeObject(new { type = "error", statuscode = 200, message = "Invalid User" });
                }
            }
            catch (HttpRequestException ex)
            {
                int statusCode = (int)HttpStatusCode.InternalServerError; // Default to InternalServerError
                if (ex.Message.Contains("404"))
                {
                    statusCode = (int)HttpStatusCode.NotFound;
                }

                return JsonConvert.SerializeObject(new { type = "error", statuscode = statusCode, message = ex.Message });
            }
            catch (Exception ex)
            {
                // For general exceptions, return a generic error status code (e.g., 500)
                return JsonConvert.SerializeObject(new { type = "error", statuscode = 500, message = ex.Message });
            }

        }
    }
}
