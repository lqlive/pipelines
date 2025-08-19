using Microsoft.Extensions.Hosting;

AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
var builder = Host.CreateApplicationBuilder(args);

var app = builder.Build();
await app.RunAsync();


