using Trackster.Api.Core.Events;
using Trackster.Api.Data;
using Microsoft.EntityFrameworkCore;
using Trackster.Api.Features.Media;
using WebSocketManager = Trackster.Api.Features.WebSockets.WebSocketManager;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DatabaseContext>();

builder.Services.AddControllers()
    .AddJsonOptions(opts => opts.JsonSerializerOptions.PropertyNamingPolicy = null);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();

var app = builder.Build();

Console.WriteLine("Applying migrations...");
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<DatabaseContext>();    
    context.Database.Migrate();
}
Console.WriteLine("Done");

Console.WriteLine("Registering EventBus...");
EventBus.Register(WebSocketManager.Instance());
EventBus.Register(WatchingNowService.Instance());
Console.WriteLine("Done");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Lifetime.ApplicationStarted.Register(EventBus.OnStarted);
app.Lifetime.ApplicationStopping.Register(EventBus.OnStopping);
app.Lifetime.ApplicationStopped.Register(EventBus.OnStopped);

//app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();
//app.MapStaticAssets();
app.UseAuthorization();
app.MapControllers();
app.UseWebSockets();

app.UseCors(setup => setup
    .SetIsOriginAllowed(_ => true)
    .AllowCredentials()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.Run();