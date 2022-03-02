global using Microsoft.EntityFrameworkCore;
global using Zhetistik.Core.DataAccess;
global using Zhetistik.Core.Interfaces;
global using Zhetistik.Core.Repositories;
global using Microsoft.AspNetCore.Mvc;
global using Zhetistik.Data.Models;
global using Zhetistik.Data.Context;
global using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Zhetistik.Data.Mapping;
using AutoMapper;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
var jwt = builder.Configuration.GetSection("Jwt:Key");
var connString = builder.Configuration.GetConnectionString("LaptopConnection");
builder.Services.AddDbContext<ZhetistikAppContext>(options=>options.UseSqlServer(connString));
builder.Services.AddIdentity<ZhetistikUser, IdentityRole>(options=>options.SignIn.RequireConfirmedAccount = true)
.AddEntityFrameworkStores<ZhetistikAppContext>().AddDefaultTokenProviders();
builder.Services.AddScoped<DapperContext>();
builder.Services.AddScoped<ICityRepository, CityRepository>();
builder.Services.AddScoped<ICountryRepository, CountryRepository>();
builder.Services.AddScoped<ILocationRepository, LocationRepository>();
builder.Services.AddScoped<ISchoolRepository, SchoolRepository>();
// builder.Services.AddScoped<IUserService, UserService>();
// builder.Services.AddScoped<IUserRepository, UserRepository>();
var mapperConfig = new MapperConfiguration(mc=>mc.AddProfile(new UserProfile()));
IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton(mapper);
builder.Services.AddCors();
builder.Services.AddOptions();
builder.Services.AddMvc(options =>
{
   options.SuppressAsyncSuffixInActionNames = false;
});
var ekey = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]);
builder.Services.AddAuthentication(options =>  
            {  
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;  
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;  
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;  
            })  
  
            // Adding Jwt Bearer  
            .AddJwtBearer(options =>  
            {  
                options.SaveToken = true;  
                options.RequireHttpsMetadata = false;  
                options.TokenValidationParameters = new TokenValidationParameters()  
                {  
                    ValidateIssuer = true,  
                    ValidateAudience = true,  
                    ValidAudience = builder.Configuration["Jwt:ValidAudience"],  
                    ValidIssuer = builder.Configuration["Jwt:ValidIssuer"],  
                    IssuerSigningKey = new SymmetricSecurityKey(key)  
                };  
            });
           
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseDeveloperExceptionPage();
    app.UseSwaggerUI();
}
else
{
    app.UseHsts();
}
app.UseRouting();
app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
