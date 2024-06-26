// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is regenerated.

namespace Microsoft.Azure.PowerShell.Cmdlets.ArcResourceBridge.Models
{
    using static Microsoft.Azure.PowerShell.Cmdlets.ArcResourceBridge.Runtime.Extensions;

    public partial class ArcResourceBridgeIdentity :
        Microsoft.Azure.PowerShell.Cmdlets.ArcResourceBridge.Models.IArcResourceBridgeIdentity,
        Microsoft.Azure.PowerShell.Cmdlets.ArcResourceBridge.Models.IArcResourceBridgeIdentityInternal
    {

        /// <summary>Backing field for <see cref="Id" /> property.</summary>
        private string _id;

        /// <summary>Resource identity path</summary>
        [Microsoft.Azure.PowerShell.Cmdlets.ArcResourceBridge.Origin(Microsoft.Azure.PowerShell.Cmdlets.ArcResourceBridge.PropertyOrigin.Owned)]
        public string Id { get => this._id; set => this._id = value; }

        /// <summary>Backing field for <see cref="ResourceGroupName" /> property.</summary>
        private string _resourceGroupName;

        /// <summary>The name of the resource group. The name is case insensitive.</summary>
        [Microsoft.Azure.PowerShell.Cmdlets.ArcResourceBridge.Origin(Microsoft.Azure.PowerShell.Cmdlets.ArcResourceBridge.PropertyOrigin.Owned)]
        public string ResourceGroupName { get => this._resourceGroupName; set => this._resourceGroupName = value; }

        /// <summary>Backing field for <see cref="ResourceName" /> property.</summary>
        private string _resourceName;

        /// <summary>Appliances name.</summary>
        [Microsoft.Azure.PowerShell.Cmdlets.ArcResourceBridge.Origin(Microsoft.Azure.PowerShell.Cmdlets.ArcResourceBridge.PropertyOrigin.Owned)]
        public string ResourceName { get => this._resourceName; set => this._resourceName = value; }

        /// <summary>Backing field for <see cref="SubscriptionId" /> property.</summary>
        private string _subscriptionId;

        /// <summary>The ID of the target subscription.</summary>
        [Microsoft.Azure.PowerShell.Cmdlets.ArcResourceBridge.Origin(Microsoft.Azure.PowerShell.Cmdlets.ArcResourceBridge.PropertyOrigin.Owned)]
        public string SubscriptionId { get => this._subscriptionId; set => this._subscriptionId = value; }

        /// <summary>Backing field for <see cref="UpgradeGraph" /> property.</summary>
        private string _upgradeGraph;

        /// <summary>Upgrade graph version, ex - stable</summary>
        [Microsoft.Azure.PowerShell.Cmdlets.ArcResourceBridge.Origin(Microsoft.Azure.PowerShell.Cmdlets.ArcResourceBridge.PropertyOrigin.Owned)]
        public string UpgradeGraph { get => this._upgradeGraph; set => this._upgradeGraph = value; }

        /// <summary>Creates an new <see cref="ArcResourceBridgeIdentity" /> instance.</summary>
        public ArcResourceBridgeIdentity()
        {

        }
    }
    public partial interface IArcResourceBridgeIdentity :
        Microsoft.Azure.PowerShell.Cmdlets.ArcResourceBridge.Runtime.IJsonSerializable
    {
        /// <summary>Resource identity path</summary>
        [Microsoft.Azure.PowerShell.Cmdlets.ArcResourceBridge.Runtime.Info(
        Required = false,
        ReadOnly = false,
        Description = @"Resource identity path",
        SerializedName = @"id",
        PossibleTypes = new [] { typeof(string) })]
        string Id { get; set; }
        /// <summary>The name of the resource group. The name is case insensitive.</summary>
        [Microsoft.Azure.PowerShell.Cmdlets.ArcResourceBridge.Runtime.Info(
        Required = false,
        ReadOnly = false,
        Description = @"The name of the resource group. The name is case insensitive.",
        SerializedName = @"resourceGroupName",
        PossibleTypes = new [] { typeof(string) })]
        string ResourceGroupName { get; set; }
        /// <summary>Appliances name.</summary>
        [Microsoft.Azure.PowerShell.Cmdlets.ArcResourceBridge.Runtime.Info(
        Required = false,
        ReadOnly = false,
        Description = @"Appliances name.",
        SerializedName = @"resourceName",
        PossibleTypes = new [] { typeof(string) })]
        string ResourceName { get; set; }
        /// <summary>The ID of the target subscription.</summary>
        [Microsoft.Azure.PowerShell.Cmdlets.ArcResourceBridge.Runtime.Info(
        Required = false,
        ReadOnly = false,
        Description = @"The ID of the target subscription.",
        SerializedName = @"subscriptionId",
        PossibleTypes = new [] { typeof(string) })]
        string SubscriptionId { get; set; }
        /// <summary>Upgrade graph version, ex - stable</summary>
        [Microsoft.Azure.PowerShell.Cmdlets.ArcResourceBridge.Runtime.Info(
        Required = false,
        ReadOnly = false,
        Description = @"Upgrade graph version, ex - stable",
        SerializedName = @"upgradeGraph",
        PossibleTypes = new [] { typeof(string) })]
        string UpgradeGraph { get; set; }

    }
    internal partial interface IArcResourceBridgeIdentityInternal

    {
        /// <summary>Resource identity path</summary>
        string Id { get; set; }
        /// <summary>The name of the resource group. The name is case insensitive.</summary>
        string ResourceGroupName { get; set; }
        /// <summary>Appliances name.</summary>
        string ResourceName { get; set; }
        /// <summary>The ID of the target subscription.</summary>
        string SubscriptionId { get; set; }
        /// <summary>Upgrade graph version, ex - stable</summary>
        string UpgradeGraph { get; set; }

    }
}