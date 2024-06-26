//
// Copyright (c) Microsoft and contributors.  All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//
// See the License for the specific language governing permissions and
// limitations under the License.
//

// Warning: This code was generated by a tool.
//
// Changes to this file may cause incorrect behavior and will be lost if the
// code is regenerated.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Microsoft.Azure.Commands.Compute.Automation.Models;
using Microsoft.Azure.Commands.ResourceManager.Common.ArgumentCompleters;
using Microsoft.Azure.Management.Compute;
using Microsoft.Azure.Management.Compute.Models;
using Microsoft.WindowsAzure.Commands.Utilities.Common;

namespace Microsoft.Azure.Commands.Compute.Automation
{
    [Cmdlet(VerbsSecurity.Grant, ResourceManager.Common.AzureRMConstants.AzureRMPrefix + "DiskAccess", DefaultParameterSetName = "DefaultParameter", SupportsShouldProcess = true)]
    [OutputType(typeof(PSAccessUri))]
    public partial class GrantAzureRmDiskAccess : ComputeAutomationBaseCmdlet
    {
        public override void ExecuteCmdlet()
        {
            base.ExecuteCmdlet();
            ExecuteClientAction(() =>
            {
                if (ShouldProcess(this.DiskName, VerbsSecurity.Grant))
                {
                    string resourceGroupName = this.ResourceGroupName;
                    string diskName = this.DiskName;
                    var grantAccessData = new GrantAccessData();
                    grantAccessData.Access = this.Access;
                    grantAccessData.DurationInSeconds = this.DurationInSecond;
                    grantAccessData.GetSecureVMGuestStateSAS = this.SecureVMGuestStateSAS;

                    var result = DisksClient.GrantAccess(resourceGroupName, diskName, grantAccessData);
                    var psObject = new PSAccessUri();
                    ComputeAutomationAutoMapperProfile.Mapper.Map<AccessUri, PSAccessUri>(result, psObject);
                    WriteObject(psObject);
                }
            });
        }

        [Parameter(
            ParameterSetName = "DefaultParameter",
            Position = 0,
            Mandatory = true,
            ValueFromPipelineByPropertyName = true)]
        [ResourceGroupCompleter]
        public string ResourceGroupName { get; set; }

        [Parameter(
            ParameterSetName = "DefaultParameter",
            Position = 1,
            Mandatory = true,
            ValueFromPipelineByPropertyName = true)]
        [ResourceNameCompleter("Microsoft.Compute/disks", "ResourceGroupName")]
        [Alias("Name")]
        public string DiskName { get; set; }

        [Parameter(
            ParameterSetName = "DefaultParameter",
            Position = 2,
            Mandatory = true)]
        public string Access { get; set; }

        [Parameter(
            ParameterSetName = "DefaultParameter",
            Position = 3,
            Mandatory = false)]
        public int DurationInSecond { get; set; }

        [Parameter(Mandatory = false, HelpMessage = "Run cmdlet in the background")]
        public SwitchParameter AsJob { get; set; }

        [Parameter(
            ParameterSetName = "DefaultParameter",
            HelpMessage = "Set this flag to true to get additional SAS for VM guest state.",
            Mandatory = false,
            ValueFromPipelineByPropertyName = true)]
        public SwitchParameter SecureVMGuestStateSAS { get; set; }
    }
}
