using FarmOps.Models.LoginModels;

namespace FarmOps.Repos
{
    public interface ILoginRepository
    {
        LoginModel GetUserForLogin(string type, string email, string password);
    }
}