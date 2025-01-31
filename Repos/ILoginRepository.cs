using FarmOps.Models.LoginModels;

namespace FarmOps.Repos
{
    public interface ILoginRepository
    {
        BasicUserDetailModel GetUserForLogin(string type, string email, string password);
    }
}