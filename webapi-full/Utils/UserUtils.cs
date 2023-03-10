using System.Net.Mail;
using System.Security.Claims;
using System.Text;
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

    /// <summary>
    /// Validate email address using the <c>MailAddress</c> class.
    /// </summary>
    public void ValidateEmail(string value)
    {
        try {
            var addr = new MailAddress(value);

            //? Handle special cases
            if (!value.Any(c => c.Equals('@')) || value.Any(Char.IsWhiteSpace)
                || !value.Any(c => c.Equals('.')) || value.EndsWith('.'))
                throw new Exception();
        } catch (Exception) {
            throw new ArgumentException("Invalid email address.");
        }
    }

    /// <summary>
    /// Opinionated validation for username.
    /// <list type="bullet">
    /// <item>No whitespaces allowed</item>
    /// <item>Maximum length of 40 characters</item>
    /// <item>Minimum length of 6 characters</item>
    /// <item>Allowed only specific non-alphanumeric characters: _ and -</item>
    /// <item>Can only contain lowercase letters</item>
    /// </list>
    /// <br/>
    /// <paramref name="value"/>: The string to validate.
    /// </summary>
    public void ValidateUserName(string value)
    {
        StringBuilder errorMessage = new();
        errorMessage.Append("<ul class='username-validation'>");

        //? Not allow whitespace
        errorMessage.Append("<li class='");
        errorMessage.Append(value.Any(Char.IsWhiteSpace) ? "invalid" : "valid");
        errorMessage.Append("'>Username cannot contain whitespaces.</li>");

        //? Max length
        errorMessage.Append("<li class='");
        errorMessage.Append(value.Length > 40 ? "invalid" : "valid");
        errorMessage.Append("'>Username cannot exceed 40 characters.</li>");

        //? Min length
        errorMessage.Append("<li class='");
        errorMessage.Append(value.Length < 6 ? "invalid" : "valid");
        errorMessage.Append("'>Username must be at least 6 characters long.</li>");

        //? Allowed only specific non-alphanumeric [_-]
        errorMessage.Append("<li class='");
        errorMessage.Append(
            value.Any(c => !char.IsLetterOrDigit(c)
            && c != '_'
            && c != '-')
                ? "invalid"
                : "valid");
        errorMessage.Append("'>The only allowed special characters are the following: -, _</li>");

        //? Only allow lowercase
        errorMessage.Append("<li class='");
        errorMessage.Append(value.Any(Char.IsUpper) ? "invalid" : "valid");
        errorMessage.Append("'>Username must be lowercase.</li>");

        errorMessage.Append("</ul>");
        if (errorMessage.ToString().Contains("invalid"))
            throw new ArgumentException(errorMessage.ToString());
    }
}