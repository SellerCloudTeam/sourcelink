using Microsoft.TeamFoundation.VersionControl.Client;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Microsoft.Build.Tasks.Tfvc.IntegrationTests
{
    public class TfvcTasksTests
    {
        [Theory]
        [InlineData(@"C:\Data\Core", @"C:\Data\Core")]
        [InlineData(@"C:\Data\Core\CoreWebAdmin", @"C:\Data\Core")]
        public void Tfvc_LocateRepository_Task(string folderPath, string workspaceFolderPath)
        {
            // Enable the TFS cache
            Workstation.CacheEnabled = true;

            LocateRepository locateRepository = new LocateRepository
            {
                Directory = folderPath
            };

            locateRepository.Execute();

            string? result = locateRepository.Id;

            Assert.NotNull(result);
            Assert.Equal(workspaceFolderPath, result);
        }

        [Theory]
        [InlineData(@"https://tfs.sellercloud.com/tfs/DefaultCollection", @"C:\Data\Core", @"C:\Data\Core")]
        [InlineData(@"https://tfs.sellercloud.com/tfs/DefaultCollection", @"C:\Data\Core\CoreWebAdmin", @"C:\Data\Core")]
        public void Tfvc_LocateRepository_Task_WithoutCache_WithTfsCollectionUrl(string tfsCollectionUrl, string folderPath, string workspaceFolderPath)
        {
            // Disable the TFS cache
            Workstation.CacheEnabled = false;

            LocateRepository locateRepository = new LocateRepository
            {
                TfsCollectionUrl = tfsCollectionUrl,
                Directory = folderPath
            };

            locateRepository.Execute();

            string? result = locateRepository.Id;

            Assert.NotNull(result);
            Assert.Equal(workspaceFolderPath, result);
        }

        [Theory]
        [InlineData(@"C:\Data\Core", @"C:\Data\Core\")]
        [InlineData(@"C:\Data\Core\CoreWebAdmin", @"C:\Data\Core\")]
        public void Tfvc_GetSourceRoots_Task(string workspaceFolderPath, string sourceRootPath)
        {
            // Enable the TFS cache
            Workstation.CacheEnabled = true;

            GetSourceRoots getSourceRoots = new GetSourceRoots
            {
                WorkspaceDirectory = workspaceFolderPath
            };

            getSourceRoots.Execute();

            IEnumerable<Framework.ITaskItem>? roots = getSourceRoots.Roots;

            Assert.NotNull(roots);
            Assert.NotNull(roots.First());
            Assert.Equal(sourceRootPath, roots.First().ItemSpec);
        }

        [Theory]
        [InlineData(@"C:\Data\Core", @"https://tfs.sellercloud.com/tfs/DefaultCollection/SellerCloud")]
        [InlineData(@"C:\Data\Core\CoreWebAdmin", @"https://tfs.sellercloud.com/tfs/DefaultCollection/SellerCloud")]
        public void Tfvc_GetRepositoryUrl_Task(string workspaceFolderPath, string repositoryUrl)
        {
            // Enable the TFS cache
            Workstation.CacheEnabled = true;

            GetRepositoryUrl getRepository = new GetRepositoryUrl
            {
                WorkspaceDirectory = workspaceFolderPath
            };

            getRepository.Execute();

            string? result = getRepository.Url;

            Assert.NotNull(result);
            Assert.Equal(repositoryUrl, result);
        }

        [Theory]
        [InlineData(@"https://tfs.sellercloud.com/tfs/DefaultCollection", @"C:\Data\Core", @"C:\Data\Core", @"https://tfs.sellercloud.com/tfs/DefaultCollection/SellerCloud")]
        [InlineData(@"https://tfs.sellercloud.com/tfs/DefaultCollection", @"C:\Data\Core\CoreWebAdmin", @"C:\Data\Core", @"https://tfs.sellercloud.com/tfs/DefaultCollection/SellerCloud")]
        public void Tfvc_GetRepositoryUrl_After_LocateRepository_Task_WithoutCache_WithTfsCollectionUrl(string tfsCollectionUrl, string folderPath, string workspaceFolderPath, string repositoryUrl)
        {
            this.Tfvc_LocateRepository_Task_WithoutCache_WithTfsCollectionUrl(tfsCollectionUrl, folderPath, workspaceFolderPath);

            this.Tfvc_GetRepositoryUrl_Task(workspaceFolderPath, repositoryUrl);
        }
    }
}
