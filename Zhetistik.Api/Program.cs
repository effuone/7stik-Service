global using Microsoft.EntityFrameworkCore;
global using Zhetistik.Core.DataAccess;
global using Zhetistik.Core.Interfaces;
global using Zhetistik.Core.Repositories;
global using Microsoft.AspNetCore.Mvc;
global using Zhetistik.Data.Models;
global using Zhetistik.Data.Context;
global using Microsoft.Extensions.FileProviders;
global using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Zhetistik.Data.Mapping;
using AutoMapper;
using Zhetistik.Core.Helpers;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
var jwt = builder.Configuration.GetSection("Jwt:Key");
var connString = builder.Configuration.GetConnectionString("ZhetistikDb");
builder.Services.AddDbContext<ZhetistikAppContext>(options=>options.UseSqlServer(connString));
builder.Services.AddDefaultIdentity<ZhetistikUser>(options=>options.SignIn.RequireConfirmedAccount = true)
.AddEntityFrameworkStores<ZhetistikAppContext>();
builder.Services.AddScoped<DapperContext>();
builder.Services.AddScoped<ICityRepository, CityRepository>();
builder.Services.AddScoped<ICountryRepository, CountryRepository>();
builder.Services.AddScoped<ILocationRepository, LocationRepository>();
builder.Services.AddScoped<ISchoolRepository, SchoolRepository>();
// builder.Services.AddScoped<IUserService, UserService>();
// builder.Services.AddScoped<IUserRepository, UserRepository>();
var mapperConfig = new MapperConfiguration(mc=>mc.AddProfile(new UserProfile()));
IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddCors();
builder.Services.AddOptions();
builder.Services.AddMvc(options =>
{
   options.SuppressAsyncSuffixInActionNames = false;
});
var ekey = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]);
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
            .AddJwtBearer(x =>
            {
                x.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var userMachine = context.HttpContext.RequestServices.GetRequiredService<UserManager<ZhetistikUser>>();
                        var user = userMachine.GetUserAsync(context.HttpContext.User);
                        if (user == null)
                        {
                            context.Fail("Unauthorized");
                        }
                        return Task.CompletedTask;
                    }
                };
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
var app = builder.Build();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "SchoolImages")),
    RequestPath = "/SchoolImages"
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
