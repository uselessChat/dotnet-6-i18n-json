using I18nJsonLibrary;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Localization;
using WebApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Cache
builder.Services.AddDistributedMemoryCache();
// Localization
builder.Services.AddLocalization();
builder.Services.AddSingleton<IStringLocalizerFactory, I18nJsonStringLocalizerFactory>();
builder.Services.AddSingleton<I18nJsonService>();
builder.Services.AddSingleton<I18nMiddleware>();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// Middlewares
app.UseMiddleware<I18nMiddleware>();
// Localization
var supportedCultures = app.Services.GetService<I18nJsonService>()!.AvailableCultures().ToList();
var requestLocalizationOptions = new RequestLocalizationOptions()
{
    // You must explicitly state which cultures your application supports.
    DefaultRequestCulture = new RequestCulture(supportedCultures.First()),
    // These are the cultures the app supports for formatting numbers, dates, etc.
    SupportedCultures = supportedCultures,
    // These are the cultures the app supports for UI strings (that we have localized resources for).
    SupportedUICultures = supportedCultures,
};
app.UseRequestLocalization(requestLocalizationOptions);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
