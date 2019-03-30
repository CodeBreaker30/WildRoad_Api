using AutoMapper;
using WildRoadApp_Api.API.Controllers;
using WildRoadApp_Api.API.DataContracts.Requests;
using WildRoadApp_Api.API.DataContracts;
using WildRoadApp_Api.Services.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WildRoadApp_Api.API.Tests.Controllers.ControllerTests
{
    [TestClass]
    public class UserControllerTests : TestBase
    {
        //NOTE: should be replaced by an interface
        UserController _controller;

        public UserControllerTests() : base()
        {
            var serviceProvider = _services.BuildServiceProvider();
            var businessService = serviceProvider.GetRequiredService<IUserService>();
            var mapper = serviceProvider.GetRequiredService<IMapper>();

            _controller = new UserController(businessService, mapper);
        }

        [TestMethod]
        public async Task CreateUser_Nominal_OK()
        {
            //Simple test
            var user = await _controller.CreateUser(new UserCreationRequest
            {
                User = new User { Id = "U1", Firstname = "Firstname 1", Lastname = "Lastname 1" },
                Date = DateTime.Now
            });

            Assert.IsNotNull(user);
        }


    }
}
