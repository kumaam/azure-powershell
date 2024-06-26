// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is regenerated.

namespace Microsoft.Azure.Management.Security
{
    using Microsoft.Rest.Azure;
    using Models;

    /// <summary>
    /// AlertsSuppressionRulesOperations operations.
    /// </summary>
    public partial interface IAlertsSuppressionRulesOperations
    {
        /// <summary>
        /// List of all the dismiss rules for the given subscription
        /// </summary>
        /// <remarks>
        /// List of all the dismiss rules for the given subscription
        /// </remarks>
        /// <param name='alertType'>
        /// Type of the alert to get rules for
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        /// <exception cref="Microsoft.Rest.Azure.CloudException">
        /// Thrown when the operation returned an invalid status code
        /// </exception>
        /// <exception cref="Microsoft.Rest.SerializationException">
        /// Thrown when unable to deserialize the response
        /// </exception>
        System.Threading.Tasks.Task<Microsoft.Rest.Azure.AzureOperationResponse<Microsoft.Rest.Azure.IPage<AlertsSuppressionRule>>> ListWithHttpMessagesAsync(string alertType = default(string), System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<string>> customHeaders = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));

        /// <summary>
        /// Get dismiss rule, with name: {alertsSuppressionRuleName}, for the given subscription
        /// </summary>
        /// <remarks>
        /// Get dismiss rule, with name: {alertsSuppressionRuleName}, for the given subscription
        /// </remarks>
        /// <param name='alertsSuppressionRuleName'>
        /// The unique name of the suppression alert rule
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        /// <exception cref="Microsoft.Rest.Azure.CloudException">
        /// Thrown when the operation returned an invalid status code
        /// </exception>
        /// <exception cref="Microsoft.Rest.SerializationException">
        /// Thrown when unable to deserialize the response
        /// </exception>
        System.Threading.Tasks.Task<Microsoft.Rest.Azure.AzureOperationResponse<AlertsSuppressionRule>> GetWithHttpMessagesAsync(string alertsSuppressionRuleName, System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<string>> customHeaders = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));

        /// <summary>
        /// Update existing rule or create new rule if it doesn&#39;t exist
        /// </summary>
        /// <remarks>
        /// Update existing rule or create new rule if it doesn&#39;t exist
        /// </remarks>
        /// <param name='alertsSuppressionRuleName'>
        /// The unique name of the suppression alert rule
        /// </param>
        /// <param name='alertsSuppressionRule'>
        /// Suppression rule object
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        /// <exception cref="Microsoft.Rest.Azure.CloudException">
        /// Thrown when the operation returned an invalid status code
        /// </exception>
        /// <exception cref="Microsoft.Rest.SerializationException">
        /// Thrown when unable to deserialize the response
        /// </exception>
        System.Threading.Tasks.Task<Microsoft.Rest.Azure.AzureOperationResponse<AlertsSuppressionRule>> UpdateWithHttpMessagesAsync(string alertsSuppressionRuleName, AlertsSuppressionRule alertsSuppressionRule, System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<string>> customHeaders = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));

        /// <summary>
        /// Delete dismiss alert rule for this subscription.
        /// </summary>
        /// <remarks>
        /// Delete dismiss alert rule for this subscription.
        /// </remarks>
        /// <param name='alertsSuppressionRuleName'>
        /// The unique name of the suppression alert rule
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        /// <exception cref="Microsoft.Rest.Azure.CloudException">
        /// Thrown when the operation returned an invalid status code
        /// </exception>
        System.Threading.Tasks.Task<Microsoft.Rest.Azure.AzureOperationResponse> DeleteWithHttpMessagesAsync(string alertsSuppressionRuleName, System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<string>> customHeaders = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));

        /// <summary>
        /// List of all the dismiss rules for the given subscription
        /// </summary>
        /// <remarks>
        /// List of all the dismiss rules for the given subscription
        /// </remarks>
        /// <param name='nextPageLink'>
        /// The NextLink from the previous successful call to List operation.
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        /// <exception cref="Microsoft.Rest.Azure.CloudException">
        /// Thrown when the operation returned an invalid status code
        /// </exception>
        /// <exception cref="Microsoft.Rest.SerializationException">
        /// Thrown when unable to deserialize the response
        /// </exception>
        System.Threading.Tasks.Task<Microsoft.Rest.Azure.AzureOperationResponse<Microsoft.Rest.Azure.IPage<AlertsSuppressionRule>>> ListNextWithHttpMessagesAsync(string nextPageLink, System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<string>> customHeaders = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));

    }
}