
using AuthenticationApp.Data;
using AuthenticationApp.Extensions;
using System.Security.Claims;

namespace AuthenticationApp.Services
{
    public class UserService : IUserService
    {
        private readonly AuthDBContext _context;

        public UserService(AuthDBContext context)
        {
            _context = context;
        }

        public AppUser AddNewUser(string provider, List<Claim> claims)
        {
            var appUser = new AppUser();
            appUser.Provider = provider;
            appUser.NameIdentifier = claims.GetClaim(ClaimTypes.NameIdentifier);
            appUser.UserName = claims.GetClaim("username");
            appUser.FirstName = claims.GetClaim(ClaimTypes.GivenName);
            appUser.LastName = claims.GetClaim(ClaimTypes.Surname);
            if (string.IsNullOrEmpty(appUser.FirstName))
            {
                appUser.FirstName = claims.GetClaim(ClaimTypes.Name);
            }
            appUser.Email = claims.GetClaim(ClaimTypes.Email);
            appUser.Mobile = claims.GetClaim(ClaimTypes.MobilePhone);
            var entity = _context.AppUsers.Add(appUser);
            _context.SaveChanges();
            return entity.Entity;
        }

        public AppUser GetUserByExternalProvider(string provider, string nameIdentifier)
        {
            var appUser = _context.AppUsers
                .Where(a => a.Provider == provider)
                .Where(a => a.NameIdentifier == nameIdentifier)
                .FirstOrDefault();

            if (appUser is not null)
            {
                return appUser;
            }

            return null;
        }

        public AppUser GetUserById(int id)
        {
            var appUser = _context.AppUsers.Find(id);
            return appUser;
        }

        public bool TryValidate(string username, string password, out List<Claim> claims)
        {
            claims = new List<Claim>();
            var appUser = _context.AppUsers
                .Where(a => a.UserName == username)
                .Where(a => a.Password == password)
                .FirstOrDefault();

            if (appUser is not null)
            {
                claims.Add(new Claim(ClaimTypes.NameIdentifier, appUser.NameIdentifier));
                claims.Add(new Claim("username", appUser.UserName));
                claims.Add(new Claim(ClaimTypes.GivenName, appUser.FirstName));
                claims.Add(new Claim(ClaimTypes.Surname, appUser.LastName));
                claims.Add(new Claim(ClaimTypes.Email, appUser.Email));
                claims.Add(new Claim(ClaimTypes.MobilePhone, appUser.Mobile));
                foreach (var role in appUser.RoleList)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
                return true;
            }

            return false;
        }
    }
}
