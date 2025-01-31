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

        public BasicUserDetailModel verifyLogin(string type, string email, string password)
        {
            BasicUserDetailModel user = _loginRepository.GetUserForLogin(type, email, password);
                return user;   
        }
    }
}
