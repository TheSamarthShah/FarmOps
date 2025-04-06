using FarmOps.Models;
using FarmOps.Models.LoginModels;
using Microsoft.Data.SqlClient;
using Dapper;

namespace FarmOps.Repos
{
    public class LoginRepository : ILoginRepository
    {
        private readonly DBContext _dbContext;
        private readonly string _connectionString;
        public LoginRepository(DBContext dBContext, string connectionString)
        {
            _dbContext = dBContext;
            _connectionString = connectionString;
        }

        public LoginModel GetUserForLogin(string type, string email, string password)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                connection.Open();

                string sql = @"
                SELECT 
                    userId AS UserId,
                    emailAddress AS EmailAddress,
                    userPassword AS UserPassword,
                    userType AS UserType,
                    isPasswordChanged AS IsPasswordChanged,
                    visaVerification AS VisaVerification,
                    lastIpAddress AS LastIpAddress,
                    lastLoginTime AS LastLoginTime,
                    isSuperuser AS IsSuperuser,
                    promoCode AS PromoCode,
                    registrationDate AS RegistrationDate,
                    industryId AS IndustryId,
                    package AS Package
                FROM tbl_Flogin 
                WHERE userType = @UserType 
                AND emailAddress = @Email 
                AND userPassword = @Password";

                LoginModel? res = connection.QueryFirstOrDefault<LoginModel>(sql, new
                {
                    UserType = type,
                    Email = email,
                    Password = password
                });

                return res ?? new LoginModel();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Database error: " + ex.Message);
            }
        }
    }
}
