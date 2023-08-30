using API.Extensions;
using API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();
// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
     //app.UseDeveloperExceptionPage();
     //app.UseSwagger();
     //app.UseSwaggerUI();
// }
app.UseCors(builder => builder
     .AllowAnyHeader()
     .AllowAnyMethod()
     .WithOrigins("https://localhost:4200"));

//app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
