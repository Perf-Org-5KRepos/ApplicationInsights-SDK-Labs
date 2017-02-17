﻿using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Wcf.Implementation;
using Microsoft.ApplicationInsights.Wcf.Tests.Channels;
using Microsoft.ApplicationInsights.Wcf.Tests.Integration;
using Microsoft.ApplicationInsights.Wcf.Tests.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.ServiceModel.Channels;
using System.Threading;
using System.Xml;

namespace Microsoft.ApplicationInsights.Wcf.Tests
{
    [TestClass]
    public class ClientTelemetryDuplexChannelTests : ChannelTestBase<IDuplexChannel>
    {
        const String TwoWayOp1 = "http://tempuri.org/ISimpleService/GetSimpleData";
        const String TwoWayOp2 = "http://tempuri.org/ISimpleService/CallFailsWithFault";
        const String OneWayOp1 = "http://tempuri.org/IOneWayService/SuccessfullOneWayCall";

        internal override IDuplexChannel GetChannel(IChannel innerChannel, Type contract)
        {
            return GetChannel(
                new ClientChannelManager(new TelemetryClient(), contract, BuildOperationMap()),
                innerChannel
                );
        }
        internal override IDuplexChannel GetChannel(IChannelManager manager, IChannel innerChannel)
        {
            return new ClientTelemetryDuplexChannel(manager, innerChannel);
        }

        [TestMethod]
        [TestCategory("Client")]
        public void Send_WhenMessageIsNull_ThrowsException()
        {
            TestTelemetryChannel.Clear();
            var innerChannel = new MockClientChannel(SvcUrl);
            var channel = GetChannel(innerChannel, typeof(ISimpleService));

            bool failed = false;
            try
            {
                channel.Send(null);
            } catch ( ArgumentNullException )
            {
                failed = true;
            }
            Assert.IsTrue(failed, "Send did not throw an exception");
        }
        [TestMethod]
        [TestCategory("Client")]
        public void BeginSend_WhenMessageIsNull_ThrowsException()
        {
            TestTelemetryChannel.Clear();
            var innerChannel = new MockClientChannel(SvcUrl);
            var channel = GetChannel(innerChannel, typeof(ISimpleService));

            bool failed = false;
            try
            {
                channel.BeginSend(null, null, null);
            } catch ( ArgumentNullException )
            {
                failed = true;
            }
            Assert.IsTrue(failed, "BeginSend did not throw an exception");
        }

        [TestMethod]
        [TestCategory("Client")]
        public void Send_OneWay_WritesTelemetry()
        {
            TestTelemetryChannel.Clear();
            var innerChannel = new MockClientChannel(SvcUrl);
            var channel = GetChannel(innerChannel, typeof(ISimpleService));

            var request = BuildMessage(OneWayOp1);
            request.Headers.MessageId = new UniqueId();
            channel.Send(request);

            var telemetry = TestTelemetryChannel.CollectedData()
                                                .OfType<DependencyTelemetry>()
                                                .FirstOrDefault();
            Assert.IsNotNull(telemetry, "No telemetry event written");
        }

        [TestMethod]
        [TestCategory("Client")]
        public void SendFollowedByMatchingReceive_WritesTelemetry()
        {
            TestTelemetryChannel.Clear();
            var innerChannel = new MockClientChannel(SvcUrl);
            var channel = GetChannel(innerChannel, typeof(ISimpleService));

            var request = BuildMessage(TwoWayOp1);
            request.Headers.MessageId = new UniqueId();
            var expectedReply = BuildMessage(TwoWayOp1);
            expectedReply.Headers.RelatesTo = request.Headers.MessageId;
            innerChannel.MessageToReceive = expectedReply;

            channel.Send(request);
            var reply = channel.Receive(TimeSpan.FromSeconds(10));

            var telemetry = TestTelemetryChannel.CollectedData()
                                                .OfType<DependencyTelemetry>()
                                                .FirstOrDefault();
            Assert.IsNotNull(telemetry, "No telemetry event written");
        }

        [TestMethod]
        [TestCategory("Client")]
        public void AsyncSendAndReceive_WritesTelemetry()
        {
            TestTelemetryChannel.Clear();
            var innerChannel = new MockClientChannel(SvcUrl);
            var channel = GetChannel(innerChannel, typeof(ISimpleService));

            var request = BuildMessage(TwoWayOp1);
            request.Headers.MessageId = new UniqueId();
            var expectedReply = BuildMessage(TwoWayOp1);
            expectedReply.Headers.RelatesTo = request.Headers.MessageId;
            innerChannel.MessageToReceive = expectedReply;

            var result = channel.BeginSend(request, null, null);
            channel.EndSend(result);
            result = channel.BeginReceive(null, null);
            var reply = channel.EndReceive(result);

            var telemetry = TestTelemetryChannel.CollectedData()
                                                .OfType<DependencyTelemetry>()
                                                .FirstOrDefault();
            Assert.IsNotNull(telemetry, "No telemetry event written");
        }

        [TestMethod]
        [TestCategory("Client")]
        public void AsyncSendAndTryReceive_WritesTelemetry()
        {
            TestTelemetryChannel.Clear();
            var innerChannel = new MockClientChannel(SvcUrl);
            var channel = GetChannel(innerChannel, typeof(ISimpleService));

            var request = BuildMessage(TwoWayOp1);
            request.Headers.MessageId = new UniqueId();
            var expectedReply = BuildMessage(TwoWayOp1);
            expectedReply.Headers.RelatesTo = request.Headers.MessageId;
            innerChannel.MessageToReceive = expectedReply;

            var result = channel.BeginSend(request, null, null);
            channel.EndSend(result);
            result = channel.BeginTryReceive(TimeSpan.FromMilliseconds(10), null, null);
            Message reply;
            Assert.IsTrue(channel.EndTryReceive(result, out reply), "EndTryReceive failed");

            var telemetry = TestTelemetryChannel.CollectedData()
                                                .OfType<DependencyTelemetry>()
                                                .FirstOrDefault();
            Assert.IsNotNull(telemetry, "No telemetry event written");
        }

        [TestMethod]
        [TestCategory("Client")]
        public void SendFollowedWithReceiveTimeout_WritesTelemetry()
        {
            TestTelemetryChannel.Clear();
            var innerChannel = new MockClientChannel(SvcUrl);
            var channel = GetChannel(innerChannel, typeof(ISimpleService));

            var request = BuildMessage(TwoWayOp1);
            request.Headers.MessageId = new UniqueId();
            innerChannel.MessageToReceive = null;

            channel.Send(request, TimeSpan.FromMilliseconds(50));
            bool failed = false;
            try
            {
                channel.Receive();
            } catch ( TimeoutException )
            {
                failed = true;
            }

            Assert.IsTrue(failed, "Receive did not fail with TimeoutException");
            // there's potentially some additional delay between our timeout firing internally,
            // and the callback firing so that telemetry can be written
            Thread.Sleep(200);

            var telemetry = TestTelemetryChannel.CollectedData()
                                                .OfType<DependencyTelemetry>()
                                                .FirstOrDefault();
            Assert.IsNotNull(telemetry, "No telemetry event written");
            Assert.IsFalse(telemetry.Success.Value, "Dependency call succeeded");
        }

        private Message BuildMessage(String action)
        {
            return Message.CreateMessage(MessageVersion.Default, action, "<text/>");
        }

        internal override ClientOperationMap BuildOperationMap()
        {
            ClientOpDescription[] ops = new ClientOpDescription[]
            {
                new ClientOpDescription { Action = TwoWayOp1, IsOneWay = false, Name = "GetSimpleData" },
                new ClientOpDescription { Action = TwoWayOp2, IsOneWay = false, Name = "CallFailsWithFault" },
                new ClientOpDescription { Action = OneWayOp1, IsOneWay = true, Name = "SuccessfullOneWayCall" },
            };
            return new ClientOperationMap(ops);
        }
    }
}
