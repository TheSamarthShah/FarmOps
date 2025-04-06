using FarmOps.Models;
using FarmOps.Models.LoginModels;
using FarmOps.Repos;
using Speckle.Newtonsoft.Json;
using System.Net;

namespace FarmOps.Services
{
    public class LoginService : ILoginService
    {
  
        private readonly ILoginRepository _loginRepository;
        public LoginService( ILoginRepository loginRepository)
        {
            _loginRepository = loginRepository;
        }

        public LoginModel verifyLogin(string type, string email, string password)
        {
            LoginModel user = _loginRepository.GetUserForLogin(type, email, password);
            return user;   
        }
    }
}
