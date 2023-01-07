// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the License.txt file in the project root for more information.

using System;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.Build.Tasks.Tfvc.Extensions
{
    public static class WorkstationExtensions
    {
        public static WorkspaceInfo? TryGetLocalWorkspaceInfoFromServer(this Workstation workstation, Uri tfsCollectionUri, string computer, string folderPath)
        {
            return workstation.TryGetLocalWorkspaceInfoFromServer(tfsCollectionUri, tfsCredentials: null, computer, folderPath);
        }

        public static WorkspaceInfo? TryGetLocalWorkspaceInfoFromServer(this Workstation workstation, Uri tfsCollectionUri, VssCredentials? tfsCredentials, string computer, string folderPath)
        {
            var cachedTfsCredentials = VssCredentials.LoadCachedCredentials(tfsCollectionUri, requireExactMatch: false);

            var tfs = new TfsTeamProjectCollection(tfsCollectionUri, tfsCredentials ?? cachedTfsCredentials);

            tfs.EnsureAuthenticated();

            var sourceControl = tfs.GetService<VersionControlServer>();

            // workstation.UpdateWorkspaceInfoCache(sourceControl, sourceControl.AuthorizedUser, out var worksspsss);

            var workspaceWithMappedPath = sourceControl.FindWorkspaceWithMappedPath(computer, folderPath);

            if (workspaceWithMappedPath == null)
            {
                return null;
            }

            var workspaceInfo = workstation.TryGetLocalWorkspaceInfoFromServer(sourceControl, workspaceWithMappedPath);

            return workspaceInfo;
        }

        private static WorkspaceInfo TryGetLocalWorkspaceInfoFromServer(this Workstation workstation, VersionControlServer sourceControl, Workspace workspace)
        {
            var workspaceName = workspace.Name;
            var workspaceOwner = workspace.OwnerName;

            workstation.UpdateWorkspaceInfoCache(sourceControl, workspaceOwner, out _);

            var workspaceInfo = workstation.GetLocalWorkspaceInfo(sourceControl, workspaceName, workspaceOwner);

            return workspaceInfo;
        }
    }
}

