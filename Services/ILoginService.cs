using FarmOps.Models.LoginModels;

namespace FarmOps.Services
{
    public interface ILoginService
    {
        BasicUserDetailModel verifyLogin(string type, string email, string password);
    }
}
