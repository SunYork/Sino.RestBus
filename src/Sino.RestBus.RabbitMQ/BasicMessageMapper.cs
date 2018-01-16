﻿using System;
using System.Linq;
using System.Net.Http;
using System.Collections.Generic;
using Sino.RestBus.Client;
using Sino.RestBus.Common.Amqp;

namespace Sino.RestBus.RabbitMQ
{
    public class BasicMessageMapper : IMessageMapper
    {
        protected readonly string[] amqpHostUris;
        protected readonly string serviceName;
        public virtual IList<AmqpConnectionInfo> ServerUris { get; protected set; }
        public virtual ExchangeKind SupportedExchangeKinds { get; protected set; }

        public BasicMessageMapper(string amqpHostUri, string serviceName)
        {
            if (String.IsNullOrWhiteSpace(amqpHostUri))
            {
                throw new ArgumentException("amqpHostUri");
            }

            if (String.IsNullOrWhiteSpace(serviceName))
            {
                throw new ArgumentException("serviceName");
            }

            this.amqpHostUris = new string[] { amqpHostUri };
            this.serviceName = serviceName;

            ServerUris = amqpHostUris.Select(u => new AmqpConnectionInfo { Uri = u, FriendlyName = StripUserInfoAndQuery(u) }).ToArray();
            SupportedExchangeKinds = ExchangeKind.Direct;
        }

        public virtual MessagingConfiguration MessagingConfig
        {
            get
            {
                return new MessagingConfiguration();
            }
        }

        public virtual string GetServiceName(HttpRequestMessage request)
        {
            return serviceName;
        }

        public virtual string GetRoutingKey(HttpRequestMessage request, ExchangeKind exchangeKind)
        {
            return null;
        }

        /// <summary>
        /// Gets the Headers for the message.
        /// </summary>
        /// <remarks>
        /// This is only useful for the headers exchange type.
        /// </remarks>
        public virtual IDictionary<string, object> GetHeaders(HttpRequestMessage request)
        {
            return null;
        }

        /// <summary>
        /// Returns the RequestOptions associated with a specified request.
        /// </summary>
        /// <remarks>
        /// This helper is useful for classes deriving from BasicMessageMapper.
        /// </remarks>
        public static RequestOptions GetRequestOptions(HttpRequestMessage request)
        {
            return MessageInvokerBase.GetRequestOptions(request);
        }

        /// <summary>
        ///  Removes the username, password and query components of an AMQP uri.
        /// </summary>
        public static string StripUserInfoAndQuery(string amqpUri)
        {
            if (amqpUri == null)
            {
                throw new ArgumentNullException("amqpUri");
            }

            amqpUri = amqpUri.Trim();

            int startIndex;
            if (amqpUri.Length > 8 && amqpUri.StartsWith("amqps://", StringComparison.OrdinalIgnoreCase))
            {
                startIndex = 8;
            }
            else if (amqpUri.Length > 7 && amqpUri.StartsWith("amqp://", StringComparison.OrdinalIgnoreCase))
            {
                startIndex = 7;
            }
            else
            {
                throw new ArgumentException("amqpUri is not in expected format.");
            }

            int endIndex = amqpUri.IndexOf('@');
            if (endIndex >= 0)
            {
                amqpUri = amqpUri.Remove(startIndex, (endIndex - startIndex) + 1);
            }

            int queryIndex = amqpUri.IndexOf('?');
            if (queryIndex >= 0)
            {
                amqpUri = amqpUri.Substring(0, queryIndex);
            }

            return amqpUri;
        }
    }

}
