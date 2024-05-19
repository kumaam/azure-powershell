using Microsoft.Azure.Commands.Network.Models;
using Microsoft.Azure.Management.Internal.Resources.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Azure.Commands.Network.NetworkWatcher.LAToAMAConverter
{
    public class ConnectionMonitorResultWithArcMachines
    {
        public List<PSNetworkWatcherMmaWorkspaceMachineConnectionMonitor> ConnectionMonitorsList { get; set; }

        public IEnumerable<GenericResource> ArcGenericResources { get; set;}
    }
}
