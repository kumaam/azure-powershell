// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is regenerated.

namespace Microsoft.Azure.PowerShell.Cmdlets.SqlVirtualMachine.Models.Api20220801Preview
{
    using static Microsoft.Azure.PowerShell.Cmdlets.SqlVirtualMachine.Runtime.Extensions;

    /// <summary>Enable AAD authentication for SQL VM.</summary>
    public partial class AadAuthenticationSettings :
        Microsoft.Azure.PowerShell.Cmdlets.SqlVirtualMachine.Models.Api20220801Preview.IAadAuthenticationSettings,
        Microsoft.Azure.PowerShell.Cmdlets.SqlVirtualMachine.Models.Api20220801Preview.IAadAuthenticationSettingsInternal
    {

        /// <summary>Backing field for <see cref="ClientId" /> property.</summary>
        private string _clientId;

        /// <summary>
        /// The client Id of the Managed Identity to query Microsoft Graph API. An empty string must be used for the system assigned
        /// Managed Identity
        /// </summary>
        [Microsoft.Azure.PowerShell.Cmdlets.SqlVirtualMachine.Origin(Microsoft.Azure.PowerShell.Cmdlets.SqlVirtualMachine.PropertyOrigin.Owned)]
        public string ClientId { get => this._clientId; set => this._clientId = value; }

        /// <summary>Creates an new <see cref="AadAuthenticationSettings" /> instance.</summary>
        public AadAuthenticationSettings()
        {

        }
    }
    /// Enable AAD authentication for SQL VM.
    public partial interface IAadAuthenticationSettings :
        Microsoft.Azure.PowerShell.Cmdlets.SqlVirtualMachine.Runtime.IJsonSerializable
    {
        /// <summary>
        /// The client Id of the Managed Identity to query Microsoft Graph API. An empty string must be used for the system assigned
        /// Managed Identity
        /// </summary>
        [Microsoft.Azure.PowerShell.Cmdlets.SqlVirtualMachine.Runtime.Info(
        Required = false,
        ReadOnly = false,
        Description = @"The client Id of the Managed Identity to query Microsoft Graph API. An empty string must be used for the system assigned Managed Identity",
        SerializedName = @"clientId",
        PossibleTypes = new [] { typeof(string) })]
        string ClientId { get; set; }

    }
    /// Enable AAD authentication for SQL VM.
    internal partial interface IAadAuthenticationSettingsInternal

    {
        /// <summary>
        /// The client Id of the Managed Identity to query Microsoft Graph API. An empty string must be used for the system assigned
        /// Managed Identity
        /// </summary>
        string ClientId { get; set; }

    }
}