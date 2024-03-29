﻿using System;
using System.Runtime.Serialization;

namespace Mail2Gotify.Exceptions
{
    /// <summary>
    /// Exception to be used when an entity could not be found during communication.
    /// </summary>
    [Serializable]
    public class NotFoundException : CommunicationException
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public NotFoundException()
        {
        }

        /// <summary>
        /// Constructor with exception message.
        /// </summary>
        public NotFoundException(string message) : base(message)
        {
        }

        /// <summary>
        /// Constructor with exception message and inner exception.
        /// </summary>
        public NotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}