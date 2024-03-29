﻿using System;
using System.Net;
using System.Runtime.Serialization;

namespace Mail2Gotify.Exceptions
{
    /// <summary>
    /// An exception to be used when there is an issue with downstream communication.
    /// </summary>
    [Serializable]
    public class CommunicationException : Exception
    {
        /// <summary>
        /// Provides the communication request.
        /// </summary>
        public string Request { get; set; }

        /// <summary>
        /// Provides the status code of the communication exception.
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// Provides the reason phrase of the communication exception.
        /// </summary>
        public string ReasonPhrase { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public CommunicationException()
        {
        }

        /// <summary>
        /// Default constructor with message.
        /// </summary>
        /// <param name="message">The message to set in the exception.</param>
        public CommunicationException(string message) : base(message)
        {
        }

        /// <summary>
        /// Default constructor with message and inner exception.
        /// </summary>
        /// <param name="message">The message to set in the exception.</param>
        /// <param name="innerException">The inner exception to set in the exception.</param>
        public CommunicationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
