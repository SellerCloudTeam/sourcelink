using System;
using System.Linq;
using TestUtilities;
using Xunit;
using static TestUtilities.KeyValuePairUtils;

namespace Microsoft.SourceLink.AzureDevOpsServer.Tfvc.UnitTests
{
    public class GetSourceLinkUrlTests
    {
        [Theory]
        [InlineData(110810, @"https://tfs.sellercloud.com/tfs/defaultcollection", @"89789a70-e50b-4399-8419-ff8c422ffaab", @"SellerCloud", @"C:\Data\Core", "$/SellerCloud")]
        [InlineData(110810, @"https://tfs.sellercloud.com/tfs/defaultcollection", @"89789a70-e50b-4399-8419-ff8c422ffaab", @"Project Name With White Space", @"C:\Data\Core", @"$/SellerCloud")]
        [InlineData(110810, @"https://tfs.sellercloud.com/tfs/defaultcollection", @"89789a70-e50b-4399-8419-ff8c422ffaab", @"Project Name With White Space", @"C:\Data\Core\TempActions", @"$/SellerCloud")]
        [InlineData(999999, @"https://abc.sample.com/tfs/collection/", @"89789a70-e50b-4399-8419-ff8c422ffaab", @"project", @"C:\Data\Core", @"$/SellerCloud")]
        public void Tfvc_BuildSourceLinkUrl(int revisionId, string collectionUrl, string projectId, string projectName, string localPath, string serverPath)
        {
            var engine = new MockEngine();

            var task = new GetSourceLinkUrl()
            {
                BuildEngine = engine,
                SourceRoot = new MockItem(
                    localPath,
                    KVP("SourceControl", "tfvc"),
                    KVP("CollectionUrl", collectionUrl),
                    KVP("ProjectId", projectId),
                    KVP("ProjectName", projectName),
                    KVP("ServerPath", serverPath),
                    KVP("RevisionId", $"{revisionId}")
                ),
            };

            bool result = task.Execute();
            AssertEx.AssertEqualToleratingWhitespaceDifferences("", engine.Log);
            AssertEx.AreEqual($"{collectionUrl.TrimEnd('/')}/{Uri.EscapeDataString(projectName)}/_api/_versioncontrol/itemContent?contentOnly=true&version={revisionId}&path={string.Join("/", serverPath.Split('/').Select(Uri.EscapeDataString))}/*", task.SourceLinkUrl);
            Assert.True(result);
        }
    }
}
