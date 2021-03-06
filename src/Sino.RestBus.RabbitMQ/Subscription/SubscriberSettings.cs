﻿using System;

namespace Sino.RestBus.RabbitMQ.Subscription
{
    public class SubscriberSettings
    {
        RestBusSubscriber _subscriber;
        SubscriberAckBehavior _ackBehavior;
        int _prefetchCount;

        public SubscriberSettings()
        {
            PrefetchCount = AmqpUtils.DEFAULT_PREFETCH_COUNT;
        }

        public SubscriberAckBehavior AckBehavior
        {
            get
            {
                return _ackBehavior;
            }
            set
            {
                EnsureNotStarted();
                _ackBehavior = value;
            }
        }

        public int PrefetchCount
        {
            get { return _prefetchCount; }
            set
            {
                EnsureNotStarted();
                if (value < 0 || value > ushort.MaxValue) throw new ArgumentException("PrefetchCount must be between 0 and 65535.");
                _prefetchCount = value;
            }
        }


        internal RestBusSubscriber Subscriber
        {
            set
            {
                if (_subscriber != null) throw new InvalidOperationException("This instance of SubscriberSettings is already in use by another subscriber.");
                _subscriber = value;
            }
        }

        private void EnsureNotStarted()
        {
            if (_subscriber != null && _subscriber.HasStarted)
            {
                throw new InvalidOperationException("This instance has already started. Properties can only be modified before starting the subscriber.");
            }
        }

        //TODO: Add other settings
        //Throttling option -- Lets subscriber throttle how many messages can be processed at once (PrefetchCount kind of takes care of this)
        //RequestedHeartBeat (use in coordination with RequestedHeartBeat query string in connection URI)

        //TODO: Discover from IMessageMapper
        //AMQP Headers for Headers exchange subscription.
        //AMQP Topic for Topic Exchange subscription.
    }
}
