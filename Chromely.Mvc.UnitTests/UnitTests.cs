using Chromely.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Reflection;

namespace Tests
{

    public class UnitTests
    {
        IServiceCollection serviceCollection;

        [SetUp]
        public void Setup()
        {
            serviceCollection = MvcConfigurationBuilder
                        .Create()
                        .AddChromelyMvcWithDefaultRoutes()
                        .UseControllersFromAssembly(Assembly.GetExecutingAssembly());
        }

        [Test]
        public void NonExisitantRouteReturns404()
        {
            var boundObject = serviceCollection.BuildServiceProvider().GetService<MvcCefSharpBoundObject>();

            var result = boundObject.GetJson("/nonExistantController/nonExistantUrl", null);

            var response = JsonConvert.DeserializeObject<Response>(result);
            Assert.AreEqual(404, response.Status);
        }


        [Test]
        public void NonExisitantActionReturns404()
        {
            var boundObject = serviceCollection.BuildServiceProvider().GetService<MvcCefSharpBoundObject>();

            var result = boundObject.GetJson("/test/nonExistantUrl", null);

            var response = JsonConvert.DeserializeObject<Response>(result);

            Assert.AreEqual(404, response.Status);
            Assert.Pass();
        }

        [Test]
        public void GetMethodWithQueryStringParameters()
        {
            var boundObject = serviceCollection.BuildServiceProvider().GetService<MvcCefSharpBoundObject>();

            var result = boundObject.GetJson("/test/getUrl?name=Rupert+Avery&age=21&birthdate=1982-06-12", null);

            var response =JsonConvert.DeserializeObject<Response>(result);
            Assert.AreEqual(200, response.Status);
        }

        [Test]
        public void GetMethodWithParameterObject()
        {
            var boundObject = serviceCollection.BuildServiceProvider().GetService<MvcCefSharpBoundObject>();
            var json = "{ name: 'Rupert Avery', age: 21, birthdate: '1982-06-12' }";

            var result = boundObject.GetJson("/test/parameterTest", json.ToExpandoObject());

            var response = JsonConvert.DeserializeObject<Response>(result);
            Assert.AreEqual(200, response.Status);
        }

        [Test]
        public void CustomAction()
        {
            var boundObject = serviceCollection.BuildServiceProvider().GetService<MvcCefSharpBoundObject>();
            var result = boundObject.GetJson("/test/custom-action", null);

            var response = JsonConvert.DeserializeObject<Response>(result);
            Assert.AreEqual(200, response.Status);
        }

        [Test]
        public void GetRequestWithNoActionDefaultsToIndex()
        {
            var boundObject = serviceCollection.BuildServiceProvider().GetService<MvcCefSharpBoundObject>();
            var result = boundObject.GetJson("/test", null);

            var response = JsonConvert.DeserializeObject<Response>(result);
            Assert.AreEqual(200, response.Status);
        }

        [Test]
        public void PostRequestWithNoActionDefaultsToPost()
        {
            var boundObject = serviceCollection.BuildServiceProvider().GetService<MvcCefSharpBoundObject>();
            var json = "{ name: 'Rupert Avery', age: 21, birthdate: '1982-06-12' }";

            var result = boundObject.PostJson("/test", null, json.ToExpandoObject());
            
            var response = JsonConvert.DeserializeObject<Response>(result);
            Assert.AreEqual(200, response.Status);
        }


        [Test]
        public void PostRequestWithJsonBody()
        {
            var boundObject = serviceCollection.BuildServiceProvider().GetService<MvcCefSharpBoundObject>();
            var json = "{ name: 'Rupert Avery', age: 21, birthdate: '1982-06-12' }";

            var result = boundObject.PostJson("/test/postTest", null, json.ToExpandoObject());

            var response = JsonConvert.DeserializeObject<Response>(result);
            Assert.AreEqual(200, response.Status);
        }

        [Test]
        public void PostRequestWithJsonArrayBody()
        {
            var boundObject = serviceCollection.BuildServiceProvider().GetService<MvcCefSharpBoundObject>();
            var json = "[{ name: 'Rupert Avery', age: 21, birthdate: '1982-06-12' }, { name: 'Jemma Avery', age: 18, birthdate: '1989-08-27' }]";

            var result = boundObject.PostJson("/test/postArrayTest", null, json.ToExpandoObjectList());

            var response = JsonConvert.DeserializeObject<Response>(result);
            Assert.AreEqual(200, response.Status);
        }

        [Test]
        public void PostRequestWithObjectGraph()
        {
            var boundObject = serviceCollection.BuildServiceProvider().GetService<MvcCefSharpBoundObject>();
            var json = @"
                {
                    activity: 'Swimming',
                    location: 'Swimming Pool',
                    date: 'Mar 25, 2019',
                    instructor: { name: 'Laarni Avery', age: 38, birthdate: '1975-12-02' },
                    participants: [{ name: 'Rupert Avery', age: 21, birthdate: '1982-06-12' }, { name: 'Jemma Avery', age: 18, birthdate: '1989-08-27' }]
                }";

            var result = boundObject.PostJson("/test/complexObjectTest", null, json.ToExpandoObject());

            var response = JsonConvert.DeserializeObject<Response>(result);
            Assert.AreEqual(200, response.Status);
        }
    }
}