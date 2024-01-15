using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Azure.Commands.Network.NetworkWatcher.LAToAMAConverter
{
    public class NetworkAgentDetails
    {
        public string SubnetId { get; set; }
        public string AgentFqdn { get; set; }
        public string AgentId { get; set; }
        public string AgentIP { get; set; }
        public string ResourceId { get; set; }
    }
}
