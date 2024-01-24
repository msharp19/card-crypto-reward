using AspNetCoreRateLimit;
using Hangfire;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Logging;
using Swashbuckle.AspNetCore.SwaggerUI;
using CryptoCreditCardRewards.API.Extensions;
using CryptoCreditCardRewards.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// TODO: remove (debug only)
//IdentityModelEventSource.ShowPII = true;

builder.ConfigureSettings();
builder.ConfigureCors();
builder.ConfigureControllers();
builder.ConfigureMVC();
builder.ConfigureWebServer();
builder.ConfigureRateLimiting();
builder.ConfigureHttpClients();
builder.ConfigureOptions();
builder.ConfigureServices();
builder.ConfigureDbContexts();
builder.ConfigureAuthentication();
builder.ConfigureHangfire();
builder.ConfigureAutomapper();
builder.ConfigureSwagger();
builder.ConfigureHostedServices();

builder.Services.LoadErrorMappings();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddResponseCompression();
builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();
builder.Services.AddLogging();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint(builder.Configuration.GetValue<string>("Swagger:EndPoint"), builder.Configuration.GetValue<string>("Swagger:Title"));
        c.RoutePrefix = string.Empty; // Set at root
        c.InjectStylesheet("/assets/css/custom-swagger-ui.css");
        c.DocExpansion(DocExpansion.None);
        c.ConfigObject.AdditionalItems.Add("syntaxHighlight", false); //Turns off syntax highlight which causing performance issues...
        c.ConfigObject.AdditionalItems.Add("theme", "agate"); //Reverts Swagger UI 2.x  theme which is simpler not much performance benefit...
    });
    app.UseDeveloperExceptionPage();
}

// Global cors policy - handle this in Azure
app.UseCors(x => x.AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(origin => true) // allow any origin
    .AllowCredentials()); // allow credentials

// Turn on rate limiting
app.UseIpRateLimiting();

app.UseHangfireDashboard();

// Use endpoint routing
app.UseRouting();

// For custom swagger css
app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(
                Path.Combine(Directory.GetCurrentDirectory(), @"Assets/CSS")),
    RequestPath = new PathString("/assets/css")
});

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ErrorHandlingMiddleware>();

// Setup endpoints
app.UseEndpoints(endpoints =>
{
    // Only add endpoint if required
    if (builder.Configuration.GetValue<bool>("EndpointOptions:Enabled"))
        endpoints.MapControllers();

    // Map hangfire
    if (app.Environment.IsDevelopment())
        endpoints.MapHangfireDashboard("/hangfire");
    else endpoints.MapHangfireDashboard("/hangfire").RequireAuthorization();
});

app.Run();