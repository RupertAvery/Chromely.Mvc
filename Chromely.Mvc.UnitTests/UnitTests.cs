using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Chromely.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Reflection;
using System.Threading.Tasks;
using Chromely.Core.Network;
using Microsoft.Extensions.Logging;

namespace Tests
{

    public class UnitTests
    {
        private IServiceCollection _serviceCollection;
        private IServiceProvider _serviceProvider;

        [SetUp]
        public void Setup()
        {
            var routeCollection = new RouteCollection();
            var controllerFactory = new DefaultControllerFactory();
            controllerFactory.Add(typeof(TestController));

            routeCollection.MapRoute("default", "/{controller}/{action}");
            _serviceCollection = new ServiceCollection()
                    .AddSingleton<IChromelyRequestTaskRunner, DefaultMvcRequestTaskRunner>()
                    .AddSingleton<IActionBuilder, DefaultActionBuilder>()
                    .AddSingleton<IModelBinder, DefaultModelBinder>()
                    .AddSingleton<IControllerFactory>(controllerFactory)
                    .AddSingleton<RouteCollection>(routeCollection)
                    .AddSingleton<IRouteResolver, DefaultRouteResolver>()
                    .AddSingleton<IControllerActivator, DefaultControllerActivator>()
                    .AddTransient<TestController>()
                    .AddTransient<IDataService, DataService>()
                    .AddTransient<IServiceCollection>(provider => _serviceCollection)
                    .AddLogging(builder => { builder.AddDebug(); })
                ;

            _serviceProvider = _serviceCollection.BuildServiceProvider();
        }

        [Test]
        public void NonExisitantRouteReturns404()
        {
            var taskRunner = _serviceProvider.GetService<IChromelyRequestTaskRunner>();

            var result = taskRunner.Run("GET", "/nonExistantController/nonExistantUrl", null, null);

            Assert.AreEqual(404, result.Status);
        }


        [Test]
        public void NonExisitantActionReturns404()
        {
            var taskRunner = _serviceProvider.GetService<IChromelyRequestTaskRunner>();

            var result = taskRunner.Run("GET", "/test/nonExistantUrl", null, null);

            Assert.AreEqual(404, result.Status);
        }

        [Test]
        public void GetMethodWithQueryStringParameters()
        {
            var taskRunner = _serviceProvider.GetService<IChromelyRequestTaskRunner>();

            var result = taskRunner.Run("GET", "/test/getUrl?name=Rupert+Avery&age=21&birthdate=1982-06-12", null, null);

            Assert.AreEqual(200, result.Status);
        }

        [Test]
        public void GetMethodWithParameterObject()
        {
            var taskRunner = _serviceProvider.GetService<IChromelyRequestTaskRunner>();
            var json = new Dictionary<string, string>()
            {
                {"name", "Rupert Avery"},
                {"age", "21"},
                {"birthdate", "1982-06-12"},
            };

            var result = taskRunner.Run("GET", "/test/parameterTest", json.ToObjectDictionary(), null);

            Assert.AreEqual(200, result.Status);
        }

        [Test]
        public void CustomRoute()
        {
            var taskRunner = _serviceProvider.GetService<IChromelyRequestTaskRunner>();
            var result = taskRunner.Run("GET", "/test/custom-action", null, null);
            Assert.AreEqual(200, result.Status);
            Assert.AreEqual(result.Data, "Success!");
        }

        [Test]
        public void GetRequestWithNoActionDefaultsToIndex()
        {
            var taskRunner = _serviceProvider.GetService<IChromelyRequestTaskRunner>();
            var result = taskRunner.Run("GET", "/test", null, null);

            Assert.AreEqual(200, result.Status);
        }

        [Test]
        public void PostRequestWithNoActionDefaultsToPost()
        {
            var taskRunner = _serviceProvider.GetService<IChromelyRequestTaskRunner>();
            var json = @"{ ""name"": ""Rupert Avery"", ""age"": 21, ""birthdate"": ""1982-06-12"" }";

            var result = taskRunner.Run("POST", "/test", null, json.ToJsonElement());

            Assert.AreEqual(200, result.Status);
        }


        [Test]
        public void PostRequestWithJsonBody()
        {
            var taskRunner = _serviceProvider.GetService<IChromelyRequestTaskRunner>();
            var json = @"{ ""name"": ""Rupert Avery"", ""age"": 21, ""birthdate"": ""1982-06-12"" }";

            var result = taskRunner.Run("POST", "/test/postTest", null, json.ToJsonElement());

            Assert.AreEqual(200, result.Status);
        }

        [Test]
        public void PostRequestWithJsonArrayBody()
        {
            var taskRunner = _serviceProvider.GetService<IChromelyRequestTaskRunner>();
            var json = "[{ \"name\": \"Rupert Avery\", \"age\": 21, \"birthdate\": \"1982-06-12\" }, { \"name\": \"Jemma Avery\", \"age\": 18, \"birthdate\": \"1989-08-27\" }]";

            var result = taskRunner.Run("POST", "/test/postArrayTest", null, json.ToJsonElement());

            Assert.AreEqual(200, result.Status);
        }

        [Test]
        public void PostRequestWithObjectGraph()
        {
            var taskRunner = _serviceProvider.GetService<IChromelyRequestTaskRunner>();
            var json = @"
                {
                    ""activity"": ""Swimming"",
                    ""location"": ""Swimming Pool"",
                    ""date"": ""Mar 25, 2019"",
                    ""instructor"": { ""name"": ""Laarni Avery"", ""age"": 38, ""birthdate"": ""1975-12-02"" },
                    ""participants"": [{ ""name"": ""Rupert Avery"", ""age"": 21, ""birthdate"": ""1982-06-12"" }, { ""name"": ""Jemma Avery"", ""age"": 18, ""birthdate"": ""1989-08-27"" }]
                }";

            var result = taskRunner.Run("POST", "/test/complexObjectTest", null, json.ToJsonElement());

            Assert.AreEqual(200, result.Status);
        }

        [Test]
        public async Task GetValueFromAsyncMethod()
        {
            var taskRunner = _serviceProvider.GetService<IChromelyRequestTaskRunner>();

            var callback = new TestJavascriptCallback();

            var response = await taskRunner.RunAsync("GET", "/test/getPersonAsync", null, null);


            var person = (Person)response.Data;

            Assert.AreEqual("Rupert Avery", person.Name);
            Assert.AreEqual(21, person.Age);
            Assert.AreEqual(new DateTime(1982, 06, 12), person.BirthDate);
        }

        [Test]
        public async Task GetEnumerableFromAsyncMethod()
        {
            var taskRunner = _serviceProvider.GetService<IChromelyRequestTaskRunner>();

            var callback = new TestJavascriptCallback();

            var response = await taskRunner.RunAsync("GET", "/test/getPeopleAsync", null, null);

            var people = (IEnumerable<Person>)response.Data;

            Assert.AreEqual(2, people.Count());
        }

    }
}