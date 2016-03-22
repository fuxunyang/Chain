﻿using System;
using System.Data;
#if !WINDOWS_UWP
using System.Runtime.Serialization;
#endif

namespace Tortuga.Chain
{
    /// <summary>
    /// This exception occurs when there is a mismatch between the database schema and the object model.
    /// </summary>
    /// <seealso cref="DataException" />
#if !WINDOWS_UWP
    [Serializable]
#endif
    public class MappingException : DataException
    {
#if !WINDOWS_UWP
        /// <summary>
        /// Initializes a new instance of the <see cref="MappingException"/> class.
        /// </summary>
        /// <param name="info">The data necessary to serialize or deserialize an object.</param>
        /// <param name="context">Description of the source and destination of the specified serialized stream.</param>
        protected MappingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="MappingException"/> class.
        /// </summary>
        public MappingException()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MappingException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public MappingException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MappingException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the <paramref name="innerException" /> parameter is not a null reference (Nothing in Visual Basic), the current exception is raised in a catch block that handles the inner exception.</param>
        public MappingException(string message, Exception innerException)
            : base(message, innerException)
        {
            
        }

    }
}
