﻿// ----------------------------------------------------------------------------------
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

using Microsoft.Azure.Management.Compute;
using Microsoft.Azure.Management.Compute.Models;
using Microsoft.Azure.PowerShell.Cmdlets.Compute.Helpers.Network;
using Microsoft.Azure.PowerShell.Cmdlets.Compute.Helpers.Network.Models;
using Microsoft.Azure.Management.Internal.Resources.Models;
using Microsoft.Azure.Commands.Common.Strategies;
using System.Collections.Generic;
using System;
using SubResource = Microsoft.Azure.Management.Compute.Models.SubResource;

namespace Microsoft.Azure.Commands.Compute.Strategies.ComputeRp
{
    static class VirtualMachineStrategy
    {
        public static ResourceStrategy<VirtualMachine> Strategy { get; }
            = ComputeStrategy.Create(
                provider: "virtualMachines",
                getOperations: client => client.VirtualMachines,
                getAsync: (o, p) => o.GetAsync(
                    p.ResourceGroupName, p.Name, null, p.CancellationToken),
                createOrUpdateAsync: (o, p) => o.CreateOrUpdateAsync(
                    p.ResourceGroupName, p.Name, p.Model, p.CancellationToken),
                createTime: c =>
                    c != null && c.OsProfile != null && c.OsProfile.WindowsConfiguration != null
                        ? 240
                        : 120);

        public static ResourceConfig<VirtualMachine> CreateVirtualMachineConfig(
            this ResourceConfig<ResourceGroup> resourceGroup,
            string name,
            ResourceConfig<NetworkInterface> networkInterface,
            ImageAndOsType imageAndOsType,
            string adminUsername,
            string adminPassword,
            string size,
            ResourceConfig<AvailabilitySet> availabilitySet,
            VirtualMachineIdentity identity,
            IEnumerable<int> dataDisks,
            IList<string> zones,
            bool ultraSSDEnabled,
            Func<IEngine, SubResource> proximityPlacementGroup,
            string hostId,
            string hostGroupId,
            string capacityReservationGroupId,
            string VmssId,
            string priority,
            string evictionPolicy,
            double? maxPrice,
            bool encryptionAtHostPresent,
            List<SshPublicKey> sshPublicKeys,
            string networkInterfaceDeleteOption = null,
            string osDiskDeleteOption = null,
            string dataDiskDeleteOption = null,
            string userData = null))

            => Strategy.CreateResourceConfig(
                resourceGroup: resourceGroup,
                name: name,
                createModel: engine => new VirtualMachine
                {
                    OsProfile = new OSProfile
                    {
                        ComputerName = name,
                        WindowsConfiguration = imageAndOsType?.CreateWindowsConfiguration(),
                        LinuxConfiguration = (imageAndOsType?.OsType != OperatingSystemTypes.Linux) ? null : new LinuxConfiguration
                        {
                            Ssh = new SshConfiguration(sshPublicKeys)
                        },
                        AdminUsername = adminUsername,
                        AdminPassword = adminPassword,
                    },
                    Identity = identity,
                    NetworkProfile = new Azure.Management.Compute.Models.NetworkProfile
                    {
                        NetworkInterfaces = new[]
                        {
                            engine.GetReference(networkInterface, networkInterfaceDeleteOption)
                        }
                    },
                    HardwareProfile = new HardwareProfile
                    {
                        VmSize = size
                    },
                    StorageProfile = new StorageProfile
                    {
                        ImageReference = imageAndOsType?.Image,
                        DataDisks = DataDiskStrategy.CreateDataDisks(
                            imageAndOsType?.DataDiskLuns, dataDisks, dataDiskDeleteOption)
                    },
                    AvailabilitySet = engine.GetReference(availabilitySet),
                    Zones = zones,
                    AdditionalCapabilities = ultraSSDEnabled ? new AdditionalCapabilities(true) : null,
                    ProximityPlacementGroup = proximityPlacementGroup(engine),
                    Host = string.IsNullOrEmpty(hostId) ? null : new SubResource(hostId),
                    VirtualMachineScaleSet = string.IsNullOrEmpty(VmssId) ? null : new SubResource(VmssId),
                    HostGroup = string.IsNullOrEmpty(hostGroupId) ? null : new SubResource(hostGroupId),
                    Priority = priority,
                    EvictionPolicy = evictionPolicy,
                    BillingProfile = (maxPrice == null) ? null : new BillingProfile(maxPrice),
                    SecurityProfile = (encryptionAtHostPresent == true) ? new SecurityProfile(encryptionAtHost: encryptionAtHostPresent) : null,
                    CapacityReservation = string.IsNullOrEmpty(capacityReservationGroupId) ? null : new CapacityReservationProfile
                    {
                        CapacityReservationGroup = new SubResource(capacityReservationGroupId)
                    },
                    UserData = userData
                });

        public static ResourceConfig<VirtualMachine> CreateVirtualMachineConfig(
            this ResourceConfig<ResourceGroup> resourceGroup,
            string name,
            ResourceConfig<NetworkInterface> networkInterface,
            OperatingSystemTypes osType,
            ResourceConfig<Disk> disk,
            string size,
            ResourceConfig<AvailabilitySet> availabilitySet,
            VirtualMachineIdentity identity,
            IEnumerable<int> dataDisks,
            IList<string> zones,
            bool ultraSSDEnabled,
            Func<IEngine, SubResource> proximityPlacementGroup,
            string hostId,
            string hostGroupId,
            string capacityReservationGroupId,
            string VmssId,
            string priority,
            string evictionPolicy,
            double? maxPrice,
            bool encryptionAtHostPresent,
            string networkInterfaceDeleteOption = null,
            string osDiskDeleteOption = null,
            string dataDiskDeleteOption = null,
            string userData = null
            )
            => Strategy.CreateResourceConfig(
                resourceGroup: resourceGroup,
                name: name,
                createModel: engine => new VirtualMachine
                {
                    NetworkProfile = new Microsoft.Azure.Management.Compute.Models.NetworkProfile
                    {
                        NetworkInterfaces = new[]
                        {
                            engine.GetReference(networkInterface, networkInterfaceDeleteOption)
                        }
                    },
                    HardwareProfile = new HardwareProfile
                    {
                        VmSize = size
                    },
                    StorageProfile = new StorageProfile
                    {
                        OsDisk = new OSDisk
                        {
                            Name = disk.Name,
                            CreateOption = DiskCreateOptionTypes.Attach,
                            OsType = osType,
                            ManagedDisk = engine.GetReference(disk, ultraSSDEnabled ? StorageAccountTypes.UltraSSDLRS : StorageAccountTypes.PremiumLRS),
                            DeleteOption = osDiskDeleteOption
                        },
                        DataDisks = DataDiskStrategy.CreateDataDisks(null, dataDisks, dataDiskDeleteOption)
                    },
                    Identity = identity,
                    AvailabilitySet = engine.GetReference(availabilitySet),
                    Zones = zones,
                    AdditionalCapabilities = ultraSSDEnabled ?  new AdditionalCapabilities(true)  : null,
                    ProximityPlacementGroup = proximityPlacementGroup(engine),
                    Host = string.IsNullOrEmpty(hostId) ? null : new SubResource(hostId),
                    VirtualMachineScaleSet = string.IsNullOrEmpty(VmssId) ? null : new SubResource(VmssId),
                    HostGroup = string.IsNullOrEmpty(hostGroupId) ? null : new SubResource(hostGroupId),
                    Priority = priority,
                    EvictionPolicy = evictionPolicy,
                    BillingProfile = (maxPrice == null) ? null : new BillingProfile(maxPrice),
                    SecurityProfile = (encryptionAtHostPresent == true) ? new SecurityProfile(encryptionAtHost: encryptionAtHostPresent) : null,
                    CapacityReservation = string.IsNullOrEmpty(capacityReservationGroupId) ? null : new CapacityReservationProfile
                    {
                        CapacityReservationGroup = new SubResource(capacityReservationGroupId)
                    },
                    UserData = userData
                });
    }
}
