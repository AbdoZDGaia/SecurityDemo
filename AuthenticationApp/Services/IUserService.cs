using AuthenticationApp.Data;
using System.Security.Claims;

namespace AuthenticationApp.Services
{
    public interface IUserService
    {
        AppUser GetUserByExternalProvider(string provider, string nameIdentifier);
        AppUser GetUserById(int id);
        AppUser AddNewUser(string provider, List<Claim> claims);
        bool TryValidate(string username, string password, out List<Claim> claims);
    }
}