﻿using WildRoadApp_Api.IoC.Configuration.DI;
using WildRoadApp_Api.Tools.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace WildRoadApp_Api.API.Tests.Controllers
{
    [TestClass]
    public class TestBase
    {
        internal IConfigurationRoot _configurationRoot;
        internal ServiceCollection _services;

        public TestBase()
        {
            _configurationRoot = ConfigurationHelper.GetIConfigurationRoot(Directory.GetCurrentDirectory());

            _services = new ServiceCollection();

            //We load EXACTLY the same settings (DI and others) than API real solution, what is much better for tests.
            _services.ConfigureBusinessServices((IConfiguration)_configurationRoot);
        }
    }
}
