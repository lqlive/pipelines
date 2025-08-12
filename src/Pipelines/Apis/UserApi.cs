using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.HttpResults;
using Pipelines.Extensions;
using Pipelines.Models.Users;
using Pipelines.Services.Users;

public static class UserApi
{
    public static RouteGroupBuilder MapUserApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/users");

        api.MapGet("/login/challenge", Challenge);
        api.MapGet("/login/with", LoginWith);
        api.MapGet("/me", GetCurrentUser);
        api.MapPost("/login", Login);
        api.MapPost("/logout", Logout);
        api.MapPost("/register", Register);

        return api;
    }

    private static async Task<IResult> LoginWith(
        HttpContext context,
        UserService userService,
        string redirectUri,
        CancellationToken cancellationToken)
    {
        // Check if user is authenticated
        if (!context.User.Identity?.IsAuthenticated ?? true)
        {
            return TypedResults.Problem(
                statusCode: 401,
                title: "Unauthorized",
                detail: "User is not authenticated");
        }

        var email = context.User.FindFirst(ClaimTypes.Email)?.Value;
        var name = context.User.FindFirst(ClaimTypes.Name)?.Value
                   ?? context.User.FindFirst(ClaimTypes.GivenName)?.Value
                   ?? context.User.Identity?.Name;
        var avatar = context.User.FindFirst("picture")?.Value
                     ?? context.User.FindFirst("avatar_url")?.Value;


        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(name))
        {
            return TypedResults.Problem(
                statusCode: 400,
                title: "Incomplete User Information",
                detail: "Unable to obtain required user information (email and name) from external provider");
        }

        var ipAddress = context.GetClientIpAddress();
        var request = new LoginWithRequest
        {
            Email = email,
            UserName = name,
            Avatar = avatar
        };

        var result = await userService.LoginWithAsync(request, ipAddress, cancellationToken);

        if (result.IsError)
        {
            return result.Errors.HandleErrors();
        }

        await SignInUserAsync(context, result.Value);

        return Results.Redirect(redirectUri);
    }

    private static IResult Challenge(string provider, string redirectUri)
    {
        return Results.Challenge(
           new AuthenticationProperties { RedirectUri = $"/api/users/login/with?redirectUri={redirectUri}" },
            [provider]
         );
    }

    private static async Task<Results<Ok, ProblemHttpResult>> Register(
        RegisterRequest request,
        UserService userService,
        CancellationToken cancellationToken)
    {
        var result = await userService.RegisterAsync(request, cancellationToken);

        if (result.IsError)
        {
            return result.Errors.HandleErrors();
        }

        return TypedResults.Ok();
    }
    private static async Task<Results<Ok, ProblemHttpResult>> Login(
          HttpContext context,
          LoginRequest request,
          UserService userService,
          CancellationToken cancellationToken)
    {
        var ipAddress = context.GetClientIpAddress();
        var result = await userService.LoginAsync(request, ipAddress, cancellationToken);

        if (result.IsError)
        {
            return result.Errors.HandleErrors();
        }

        var userResponse = result.Value;

        await SignInUserAsync(context, userResponse);
        return TypedResults.Ok();
    }

    private static async Task<Results<Ok<UserResponse>, ProblemHttpResult>> GetCurrentUser(
        HttpContext context,
        UserService userService,
        CancellationToken cancellationToken)
    {
        // Check if user is authenticated
        if (!context.User.Identity?.IsAuthenticated ?? true)
        {
            return TypedResults.Problem(
                statusCode: 401,
                title: "Unauthorized",
                detail: "User is not authenticated");
        }

        // Get user ID from claims
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return TypedResults.Problem(
                statusCode: 401,
                title: "Invalid User",
                detail: "User ID not found in claims");
        }

        // Get user from database
        var result = await userService.GetByIdAsync(userId, cancellationToken);
        if (result.IsError)
        {
            return result.Errors.HandleErrors();
        }

        return TypedResults.Ok(result.Value);
    }

    private static async Task<Results<Ok, ProblemHttpResult>> Logout(HttpContext context, CancellationToken cancellationToken)
    {
        await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return TypedResults.Ok();
    }

    private static async Task SignInUserAsync(HttpContext context, UserResponse user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Name),
            new(ClaimTypes.Email, user.Email?? string.Empty),
            new(ClaimTypes.Role,"user"),
            new("role", "user"),
            new("sub", user.Id.ToString()),
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var properties = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
        };

        await context.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            properties);
    }
}