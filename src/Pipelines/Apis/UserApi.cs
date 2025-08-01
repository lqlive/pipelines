using System;
using System.Security.Claims;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.HttpResults;

using Pipelines.Core.Entities.Users;
using Pipelines.Extensions;
using Pipelines.Models.Users;
using Pipelines.Services;

public static class UserApi
{
    public static RouteGroupBuilder MapUserApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/users");

        api.MapPost("/login", Login);
        api.MapPost("/logout", Logout);
        return api;
    }
    private static async Task<Results<Ok, ProblemHttpResult>> Login(
          HttpContext context,
          LoginRequest request,
          UserService userService,
          CancellationToken cancellationToken)
    {
        var ipAddress = context.GetClientIpAddress();
        var result = await userService.LoginAsync(request, ipAddress,cancellationToken);

        if (result.IsError)
        {
            return result.Errors.HandleErrors();
        }

        var userResponse = result.Value;

        await SignInUserAsync(context, userResponse);
        return TypedResults.Ok();
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
            new("provider", user.Provider.ToString()),
            new("role", "user")
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
        };

        await context.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);
    }
}