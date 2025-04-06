using FarmOps.Models.LoginModels;
using FarmOps.Models.Signup;
using FarmOps.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Data;
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
        [HttpPost("login")]
        public string Post([FromBody] LoginRequest data)
        {
            try
            {
                //LoginRequest data = JsonConvert.DeserializeObject<LoginRequest>(value);
                LoginModel user = null;
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
                if (user.EmailAddress != null)
                {
                    var claims = new[]
                    {
                        new Claim(ClaimTypes.Name, user.UserId)
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

                    return JsonConvert.SerializeObject(new { type = "success", statuscode = 200, data = user, token = tokenString, profilePic_bas64Str = base64String });
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

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp(SignUp_Save signUpDetails)
        {

            // Declare the UserId variable that will be generated
            string userId = string.Empty;

            // Check the type and call the GetNextNo procedure accordingly
            try
            {
                // Check if email already exists in tbl_FLogin
                if (await IsEmailExists(signUpDetails.Email))
                {
                    return BadRequest(new { message = "Email is already registered." });
                }
                // Call the GetNextNo stored procedure based on Type
                if (signUpDetails.Type == "W" || signUpDetails.Type == "S")
                {
                    // Call GetNextNo for WorkerId
                    userId = await GetNextNo("WorkerId");
                }
                else if (signUpDetails.Type == "C")
                {
                    // Call GetNextNo for GrowerId
                    userId = await GetNextNo("GrowerId");
                }
                else if (signUpDetails.Type == "M")
                {
                    // Call GetNextNo for MonitorId
                    userId = await GetNextNo("MonitorId");
                }
                else
                {
                    return BadRequest(new { message = "Invalid Type provided" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error generating UserId", error = ex.Message });
            }

            // Create the UserDetailsTbl and Login_Insert objects from SignUp_Save
            var userDetails = new UserDetailsTbl
            {
                UserId = userId, // Dynamic UserId based on type
                FirstName = signUpDetails.FirstName,
                LastName = signUpDetails.LastName,
                MiddleName = signUpDetails.MiddleName,
                Picture = signUpDetails.Picture,
                Dob = signUpDetails.Dob,
                PassportNumber = signUpDetails.PassportNumber,
                Phone = signUpDetails.Phone,
                PayRate = signUpDetails.PayRate,
                Ird = signUpDetails.Ird,
                ForkliftCert = signUpDetails.ForkliftCert,
                Ir330 = signUpDetails.Ir330,
                Licence = signUpDetails.Licence,
                WorkAuth = signUpDetails.WorkAuth,
                WorkType = signUpDetails.WorkType,
                WorkEvId = signUpDetails.WorkEvId,
                Document = signUpDetails.Document,
                Ir330Document = signUpDetails.Ir330Document,
                AccNum = signUpDetails.AccNum,
                PayrollId = signUpDetails.PayrollId,
                PreEmployment = signUpDetails.PreEmployment,
                SignPic = signUpDetails.SignPic
            };

            var loginDetails = new Login_Insert
            {
                Id = userId, // Dynamic UserId
                Email = signUpDetails.Email,
                Password = signUpDetails.Password,
                Type = signUpDetails.Type
            };

            string insertUserDetailsQuery = @"
    INSERT INTO tbl_FUserDetails (
        UserId, FirstName, LastName, MiddleName, Picture, Dob, PassportNumber, 
        Phone, PayRate, Ird, ForkliftCert, Ir330, Licence, WorkAuth, WorkType, 
        WorkEvId, Document, Ir330Document, AccNum, PayrollId, PreEmployment, SignPic
    )
    VALUES (
        @UserId, @FirstName, @LastName, @MiddleName, @Picture, @Dob, @PassportNumber, 
        @Phone, @PayRate, @Ird, @ForkliftCert, @Ir330, @Licence, @WorkAuth, @WorkType, 
        @WorkEvId, @Document, @Ir330Document, @AccNum, @PayrollId, @PreEmployment, @SignPic
    );";

            string insertLoginDetailsQuery = @"
    INSERT INTO tbl_FLogin (
        userId, emailAddress, userPassword, userType
    )
    VALUES (
        @userId, @emailAddress, @userPassword, @userType
    );";

            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync();

                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // Insert into tbl_FUserDetails
                            using (var command = new SqlCommand(insertUserDetailsQuery, connection, transaction))
                            {
                                // Add parameters for tbl_FUserDetails insert
                                command.Parameters.AddWithValue("@UserId", userDetails.UserId);
                                command.Parameters.AddWithValue("@FirstName", userDetails.FirstName);
                                command.Parameters.AddWithValue("@LastName", userDetails.LastName);
                                command.Parameters.AddWithValue("@MiddleName", userDetails.MiddleName);
                                command.Parameters.AddWithValue("@Picture", userDetails.Picture ?? (object)DBNull.Value);
                                command.Parameters.AddWithValue("@Dob", userDetails.Dob);
                                command.Parameters.AddWithValue("@PassportNumber", userDetails.PassportNumber);
                                command.Parameters.AddWithValue("@Phone", userDetails.Phone);
                                command.Parameters.AddWithValue("@PayRate", userDetails.PayRate);
                                command.Parameters.AddWithValue("@Ird", userDetails.Ird);
                                command.Parameters.AddWithValue("@ForkliftCert", userDetails.ForkliftCert);
                                command.Parameters.AddWithValue("@Ir330", userDetails.Ir330);
                                command.Parameters.AddWithValue("@Licence", userDetails.Licence);
                                command.Parameters.AddWithValue("@WorkAuth", userDetails.WorkAuth);
                                command.Parameters.AddWithValue("@WorkType", userDetails.WorkType);
                                command.Parameters.AddWithValue("@WorkEvId", userDetails.WorkEvId);
                                command.Parameters.AddWithValue("@Document", userDetails.Document ?? (object)DBNull.Value);
                                command.Parameters.AddWithValue("@Ir330Document", userDetails.Ir330Document ?? (object)DBNull.Value);
                                command.Parameters.AddWithValue("@AccNum", userDetails.AccNum);
                                command.Parameters.AddWithValue("@PayrollId", userDetails.PayrollId);
                                command.Parameters.AddWithValue("@PreEmployment", userDetails.PreEmployment ?? (object)DBNull.Value);
                                command.Parameters.AddWithValue("@SignPic", userDetails.SignPic ?? (object)DBNull.Value);

                                await command.ExecuteNonQueryAsync();
                            }

                            // Insert into tbl_FLogin
                            using (var command = new SqlCommand(insertLoginDetailsQuery, connection, transaction))
                            {
                                // Add parameters for tbl_FLogin insert
                                command.Parameters.AddWithValue("@userId", loginDetails.Id);
                                command.Parameters.AddWithValue("@emailAddress", loginDetails.Email);
                                command.Parameters.AddWithValue("@userPassword", loginDetails.Password);
                                command.Parameters.AddWithValue("@userType", loginDetails.Type);

                                await command.ExecuteNonQueryAsync();
                            }

                            // Commit the transaction if both inserts are successful
                            transaction.Commit();

                            return Ok(new { message = "Signup successful!", userId = loginDetails.Id });
                        }
                        catch (Exception ex)
                        {
                            // Rollback the transaction if an error occurs
                            transaction.Rollback();
                            return StatusCode(500, new { message = "An error occurred during signup", error = ex.Message });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during database operation", error = ex.Message });
            }
        }

        // Helper method to check if email exists
        private async Task<bool> IsEmailExists(string email)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                var query = "SELECT COUNT(*) FROM tbl_FLogin WHERE emailAddress = @Email";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    var count = (int)await command.ExecuteScalarAsync();
                    return count > 0;
                }
            }
        }

        private async Task<string> GetNextNo(string columnName)
        {
            string nextValue = string.Empty;

            // Call the GetNextNo stored procedure
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("dbo.GetNextNo", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ColumnName", columnName);

                    // Output parameter for the next value
                    SqlParameter outputParam = new SqlParameter("@NextValue", SqlDbType.NVarChar, 50)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(outputParam);

                    await command.ExecuteNonQueryAsync();

                    nextValue = outputParam.Value.ToString();
                }
            }

            return nextValue;
        }


    }
}
