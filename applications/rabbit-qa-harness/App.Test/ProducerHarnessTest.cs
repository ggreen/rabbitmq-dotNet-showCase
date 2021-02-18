using System;
using Imani.Solutions.Core.API.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace rabbit_qa_harness.App.Test
{
    /// <summary>
    /// Testing for ProducerHarness
    /// 
    /// author: Gregory Green
    /// </summary>
    [TestClass]
    public class ProducerHarnessTest
    {
        private Mock<ISettings> config;
        private string exceptMessage = "hello";
        private string contentType = "text/plain";

        [TestInitialize]
        public void InitializeProducerHarnessTest()
        {
            config = new Mock<ISettings>();
        }

        [TestMethod]
        public void GetMessage()
        {
            config.Setup(c => c.GetProperty("MESSAGE", "")).Returns(exceptMessage);
            var actual = ProducerHarness.GetMessage(config.Object, contentType);
            Assert.IsNotNull(actual);

        }

        [TestMethod]
        public void GetMessage_MessageSize()
        {
            config.Setup(c => c.GetProperty("MESSAGE", "")).Returns("");
            int expectedSize = 10;
            config.Setup(c => c.GetPropertyInteger("MESSAGE_SIZE", 0)).Returns(expectedSize);

            var actual = ProducerHarness.GetMessage(config.Object, contentType);
            Assert.IsNotNull(actual);
            Assert.AreEqual(actual.Length, expectedSize);
        }

        [TestMethod]
        public void GetMessage_JSON()
        {
            config.Setup(c => c.GetProperty("MESSAGE", "")).Returns("");
            int expectedSize = 30;
            config.Setup(c => c.GetPropertyInteger("MESSAGE_SIZE", 0)).Returns(expectedSize);
            config.Setup(c => c.GetProperty("CONTENT_TYPE", It.IsAny<string>())).Returns("application/json");


            var actual = ProducerHarness.GetMessage(config.Object, "application/json");
            Console.WriteLine($"actual: {actual}");
            Assert.IsNotNull(actual);
            Assert.AreEqual(actual.Length, expectedSize);

            Assert.IsTrue(actual.Contains("{"));
            Assert.IsTrue(actual.Contains("}"));
        }
    }
}