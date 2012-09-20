using System;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Events
{
    public interface IPublisher
    {
        event EventHandler<MsgEventArgs> MessageSent;
        void Send(string message);
    }

    public class Publisher : IPublisher
    {
        public event EventHandler<MsgEventArgs> MessageSent;

        public void Send(string message)
        {
            if (MessageSent != null)
                MessageSent(this, new MsgEventArgs{Message = message});
        }
    }

    public class MsgEventArgs : EventArgs
    {
        public string Message;
    }

    public class PublisherTests
    {
        [Fact]
        public void FactMethodName()
        {
            string raisedMessage = null;
            var publisher = new Publisher();
            publisher.MessageSent += (obj, args) => raisedMessage = args.Message;

            publisher.Send("some message");

            raisedMessage.Should().Be("some message");
        }
    }

    public interface ISubscriber
    {
        void Subscribe(IPublisher publisher);
    }

    public class Subscriber : ISubscriber
    {
        public string ReceivedMessage;
        public string Name;

        public void Subscribe(IPublisher publisher)
        {
            publisher.MessageSent += (sender, args) => { ReceivedMessage = args.Message;
                                                           Console.Out.WriteLine("ReceivedMessage = {0}, subscriber {1}", ReceivedMessage, Name); };
        }
    }

    public class SubscriberTests
    {
        [Fact]
        public void should_subscribe_to_publisher_and_save_message()
        {
            var publisher = Substitute.For<IPublisher>();
            var subscriber = new Subscriber();

            subscriber.Subscribe(publisher);

            const string someMessage = "some message";
            publisher.MessageSent += Raise.EventWith(new MsgEventArgs {Message = someMessage});

            subscriber.ReceivedMessage.Should().Be(someMessage);
        }
    }

    public class EventsIntegrationTests
    {
        [Fact]
        public void check_evrthng()
        {
            var publisher = new Publisher();
            var subscriber1 = new Subscriber {Name = "boo1"};
            var subscriber2 = new Subscriber {Name = "boo2"};
            subscriber1.Subscribe(publisher);
            subscriber2.Subscribe(publisher);

            publisher.Send("foo");
            publisher.Send("f00000oo");
        }
    }
}
