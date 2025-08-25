using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;

namespace Pipelines.Session;

public class PostConfigureCookieTicketStore(ITicketStore ticketStore) :
    IPostConfigureOptions<CookieAuthenticationOptions>
{
    public void PostConfigure(string? name, CookieAuthenticationOptions options)
    {
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.Cookie.Name = "Pipelines.Session";
        options.SessionStore = ticketStore;
    }
}