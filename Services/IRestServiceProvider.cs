using System;

namespace Mail2Gotify.Services
{
    /// <summary>
    /// An interface that defines a means to access a rest service's configuration.
    /// </summary>
    public interface IRestServiceProvider
    {
        /// <summary>
        /// Retrieves the header needed to be authorized on the service.
        /// </summary>
        /// <returns>The header.</returns>
        string GetHeader();

        /// <summary>
        /// Retrieves the service uri configuration value.
        /// </summary>
        /// <returns>The service uri.</returns>
        Uri GetServiceUri();
    }
}