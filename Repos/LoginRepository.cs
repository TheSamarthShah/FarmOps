using FarmOps.Common;
using FarmOps.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Speckle.Newtonsoft.Json;
using System.Data;
using System.Net;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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

        public BasicUserDetailModel GetUserForLogin(string type, string email, string password) {
            try
            {
                LoginModel? user = _dbContext.LoginModels.FirstOrDefault(u => u.AccountType == type && u.Email == email && u.Password == password);
                DataTable Userdetails = new DataTable();
                BasicUserDetailModel UserDetail = new BasicUserDetailModel();
                if (user != null)
                {
                    using (var connection = new SqlConnection(_connectionString))
                    {
                        try
                        {
                            connection.Open(); 
                            string query = DbConstants.selectBasicUserDetail;
                            //create table name
                            string tableName = "tbl_worker";
                            string idClmName = "WorkersId";
                            if(type  == "Contractor")
                            {
                                tableName = "tbl_grower";
                                idClmName = "GrowersId";
                            }
                            else if (type == "Monitor")
                            {
                                tableName = "tbl_monitor";
                                idClmName = "MonitorsId";
                            }
                            query = query.Replace("[IDCOLUMNNAME]", idClmName);
                            query = query.Replace("[TABLENAME]", tableName);
                            
                            var command = new SqlCommand(query, connection);
                            command.Parameters.AddWithValue("@p0", user.UserId);
                            SqlDataAdapter sa = new SqlDataAdapter(command);
                            sa.Fill(Userdetails);
                            sa.Dispose();

                            var row = Userdetails.Rows[0];
                            UserDetail.Id = row["Id"].ToString();
                            UserDetail.FirstName = row["FirstName"].ToString();
                            UserDetail.LastName = row["LastName"].ToString();
                            UserDetail.MiddleName = row["MiddleName"].ToString();
                            UserDetail.DOB = row["DOB"].ToString();
                            UserDetail.PassportNo = row["PassportNo"].ToString();
                            UserDetail.PhotoUrl = row["PhotoUrl"].ToString();
                            UserDetail.PhoneNo = row["PhoneNo"].ToString();
                            UserDetail.Email = email;

                        }
                        catch (Exception ex)
                        {
                            // Handle exceptions (logging, rethrow, etc.)
                            Console.WriteLine(ex.Message);
                        }
                    }
                    //var Userdetail = _dbContext.Database.ExecuteSqlInterpolated(DbConstants.selectBasicUserDetail);
                }    
                return UserDetail;
            }
            catch (Exception ex)
            {
                // For general exceptions, return a generic error status code (e.g., 500)
                throw new ApplicationException(ex.Message);
            }
        }
    }
}
