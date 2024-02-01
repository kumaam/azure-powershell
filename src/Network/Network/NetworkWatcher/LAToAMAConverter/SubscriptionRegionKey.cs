using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Azure.Commands.Network.NetworkWatcher.LAToAMAConverter
{
    public class SubscriptionRegionKey
    {
        public SubscriptionRegionKey() { }

        public SubscriptionRegionKey(string subscriptionId, string region)
        {
            SubscriptionId = subscriptionId;
            Region = region;
        }

        public string SubscriptionId { get; set; }

        public string Region { get; set; }

        public bool Equals(SubscriptionRegionKey other)
        {
            return SubscriptionId.Equals(other.SubscriptionId, StringComparison.OrdinalIgnoreCase) && Region.Equals(other.Region, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            SubscriptionRegionKey other = (SubscriptionRegionKey)obj;
            return SubscriptionId == other.SubscriptionId && Region == other.Region;
        }

        public override int GetHashCode()
        {
            int hashCode = -1110236611;
            hashCode = (hashCode * -1521134295) + (string.IsNullOrEmpty(Region) ? 0 : Region.GetHashCode());
            hashCode = (hashCode * -1521134295) + (string.IsNullOrEmpty(SubscriptionId) ? 0 : SubscriptionId.GetHashCode());
            return hashCode;
        }
    }
}
