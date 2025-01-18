using FarmOps.Models;
using FarmOps.Repos;
using Speckle.Newtonsoft.Json;
using System.Net;

namespace FarmOps.Services
{
    public class LoginService : ILoginService
    {
        private readonly DBContext _dbContext;
        private readonly ILoginRepository _loginRepository;
        public LoginService(DBContext dBContext, ILoginRepository loginRepository)
        {
            _dbContext = dBContext;
            _loginRepository = loginRepository;
        }

        public BasicUserDetailModel verifyLogin(string type, string email, string password)
        {
            BasicUserDetailModel user = _loginRepository.GetUserForLogin(type, email, password);
                return user;   
        }
    }
}
