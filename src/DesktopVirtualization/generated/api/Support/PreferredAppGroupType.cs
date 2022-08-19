// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is regenerated.

namespace Microsoft.Azure.PowerShell.Cmdlets.DesktopVirtualization.Support
{

    /// <summary>
    /// The type of preferred application group type, default to Desktop Application Group
    /// </summary>
    public partial struct PreferredAppGroupType :
        System.IEquatable<PreferredAppGroupType>
    {
        public static Microsoft.Azure.PowerShell.Cmdlets.DesktopVirtualization.Support.PreferredAppGroupType Desktop = @"Desktop";

        public static Microsoft.Azure.PowerShell.Cmdlets.DesktopVirtualization.Support.PreferredAppGroupType None = @"None";

        public static Microsoft.Azure.PowerShell.Cmdlets.DesktopVirtualization.Support.PreferredAppGroupType RailApplications = @"RailApplications";

        /// <summary>the value for an instance of the <see cref="PreferredAppGroupType" /> Enum.</summary>
        private string _value { get; set; }

        /// <summary>Conversion from arbitrary object to PreferredAppGroupType</summary>
        /// <param name="value">the value to convert to an instance of <see cref="PreferredAppGroupType" />.</param>
        internal static object CreateFrom(object value)
        {
            return new PreferredAppGroupType(global::System.Convert.ToString(value));
        }

        /// <summary>Compares values of enum type PreferredAppGroupType</summary>
        /// <param name="e">the value to compare against this instance.</param>
        /// <returns><c>true</c> if the two instances are equal to the same value</returns>
        public bool Equals(Microsoft.Azure.PowerShell.Cmdlets.DesktopVirtualization.Support.PreferredAppGroupType e)
        {
            return _value.Equals(e._value);
        }

        /// <summary>Compares values of enum type PreferredAppGroupType (override for Object)</summary>
        /// <param name="obj">the value to compare against this instance.</param>
        /// <returns><c>true</c> if the two instances are equal to the same value</returns>
        public override bool Equals(object obj)
        {
            return obj is PreferredAppGroupType && Equals((PreferredAppGroupType)obj);
        }

        /// <summary>Returns hashCode for enum PreferredAppGroupType</summary>
        /// <returns>The hashCode of the value</returns>
        public override int GetHashCode()
        {
            return this._value.GetHashCode();
        }

        /// <summary>Creates an instance of the <see cref="PreferredAppGroupType"/> Enum class.</summary>
        /// <param name="underlyingValue">the value to create an instance for.</param>
        private PreferredAppGroupType(string underlyingValue)
        {
            this._value = underlyingValue;
        }

        /// <summary>Returns string representation for PreferredAppGroupType</summary>
        /// <returns>A string for this value.</returns>
        public override string ToString()
        {
            return this._value;
        }

        /// <summary>Implicit operator to convert string to PreferredAppGroupType</summary>
        /// <param name="value">the value to convert to an instance of <see cref="PreferredAppGroupType" />.</param>

        public static implicit operator PreferredAppGroupType(string value)
        {
            return new PreferredAppGroupType(value);
        }

        /// <summary>Implicit operator to convert PreferredAppGroupType to string</summary>
        /// <param name="e">the value to convert to an instance of <see cref="PreferredAppGroupType" />.</param>

        public static implicit operator string(Microsoft.Azure.PowerShell.Cmdlets.DesktopVirtualization.Support.PreferredAppGroupType e)
        {
            return e._value;
        }

        /// <summary>Overriding != operator for enum PreferredAppGroupType</summary>
        /// <param name="e1">the value to compare against <paramref name="e2" /></param>
        /// <param name="e2">the value to compare against <paramref name="e1" /></param>
        /// <returns><c>true</c> if the two instances are not equal to the same value</returns>
        public static bool operator !=(Microsoft.Azure.PowerShell.Cmdlets.DesktopVirtualization.Support.PreferredAppGroupType e1, Microsoft.Azure.PowerShell.Cmdlets.DesktopVirtualization.Support.PreferredAppGroupType e2)
        {
            return !e2.Equals(e1);
        }

        /// <summary>Overriding == operator for enum PreferredAppGroupType</summary>
        /// <param name="e1">the value to compare against <paramref name="e2" /></param>
        /// <param name="e2">the value to compare against <paramref name="e1" /></param>
        /// <returns><c>true</c> if the two instances are equal to the same value</returns>
        public static bool operator ==(Microsoft.Azure.PowerShell.Cmdlets.DesktopVirtualization.Support.PreferredAppGroupType e1, Microsoft.Azure.PowerShell.Cmdlets.DesktopVirtualization.Support.PreferredAppGroupType e2)
        {
            return e2.Equals(e1);
        }
    }
}