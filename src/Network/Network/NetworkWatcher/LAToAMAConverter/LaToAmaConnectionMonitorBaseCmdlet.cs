// ----------------------------------------------------------------------------------
//
// Copyright Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------

using Microsoft.Azure.Commands.Common.Authentication;
using Microsoft.Azure.Commands.Common.Authentication.Abstractions;
using Microsoft.Azure.Commands.Common.Authentication.ResourceManager;
using Microsoft.Azure.Commands.Network.Models;
using Microsoft.Azure.Commands.Profile.Models;
using Microsoft.Azure.Commands.ResourceManager.Common;
using Microsoft.Azure.Management.Network;
using Microsoft.Azure.Management.Network.Models;
using Microsoft.Azure.Management.ResourceGraph.Models;
using Microsoft.Azure.Management.ResourceGraph;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.OperationalInsights;
using Microsoft.Azure.Commands.OperationalInsights.Client;
using Microsoft.Rest;
using Microsoft.Azure.Management.Internal.Resources;
using Microsoft.Azure.Management.Internal.Resources.Utilities;
using Microsoft.Azure.Management.Internal.Resources.Models;
using System.Runtime.Caching;
using Microsoft.WindowsAzure.Commands.Utilities.Common;
using Microsoft.Azure.Commands.Common.Strategies;
using Microsoft.Azure.Management.Monitor.Version2018_09_01.Models;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Management.Automation.Language;
using System.Reflection;
using Microsoft.Azure.Commands.Network.NetworkWatcher.LAToAMAConverter.ArrayExtensions;

namespace Microsoft.Azure.Commands.Network.NetworkWatcher.LAToAMAConverter
{
    public abstract class LaToAmaConnectionMonitorBaseCmdlet : ConnectionMonitorBaseCmdlet
    {
        protected Action<string> WarningLog;
        protected ISubscriptionClientWrapper SubscriptionAndTenantClient = null;

        protected ResourceManagementClient ArmClient
        {
            get
            {
                return this._armClient ??
                       (this._armClient = AzureSession.Instance.ClientFactory.CreateArmClient<ResourceManagementClient>(
                           context: this.DefaultContext,
                           endpoint: AzureEnvironment.Endpoint.ResourceManager));
            }
            set
            {
                this._armClient = value;
            }
        }

        protected IEnumerable<AzureSubscription> GetAllSubscriptionsByUserContext(IProfileOperations profile, IAzureTokenCache cache)
        {
            var tenantId = DefaultContext.Tenant.Id;
            return ListAllSubscriptionsForTenant(tenantId, profile, cache);
        }

        /// <summary>
        /// Get All the connection Monitors under user context subscriptions
        /// </summary>
        /// <param name="subscriptionsList">user context subscriptions</param>
        /// <param name="region">connection monitor region</param>
        /// <returns>collection of all the ConnectionMonitor Resource Detail</returns>
        protected IEnumerable<GenericResource> GetConnectionMonitorBySubscriptions(IEnumerable<string> subscriptionsList, string region = null)
        {
            List<GenericResource> genericCMResources = new List<GenericResource>();

            foreach (string subId in subscriptionsList)
            {
                //Need to check a better solution
                if (DefaultContext.Subscription.Id != subId)
                {
                    DefaultContext.Subscription.Id = subId;
                    _armClient = null;
                }

                List<GenericResource> cmGenericInfoList = ArmClient.FilterResources(new Management.Internal.Resources.Utilities.Models.FilterResourcesOptions()
                { ResourceType = CommonConstants.ConnectionMonitorResourceType });

                genericCMResources.AddRange(cmGenericInfoList);
            }

            if (!string.IsNullOrEmpty(region))
            {
                genericCMResources.Where(w => w?.Location.Equals(region, StringComparison.OrdinalIgnoreCase) == true);
            }

            return genericCMResources;
        }

        protected ConnectionMonitorResult GetConnectionMonitorResult(string resourceGroupName, string name, string connectionMonitorName)
        {
            return this.ConnectionMonitors.Get(resourceGroupName, name, connectionMonitorName);
        }

        protected PSNetworkWatcherMmaWorkspaceMachineConnectionMonitor MapConnectionMonitorResultToPSMmaWorkspaceMachineConnectionMonitor(ConnectionMonitorResult connectionMonitor)
        {
            PSNetworkWatcherMmaWorkspaceMachineConnectionMonitor psMmaWorkspaceMachineConnectionMonitor = new PSNetworkWatcherMmaWorkspaceMachineConnectionMonitor()
            {
                Name = connectionMonitor.Name,
                Id = connectionMonitor.Id,
                Etag = connectionMonitor.Etag,
                ProvisioningState = connectionMonitor.ProvisioningState,
                Type = connectionMonitor.Type,
                Location = connectionMonitor.Location,
                StartTime = connectionMonitor.StartTime,
                Tags = new Dictionary<string, string>(),
                ConnectionMonitorType = connectionMonitor.ConnectionMonitorType,
                Notes = connectionMonitor.Notes,
                Endpoints = new List<PSNetworkWatcherConnectionMonitorEndpointObject>(),
                TestGroups = new List<PSNetworkWatcherConnectionMonitorTestGroupObject>()
            };

            if (connectionMonitor.Tags != null)
            {
                foreach (KeyValuePair<string, string> KeyValue in connectionMonitor.Tags)
                {
                    psMmaWorkspaceMachineConnectionMonitor.Tags.Add(KeyValue.Key, KeyValue.Value);
                }
            }

            if (connectionMonitor.Outputs != null)
            {
                psMmaWorkspaceMachineConnectionMonitor.Outputs = new List<PSNetworkWatcherConnectionMonitorOutputObject>();
                foreach (ConnectionMonitorOutput output in connectionMonitor.Outputs)
                {
                    psMmaWorkspaceMachineConnectionMonitor.Outputs.Add(
                        new PSNetworkWatcherConnectionMonitorOutputObject()
                        {
                            Type = output.Type,
                            WorkspaceSettings = new PSConnectionMonitorWorkspaceSettings()
                            {
                                WorkspaceResourceId = output.WorkspaceSettings?.WorkspaceResourceId
                            }
                        });
                }
            }

            if (connectionMonitor.TestGroups != null)
            {
                foreach (ConnectionMonitorTestGroup testGroup in connectionMonitor.TestGroups)
                {
                    PSNetworkWatcherConnectionMonitorTestGroupObject testGroupObject = new PSNetworkWatcherConnectionMonitorTestGroupObject()
                    {
                        Name = testGroup.Name,
                        Disable = testGroup.Disable,
                        TestConfigurations = new List<PSNetworkWatcherConnectionMonitorTestConfigurationObject>(),
                        Sources = new List<PSNetworkWatcherConnectionMonitorEndpointObject>(),
                        Destinations = new List<PSNetworkWatcherConnectionMonitorEndpointObject>()
                    };

                    if (testGroup.Sources != null)
                    {
                        foreach (string sourceEndpointName in testGroup.Sources)
                        {
                            ConnectionMonitorEndpoint sourceEndpoint = GetEndpoinByName(connectionMonitor.Endpoints, sourceEndpointName);

                            PSNetworkWatcherConnectionMonitorEndpointObject EndpointObject =
                                NetworkResourceManagerProfile.Mapper.Map<PSNetworkWatcherConnectionMonitorEndpointObject>(sourceEndpoint);

                            testGroupObject.Sources.Add(EndpointObject);
                            // Might contains duplicate endpoints, need to check.
                            bool endpointExists = psMmaWorkspaceMachineConnectionMonitor.Endpoints.Any(tc => tc.Name == EndpointObject.Name);
                            if (!endpointExists)
                            {
                                psMmaWorkspaceMachineConnectionMonitor.Endpoints.Add(EndpointObject);
                            }
                        }
                    }

                    if (testGroup.Destinations != null)
                    {
                        foreach (string destinationEndpointName in testGroup.Destinations)
                        {
                            ConnectionMonitorEndpoint destinationEndpoint = GetEndpoinByName(connectionMonitor.Endpoints, destinationEndpointName);

                            PSNetworkWatcherConnectionMonitorEndpointObject EndpointObject =
                                NetworkResourceManagerProfile.Mapper.Map<PSNetworkWatcherConnectionMonitorEndpointObject>(destinationEndpoint);

                            testGroupObject.Destinations.Add(EndpointObject);
                            bool endpointExists = psMmaWorkspaceMachineConnectionMonitor.Endpoints.Any(tc => tc.Name == EndpointObject.Name);
                            if (!endpointExists)
                            {
                                psMmaWorkspaceMachineConnectionMonitor.Endpoints.Add(EndpointObject);
                            }
                        }
                    }

                    // Test Configuration
                    if (testGroup.TestConfigurations != null)
                    {
                        foreach (string testConfigurationName in testGroup.TestConfigurations)
                        {
                            ConnectionMonitorTestConfiguration testConfiguration = GetTestConfigurationByName(connectionMonitor.TestConfigurations, testConfigurationName);

                            PSNetworkWatcherConnectionMonitorTestConfigurationObject testConfigurationObject = new PSNetworkWatcherConnectionMonitorTestConfigurationObject()
                            {
                                Name = testConfiguration.Name,
                                PreferredIPVersion = testConfiguration.PreferredIPVersion,
                                TestFrequencySec = testConfiguration.TestFrequencySec,
                                SuccessThreshold = testConfiguration.SuccessThreshold == null ? null :
                                    new PSNetworkWatcherConnectionMonitorSuccessThreshold()
                                    {
                                        ChecksFailedPercent = testConfiguration.SuccessThreshold.ChecksFailedPercent,
                                        RoundTripTimeMs = testConfiguration.SuccessThreshold.RoundTripTimeMs
                                    },
                                ProtocolConfiguration = this.GetPSProtocolConfiguration(testConfiguration)
                            };

                            testGroupObject.TestConfigurations.Add(testConfigurationObject);
                        }
                    }

                    psMmaWorkspaceMachineConnectionMonitor.TestGroups.Add(testGroupObject);
                }
            }

            return psMmaWorkspaceMachineConnectionMonitor;
        }

        protected ConnectionMonitorResult MapPSMmaWorkspaceMachineConnectionMonitorToConnectionMonitorResult(PSNetworkWatcherMmaWorkspaceMachineConnectionMonitor connectionMonitor)
        {
            ConnectionMonitorResult connectionMonitorResult = new ConnectionMonitorResult(
                connectionMonitor.Name, connectionMonitor.Id, connectionMonitor.Etag, connectionMonitor.Type,
                connectionMonitor.Location);

            connectionMonitorResult.Endpoints = new List<ConnectionMonitorEndpoint>();
            connectionMonitorResult.TestGroups = new List<ConnectionMonitorTestGroup>();
            connectionMonitorResult.TestConfigurations = new List<ConnectionMonitorTestConfiguration>();
            connectionMonitorResult.Outputs = new List<ConnectionMonitorOutput>();

            if (connectionMonitor.Endpoints != null)
            {
                foreach (PSNetworkWatcherConnectionMonitorEndpointObject endpoint in connectionMonitor.Endpoints)
                {
                    ConnectionMonitorEndpoint EndpointObject =
                        NetworkResourceManagerProfile.Mapper.Map<ConnectionMonitorEndpoint>(endpoint);

                    connectionMonitorResult.Endpoints.Add(EndpointObject);
                }
            }

            if (connectionMonitor.Outputs != null)
            {
                foreach (PSNetworkWatcherConnectionMonitorOutputObject output in connectionMonitor.Outputs)
                {
                    connectionMonitorResult.Outputs.Add(
                        new ConnectionMonitorOutput()
                        {
                            Type = output.Type,
                            WorkspaceSettings = new ConnectionMonitorWorkspaceSettings()
                            {
                                WorkspaceResourceId = output.WorkspaceSettings?.WorkspaceResourceId
                            }
                        });
                }
            }

            if (connectionMonitor.TestGroups != null)
            {
                foreach (PSNetworkWatcherConnectionMonitorTestGroupObject testGroup in connectionMonitor.TestGroups)
                {
                    ConnectionMonitorTestGroup testGroupObject = new ConnectionMonitorTestGroup()
                    {
                        Name = testGroup.Name,
                        Disable = testGroup.Disable,
                        Sources = new List<string>(),
                        Destinations = new List<string>(),
                        TestConfigurations = new List<string>()
                    };

                    foreach (PSNetworkWatcherConnectionMonitorEndpointObject sourceEndpoint in testGroup?.Sources)
                    {
                        testGroupObject.Sources.Add(sourceEndpoint.Name);
                    }

                    foreach (PSNetworkWatcherConnectionMonitorEndpointObject destinationEndpoint in testGroup.Destinations)
                    {
                        testGroupObject.Destinations.Add(destinationEndpoint.Name);
                    }

                    // Test Configuration
                    foreach (PSNetworkWatcherConnectionMonitorTestConfigurationObject testConfiguration in testGroup?.TestConfigurations)
                    {
                        testGroupObject.TestConfigurations.Add(testConfiguration.Name);

                        bool testConfigurationExists = connectionMonitorResult.TestConfigurations.Any(tc => tc.Name == testConfiguration.Name);
                        if (!testConfigurationExists)
                        {
                            ConnectionMonitorTestConfiguration testConfigurationObject = new ConnectionMonitorTestConfiguration()
                            {
                                Name = testConfiguration.Name,
                                PreferredIPVersion = testConfiguration.PreferredIPVersion,
                                TestFrequencySec = testConfiguration.TestFrequencySec,
                                SuccessThreshold = testConfiguration.SuccessThreshold == null ? null :
                                    new ConnectionMonitorSuccessThreshold()
                                    {
                                        ChecksFailedPercent = testConfiguration.SuccessThreshold.ChecksFailedPercent,
                                        RoundTripTimeMs = testConfiguration.SuccessThreshold.RoundTripTimeMs
                                    }
                            };

                            if (testConfiguration.ProtocolConfiguration is PSNetworkWatcherConnectionMonitorHttpConfiguration)
                            {
                                PSNetworkWatcherConnectionMonitorHttpConfiguration config = (PSNetworkWatcherConnectionMonitorHttpConfiguration)testConfiguration.ProtocolConfiguration;
                                testConfigurationObject.HttpConfiguration = new ConnectionMonitorHttpConfiguration()
                                {
                                    Port = config?.Port,
                                    Method = config?.Method,
                                    Path = config?.Path,
                                    RequestHeaders = GetRequestHeaders(config?.RequestHeaders),
                                    ValidStatusCodeRanges = config?.ValidStatusCodeRanges,
                                    PreferHTTPS = config?.PreferHTTPS
                                };

                                testConfigurationObject.Protocol = "Http";
                            }

                            else if (testConfiguration.ProtocolConfiguration is PSNetworkWatcherConnectionMonitorIcmpConfiguration)
                            {
                                testConfigurationObject.IcmpConfiguration = new ConnectionMonitorIcmpConfiguration()
                                {
                                    DisableTraceRoute = ((PSNetworkWatcherConnectionMonitorIcmpConfiguration)testConfiguration.ProtocolConfiguration).DisableTraceRoute
                                };

                                testConfigurationObject.Protocol = "Icmp";
                            }

                            else if (testConfiguration.ProtocolConfiguration is PSNetworkWatcherConnectionMonitorTcpConfiguration)
                            {
                                PSNetworkWatcherConnectionMonitorTcpConfiguration config = (PSNetworkWatcherConnectionMonitorTcpConfiguration)testConfiguration.ProtocolConfiguration;
                                testConfigurationObject.TcpConfiguration = new ConnectionMonitorTcpConfiguration()
                                {
                                    DisableTraceRoute = config?.DisableTraceRoute,
                                    Port = config?.Port,
                                    DestinationPortBehavior = config?.DestinationPortBehavior
                                };

                                testConfigurationObject.Protocol = "Tcp";
                            }

                            connectionMonitorResult.TestConfigurations.Add(testConfigurationObject);
                        }
                    }
                    connectionMonitorResult.TestGroups.Add(testGroupObject);
                }
            }

            return connectionMonitorResult;
        }

        /// <summary>
        /// Get All the CMs which has MMAWorkspaceMachine as endpoint
        /// </summary>
        /// <param name="connectionMonitors">Basic details of CM like id, name , location, type</param>
        /// <param name="endpointType">endpointType = MMAWorkspaceMachine</param>
        /// <param name="workSpaceId">work space Id</param>
        /// <returns>collection of connection monitor results</returns>
        protected async Task<List<ConnectionMonitorResult>> GetConnectionMonitorHasMMAWorkspaceMachineEndpoint(IEnumerable<GenericResource> connectionMonitors, string endpointType, string workSpaceId = null)
        {
            List<Task<ConnectionMonitorResult>> listCM = new List<Task<ConnectionMonitorResult>>();
            foreach (var cm in connectionMonitors)
            {
                string subscriptionId = GetSubscriptionIdByResourceId(cm.Id);
                if (DefaultContext.Subscription.Id != subscriptionId)
                {
                    DefaultContext.Subscription.Id = subscriptionId;
                    NetworkClient = new NetworkClient(DefaultContext);
                }
                ConnectionMonitorDetails cmBasicDetails = GetConnectionMonitorDetails(cm.Id);
                listCM.Add(ConnectionMonitors.GetAsync(cmBasicDetails.ResourceGroupName, cmBasicDetails.NetworkWatcherName, cmBasicDetails.ConnectionMonitorName));
            }

            var listConnectionMonitorResult = await Task.WhenAll(listCM);
            // if we remove workspace id as mandatory param
            if (workSpaceId != null)
            {
                return listConnectionMonitorResult?.Where(w => w.Endpoints?.Any(a => a.Type?.Equals(endpointType, StringComparison.OrdinalIgnoreCase) == true
                && a.ResourceId?.Equals(workSpaceId, StringComparison.OrdinalIgnoreCase) == true) == true).ToList();
            }
            else
            {
                return listConnectionMonitorResult
                .Where(w => w.Endpoints?.Any(a => a.Type?.Equals(endpointType, StringComparison.OrdinalIgnoreCase) == true) == true).ToList();
            }
        }

        /// <summary>
        /// Get the ARC resource details from connection monitor list(which contains MMAWorkspaceMachine endpoints)
        /// </summary>
        /// <param name="mmaMachineCMs">All CMs which contains MMAWorkspaceMachine endpoints or MMAWorkspaceNetwork endpoint</param>
        /// <returns>OperationalInsightsQueryResults data which contains ARC resource details</returns>
        protected async Task<List<Azure.OperationalInsights.Models.QueryResults>> GetNetworkAgentLAWorkSpaceData(IEnumerable<PSNetworkWatcherMmaWorkspaceMachineConnectionMonitor> mmaMachineCMs)
        {
            IEnumerable<PSNetworkWatcherConnectionMonitorEndpointObject> cmAllMMAEndpoints = GetAllMMAEndpoints(mmaMachineCMs);
            var getDistinctWorkSpaceAndAddress = cmAllMMAEndpoints?.GroupBy(g => new { g.ResourceId, g.Address }).Select(s => s.FirstOrDefault());
            return await QueryForLaWorkSpaceNetworkAgentData(getDistinctWorkSpaceAndAddress);
        }
        protected async Task<List<PSNetworkWatcherMmaWorkspaceMachineConnectionMonitor>> MigrateCM(IEnumerable<PSNetworkWatcherMmaWorkspaceMachineConnectionMonitor> mmaMachineCMs)
        {
            List<PSNetworkWatcherConnectionMonitorEndpointObject> mmaEndpoints = mmaMachineCMs.SelectMany(s => s.Endpoints.Where(w => w != null && (w.Type.Equals(CommonConstants.MMAWorkspaceMachineEndpointResourceType, StringComparison.OrdinalIgnoreCase)
            || w.Type.Equals(CommonConstants.MMAWorkspaceNetworkEndpointResourceType, StringComparison.OrdinalIgnoreCase)))).ToList();

            // remove entries where arc machine details are not available
            Dictionary<string, List<NetworkAgentDetails>> arcmMachineDetails = await FetchArcMachineDetails(mmaEndpoints.AsEnumerable());

            // fetch all ARC machines and now call ARM to get location of each machine.
            List<string> arcMachines = arcmMachineDetails.Values.SelectMany(s => s).Select(s => s.ResourceId).Distinct().ToList();

            Dictionary<string, string> arcMachineToRegion = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            // TODO - fetch all regions of ARC machines and update arcMachineToRegion dictionary.
            IEnumerable<GenericResource> arcGenericResources = GetResourcesById(arcMachines);
            arcGenericResources.ToList().ForEach(arcMachine =>
            {
                arcMachineToRegion.Add(arcMachine.Id, arcMachine.Location);
            });


            List<PSNetworkWatcherMmaWorkspaceMachineConnectionMonitor> updatedCMs = new List<PSNetworkWatcherMmaWorkspaceMachineConnectionMonitor>();
            List<PSNetworkWatcherMmaWorkspaceMachineConnectionMonitor> notUpdatedCMs = new List<PSNetworkWatcherMmaWorkspaceMachineConnectionMonitor>();
            mmaMachineCMs.ForEach(cm =>
            {
                var endpoints = cm.Endpoints.Where(w => w != null && (w.Type.Equals(CommonConstants.MMAWorkspaceMachineEndpointResourceType, StringComparison.OrdinalIgnoreCase)
            || w.Type.Equals(CommonConstants.MMAWorkspaceNetworkEndpointResourceType, StringComparison.OrdinalIgnoreCase))).ToList();

                // Validate if all endpoints can be converted to arc.
                List<string> endointsNotConverted = new List<string>();

                endpoints.ForEach(endpoint =>
                {
                    if (!arcmMachineDetails.ContainsKey(endpoint.ResourceId))
                        endointsNotConverted.Add(endpoint.ResourceId);

                    List<NetworkAgentDetails> arcDetails = new List<NetworkAgentDetails>();
                    if (endpoint.Type.Equals(CommonConstants.MMAWorkspaceMachineEndpointResourceType, StringComparison.OrdinalIgnoreCase))
                    {
                        arcDetails = arcmMachineDetails[endpoint.ResourceId];
                        // TODO this check is needed, add this check back.
                        // Where(arc => endpoint.Address.Equals(arc.AgentIP)).ToList();
                    }
                    else if(endpoint.Type.Equals(CommonConstants.MMAWorkspaceNetworkEndpointResourceType, StringComparison.OrdinalIgnoreCase))
                    {
                        arcDetails = arcmMachineDetails[endpoint.ResourceId].Where(arc => endpoint?.Scope?.Include?.Equals(arc.SubnetId) ?? false).ToList();
                    }
                    
                    if (!arcDetails.Any())
                        endointsNotConverted.Add(endpoint.ResourceId);
                });

                // Not migrating even if a single endpoint can't migrated to ARC. Else condition handles scenario where all CMs endpoints (MMA ones) can be migrated to ARC.
                if (endointsNotConverted.Any())
                {
                    notUpdatedCMs.Add(cm);
                }
                else
                {
                    Dictionary<string, List<string>> regionalEndpoints = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
                    cm.TestGroups.ForEach(tg => tg.Sources.ToList().ForEach(s =>
                    {
                        var regions = GetRegionOfEndpoint(s, cm.Location, arcmMachineDetails, arcMachineToRegion);
                        regions.ForEach(region =>
                        {
                            if (!regionalEndpoints.ContainsKey(region))
                                regionalEndpoints.Add(region, new List<string>());

                            if (!regionalEndpoints[region].Contains(s.Name))
                                regionalEndpoints[region].Add(s.Name);
                        });
                    }));

                    // iterate regionalEndpoints
                    // for 1st region - duplicate CM and update MMAMachine endpoints simply.
                    //                - update MMANetwork and needed, add more endpoind and update those in test groups
                    // for 2nd region - duplicate CM and perform same operation as above (tweak name a bit)
                    //                - removes the MMAMachine or others endpoints which are already added in 1st region and corresponding testGroups.

                    bool sameCM = true;
                    int cmIteration = 0;
                    regionalEndpoints.ForEach(regionalEndpoint =>
                    {
                        PSNetworkWatcherMmaWorkspaceMachineConnectionMonitor newCM = cm.Copy();
                        //NetworkResourceManagerProfile.Mapper.Map<PSNetworkWatcherMmaWorkspaceMachineConnectionMonitor>(cm);

                        newCM.Location = regionalEndpoint.Key;
                        if (!sameCM)
                        {
                            newCM.Name = newCM.Name + "_" + cmIteration;
                            newCM.Id = "/subscriptions/" + this.DefaultContext.Subscription.Id + "/resourceGroups/networkwatcherrg/providers/Microsoft.Network/networkWatchers/NetworkWatcher_" + newCM.Location + "/connectionMonitors/" + newCM.Name;
                        }


                        foreach (var endpoint in newCM.Endpoints)
                        {
                            // Remove endpoints from test groups if they are not part of this region. Later we'll update the test-groups for parity.
                            if (!regionalEndpoint.Value.Contains(endpoint.Name))
                            {
                                newCM.TestGroups.ForEach(tg =>
                                {
                                    var sourcesToRemove = tg.Sources.Where(s => s.Name.Equals(endpoint.Name, StringComparison.InvariantCultureIgnoreCase)).ToList();
                                    tg.Sources = tg.Sources.Except(sourcesToRemove).ToList();
                                });
                            }


                            if (endpoint.Type.Equals(CommonConstants.MMAWorkspaceMachineEndpointResourceType, StringComparison.OrdinalIgnoreCase))
                            {
                                // Changing the endpoint type to AzureArcVM, only type + resourceId needs to be changed.
                                endpoint.Type = CommonConstants.AzureArcVMType;
                                endpoint.ResourceId = arcmMachineDetails[endpoint.ResourceId].FirstOrDefault().ResourceId;
                                // TODO endpoint.ResourceId = arcmMachineDetails[endpoint.ResourceId].Where(arc => endpoint.Address.Equals(arc.AgentIP)).FirstOrDefault().ResourceId;
                            }

                            // ARCNetwork support is not there in PS.
                            else if (endpoint.Type.Equals(CommonConstants.MMAWorkspaceNetworkEndpointResourceType, StringComparison.OrdinalIgnoreCase))
                            {
                            }
                        }

                        //cm.Endpoints might have unused endpoints, remove them. For every endpoint in cm.Endpoints, check if it exists in cm.Sources/destination, if not remove it.

                        var tgsToRemove = new List<PSNetworkWatcherConnectionMonitorTestGroupObject>();
                        newCM.TestGroups.ForEach(tg =>
                        {
                            if (!tg.Sources.Any())
                                tgsToRemove.Add(tg);
                        });
                        newCM.TestGroups = newCM.TestGroups.Except(tgsToRemove).ToList();

                        updatedCMs.Add(newCM);
                        sameCM = false;
                        cmIteration++;
                    });
                }
            });

            return updatedCMs;
        }

        protected IEnumerable<GenericResource> GetResourcesById(IEnumerable<string> resourceIds)
        {
            List<GenericResource> genericResources = new List<GenericResource>();
            Parallel.ForEach(resourceIds, id =>
            {
                //Need to check API Version
                genericResources.Add(ArmClient.Resources.GetById(id, "2022-12-27"));
            });

            return genericResources;
        }

        private List<string> GetRegionOfEndpoint(PSNetworkWatcherConnectionMonitorEndpointObject endpoint, string cmRegion, Dictionary<string, List<NetworkAgentDetails>> laToArcDetails, Dictionary<string, string> arcMachineToRegion)
        {
            if (!endpoint.Type.Equals(CommonConstants.MMAWorkspaceMachineEndpointResourceType, StringComparison.OrdinalIgnoreCase) && !endpoint.Type.Equals(CommonConstants.MMAWorkspaceNetworkEndpointResourceType, StringComparison.OrdinalIgnoreCase))
            {
                return new List<string> { cmRegion };
            }
            else
            {
                string resourceId = endpoint.ResourceId;
                List<string> arcDetails = laToArcDetails[resourceId].Select(s=> s.ResourceId).ToList();
                List<string> regions = new List<string>();
                arcDetails.ForEach(s => {
                    arcMachineToRegion.TryGetValue(s, out string region);
                    if (!regions.Contains(region))
                    {
                        regions.Add(region);
                    }
                });

                return regions;
            }
        }

            protected async Task<List<PSNetworkWatcherMmaWorkspaceMachineConnectionMonitor>> MigrateCMs(IEnumerable<PSNetworkWatcherMmaWorkspaceMachineConnectionMonitor> mmaMachineCMs)
        {
            var CmWithMmaEndpoints = mmaMachineCMs.Select(s => new
            {
                CM = s,
                Endpoints = s.Endpoints.Where(w => w != null && (w.Type.Equals(CommonConstants.MMAWorkspaceMachineEndpointResourceType, StringComparison.OrdinalIgnoreCase)
            || w.Type.Equals(CommonConstants.MMAWorkspaceNetworkEndpointResourceType, StringComparison.OrdinalIgnoreCase)))
            });

            // Need to refactor for distinct data
            var getCmWithEndpointsAndNetworkAgentDataList = await Task.WhenAll(CmWithMmaEndpoints.Select(async s => new
            {
                Cm = s.CM,
                EndpointWithNetworkAgentData = await QueryForLaWorkSpaceNetworkAgentData1(s.Endpoints)
            }));

            //WriteInformation($"Before update\n{JsonConvert.SerializeObject(getCmWithEndpointsAndNetworkAgentDataList, Formatting.Indented)}\n", new string[] { "PSHOST" });

            List<PSNetworkWatcherMmaWorkspaceMachineConnectionMonitor> UpdatedArcCmList = new List<PSNetworkWatcherMmaWorkspaceMachineConnectionMonitor>();

            getCmWithEndpointsAndNetworkAgentDataList.ForEach(data =>
            {
                Dictionary<string, List<PSNetworkWatcherConnectionMonitorEndpointObject>> dictDestinationMMANetworkIdToSubnetAddresses = null;
                Dictionary<string, List<PSNetworkWatcherConnectionMonitorEndpointObject>> dictSourcesMMANetworkIdToSubnetAddresses = null;

                //Destination update
                data.Cm.TestGroups.Where(w => w.Destinations.Any(iw => iw.Type.Equals(CommonConstants.MMAWorkspaceMachineEndpointResourceType, StringComparison.OrdinalIgnoreCase)
                || iw.Type.Equals(CommonConstants.MMAWorkspaceNetworkEndpointResourceType, StringComparison.OrdinalIgnoreCase)))
                ?.ForEach(testGrp =>
                {
                    //copyableTestGrp = copyableTestGrp ?? GetCopiedTestGroupObject(testGrp);
                    //copyableEndpoints = copyableEndpoints ?? new List<PSNetworkWatcherConnectionMonitorEndpointObject>();
                    //copyableTestGrp.Destinations = new List<PSNetworkWatcherConnectionMonitorEndpointObject>();

                    List<PSNetworkWatcherConnectionMonitorEndpointObject> destinationMMAWorkspaceNetworkEndpoints = new List<PSNetworkWatcherConnectionMonitorEndpointObject>();
                    dictDestinationMMANetworkIdToSubnetAddresses = dictDestinationMMANetworkIdToSubnetAddresses ?? new Dictionary<string, List<PSNetworkWatcherConnectionMonitorEndpointObject>>();

                    testGrp.Destinations.Where(d => d.Type.Equals(CommonConstants.MMAWorkspaceMachineEndpointResourceType, StringComparison.OrdinalIgnoreCase)
                    || d.Type.Equals(CommonConstants.MMAWorkspaceNetworkEndpointResourceType, StringComparison.OrdinalIgnoreCase))
                    .ForEach(destination =>
                    {
                        if (data.EndpointWithNetworkAgentData.ContainsKey(destination.ResourceId)
                        && data.EndpointWithNetworkAgentData[destination.ResourceId]?.Results?.Count() > 0)
                        {
                            bool isUpdatedEndpoint = UpdateTestGrpEndpoints(destination, data.EndpointWithNetworkAgentData,
                                   destinationMMAWorkspaceNetworkEndpoints, dictDestinationMMANetworkIdToSubnetAddresses);

                            if (isUpdatedEndpoint && !UpdatedArcCmList.Any(a => a.Id.Equals(data.Cm.Id, StringComparison.OrdinalIgnoreCase)))
                            {
                                UpdatedArcCmList.Add(data.Cm);
                            }
                        }
                    });
                });

                //Sources update
                data.Cm.TestGroups.Where(w => w.Sources.Any(iw => iw.Type.Equals(CommonConstants.MMAWorkspaceMachineEndpointResourceType, StringComparison.OrdinalIgnoreCase)
                || iw.Type.Equals(CommonConstants.MMAWorkspaceNetworkEndpointResourceType, StringComparison.OrdinalIgnoreCase)))
                ?.ForEach(testGrp =>
                {
                    List<PSNetworkWatcherConnectionMonitorEndpointObject> sourceMMAWorkspaceNetworkEndpoints = new List<PSNetworkWatcherConnectionMonitorEndpointObject>();
                    dictSourcesMMANetworkIdToSubnetAddresses = dictSourcesMMANetworkIdToSubnetAddresses ?? new Dictionary<string, List<PSNetworkWatcherConnectionMonitorEndpointObject>>();

                    testGrp.Sources.Where(d => d.Type.Equals(CommonConstants.MMAWorkspaceMachineEndpointResourceType, StringComparison.OrdinalIgnoreCase)
                    || d.Type.Equals(CommonConstants.MMAWorkspaceNetworkEndpointResourceType, StringComparison.OrdinalIgnoreCase))
                    .ForEach(source =>
                    {
                        if (data.EndpointWithNetworkAgentData.ContainsKey(source.ResourceId)
                        && data.EndpointWithNetworkAgentData[source.ResourceId]?.Results?.Count() > 0)
                        {
                            bool isUpdatedEndpoint = UpdateTestGrpEndpoints(source, data.EndpointWithNetworkAgentData,
                                sourceMMAWorkspaceNetworkEndpoints, dictSourcesMMANetworkIdToSubnetAddresses);

                            if (isUpdatedEndpoint && !UpdatedArcCmList.Any(a => a.Id.Equals(data.Cm.Id, StringComparison.OrdinalIgnoreCase)))
                            {
                                UpdatedArcCmList.Add(data.Cm);
                            }
                        }
                    });

                    // For MMAWorkspaceNetwork
                    // Need to Check this condition as it required extra check or not
                    if (data.Cm.Endpoints.Any(a => a.Type.Equals(CommonConstants.MMAWorkspaceNetworkEndpointResourceType, StringComparison.OrdinalIgnoreCase))
                    || testGrp.Sources.Any(a => a.Type.Equals(CommonConstants.MMAWorkspaceNetworkEndpointResourceType, StringComparison.OrdinalIgnoreCase))
                    || testGrp.Destinations.Any(a => a.Type.Equals(CommonConstants.MMAWorkspaceNetworkEndpointResourceType, StringComparison.OrdinalIgnoreCase))
                    )
                    {
                        //Remove the MMAWorkspaceNetwork endpoint and update the sources endpoints
                        UpdateMmaNetworkTestGroup(testGrp.Sources, dictSourcesMMANetworkIdToSubnetAddresses);

                        //Remove the MMAWorkspaceNetwork endpoint and update the Destination Endpoints
                        UpdateMmaNetworkTestGroup(testGrp.Destinations, dictDestinationMMANetworkIdToSubnetAddresses);

                        //updating the endpoints
                        UpdatedMMaNetworkEndpointCollection(data.Cm, dictSourcesMMANetworkIdToSubnetAddresses, dictDestinationMMANetworkIdToSubnetAddresses);
                    }
                });

                //Need to discuss update endpoints reference from cm data.
            });

            // WriteInformation($"After Modification in CMs: \n{JsonConvert.SerializeObject(getCmWithEndpointsAndNetworkAgentDataList, Formatting.Indented)}\n", new string[] { "PSHOST" });

            return UpdatedArcCmList;
        }

        private bool UpdateTestGrpEndpoints(PSNetworkWatcherConnectionMonitorEndpointObject testGrpEndpoint,
            Dictionary<string, Azure.OperationalInsights.Models.QueryResults> EndpointQueryResult,
            List<PSNetworkWatcherConnectionMonitorEndpointObject> mmaWorkspaceNetworkEndpoints,
            Dictionary<string, List<PSNetworkWatcherConnectionMonitorEndpointObject>> dictMMANetworkIdToSubnetAddresses)
        {
            var queryResult = EndpointQueryResult[testGrpEndpoint.ResourceId]?.Results;
            if (queryResult != null)
            {
                bool isUpdated = false;
                // For MMAWorkspaceNetwork
                if (testGrpEndpoint.Type.Equals(CommonConstants.MMAWorkspaceNetworkEndpointResourceType, StringComparison.OrdinalIgnoreCase)
                && testGrpEndpoint?.Scope?.Include != null)
                {
                    var matchedArcSubnetAddressesList = queryResult.Where(w =>
                    testGrpEndpoint?.Scope?.Include.Any(a => a.Address.Equals(w["SubnetId"], StringComparison.OrdinalIgnoreCase)) == true);

                    //Need to discuss a point here, if not a single subnet is matched with include addresses

                    PSNetworkWatcherConnectionMonitorEndpointObject endpointObject = null;
                    int i = 0;
                    foreach (var arcDetail in matchedArcSubnetAddressesList)
                    {
                        endpointObject = new PSNetworkWatcherConnectionMonitorEndpointObject
                        {
                            Address = arcDetail["AgentIP"],
                            Name = $"{arcDetail["AgentFqdn"]}-{i++}",
                            //Need to check for resourceId and other field as well
                            Type = CommonConstants.AzureArcVMType,
                        };

                        mmaWorkspaceNetworkEndpoints.Add(endpointObject);
                        if (!dictMMANetworkIdToSubnetAddresses.ContainsKey(testGrpEndpoint?.ResourceId))
                        {
                            dictMMANetworkIdToSubnetAddresses.Add(testGrpEndpoint?.ResourceId, mmaWorkspaceNetworkEndpoints);
                        }
                    }
                    //Check for MMANetwork CM entry
                    isUpdated = true;
                }
                else
                {
                    //Need to check, for MMaWorkSpaceMachine, we are using first records and updating the data
                    var firstResult = queryResult.FirstOrDefault();
                    if (firstResult != null && !string.IsNullOrEmpty(firstResult["ResourceId"]))
                    {
                        testGrpEndpoint.ResourceId = firstResult["ResourceId"];
                        testGrpEndpoint.Name = firstResult["AgentFqdn"];
                        testGrpEndpoint.Type = CommonConstants.AzureArcVMType;
                        // For AzureArcVM type, address properties not required, passing empty
                        testGrpEndpoint.Address = string.Empty;

                        isUpdated = true;
                    }
                }

                return isUpdated;
            }

            return false;
        }

        private PSNetworkWatcherConnectionMonitorTestGroupObject GetCopiedTestGroupObject(PSNetworkWatcherConnectionMonitorTestGroupObject currentTestGrpObject)
        {
            return new PSNetworkWatcherConnectionMonitorTestGroupObject
            {
                Name = currentTestGrpObject.Name,
                Disable = currentTestGrpObject.Disable,
                TestConfigurations = currentTestGrpObject.TestConfigurations,
            };
        }

        private void UpdateMmaNetworkTestGroup(List<PSNetworkWatcherConnectionMonitorEndpointObject> endpoints, Dictionary<string, List<PSNetworkWatcherConnectionMonitorEndpointObject>> dict)
        {
            var matchedItems = dict?
                .Where(w => endpoints.Any(a => a?.ResourceId.Equals(w.Key, StringComparison.OrdinalIgnoreCase) == true
                    && a.Type.Equals(CommonConstants.MMAWorkspaceNetworkEndpointResourceType, StringComparison.OrdinalIgnoreCase)))?.ToList();

            if (matchedItems != null && matchedItems.Count > 0)
            {
                endpoints.AddRange(matchedItems.SelectMany(s => s.Value).Distinct());
                endpoints.RemoveAll(r => matchedItems.Any(a => a.Key.Equals(r?.ResourceId, StringComparison.OrdinalIgnoreCase) == true));
            }
        }

        private void UpdatedMMaNetworkEndpointCollection(PSNetworkWatcherMmaWorkspaceMachineConnectionMonitor cm,
        Dictionary<string, List<PSNetworkWatcherConnectionMonitorEndpointObject>> sourceDict,
        Dictionary<string, List<PSNetworkWatcherConnectionMonitorEndpointObject>> destinationDict)
        {
            var combinedList = new List<KeyValuePair<string, List<PSNetworkWatcherConnectionMonitorEndpointObject>>>();

            if (sourceDict != null)
            {
                combinedList.AddRange(sourceDict);
            }

            if (destinationDict != null)
            {
                // Only add entries from dict2 that do not exist in dict1
                combinedList.AddRange(destinationDict.Where(x => !combinedList.Any(a => a.Key.Contains(x.Key))));
            }

            // Update the endpoints collections
            cm.Endpoints.AddRange(combinedList.SelectMany(s => s.Value).Distinct()?.ToList());

            // removing the MMANetwork endpoint from endpoint collection
            cm.Endpoints.RemoveAll(r => sourceDict?.Any(a => a.Key.Equals(r?.ResourceId, StringComparison.OrdinalIgnoreCase) == true) == true
            || destinationDict?.Any(a => a.Key.Equals(r?.ResourceId, StringComparison.OrdinalIgnoreCase) == true) == true);
        }

        private IEnumerable<PSNetworkWatcherConnectionMonitorEndpointObject> GetAllMMAEndpoints(IEnumerable<PSNetworkWatcherMmaWorkspaceMachineConnectionMonitor> mmaMachineCMs)
        {
            var cmEndPoints = mmaMachineCMs?.Select(s => s.Endpoints);
            var cmAllMMAEndpoints = cmEndPoints?.SelectMany(s => s.Where(w => w != null && (w.Type.Equals(CommonConstants.MMAWorkspaceMachineEndpointResourceType, StringComparison.OrdinalIgnoreCase)
            || w.Type.Equals(CommonConstants.MMAWorkspaceNetworkEndpointResourceType, StringComparison.OrdinalIgnoreCase))));
            return cmAllMMAEndpoints;
        }

        protected void QueryForArg(string query)
        {
            ResourceGraphClient rgClient = AzureSession.Instance.ClientFactory.CreateArmClient<ResourceGraphClient>(DefaultContext, AzureEnvironment.Endpoint.ResourceManager);
            QueryRequest request = new QueryRequest
            {
                Query = query
            };
            QueryResponse response = rgClient.Resources(request);
            var data = JsonConvert.DeserializeObject<object>(response.Data.ToString());
            WriteInformation($"======================Arc resources details===============================\n{JsonConvert.SerializeObject(data, Formatting.Indented)}\n", new string[] { "PSHOST" });
        }

        /// <summary>
        /// For testing the LA workspace data, just pass Query and work space Id guid for getting data by passing the Query or using hardcoded one
        /// </summary>
        protected void QueryForLaWorkSpace(string workspaceId, string query)
        {
            IList<string> workspaces = new List<string>() { workspaceId };
            OperationalInsightsDataClient.WorkspaceId = workspaceId;
            var data = OperationalInsightsDataClient.Query(query ?? CommonConstants.Query, CommonConstants.TimeSpanForLAQuery, workspaces);
            var resultData = data.Results;
            WriteInformation($"{JsonConvert.SerializeObject(resultData.ToList(), Formatting.Indented)}\n", new string[] { "PSHOST" });
        }

        private async Task<List<Azure.OperationalInsights.Models.QueryResults>> QueryForLaWorkSpaceNetworkAgentData(IEnumerable<PSNetworkWatcherConnectionMonitorEndpointObject> allDistantCMEndpoints)
        {
            IEnumerable<PSNetworkWatcherConnectionMonitorEndpointObject> endpointsGroupedBySubsAndRG = GetGroupedByDistinctEndpoints(allDistantCMEndpoints);
            var arcResourceIdDetails = endpointsGroupedBySubsAndRG?.Select(endPointObj => GetNetworkingDataAsync(endPointObj));
            var getAllArcResourceDetails = await Task.WhenAll(arcResourceIdDetails);
            return getAllArcResourceDetails?.ToList();
        }

        private Dictionary<string, Task<Azure.OperationalInsights.Models.QueryResults>> workSpaceArcDetails = new Dictionary<string, Task<Azure.OperationalInsights.Models.QueryResults>>(StringComparer.OrdinalIgnoreCase);
        private async Task<Dictionary<string, Azure.OperationalInsights.Models.QueryResults>> QueryForLaWorkSpaceNetworkAgentData1(IEnumerable<PSNetworkWatcherConnectionMonitorEndpointObject> allDistantCMEndpoints)
        {
            IEnumerable<PSNetworkWatcherConnectionMonitorEndpointObject> endpointsGroupedBySubsAndRG = GetGroupedByDistinctEndpoints(allDistantCMEndpoints);
            // Same resource Id can be used in endpoints, fetching distinct resourceIds endpoints
            var endpointsDistinctResourceId = endpointsGroupedBySubsAndRG.GroupBy(g => g.ResourceId).Select(s => s.FirstOrDefault());

            foreach (var endpoint in endpointsDistinctResourceId)
            {
                if (!workSpaceArcDetails.ContainsKey(endpoint?.ResourceId))
                {
                    workSpaceArcDetails.Add(endpoint?.ResourceId, GetNetworkingDataAsync(endpoint));
                }
            }

            var queryResults = await Task.WhenAll(workSpaceArcDetails.Values);
            var resultDictionary = workSpaceArcDetails.Keys.Zip(queryResults, (key, value) => new { key, value }).ToDictionary(x => x.key, x => x.value);
            return resultDictionary;
        }

        private async Task<Dictionary<string, List<NetworkAgentDetails>>> FetchArcMachineDetails(IEnumerable<PSNetworkWatcherConnectionMonitorEndpointObject> allDistantCMEndpoints)
        {
            Dictionary<string, Azure.OperationalInsights.Models.QueryResults> response = await QueryForLaWorkSpaceNetworkAgentData1(allDistantCMEndpoints);

            Dictionary<string, List<NetworkAgentDetails>> resultDictionary = new Dictionary<string, List<NetworkAgentDetails>>();

            response = response.Where(data => data.Value.Results.Count() > 0).ToDictionary(data => data.Key, data => data.Value);

            response.ForEach(data => resultDictionary.Add(data.Key, GetNetworkAgentDetails(data.Value)));

            return resultDictionary;
        }

        private List<NetworkAgentDetails> GetNetworkAgentDetails(Azure.OperationalInsights.Models.QueryResults queryResults)
        {
            List<NetworkAgentDetails> data = new List<NetworkAgentDetails>();
            queryResults.Results.ForEach(result => data.Add(new NetworkAgentDetails()
            {
                ResourceId = result["ResourceId"].ToString(),
                AgentId = result["AgentId"].ToString(),
                SubnetId = result["SubnetId"].ToString(),
                AgentIP = result["AgentIP"].ToString(),
                AgentFqdn = result["AgentFqdn"].ToString()
            }));

            return data;
        }


        private static IEnumerable<PSNetworkWatcherConnectionMonitorEndpointObject> GetGroupedByDistinctEndpoints(IEnumerable<PSNetworkWatcherConnectionMonitorEndpointObject> allDistantCMEndpoints)
        {
            return allDistantCMEndpoints?.GroupBy(g => new
            {
                subs = NetworkWatcherUtility.GetSubscription(g.ResourceId),
                rg = NetworkWatcherUtility.GetResourceValue(g.ResourceId, "/resourceGroups")
            }).OrderBy(g => g.Key.subs).ThenBy(g => g.Key.rg).SelectMany(g => g).Distinct();
        }

        private async Task<Azure.OperationalInsights.Models.QueryResults> GetNetworkingDataAsync(PSNetworkWatcherConnectionMonitorEndpointObject cmEndpoint)
        {
            try
            {
                return await GetEndpointNetworkAgentData(cmEndpoint);
            }
            catch (Exception ex)
            {
                WriteInformation($"This is error while performing on this resource Id {cmEndpoint.ResourceId}, Error:  {ex}", new string[] { "PSHOST" });
                return null;
            }
        }

        private async Task<Azure.OperationalInsights.Models.QueryResults> GetEndpointNetworkAgentData(PSNetworkWatcherConnectionMonitorEndpointObject addressToWorkSpace)
        {
            IList<string> workspaces = new List<string>() { addressToWorkSpace.ResourceId };
            string subscriptionId = NetworkWatcherUtility.GetSubscription(addressToWorkSpace.ResourceId);
            string workSpaceRG = NetworkWatcherUtility.GetResourceValue(addressToWorkSpace.ResourceId, "/resourceGroups");
            if (DefaultContext.Subscription.Id != subscriptionId)
            {
                DefaultContext.Subscription.Id = subscriptionId;
                _operationalInsightsDataClient = null;
                operationalInsightsClient = null;
                _armClient = null;
            }

            bool isRGExists = ArmClient.ResourceGroups.CheckExistence(workSpaceRG);
            if (!isRGExists || !OperationalInsightsClient.FilterPSWorkspaces(workSpaceRG, null)?.Any(a => a.ResourceId.Equals(addressToWorkSpace?.ResourceId, StringComparison.InvariantCultureIgnoreCase)) == true)
            {
                WriteInformation($"Please remove or update this endpoint, this workspace resource '{addressToWorkSpace.ResourceId}' doesn't exist and it's being used in this endpoint.\n Endpoint Details :\n{JsonConvert.SerializeObject(addressToWorkSpace, Formatting.Indented)}\n", new string[] { "PSHOST" });
                return null;
            }

            OperationalInsightsDataClient.WorkspaceId = addressToWorkSpace.ResourceId;
            return await OperationalInsightsDataClient.QueryAsync(CommonConstants.Query, CommonConstants.TimeSpanForLAQuery, workspaces);
        }

        /// <summary>
        /// List All Subscriptions For User Tenant
        /// </summary>
        /// <param name="tenantId">User Tenant ID</param>
        /// <param name="profile">IProfileOperations object</param>
        /// <param name="cache">IAzureTokenCache object</param>
        /// <returns>collection of AzureSubscription</returns>
        private IEnumerable<AzureSubscription> ListAllSubscriptionsForTenant(string tenantId, IProfileOperations profile, IAzureTokenCache cache)
        {
            IAzureAccount account = profile.DefaultContext.Account;
            IAzureEnvironment environment = profile.DefaultContext.Environment;
            SecureString password = null;
            string promptBehavior = ShowDialog.Never;
            IAccessToken accessToken = null;
            try
            {
                accessToken = AcquireAccessToken(account, environment, tenantId, password, promptBehavior, null, cache);
            }
            catch (Exception e)
            {
                WriteWarningMessage(e.Message);
                //WriteDebugMessage(string.Format(ProfileMessages.UnableToAqcuireToken, tenantId, e.ToString()));
                return new List<AzureSubscription>();
            }

            return SubscriptionAndTenantClient?.ListAllSubscriptionsForTenant(accessToken, account, environment);
        }

        private IAccessToken AcquireAccessToken(IAzureAccount account, IAzureEnvironment environment, string tenantId, SecureString password,
           string promptBehavior, Action<string> promptAction, IAzureTokenCache cache, string resourceId = AzureEnvironment.Endpoint.ActiveDirectoryServiceEndpointResourceId)
        {
            if (account.Type.Equals(AzureAccount.AccountType.AccessToken, StringComparison.OrdinalIgnoreCase))
            {
                tenantId = tenantId ?? account.GetCommonTenant();
                return new SimpleAccessToken(account, tenantId);
            }

            return AzureSession.Instance.AuthenticationFactory.Authenticate(
                account,
                environment,
                tenantId,
                password,
                promptBehavior,
                promptAction,
                cache,
                resourceId);
        }

        /// <summary>
        /// Write warning message
        /// </summary>
        /// <param name="message">warning message</param>
        private void WriteWarningMessage(string message)
        {
            if (WarningLog != null)
            {
                WarningLog(message);
            }
        }

        /// <summary>
        /// Get the subscription id from resource id
        /// </summary>
        /// <param name="resourceId">resource id</param>
        /// <returns>subscription id</returns>
        /// <exception cref="ArgumentException"></exception>
        private static string GetSubscriptionIdByResourceId(string resourceId)
        {
            string[] array = resourceId.Split(new char[1] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (array.Length < 8)
            {
                throw new ArgumentException("Invalid format of the resource identifier.", "idFromServer");
            }

            return array[1];
        }

        private PSNetworkWatcherConnectionMonitorProtocolConfiguration GetPSProtocolConfiguration(ConnectionMonitorTestConfiguration testConfiguration)
        {
            if (testConfiguration.TcpConfiguration != null)
            {
                return new PSNetworkWatcherConnectionMonitorTcpConfiguration()
                {
                    Port = testConfiguration.TcpConfiguration.Port,
                    DisableTraceRoute = testConfiguration.TcpConfiguration.DisableTraceRoute,
                    DestinationPortBehavior = testConfiguration.TcpConfiguration.DestinationPortBehavior
                };
            }

            if (testConfiguration.HttpConfiguration != null)
            {
                return new PSNetworkWatcherConnectionMonitorHttpConfiguration()
                {
                    Port = testConfiguration.HttpConfiguration.Port,
                    Method = testConfiguration.HttpConfiguration.Method,
                    Path = testConfiguration.HttpConfiguration.Path,
                    PreferHTTPS = testConfiguration.HttpConfiguration.PreferHTTPS,
                    ValidStatusCodeRanges = testConfiguration.HttpConfiguration.ValidStatusCodeRanges?.ToList(),
                    RequestHeaders = this.GetPSRequestHeaders(testConfiguration.HttpConfiguration.RequestHeaders?.ToList())
                };
            }

            if (testConfiguration.IcmpConfiguration != null)
            {
                return new PSNetworkWatcherConnectionMonitorIcmpConfiguration()
                {
                    DisableTraceRoute = testConfiguration.IcmpConfiguration.DisableTraceRoute
                };
            }

            return null;
        }

        private List<PSHTTPHeader> GetPSRequestHeaders(List<HTTPHeader> headers)
        {
            if (headers == null)
            {
                return null;
            }

            List<PSHTTPHeader> psHeaders = new List<PSHTTPHeader>();
            foreach (HTTPHeader header in headers)
            {
                psHeaders.Add(
                    new PSHTTPHeader()
                    {
                        Name = header.Name,
                        Value = header.Value
                    });
            }

            return psHeaders;
        }

        private List<HTTPHeader> GetRequestHeaders(List<PSHTTPHeader> psHeaders)
        {
            if (psHeaders == null)
            {
                return null;
            }

            List<HTTPHeader> headers = new List<HTTPHeader>();
            foreach (PSHTTPHeader header in psHeaders)
            {
                headers.Add(
                    new HTTPHeader()
                    {
                        Name = header.Name,
                        Value = header.Value
                    });
            }

            return headers;
        }

        private OperationalInsightsDataClient _operationalInsightsDataClient;
        private OperationalInsightsDataClient OperationalInsightsDataClient
        {
            get
            {
                if (_operationalInsightsDataClient == null)
                {
                    ServiceClientCredentials clientCredentials = AzureSession.Instance.AuthenticationFactory.GetServiceClientCredentials(DefaultContext, AzureEnvironment.ExtendedEndpoint.OperationalInsightsEndpoint);

                    _operationalInsightsDataClient =
                        AzureSession.Instance.ClientFactory.CreateCustomArmClient<OperationalInsightsDataClient>(clientCredentials);
                    _operationalInsightsDataClient.Preferences.IncludeRender = false;
                    _operationalInsightsDataClient.Preferences.IncludeStatistics = false;
                    _operationalInsightsDataClient.NameHeader = "LogAnalyticsPSClient";

                    Uri targetUri = null;
                    DefaultContext.Environment.TryGetEndpointUrl(
                        AzureEnvironment.ExtendedEndpoint.OperationalInsightsEndpoint, out targetUri);
                    if (targetUri == null)
                    {
                        throw new Exception("Operational Insights is not supported in this Azure Environment");
                    }

                    _operationalInsightsDataClient.BaseUri = targetUri;

                    if (targetUri.AbsoluteUri.Contains("localhost"))
                    {
                        ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                    }
                }

                return _operationalInsightsDataClient;
            }
            set
            {
                _operationalInsightsDataClient = value;
            }
        }

        private OperationalInsightsClient operationalInsightsClient;
        private OperationalInsightsClient OperationalInsightsClient
        {
            get
            {
                if (operationalInsightsClient == null)
                {
                    operationalInsightsClient = new OperationalInsightsClient(DefaultProfile.DefaultContext);
                }

                return operationalInsightsClient;
            }
            set
            {
                operationalInsightsClient = value;
            }
        }

        private ResourceManagementClient _armClient;

        //private MemoryCache EndpointNetworkAgentCache = MemoryCache.Default;
        Dictionary<string, Azure.OperationalInsights.Models.QueryResults> EndpointNetworkAgentCache = new Dictionary<string, Azure.OperationalInsights.Models.QueryResults>() { };

    }

    public static class ObjectExtensions
    {
        private static readonly MethodInfo CloneMethod = typeof(Object).GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance);

        public static bool IsPrimitive(this Type type)
        {
            if (type == typeof(String)) return true;
            return (type.IsValueType & type.IsPrimitive);
        }

        public static Object Copy(this Object originalObject)
        {
            return InternalCopy(originalObject, new Dictionary<Object, Object>(new ReferenceEqualityComparer()));
        }
        private static Object InternalCopy(Object originalObject, IDictionary<Object, Object> visited)
        {
            if (originalObject == null) return null;
            var typeToReflect = originalObject.GetType();
            if (IsPrimitive(typeToReflect)) return originalObject;
            if (visited.ContainsKey(originalObject)) return visited[originalObject];
            if (typeof(Delegate).IsAssignableFrom(typeToReflect)) return null;
            var cloneObject = CloneMethod.Invoke(originalObject, null);
            if (typeToReflect.IsArray)
            {
                var arrayType = typeToReflect.GetElementType();
                if (IsPrimitive(arrayType) == false)
                {
                    Array clonedArray = (Array)cloneObject;
                    clonedArray.ForEach((array, indices) => array.SetValue(InternalCopy(clonedArray.GetValue(indices), visited), indices));
                }

            }
            visited.Add(originalObject, cloneObject);
            CopyFields(originalObject, visited, cloneObject, typeToReflect);
            RecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect);
            return cloneObject;
        }

        private static void RecursiveCopyBaseTypePrivateFields(object originalObject, IDictionary<object, object> visited, object cloneObject, Type typeToReflect)
        {
            if (typeToReflect.BaseType != null)
            {
                RecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect.BaseType);
                CopyFields(originalObject, visited, cloneObject, typeToReflect.BaseType, BindingFlags.Instance | BindingFlags.NonPublic, info => info.IsPrivate);
            }
        }

        private static void CopyFields(object originalObject, IDictionary<object, object> visited, object cloneObject, Type typeToReflect, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy, Func<FieldInfo, bool> filter = null)
        {
            foreach (FieldInfo fieldInfo in typeToReflect.GetFields(bindingFlags))
            {
                if (filter != null && filter(fieldInfo) == false) continue;
                if (IsPrimitive(fieldInfo.FieldType)) continue;
                var originalFieldValue = fieldInfo.GetValue(originalObject);
                var clonedFieldValue = InternalCopy(originalFieldValue, visited);
                fieldInfo.SetValue(cloneObject, clonedFieldValue);
            }
        }
        public static T Copy<T>(this T original)
        {
            return (T)Copy((Object)original);
        }
    }

    public class ReferenceEqualityComparer : EqualityComparer<Object>
    {
        public override bool Equals(object x, object y)
        {
            return ReferenceEquals(x, y);
        }
        public override int GetHashCode(object obj)
        {
            if (obj == null) return 0;
            return obj.GetHashCode();
        }
    }

    namespace ArrayExtensions
    {
        public static class ArrayExtensions
        {
            public static void ForEach(this Array array, Action<Array, int[]> action)
            {
                if (array.LongLength == 0) return;
                ArrayTraverse walker = new ArrayTraverse(array);
                do action(array, walker.Position);
                while (walker.Step());
            }
        }

        internal class ArrayTraverse
        {
            public int[] Position;
            private int[] maxLengths;

            public ArrayTraverse(Array array)
            {
                maxLengths = new int[array.Rank];
                for (int i = 0; i < array.Rank; ++i)
                {
                    maxLengths[i] = array.GetLength(i) - 1;
                }
                Position = new int[array.Rank];
            }

            public bool Step()
            {
                for (int i = 0; i < Position.Length; ++i)
                {
                    if (Position[i] < maxLengths[i])
                    {
                        Position[i]++;
                        for (int j = 0; j < i; j++)
                        {
                            Position[j] = 0;
                        }
                        return true;
                    }
                }
                return false;
            }
        }
    }
}