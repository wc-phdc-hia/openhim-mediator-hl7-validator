﻿using AutoFixture;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using OpenHim.Mediator.Hl7Validator.Configuration;
using OpenHim.Mediator.Hl7Validator.Controllers;
using System.IO;
using System.Threading.Tasks;

namespace OpenHim.Mediator.Hl7Validator.Tests.Controllers
{
    [TestFixture]
    public class Hl7ValidationRequestsControllerTests
    {
        private Hl7ValidationRequestsController controllerUnderTest;
        private Fixture fixture;
        private Hl7Config hl7Config;

        [SetUp]
        public void SetUp()
        {
            fixture = new Fixture();
            hl7Config = fixture.Create<Hl7Config>();

            var hl7MessageData = @"MSH|^~\&|SENDING_APPLICATION|SENDING_FACILITY|RECEIVING_APPLICATION|RECEIVING_FACILITY|20110614075841||ACK|1407511|P|2.5.1||||||";
            var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(hl7MessageData));

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Body = stream;
            httpContext.Request.ContentLength = stream.Length;

            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
            };

            controllerUnderTest = new Hl7ValidationRequestsController(Options.Create(hl7Config)) { ControllerContext = controllerContext };
        }

        [Test]
        public async Task Post_WhenBodyEmpty_ReturnsBadRequest()
        {            
            //Arrange
            string emptyBodyData = string.Empty;
            var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(emptyBodyData));

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Body = stream;
            httpContext.Request.ContentLength = stream.Length;

            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
            };

            controllerUnderTest.ControllerContext = controllerContext;

            // Act
            var result = await controllerUnderTest.Post();

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            Assert.That((result as BadRequestObjectResult).Value, Is.EqualTo("Request body may not be null or empty"));
        }

        [Test, Ignore("How do I simulate a null body with the DefaultHttpContext that uses a memory stream for the body which does not allow null as input")]
        public async Task Post_WhenBodyNull_ReturnsBadRequest()
        {
            // Act
            var result = await controllerUnderTest.Post();

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            Assert.That((result as BadRequestObjectResult).Value, Is.EqualTo("Request body may not be null or empty"));
        }

        [Test]
        public async Task Post_ReturnsOkResult()
        {
            // Act & Assert
            Assert.That(await controllerUnderTest.Post(), Is.InstanceOf<OkObjectResult>());
        }
    }
}