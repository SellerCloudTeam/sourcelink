// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the License.txt file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Tasks.SourceControl;
using Microsoft.Build.Utilities;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace Microsoft.Build.Tasks.Tfvc
{
    public sealed class GetSourceRoots : RepositoryTask
    {
        /// <summary>
        /// Returns items describing source roots:
        /// 
        /// Metadata
        ///   SourceControl: "tfvc"
        ///   Identity: Directory.
        ///   CollectionUrl: Collection URL.
        ///   ProjectId: Project GUID.
        ///   ProjectName: Project Name.
        ///   RelativeUrl: Relative URL within the project.
        ///   RevisionId: Revision (changeset number).
        /// </summary>
        [Output]
        public ITaskItem[]? Roots { get; private set; }

        protected override bool Execute(WorkspaceInfo workspaceInfo)
        {
            var result = new List<TaskItem>();

            using var collection = new TfsTeamProjectCollection(workspaceInfo.ServerUri);

            var vcServer = collection.GetService<VersionControlServer>();
            var changesetId = vcServer.GetLatestChangesetId().ToString();

            var workspace = workspaceInfo.GetWorkspace(collection);
            var collectionUrl = collection.Uri.ToString();

            // TODO: eliminate redundant mappings - we can use RepositoryRoot calculation here
            // E.g. A\B -> $/X/A/B, A\C -> $/X/A/C can be reduced to A -> $/X/A

            foreach (var folder in workspace.Folders)
            {
                if (!folder.IsCloaked)
                {
                    var project = workspace.GetTeamProjectForLocalPath(folder.LocalItem);

                    // Extract GUID from ArtifactUri "vstfs:///Classification/TeamProject/{Guid}":
                    var projectId = Path.GetFileName(project.ArtifactUri.GetPath());

                    var projectName = project.Name;

                    // SourceLink.AzureRepos will map each source root to:
                    // {RepositoryUrl}/_versionControl?path={ServerPath}&version={RevisionId}

                    // SourceRoot paths are required to end with a slash or backslash
                    var sourceRoot = folder.LocalItem.EndsWithSeparator()
                        ? folder.LocalItem
                        : $"{folder.LocalItem}{Path.DirectorySeparatorChar}";
                    var item = new TaskItem(sourceRoot);
                    item.SetMetadata("SourceControl", "tfvc");
                    item.SetMetadata("CollectionUrl", collectionUrl);
                    item.SetMetadata("ProjectId", projectId);
                    item.SetMetadata("ProjectName", projectName);
                    item.SetMetadata("ServerPath", folder.ServerItem);
                    item.SetMetadata("RevisionId", changesetId);
                    result.Add(item);
                }
            }

            Roots = result.ToArray();
            return true;
        }
    }
}
