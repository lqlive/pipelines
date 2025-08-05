using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Pipelines.Core.Entities.Users;
using Pipelines.Core.Management;
using Pipelines.Services.Users;
using Pipelines.Session;
using Pipelines.Storage.PostgreSQL.Management;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPipelinesCore()
    .AddPostgreSQLDatabase();

builder.Services.AddOpenApi();

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

builder.Services.AddStackExchangeRedisCache(options => { options.Configuration = "localhost:6379"; });
builder.Services.AddScoped<DistributedTicketStore>();
builder.Services.AddScoped<UserService>();

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

app.Run();