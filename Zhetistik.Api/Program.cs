global using Microsoft.EntityFrameworkCore;
global using Zhetistik.Core.DataAccess;
global using Zhetistik.Core.Interfaces;
global using Zhetistik.Core.Repositories;
global using Microsoft.AspNetCore.Mvc;
global using Zhetistik.Data.Models;
global using Zhetistik.Data.Context;
global using System.ComponentModel.DataAnnotations;
global using Zhetistik.Core.Services;
global using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using Zhetistik.Data.MailAccess;
using Zhetistik.Data.Repositories;
using Zhetistik.Api.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
var jwt = builder.Configuration.GetSection(nameof(Zhetistik.Api.JwtConfig)).Get<Zhetistik.Api.JwtConfig>().Key;
var audience = builder.Configuration.GetSection(nameof(Zhetistik.Api.JwtConfig)).Get<Zhetistik.Api.JwtConfig>().ValidAudience;
var issuer = builder.Configuration.GetSection(nameof(Zhetistik.Api.JwtConfig)).Get<Zhetistik.Api.JwtConfig>().ValidIssuer;
var mailSettings = builder.Configuration.GetSection(nameof(MailSettings)).Get<Zhetistik.Data.MailAccess.MailSettings>();


var connectionString = builder.Configuration.GetConnectionString("LaptopConnection");
builder.Services.AddDbContext<ZhetistikAppContext>(options=>options.UseSqlServer(connectionString));
builder.Services.AddIdentity<ZhetistikUser, IdentityRole>(options=>options.SignIn.RequireConfirmedAccount = true)
.AddEntityFrameworkStores<ZhetistikAppContext>().AddDefaultTokenProviders();


builder.Services.AddScoped<DapperContext>();
builder.Services.AddScoped<IMailSender, MailSender>();
builder.Services.AddScoped<ICityRepository, CityRepository>();
builder.Services.AddScoped<ICountryRepository, CountryRepository>();
builder.Services.AddScoped<ILocationRepository, LocationRepository>();
builder.Services.AddScoped<IAchievementRepository, AchievementRepository>();
builder.Services.AddScoped<ISchoolRepository, SchoolRepository>();
builder.Services.AddScoped<IFileRepository, FileRepository>();
builder.Services.AddScoped<IPortfolioRepository, PortfolioRepository>();
builder.Services.AddScoped<ICandidateRepository, CandidateRepository>();
builder.Services.AddScoped<IAchievementTypeRepository, AchievementTypeRepository>();


builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.AddTransient<IMailSender, MailSender>();


builder.Services.AddSwaggerGen();
builder.Services.AddCors(options=>options.AddPolicy("MyPolicies", builder=>{
    builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
}));
builder.Services.AddOptions();
builder.Services.AddMvc(options =>
{
   options.SuppressAsyncSuffixInActionNames = false;
});
var key = Encoding.ASCII.GetBytes(jwt);
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
                    ValidAudience = audience,  
                    ValidIssuer = issuer,  
                    IssuerSigningKey = new SymmetricSecurityKey(key)  
                };  
            });
           
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseDeveloperExceptionPage();
    app.UseSwaggerUI();
    app.UseHttpsRedirection();
}
else
{
    app.UseHsts();
}
app.UseRouting();
app.UseCors("MyPolicies");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
