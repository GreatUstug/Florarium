using System.Text.Json;
using System.Text.Json.Serialization;
using Florarium.Data;
using Florarium.Data.Seed;
using Microsoft.EntityFrameworkCore;
using Florarium.Api.Services;

var builder = WebApplication.CreateBuilder(args);

var connStr = builder.Configuration.GetConnectionString("Default")
              ?? throw new InvalidOperationException("ConnectionStrings:Default is not configured");

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(connStr, npgsql =>
            npgsql.MigrationsAssembly("Florarium.Data"))
        .UseSnakeCaseNamingConvention());

builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.Converters
            .Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
    });

builder.Services.AddProblemDetails();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (System.IO.File.Exists(xmlPath))
        c.IncludeXmlComments(xmlPath);
});

builder.Services.AddScoped<FlowersService>();

builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.WithOrigins("http://localhost:5173", "http://localhost:3000")
        .AllowAnyHeader()
        .AllowAnyMethod()));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
    await DbSeeder.SeedAsync(db);
}

app.UseExceptionHandler();
app.UseStatusCodePages();

app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

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
