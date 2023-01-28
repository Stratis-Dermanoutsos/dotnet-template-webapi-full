using System.Security.Claims;
using webapi_full.Extensions;
using webapi_full.IUtils;
using webapi_full.Models;

namespace webapi_full.Utils;

public class UserUtils : IUserUtils
{
    private readonly ApplicationDbContext context;

    public UserUtils(ApplicationDbContext context) => this.context = context;

    public int GetLoggedUserId(ClaimsPrincipal principal)
    {
        ClaimsIdentity? claimsIdentity = principal.Identity as ClaimsIdentity;

        return Convert.ToInt32(claimsIdentity?.FindFirst(ClaimTypes.Sid)?.Value);
    }

    public User GetLoggedUser(ClaimsPrincipal principal) => this.context.Users.GetAssured(this.GetLoggedUserId(principal));

    public User? GetByEmail(string email) => this.context.Users.SingleOrDefault(user => user.Email.Equals(email));

    public User? GetByUserName(string userName) => this.context.Users.SingleOrDefault(user => user.UserName.Equals(userName));
}