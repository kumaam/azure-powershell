// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is regenerated.

namespace Microsoft.Azure.PowerShell.Cmdlets.NetworkCloud.Support
{

    /// <summary>The provisioning state of the baseboard management controller key set.</summary>
    public partial struct BmcKeySetProvisioningState :
        System.IEquatable<BmcKeySetProvisioningState>
    {
        public static Microsoft.Azure.PowerShell.Cmdlets.NetworkCloud.Support.BmcKeySetProvisioningState Accepted = @"Accepted";

        public static Microsoft.Azure.PowerShell.Cmdlets.NetworkCloud.Support.BmcKeySetProvisioningState Canceled = @"Canceled";

        public static Microsoft.Azure.PowerShell.Cmdlets.NetworkCloud.Support.BmcKeySetProvisioningState Failed = @"Failed";

        public static Microsoft.Azure.PowerShell.Cmdlets.NetworkCloud.Support.BmcKeySetProvisioningState Provisioning = @"Provisioning";

        public static Microsoft.Azure.PowerShell.Cmdlets.NetworkCloud.Support.BmcKeySetProvisioningState Succeeded = @"Succeeded";

        /// <summary>
        /// the value for an instance of the <see cref="BmcKeySetProvisioningState" /> Enum.
        /// </summary>
        private string _value { get; set; }

        /// <summary>Creates an instance of the <see cref="BmcKeySetProvisioningState"/> Enum class.</summary>
        /// <param name="underlyingValue">the value to create an instance for.</param>
        private BmcKeySetProvisioningState(string underlyingValue)
        {
            this._value = underlyingValue;
        }

        /// <summary>Conversion from arbitrary object to BmcKeySetProvisioningState</summary>
        /// <param name="value">the value to convert to an instance of <see cref="BmcKeySetProvisioningState" />.</param>
        internal static object CreateFrom(object value)
        {
            return new BmcKeySetProvisioningState(global::System.Convert.ToString(value));
        }

        /// <summary>Compares values of enum type BmcKeySetProvisioningState</summary>
        /// <param name="e">the value to compare against this instance.</param>
        /// <returns><c>true</c> if the two instances are equal to the same value</returns>
        public bool Equals(Microsoft.Azure.PowerShell.Cmdlets.NetworkCloud.Support.BmcKeySetProvisioningState e)
        {
            return _value.Equals(e._value);
        }

        /// <summary>Compares values of enum type BmcKeySetProvisioningState (override for Object)</summary>
        /// <param name="obj">the value to compare against this instance.</param>
        /// <returns><c>true</c> if the two instances are equal to the same value</returns>
        public override bool Equals(object obj)
        {
            return obj is BmcKeySetProvisioningState && Equals((BmcKeySetProvisioningState)obj);
        }

        /// <summary>Returns hashCode for enum BmcKeySetProvisioningState</summary>
        /// <returns>The hashCode of the value</returns>
        public override int GetHashCode()
        {
            return this._value.GetHashCode();
        }

        /// <summary>Returns string representation for BmcKeySetProvisioningState</summary>
        /// <returns>A string for this value.</returns>
        public override string ToString()
        {
            return this._value;
        }

        /// <summary>Implicit operator to convert string to BmcKeySetProvisioningState</summary>
        /// <param name="value">the value to convert to an instance of <see cref="BmcKeySetProvisioningState" />.</param>

        public static implicit operator BmcKeySetProvisioningState(string value)
        {
            return new BmcKeySetProvisioningState(value);
        }

        /// <summary>Implicit operator to convert BmcKeySetProvisioningState to string</summary>
        /// <param name="e">the value to convert to an instance of <see cref="BmcKeySetProvisioningState" />.</param>

        public static implicit operator string(Microsoft.Azure.PowerShell.Cmdlets.NetworkCloud.Support.BmcKeySetProvisioningState e)
        {
            return e._value;
        }

        /// <summary>Overriding != operator for enum BmcKeySetProvisioningState</summary>
        /// <param name="e1">the value to compare against <paramref name="e2" /></param>
        /// <param name="e2">the value to compare against <paramref name="e1" /></param>
        /// <returns><c>true</c> if the two instances are not equal to the same value</returns>
        public static bool operator !=(Microsoft.Azure.PowerShell.Cmdlets.NetworkCloud.Support.BmcKeySetProvisioningState e1, Microsoft.Azure.PowerShell.Cmdlets.NetworkCloud.Support.BmcKeySetProvisioningState e2)
        {
            return !e2.Equals(e1);
        }

        /// <summary>Overriding == operator for enum BmcKeySetProvisioningState</summary>
        /// <param name="e1">the value to compare against <paramref name="e2" /></param>
        /// <param name="e2">the value to compare against <paramref name="e1" /></param>
        /// <returns><c>true</c> if the two instances are equal to the same value</returns>
        public static bool operator ==(Microsoft.Azure.PowerShell.Cmdlets.NetworkCloud.Support.BmcKeySetProvisioningState e1, Microsoft.Azure.PowerShell.Cmdlets.NetworkCloud.Support.BmcKeySetProvisioningState e2)
        {
            return e2.Equals(e1);
        }
    }
}