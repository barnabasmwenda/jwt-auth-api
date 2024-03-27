using Microsoft.EntityFrameworkCore;
using TodoApi.AuthenticationService.Data;
using TodoApi.AuthenticationService.Interfaces;
using TodoApi.AuthenticationService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TodoAuthenticationServiceDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddSingleton<JwtTokenService>();

builder.Services.AddSingleton<PasswordHashService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Use(async (context, next) =>
{
    context.Response.Headers.Add("Content-Security-Policy", "...");
    context.Response.Headers.Add("Strict-Transport-Security", "...");
    await next();
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
