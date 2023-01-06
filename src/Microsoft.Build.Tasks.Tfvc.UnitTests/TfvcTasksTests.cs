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
        [InlineData(@"C:\Data\Core", @"C:\Data\Core\")]
        [InlineData(@"C:\Data\Core\CoreWebAdmin", @"C:\Data\Core\")]
        public void Tfvc_GetSourceRoots_Task(string workspaceFolderPath, string sourceRootPath)
        {
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
        [InlineData(@"C:\Data\Core", @"https://tfs.sellercloud.com/tfs/defaultcollection/SellerCloud")]
        [InlineData(@"C:\Data\Core\CoreWebAdmin", @"https://tfs.sellercloud.com/tfs/defaultcollection/SellerCloud")]
        public void Tfvc_GetRepositoryUrl_Task(string workspaceFolderPath, string repositoryUrl)
        {
            GetRepositoryUrl getRepository = new GetRepositoryUrl
            {
                WorkspaceDirectory = workspaceFolderPath
            };

            getRepository.Execute();

            string? result = getRepository.Url;

            Assert.NotNull(result);
            Assert.Equal(repositoryUrl, result);
        }
    }
}
