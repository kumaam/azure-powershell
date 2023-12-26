﻿using System.Net;

namespace Microsoft.Azure.Commands.Network.NetworkWatcher.LAToAMAConverter.CMResource.Extensions
{
    /// <summary>
    /// The http utility.
    /// </summary>
    public static class HttpUtility
    {
        /// <summary>
        /// Returns true if the status code corresponds to a successful request.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        public static bool IsSuccessfulRequest(this HttpStatusCode statusCode)
        {
            return IsSuccessfulRequest((int)statusCode);
        }

        /// <summary>
        /// Returns true if the status code corresponds to a server failure request.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        public static bool IsServerFailureRequest(this HttpStatusCode statusCode)
        {
            return IsServerFailureRequest((int)statusCode);
        }

        /// <summary>
        /// Returns true if the status code corresponds to client failure.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        public static bool IsClientFailureRequest(this HttpStatusCode statusCode)
        {
            return IsClientFailureRequest((int)statusCode);
        }

        /// <summary>
        /// Returns true if the status code corresponds to a successful request.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        private static bool IsSuccessfulRequest(int statusCode)
        {
            return statusCode >= 200 && statusCode <= 299 || statusCode == 304;
        }

        /// <summary>
        /// Returns true if the status code corresponds to client failure.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        private static bool IsClientFailureRequest(int statusCode)
        {
            return statusCode == 505 || statusCode == 501 || statusCode >= 400 && statusCode < 500 && statusCode != 408;
        }

        /// <summary>
        /// Returns true if the status code corresponds to a server failure request.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        private static bool IsServerFailureRequest(int statusCode)
        {
            return statusCode >= 500 && statusCode <= 599 && statusCode != 505 && statusCode != 501 || statusCode == 408;
        }
    }
}
