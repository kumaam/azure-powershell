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
    [Cmdlet(VerbsCommon.New, ResourceManager.Common.AzureRMConstants.AzureRMPrefix + "Gallery", DefaultParameterSetName = "DefaultParameter", SupportsShouldProcess = true)]
    [OutputType(typeof(PSGallery))]
    public partial class NewAzureRmGallery : ComputeAutomationBaseCmdlet
    {
        public override void ExecuteCmdlet()
        {
            base.ExecuteCmdlet();
            ExecuteClientAction(() =>
            {
                if (ShouldProcess(this.Name, VerbsCommon.New))
                {
                    string resourceGroupName = this.ResourceGroupName; ;
                    string galleryName = this.Name;
                    Gallery gallery = new Gallery();
                    gallery.Location = this.Location;
                    if (this.IsParameterBound(c => c.Permission)){
                        gallery.SharingProfile = new SharingProfile();
                        gallery.SharingProfile.Permissions = this.Permission;
                    }

                    if (this.IsParameterBound(c => c.Description))
                    {
                        gallery.Description = this.Description;
                    }

                    if (this.IsParameterBound(c => c.Tag))
                    {
                        gallery.Tags = this.Tag.Cast<DictionaryEntry>().ToDictionary(ht => (string)ht.Key, ht => (string)ht.Value);
                    }

                    var result = GalleriesClient.CreateOrUpdate(resourceGroupName, galleryName, gallery);
                    var psObject = new PSGallery();
                    ComputeAutomationAutoMapperProfile.Mapper.Map<Gallery, PSGallery>(result, psObject);
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

        [Alias("GalleryName")]
        [Parameter(
            ParameterSetName = "DefaultParameter",
            Position = 1,
            Mandatory = true,
            ValueFromPipelineByPropertyName = true)]
        public string Name { get; set; }

        [Parameter(Mandatory = false, HelpMessage = "Run cmdlet in the background")]
        public SwitchParameter AsJob { get; set; }

        [Parameter(
            Mandatory = true,
            Position = 2,
            ValueFromPipelineByPropertyName = true)]
        [LocationCompleter("Microsoft.Compute/Galleries")]
        [ValidateNotNullOrEmpty]
        public string Location { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = true)]
        public string Description { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = true)]
        public Hashtable Tag { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "This property allows you to specify the permission of sharing gallery. Possible values are: 'Private' and 'Groups'.")]
        [PSArgumentCompleter("Private","Groups")]
        public string Permission { get; set; }
    }

    [Cmdlet(VerbsData.Update, ResourceManager.Common.AzureRMConstants.AzureRMPrefix + "Gallery", DefaultParameterSetName = "DefaultParameter", SupportsShouldProcess = true)]
    [OutputType(typeof(PSGallery))]
    public partial class UpdateAzureRmGallery : ComputeAutomationBaseCmdlet
    {
        public override void ExecuteCmdlet()
        {
            base.ExecuteCmdlet();
            ExecuteClientAction(() =>
            {
                if (ShouldProcess(this.Name, VerbsData.Update))
                {
                    string resourceGroupName;
                    string galleryName;
                    switch (this.ParameterSetName)
                    {
                        case "ResourceIdParameter":
                            resourceGroupName = GetResourceGroupName(this.ResourceId);
                            galleryName = GetResourceName(this.ResourceId, "Microsoft.Compute/Galleries");
                            break;
                        case "ObjectParameter":
                            resourceGroupName = GetResourceGroupName(this.InputObject.Id);
                            galleryName = GetResourceName(this.InputObject.Id, "Microsoft.Compute/Galleries");
                            break;
                        default:
                            resourceGroupName = this.ResourceGroupName;
                            galleryName = this.Name;
                            break;
                    }

                    Gallery gallery = new Gallery();

                    if (this.ParameterSetName == "ObjectParameter")
                    {
                        ComputeAutomationAutoMapperProfile.Mapper.Map<PSGallery, Gallery>(this.InputObject, gallery);
                    }
                    else
                    {
                        gallery = GalleriesClient.Get(resourceGroupName, galleryName);
                    }

                    if (this.IsParameterBound(c => c.Description))
                    {
                        gallery.Description = this.Description;
                    }

                    if (this.IsParameterBound(c => c.Tag))
                    {
                        gallery.Tags = this.Tag.Cast<DictionaryEntry>().ToDictionary(ht => (string)ht.Key, ht => (string)ht.Value);
                    }
                    if (this.IsParameterBound(c => c.Permission))
                    {
                        if (gallery.SharingProfile == null)
                        {
                            gallery.SharingProfile = new SharingProfile();
                        }
                        gallery.SharingProfile.Permissions = this.Permission;
                    }

                    SharingUpdate sharingUpdate = new SharingUpdate();
                    if (this.Share.IsPresent)
                    {
                        if (this.Reset.IsPresent)
                        {
                            // if sub or tenant is present return error 
                            if (this.IsParameterBound(c => c.Subscription) || this.IsParameterBound(c => c.Tenant))
                            {
                                throw new Exception("Parameter '-Reset' cannot be used with parameters '-Tenant' or '-Subscription'.");
                            }
                            else
                            {
                                sharingUpdate.OperationType = "Reset";
                            }
                        }
                        if (this.IsParameterBound(c => c.Subscription))
                        {
                            if (sharingUpdate.Groups == null)
                            {
                                sharingUpdate.Groups = new List<SharingProfileGroup>();
                            }
                            SharingProfileGroup sharingProfile = new SharingProfileGroup();
                            sharingProfile.Type = "Subscriptions";
                            sharingProfile.Ids = new List<string>();
                            foreach (var id in this.Subscription)
                            {
                                sharingProfile.Ids.Add(id);
                            }
                            sharingUpdate.Groups.Add(sharingProfile);
                        }
                        if (this.IsParameterBound(c => c.Tenant))
                        {
                            if (sharingUpdate.Groups == null)
                            {
                                sharingUpdate.Groups = new List<SharingProfileGroup>();
                            }
                            SharingProfileGroup sharingProfile = new SharingProfileGroup();
                            sharingProfile.Type = "AADTenants";
                            sharingProfile.Ids = new List<string>();
                            foreach (var id in this.Tenant)
                            {
                                sharingProfile.Ids.Add(id);
                            }
                            sharingUpdate.Groups.Add(sharingProfile);
                        }

                    }
                    else if (this.IsParameterBound(c => c.Subscription) || this.IsParameterBound(c => c.Tenant) || this.Reset.IsPresent)
                    {
                        throw new Exception("Parameters '-Subscription', '-Tenant', and '-Reset' must be used with '-Share' parameter.");
                    }
                    
                    var result = GalleriesClient.CreateOrUpdate(resourceGroupName, galleryName, gallery);
                    if (this.Share.IsPresent)
                    {
                        GallerySharingProfileClient.Update(resourceGroupName, galleryName, sharingUpdate);
                        result = GalleriesClient.Get(ResourceGroupName, galleryName);
                    }
                    var psObject = new PSGallery();
                    ComputeAutomationAutoMapperProfile.Mapper.Map<Gallery, PSGallery>(result, psObject);
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

        [Alias("GalleryName")]
        [Parameter(
            ParameterSetName = "DefaultParameter",
            Position = 1,
            Mandatory = true,
            ValueFromPipelineByPropertyName = true)]
        public string Name { get; set; }

        [Parameter(
            ParameterSetName = "ResourceIdParameter",
            Position = 0,
            Mandatory = true,
            ValueFromPipelineByPropertyName = true)]
        public string ResourceId { get; set; }

        [Alias("Gallery")]
        [Parameter(
            ParameterSetName = "ObjectParameter",
            Position = 0,
            Mandatory = true,
            ValueFromPipeline = true)]
        public PSGallery InputObject { get; set; }

        [Parameter(Mandatory = false, HelpMessage = "Run cmdlet in the background")]
        public SwitchParameter AsJob { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = true)]
        public string Description { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = true)]
        public Hashtable Tag { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "This property allows you to specify the permission of the sharing gallery. Possible values are: 'Private' and 'Groups'.")]
        [PSArgumentCompleter("Private", "Groups")]
        public string Permission { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "A list of subscription ids the gallery is aimed to be shared to.")]
        public string[] Subscription { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "A list of tenant ids the gallery is aimed to be shared to.")]
        public string[] Tenant { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "Update sharing profile of the gallery.")]
        public SwitchParameter Share { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "Resets the sharing permission of the gallery to 'Private'.")]
        public SwitchParameter Reset { get; set; }
    }
}
