using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using webapi_full.Attributes;
using webapi_full.Entities.Request;
using webapi_full.Exceptions;
using webapi_full.Extensions;
using webapi_full.IUtils;
using webapi_full.Models;

namespace webapi_full.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Logged]
public class UserController : ControllerBase
{
    private readonly ApplicationDbContext dbContext;
    private readonly IEncryptionUtils encryptionUtils;
    private readonly IConfiguration configuration;
    private readonly IUserUtils userUtils;

    public UserController(IConfiguration configuration, ApplicationDbContext dbContext, IEncryptionUtils encryptionUtils, IUserUtils userUtils)
    {
        this.configuration = configuration;
        this.dbContext = dbContext;
        this.encryptionUtils = encryptionUtils;
        this.userUtils = userUtils;
    }

    /// <summary>
    /// Get all users.
    /// </summary>
    [Authorize]
    [HttpGet]
    [Route("all")]
    public IActionResult GetAll() => Ok(this.dbContext.Users.GetAll());

    /// <summary>
    /// Create a new user account.
    /// <br/>
    /// <paramref name="entity" />: The user's information.
    /// </summary>
    [HttpPost]
    [Route("register")]
    public IActionResult Register([FromBody] UserToCreate entity)
    {
        //* Check if email exists
        if (userUtils.GetByEmail(entity.Email) is not null)
            throw new ConflictException($"Email {entity.Email} belongs to another user.");

        //* Get the User object
        User user = entity.ToUser();

        //* Encrypt the user's password
        user.Password = this.encryptionUtils.Encrypt(user.Password);

        //* Add the user to the database
        this.dbContext.Users.Add(user);
        this.dbContext.SaveChanges();

        return Ok(user.DateIn);
    }

    /// <summary>
    /// Create a new session for a registered user.
    /// <br/>
    /// <paramref name="credentials" />: The user's login credentials.
    /// </summary>
    [HttpPost]
    [Route("login")]
    public IActionResult Login([FromBody] UserCredentials credentials)
    {
        User? user = userUtils.GetByEmail(credentials.Email);

        if (user is null)
            throw new NotFoundException($"There is no user account associated with the email address '{credentials.Email}'.");

        if (!encryptionUtils.Check(credentials.Password, user.Password))
            throw new BadRequestException("Wrong email or password.");

        //* Create claims details based on the user information
        var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, configuration["Jwt:Subject"] ??
                    throw new ArgumentNullException("JWT subject is null")),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                new Claim(ClaimTypes.Sid, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email)
            };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ??
                    throw new ArgumentNullException("JWT key is null")));
        var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
        var token = new JwtSecurityToken(
            configuration["Jwt:Issuer"],
            configuration["Jwt:Audience"],
            claims,
            expires: DateTime.UtcNow.AddMinutes(25),
            signingCredentials: signIn);

        return Ok(new JwtSecurityTokenHandler().WriteToken(token));
    }
}