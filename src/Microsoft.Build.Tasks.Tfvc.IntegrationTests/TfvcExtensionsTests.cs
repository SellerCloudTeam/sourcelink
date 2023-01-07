using Microsoft.TeamFoundation.VersionControl.Client;
using System.Linq;
using Xunit;
using System;
using Microsoft.Build.Tasks.Tfvc.Extensions;

namespace Microsoft.Build.Tasks.Tfvc.IntegrationTests
{
    public class TfvcExtensionsTests
    {
        [Theory]
        [InlineData(@"https://tfs.sellercloud.com/tfs/DefaultCollection", @"desktop-s5gen7e", @"C:\Data\Core", @"C:\Data\Core")]
        [InlineData(@"https://tfs.sellercloud.com/tfs/DefaultCollection", @"desktop-s5gen7e", @"C:\Data\Core\CoreWebAdmin", @"C:\Data\Core")]
        // [InlineData(@"https://tfs.sellercloud.com/tfs/DefaultCollection", @"dev-eu", @"D:\Bs\7\2\src\CoreWebAdmin", @"D:\Bs\7\2\src")]
        // [InlineData(@"https://tfs.sellercloud.com/tfs/DefaultCollection", @"dev-eu", @"D:\Ba\6\1\src\CoreWebAdmin", @"D:\Ba\6\1\src")]
        public void Tfvc_LocateRepository_Task_WithoutCache_WithTfsCollectionUrl(string tfsCollectionUrl, string computer, string folderPath, string workspaceFolderPath)
        {
            // Disable the TFS cache
            Workstation.CacheEnabled = false;

            var workstation = Workstation.Current;

            var workspaceInfo = workstation.TryGetLocalWorkspaceInfoFromServer(new Uri(tfsCollectionUrl), computer, folderPath);

            string? result = workspaceInfo?.MappedPaths?.FirstOrDefault();

            Assert.NotNull(workspaceInfo);
            Assert.NotNull(result);
            Assert.Equal(workspaceFolderPath, result);
        }

        // BUG: Workstation.UpdateWorkspaceInfoCache is tightly coupled with Environment.MachineName
    }
}
