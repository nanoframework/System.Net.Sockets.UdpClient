using nanoFramework.TestFramework;
using System;
using System.Net.Sockets;

namespace UnitTestUdpClient
{
    [TestClass]
    public class TestConstructors
    {
        [TestMethod]
        public void TestDefault()
        {
            UdpClient client = new UdpClient(5000);

            client.Dispose();

            Assert.Throws(typeof(ObjectDisposedException),() => { var c = client.Client; });
        }
    }
}
