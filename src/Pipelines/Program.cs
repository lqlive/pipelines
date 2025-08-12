using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Pipelines.Core.Entities.Users;
using Pipelines.Core.Management;
using Pipelines.Provider.GitHub;
using Pipelines.Services.Identity;
using Pipelines.Services.Remotes;
using Pipelines.Services.Users;
using Pipelines.Session;
using Pipelines.Storage.PostgreSQL.Management;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPipelinesCore()
    .AddPostgreSQLDatabase()
    .AddGitHub(options =>
    {
        var githubSection = builder.Configuration.GetSection("RemoteClient:GitHub");
        options.ClientId = githubSection["ClientId"]!;
        options.ClientSecret = githubSection["ClientSecret"]!;
        options.AuthorizationEndpoint = githubSection["AuthorizationEndpoint"]!;
        options.CallbackPath = githubSection["CallbackPath"]!;
        
        var scopes = githubSection.GetSection("Scope").Get<string[]>();
        if (scopes != null)
        {
            foreach (var scope in scopes)
            {
                options.Scopes.Add(scope);
            }
        }
    });

builder.Services.AddOpenApi();

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

builder.Services.AddStackExchangeRedisCache(options => { options.Configuration = "localhost:6379"; });
builder.Services.AddScoped<DistributedTicketStore>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<RemoteService>();
builder.Services.AddScoped<IdentityService>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie((options) =>
    {
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.Cookie.Name = "Pipelines.Session";

    })
    .AddMicrosoftAccount(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Microsoft:ClientId"]!;
        options.ClientSecret = builder.Configuration["Authentication:Microsoft:ClientSecret"]!;
        options.CallbackPath = "/auth/microsoft/callback";
    });

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(
                "https://localhost:3000", "http://localhost:3000",  // React/Next.js default
                "https://localhost:5173", "http://localhost:5173",  // Vite default
                "https://localhost:4173", "http://localhost:4173"   // Vite preview
              )
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapUserApiV1();
app.MapRemoteApiV1();

app.Run();