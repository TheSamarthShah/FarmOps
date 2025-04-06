using FarmOps.Models.LoginModels;

namespace FarmOps.Services
{
    public interface ILoginService
    {
        LoginModel verifyLogin(string type, string email, string password);
    }
}
