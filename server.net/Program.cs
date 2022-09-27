using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Stickers.Bot;
using Stickers.Utils;
using Stickers.Search;
using Stickers.Service;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .AddJsonFile("appsettings.local.json", true);

builder.Configuration.AddEnvironmentVariables();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddSingleton<DapperContext>()
    .AddSingleton<OfficialStickersSearchHandler>()
    .AddSingleton<BlobService>()
    .AddSingleton<StickerDatabase>()
    .AddSingleton<SessionService>()
    .AddSingleton<StickerService>()
    .AddSingleton<SearchService>();
// Create the Bot Framework Authentication to be used with the Bot Adapter.
//builder.Services.AddSingleton<BotFrameworkAuthentication, ConfigurationBotFrameworkAuthentication>();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// Create the Bot Adapter with error handling enabled.
builder.Services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();
// Create the bot as a transient. In this case the ASP Controller is expecting an IBot.
builder.Services.AddTransient<IBot, TeamsMessagingExtensionsBot>();

// Adding Authentication  
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy =>
    {
        policy.RequireClaim("wids", new string[] { "62e90394-69f5-4237-9190-012177145e10", "69091246-20e8-4a56-aa4d-066075b2a7a8" });
        policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
        policy.AddRequirements(new AuthorizationRequirement("AdminOnly", JwtBearerDefaults.AuthenticationScheme));
    });
});
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration, ConfigKeys.AAD_SECTION);
builder.Services.AddSingleton<IAuthorizationHandler, AuthorizationHandler>();

builder.Services.AddMemoryCache();

// Add http services to the container.
builder.Services.AddHttpClient();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddCors();
}


var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
    app.UseCors(x => x
          .AllowAnyOrigin()
          .AllowAnyMethod()
          .AllowAnyHeader()); // configure CORS
}

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

// Eror Handler
app.UseMiddleware(typeof(GlobalErrorHandling));

// app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
