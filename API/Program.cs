using API.Data;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using API.Middleware;
using API.Services;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<DataContext>(opt =>
{
     opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddCors();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPhotoService, PhotoService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));
builder.Services.AddIdentityServices(builder.Configuration);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();
// Configure the HTTP request pipeline.
 if (app.Environment.IsDevelopment())
 {
     //app.UseDeveloperExceptionPage();
     app.UseSwagger();
     app.UseSwaggerUI();
 }
app.UseCors(builder => builder
     .AllowAnyHeader()
     .AllowAnyMethod()
     .WithOrigins("https://localhost:4200"));
 
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

using var scope    = app.Services.CreateScope();
var       services = scope.ServiceProvider;
try
{
     var context = services.GetRequiredService<DataContext>();
     await context.Database.MigrateAsync();
     await Seed.SeedUsers(context);
}
catch (Exception e)
{
     var logger = services.GetService<ILogger<Program>>();
     logger.LogError(e, "An error occured during migrations");
}

app.Run();
