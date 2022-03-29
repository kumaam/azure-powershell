// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is regenerated.

namespace Microsoft.Azure.PowerShell.Cmdlets.Websites.Models.Api20210201
{
    using static Microsoft.Azure.PowerShell.Cmdlets.Websites.Runtime.Extensions;

    /// <summary>Triggers for auto-heal.</summary>
    public partial class AutoHealTriggers :
        Microsoft.Azure.PowerShell.Cmdlets.Websites.Models.Api20210201.IAutoHealTriggers,
        Microsoft.Azure.PowerShell.Cmdlets.Websites.Models.Api20210201.IAutoHealTriggersInternal
    {

        /// <summary>Internal Acessors for Request</summary>
        Microsoft.Azure.PowerShell.Cmdlets.Websites.Models.Api20210201.IRequestsBasedTrigger Microsoft.Azure.PowerShell.Cmdlets.Websites.Models.Api20210201.IAutoHealTriggersInternal.Request { get => (this._request = this._request ?? new Microsoft.Azure.PowerShell.Cmdlets.Websites.Models.Api20210201.RequestsBasedTrigger()); set { {_request = value;} } }

        /// <summary>Internal Acessors for SlowRequest</summary>
        Microsoft.Azure.PowerShell.Cmdlets.Websites.Models.Api20210201.ISlowRequestsBasedTrigger Microsoft.Azure.PowerShell.Cmdlets.Websites.Models.Api20210201.IAutoHealTriggersInternal.SlowRequest { get => (this._slowRequest = this._slowRequest ?? new Microsoft.Azure.PowerShell.Cmdlets.Websites.Models.Api20210201.SlowRequestsBasedTrigger()); set { {_slowRequest = value;} } }

        /// <summary>Backing field for <see cref="PrivateBytesInKb" /> property.</summary>
        private int? _privateBytesInKb;

        /// <summary>A rule based on private bytes.</summary>
        [Microsoft.Azure.PowerShell.Cmdlets.Websites.Origin(Microsoft.Azure.PowerShell.Cmdlets.Websites.PropertyOrigin.Owned)]
        public int? PrivateBytesInKb { get => this._privateBytesInKb; set => this._privateBytesInKb = value; }

        /// <summary>Backing field for <see cref="Request" /> property.</summary>
        private Microsoft.Azure.PowerShell.Cmdlets.Websites.Models.Api20210201.IRequestsBasedTrigger _request;

        /// <summary>A rule based on total requests.</summary>
        [Microsoft.Azure.PowerShell.Cmdlets.Websites.Origin(Microsoft.Azure.PowerShell.Cmdlets.Websites.PropertyOrigin.Owned)]
        internal Microsoft.Azure.PowerShell.Cmdlets.Websites.Models.Api20210201.IRequestsBasedTrigger Request { get => (this._request = this._request ?? new Microsoft.Azure.PowerShell.Cmdlets.Websites.Models.Api20210201.RequestsBasedTrigger()); set => this._request = value; }

        /// <summary>Request Count.</summary>
        [Microsoft.Azure.PowerShell.Cmdlets.Websites.Origin(Microsoft.Azure.PowerShell.Cmdlets.Websites.PropertyOrigin.Inlined)]
        public int? RequestCount { get => ((Microsoft.Azure.PowerShell.Cmdlets.Websites.Models.Api20210201.IRequestsBasedTriggerInternal)Request).Count; set => ((Microsoft.Azure.PowerShell.Cmdlets.Websites.Models.Api20210201.IRequestsBasedTriggerInternal)Request).Count = value ?? default(int); }

        /// <summary>Time interval.</summary>
        [Microsoft.Azure.PowerShell.Cmdlets.Websites.Origin(Microsoft.Azure.PowerShell.Cmdlets.Websites.PropertyOrigin.Inlined)]
        public string RequestTimeInterval { get => ((Microsoft.Azure.PowerShell.Cmdlets.Websites.Models.Api20210201.IRequestsBasedTriggerInternal)Request).TimeInterval; set => ((Microsoft.Azure.PowerShell.Cmdlets.Websites.Models.Api20210201.IRequestsBasedTriggerInternal)Request).TimeInterval = value ?? null; }

        /// <summary>Backing field for <see cref="SlowRequest" /> property.</summary>
        private Microsoft.Azure.PowerShell.Cmdlets.Websites.Models.Api20210201.ISlowRequestsBasedTrigger _slowRequest;

        /// <summary>A rule based on request execution time.</summary>
        [Microsoft.Azure.PowerShell.Cmdlets.Websites.Origin(Microsoft.Azure.PowerShell.Cmdlets.Websites.PropertyOrigin.Owned)]
        internal Microsoft.Azure.PowerShell.Cmdlets.Websites.Models.Api20210201.ISlowRequestsBasedTrigger SlowRequest { get => (this._slowRequest = this._slowRequest ?? new Microsoft.Azure.PowerShell.Cmdlets.Websites.Models.Api20210201.SlowRequestsBasedTrigger()); set => this._slowRequest = value; }

        /// <summary>Request Count.</summary>
        [Microsoft.Azure.PowerShell.Cmdlets.Websites.Origin(Microsoft.Azure.PowerShell.Cmdlets.Websites.PropertyOrigin.Inlined)]
        public int? SlowRequestCount { get => ((Microsoft.Azure.PowerShell.Cmdlets.Websites.Models.Api20210201.ISlowRequestsBasedTriggerInternal)SlowRequest).Count; set => ((Microsoft.Azure.PowerShell.Cmdlets.Websites.Models.Api20210201.ISlowRequestsBasedTriggerInternal)SlowRequest).Count = value ?? default(int); }

        /// <summary>Request Path.</summary>
        [Microsoft.Azure.PowerShell.Cmdlets.Websites.Origin(Microsoft.Azure.PowerShell.Cmdlets.Websites.PropertyOrigin.Inlined)]
        public string SlowRequestPath { get => ((Microsoft.Azure.PowerShell.Cmdlets.Websites.Models.Api20210201.ISlowRequestsBasedTriggerInternal)SlowRequest).Path; set => ((Microsoft.Azure.PowerShell.Cmdlets.Websites.Models.Api20210201.ISlowRequestsBasedTriggerInternal)SlowRequest).Path = value ?? null; }

        /// <summary>Time interval.</summary>
        [Microsoft.Azure.PowerShell.Cmdlets.Websites.Origin(Microsoft.Azure.PowerShell.Cmdlets.Websites.PropertyOrigin.Inlined)]
        public string SlowRequestTimeInterval { get => ((Microsoft.Azure.PowerShell.Cmdlets.Websites.Models.Api20210201.ISlowRequestsBasedTriggerInternal)SlowRequest).TimeInterval; set => ((Microsoft.Azure.PowerShell.Cmdlets.Websites.Models.Api20210201.ISlowRequestsBasedTriggerInternal)SlowRequest).TimeInterval = value ?? null; }

        /// <summary>Time taken.</summary>
        [Microsoft.Azure.PowerShell.Cmdlets.Websites.Origin(Microsoft.Azure.PowerShell.Cmdlets.Websites.PropertyOrigin.Inlined)]
        public string SlowRequestTimeTaken { get => ((Microsoft.Azure.PowerShell.Cmdlets.Websites.Models.Api20210201.ISlowRequestsBasedTriggerInternal)SlowRequest).TimeTaken; set => ((Microsoft.Azure.PowerShell.Cmdlets.Websites.Models.Api20210201.ISlowRequestsBasedTriggerInternal)SlowRequest).TimeTaken = value ?? null; }

        /// <summary>Backing field for <see cref="SlowRequestsWithPath" /> property.</summary>
        private Microsoft.Azure.PowerShell.Cmdlets.Websites.Models.Api20210201.ISlowRequestsBasedTrigger[] _slowRequestsWithPath;

        /// <summary>A rule based on multiple Slow Requests Rule with path</summary>
        [Microsoft.Azure.PowerShell.Cmdlets.Websites.Origin(Microsoft.Azure.PowerShell.Cmdlets.Websites.PropertyOrigin.Owned)]
        public Microsoft.Azure.PowerShell.Cmdlets.Websites.Models.Api20210201.ISlowRequestsBasedTrigger[] SlowRequestsWithPath { get => this._slowRequestsWithPath; set => this._slowRequestsWithPath = value; }

        /// <summary>Backing field for <see cref="StatusCode" /> property.</summary>
        private Microsoft.Azure.PowerShell.Cmdlets.Websites.Models.Api20210201.IStatusCodesBasedTrigger[] _statusCode;

        /// <summary>A rule based on status codes.</summary>
        [Microsoft.Azure.PowerShell.Cmdlets.Websites.Origin(Microsoft.Azure.PowerShell.Cmdlets.Websites.PropertyOrigin.Owned)]
        public Microsoft.Azure.PowerShell.Cmdlets.Websites.Models.Api20210201.IStatusCodesBasedTrigger[] StatusCode { get => this._statusCode; set => this._statusCode = value; }

        /// <summary>Backing field for <see cref="StatusCodesRange" /> property.</summary>
        private Microsoft.Azure.PowerShell.Cmdlets.Websites.Models.Api20210201.IStatusCodesRangeBasedTrigger[] _statusCodesRange;

        /// <summary>A rule based on status codes ranges.</summary>
        [Microsoft.Azure.PowerShell.Cmdlets.Websites.Origin(Microsoft.Azure.PowerShell.Cmdlets.Websites.PropertyOrigin.Owned)]
        public Microsoft.Azure.PowerShell.Cmdlets.Websites.Models.Api20210201.IStatusCodesRangeBasedTrigger[] StatusCodesRange { get => this._statusCodesRange; set => this._statusCodesRange = value; }

        /// <summary>Creates an new <see cref="AutoHealTriggers" /> instance.</summary>
        public AutoHealTriggers()
        {

        }
    }
    /// Triggers for auto-heal.
    public partial interface IAutoHealTriggers :
        Microsoft.Azure.PowerShell.Cmdlets.Websites.Runtime.IJsonSerializable
    {
        /// <summary>A rule based on private bytes.</summary>
        [Microsoft.Azure.PowerShell.Cmdlets.Websites.Runtime.Info(
        Required = false,
        ReadOnly = false,
        Description = @"A rule based on private bytes.",
        SerializedName = @"privateBytesInKB",
        PossibleTypes = new [] { typeof(int) })]
        int? PrivateBytesInKb { get; set; }
        /// <summary>Request Count.</summary>
        [Microsoft.Azure.PowerShell.Cmdlets.Websites.Runtime.Info(
        Required = false,
        ReadOnly = false,
        Description = @"Request Count.",
        SerializedName = @"count",
        PossibleTypes = new [] { typeof(int) })]
        int? RequestCount { get; set; }
        /// <summary>Time interval.</summary>
        [Microsoft.Azure.PowerShell.Cmdlets.Websites.Runtime.Info(
        Required = false,
        ReadOnly = false,
        Description = @"Time interval.",
        SerializedName = @"timeInterval",
        PossibleTypes = new [] { typeof(string) })]
        string RequestTimeInterval { get; set; }
        /// <summary>Request Count.</summary>
        [Microsoft.Azure.PowerShell.Cmdlets.Websites.Runtime.Info(
        Required = false,
        ReadOnly = false,
        Description = @"Request Count.",
        SerializedName = @"count",
        PossibleTypes = new [] { typeof(int) })]
        int? SlowRequestCount { get; set; }
        /// <summary>Request Path.</summary>
        [Microsoft.Azure.PowerShell.Cmdlets.Websites.Runtime.Info(
        Required = false,
        ReadOnly = false,
        Description = @"Request Path.",
        SerializedName = @"path",
        PossibleTypes = new [] { typeof(string) })]
        string SlowRequestPath { get; set; }
        /// <summary>Time interval.</summary>
        [Microsoft.Azure.PowerShell.Cmdlets.Websites.Runtime.Info(
        Required = false,
        ReadOnly = false,
        Description = @"Time interval.",
        SerializedName = @"timeInterval",
        PossibleTypes = new [] { typeof(string) })]
        string SlowRequestTimeInterval { get; set; }
        /// <summary>Time taken.</summary>
        [Microsoft.Azure.PowerShell.Cmdlets.Websites.Runtime.Info(
        Required = false,
        ReadOnly = false,
        Description = @"Time taken.",
        SerializedName = @"timeTaken",
        PossibleTypes = new [] { typeof(string) })]
        string SlowRequestTimeTaken { get; set; }
        /// <summary>A rule based on multiple Slow Requests Rule with path</summary>
        [Microsoft.Azure.PowerShell.Cmdlets.Websites.Runtime.Info(
        Required = false,
        ReadOnly = false,
        Description = @"A rule based on multiple Slow Requests Rule with path",
        SerializedName = @"slowRequestsWithPath",
        PossibleTypes = new [] { typeof(Microsoft.Azure.PowerShell.Cmdlets.Websites.Models.Api20210201.ISlowRequestsBasedTrigger) })]
        Microsoft.Azure.PowerShell.Cmdlets.Websites.Models.Api20210201.ISlowRequestsBasedTrigger[] SlowRequestsWithPath { get; set; }
        /// <summary>A rule based on status codes.</summary>
        [Microsoft.Azure.PowerShell.Cmdlets.Websites.Runtime.Info(
        Required = false,
        ReadOnly = false,
        Description = @"A rule based on status codes.",
        SerializedName = @"statusCodes",
        PossibleTypes = new [] { typeof(Microsoft.Azure.PowerShell.Cmdlets.Websites.Models.Api20210201.IStatusCodesBasedTrigger) })]
        Microsoft.Azure.PowerShell.Cmdlets.Websites.Models.Api20210201.IStatusCodesBasedTrigger[] StatusCode { get; set; }
        /// <summary>A rule based on status codes ranges.</summary>
        [Microsoft.Azure.PowerShell.Cmdlets.Websites.Runtime.Info(
        Required = false,
        ReadOnly = false,
        Description = @"A rule based on status codes ranges.",
        SerializedName = @"statusCodesRange",
        PossibleTypes = new [] { typeof(Microsoft.Azure.PowerShell.Cmdlets.Websites.Models.Api20210201.IStatusCodesRangeBasedTrigger) })]
        Microsoft.Azure.PowerShell.Cmdlets.Websites.Models.Api20210201.IStatusCodesRangeBasedTrigger[] StatusCodesRange { get; set; }

    }
    /// Triggers for auto-heal.
    internal partial interface IAutoHealTriggersInternal

    {
        /// <summary>A rule based on private bytes.</summary>
        int? PrivateBytesInKb { get; set; }
        /// <summary>A rule based on total requests.</summary>
        Microsoft.Azure.PowerShell.Cmdlets.Websites.Models.Api20210201.IRequestsBasedTrigger Request { get; set; }
        /// <summary>Request Count.</summary>
        int? RequestCount { get; set; }
        /// <summary>Time interval.</summary>
        string RequestTimeInterval { get; set; }
        /// <summary>A rule based on request execution time.</summary>
        Microsoft.Azure.PowerShell.Cmdlets.Websites.Models.Api20210201.ISlowRequestsBasedTrigger SlowRequest { get; set; }
        /// <summary>Request Count.</summary>
        int? SlowRequestCount { get; set; }
        /// <summary>Request Path.</summary>
        string SlowRequestPath { get; set; }
        /// <summary>Time interval.</summary>
        string SlowRequestTimeInterval { get; set; }
        /// <summary>Time taken.</summary>
        string SlowRequestTimeTaken { get; set; }
        /// <summary>A rule based on multiple Slow Requests Rule with path</summary>
        Microsoft.Azure.PowerShell.Cmdlets.Websites.Models.Api20210201.ISlowRequestsBasedTrigger[] SlowRequestsWithPath { get; set; }
        /// <summary>A rule based on status codes.</summary>
        Microsoft.Azure.PowerShell.Cmdlets.Websites.Models.Api20210201.IStatusCodesBasedTrigger[] StatusCode { get; set; }
        /// <summary>A rule based on status codes ranges.</summary>
        Microsoft.Azure.PowerShell.Cmdlets.Websites.Models.Api20210201.IStatusCodesRangeBasedTrigger[] StatusCodesRange { get; set; }

    }
}