// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the License.txt file in the project root for more information.

using System;
using System.Linq;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace Microsoft.Build.Tasks.Tfvc.Extensions
{
    public static class VersionControlServerExtensions
    {
        public static Workspace FindWorkspaceWithMappedPath(this VersionControlServer sourceControl, string computer, string folderPath)
        {
            var workspacesOnComputer = sourceControl.QueryWorkspaces(workspaceName: null, workspaceOwner: null, computer: computer);

            var workspacesWithMappedPath = workspacesOnComputer
                .Where(workspace => workspace.IsLocalPathMapped(folderPath))
                .ToList();

            if (workspacesWithMappedPath.Count > 1)
            {
                throw new ApplicationException($"Found multiple, conflicting TFS workspaces with mapped directory '{folderPath}' on machine {computer}.");
            }

            return workspacesWithMappedPath.FirstOrDefault();
        }
    }
}

