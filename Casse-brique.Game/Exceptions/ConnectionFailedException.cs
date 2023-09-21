using Godot;
using System;

namespace Cassebrique.Exceptions
{
    /// <summary>
    /// Exception when connection to server failed
    /// </summary>
    public class ConnectionFailedException : Exception
    {
        /// <summary>
        /// Error code
        /// </summary>
        public Error Error { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="error"></param>
        public ConnectionFailedException(Error error)
        {
            Error = error;
        }

        public override string ToString()
        {
            return "Exception trying to connect with error code "+Error;
        }
    }
}
