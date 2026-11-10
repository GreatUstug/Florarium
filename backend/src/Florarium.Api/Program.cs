using System.Text.Json;
using System.Text.Json.Serialization;
using Florarium.Data;
using Florarium.Data.Seed;
using Florarium.Api.Services;
using Florarium.Domain.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connStr = builder.Configuration.GetConnectionString("Default")
              ?? throw new InvalidOperationException("ConnectionStrings:Default is not configured");

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(connStr, npgsql => npgsql.MigrationsAssembly("Florarium.Data"))
       .UseSnakeCaseNamingConvention());

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters
        .Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)));

builder.Services.AddProblemDetails();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        c.IncludeXmlComments(xmlPath);

    c.CustomSchemaIds(type => type.FullName);
});

builder.Services.AddScoped<FlowersService>();
builder.Services.AddScoped<DictionariesService>();
builder.Services.AddScoped<CompositionsService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.WithOrigins("http://localhost:5173", "http://localhost:3000")
     .AllowAnyHeader()
     .AllowAnyMethod()));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.None;
        options.ExpireTimeSpan = TimeSpan.FromDays(14);
        options.SlidingExpiration = true;

        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };
        options.Events.OnRedirectToAccessDenied = context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var db = services.GetRequiredService<AppDbContext>();

    await db.Database.MigrateAsync();

    var hasher = services.GetRequiredService<IPasswordHasher<User>>();
    await DbSeeder.SeedAsync(db, hasher);
}

app.UseExceptionHandler();
app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/health", async (AppDbContext db, CancellationToken cancellationToken) =>
{
    var databaseAvailable = await db.Database.CanConnectAsync(cancellationToken);

    return databaseAvailable
        ? Results.Ok(new { status = "ok", database = "available" })
        : Results.Json(
            new { status = "error", database = "unavailable" },
            statusCode: StatusCodes.Status503ServiceUnavailable);
});

app.Run();