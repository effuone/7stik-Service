// using Xunit;
// using Zhetistik.Core.Repositories;
// using Zhetistik.Core.Interfaces;
// using Moq;
// using Zhetistik.Data.Models;
// using Microsoft.Extensions.Logging;
// using Zhetistik.Api.Controllers;
// using System;
// using Microsoft.AspNetCore.Mvc;
// using System.Threading.Tasks;

// namespace Zhetistik.UnitTests;

// public class CitiesControllerTests
// {
//     // [Fact]
//     // public async Task GetCityAsync_WithUnexistingCity_ReturnsNotFound()
//     // {
//     //     //Arange
//     //     var repositoryStub = new Mock<ICityRepository>();
//     //     repositoryStub.Setup(repo=>repo.GetAsync(It.IsAny<int>())).ReturnsAsync((City) null);

//     //     var loggerStub = new Mock<ILogger<CityController>>();

//     //     var controller = new CityController(repositoryStub.Object, loggerStub.Object);
//     //     //Act
//     //     var rnd = new Random();
//     //     var result = await controller.GetCityAsync(rnd.Next(0, int.MaxValue));

//     //     //Assert
//     //     Assert.IsType<NotFoundObjectResult>(result.Result);
//     // }
// }