using Microsoft.AspNetCore.Identity;
using Pipelines.Core.Entities.Users;
using Pipelines.Core.Management;
using Pipelines.Provider.GitHub;
using Pipelines.Services.Remotes;
using Pipelines.Session;
using Pipelines.Storage.PostgreSQL.Management;
using Microsoft.AspNetCore.Authentication.Cookies;
using Pipelines.Services;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

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

// Redis 配置
builder.Services.AddStackExchangeRedisCache(options => { options.Configuration = "localhost:6379"; });
builder.Services.AddSingleton<IConnectionMultiplexer>(provider =>
{
    return ConnectionMultiplexer.Connect("localhost:6379");
});

// 会话管理服务
builder.Services.AddScoped<ISessionManager<Pipelines.Session.ISession>, SessionManager>();
builder.Services.AddSingleton<ITicketStore, DistributedTicketStore>();
builder.Services.AddSingleton<IPostConfigureOptions<CookieAuthenticationOptions>, PostConfigureCookieTicketStore>();

// 其他服务
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<RemoteService>();
builder.Services.AddScoped<IdentityService>();
builder.Services.AddScoped<RepositoryService>();
builder.Services.AddScoped<BuildService>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthorization();
 
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie()
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
app.MapBuildApi();
app.MapRepositoryApiV1();

app.Run();