global using Microsoft.EntityFrameworkCore;
global using Zhetistik.Core.DataAccess;
global using Zhetistik.Core.Interfaces;
global using Zhetistik.Core.Repositories;
global using Microsoft.AspNetCore.Mvc;
global using Zhetistik.Data.Models;
global using Zhetistik.Data.Context;
global using Microsoft.Extensions.FileProviders;
global using System.ComponentModel.DataAnnotations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var connString = builder.Configuration.GetConnectionString("LaptopConnection");
builder.Services.AddDbContext<ZhetistikAppContext>(options=>options.UseSqlServer(connString));
builder.Services.AddScoped<DapperContext>();
builder.Services.AddScoped<ICityRepository, CityRepository>();
builder.Services.AddScoped<ICountryRepository, CountryRepository>();
builder.Services.AddScoped<ILocationRepository, LocationRepository>();
builder.Services.AddScoped<ISchoolRepository, SchoolRepository>();
builder.Services.AddMvc(options =>
{
   options.SuppressAsyncSuffixInActionNames = false;
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
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
