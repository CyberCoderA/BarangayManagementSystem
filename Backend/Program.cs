using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

DotNetEnv.Env.Load();

var baseConnectionString = builder.Configuration.GetConnectionString("SupabaseConnection");

var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");

if (string.IsNullOrEmpty(baseConnectionString) || string.IsNullOrEmpty(dbPassword))
{
    throw new InvalidOperationException("Database connection string or password is missing.");
}

var connectionString = baseConnectionString.Replace("{DB_PASSWORD}", dbPassword);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();