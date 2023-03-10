using System.Security.Claims;
using webapi_full.Models;

namespace webapi_full.IUtils;

public interface IUserUtils
{
    /// <summary>
    /// Get the Id of the currently signed in user.
    /// <br />
    /// <returns>Returns the Id of the User object.</returns>
    /// </summary>
    int GetLoggedUserId(ClaimsPrincipal principal);

    /// <summary>
    /// Get the currently signed in user.
    /// <br/>
    /// <returns>
    /// Returns the User object.
    /// If the User is not found, null is returned instead.
    /// </returns>
    /// </summary>
    User GetLoggedUser(ClaimsPrincipal principal);

    /// <summary>
    /// Get a user by email.
    /// <br/>
    /// <paramref name="email" />: The email of the user.
    /// <br/>
    /// <returns>
    /// Returns the User object.
    /// If the User is not found, null is returned instead.
    /// </returns>
    /// </summary>
    User? GetByEmail(string email);

    /// <summary>
    /// Get a user by userName.
    /// <br/>
    /// <paramref name="userName" />: The userName of the user.
    /// <br/>
    /// <returns>
    /// Returns the User object.
    /// If the User is not found, null is returned instead.
    /// </returns>
    /// </summary>
    User? GetByUserName(string userName);

    /// <summary>
    /// Validate an email address.
    /// <br/>
    /// <paramref name="value" />: The email to validate.
    /// <br/>
    /// Throws if the email is invalid.
    /// </summary>
    void ValidateEmail(string value);

    /// <summary>
    /// Validate username.
    /// <br/>
    /// <paramref name="value" />: The username to validate.
    /// <br/>
    /// Throws if the username is invalid.
    /// </summary>
    void ValidateUserName(string value);
}