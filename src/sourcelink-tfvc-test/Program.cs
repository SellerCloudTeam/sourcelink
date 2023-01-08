using Microsoft.Build.Tasks.Tfvc;
using Microsoft.TeamFoundation.VersionControl.Client;

static class Program
{
    static void Main(string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine(@"Usage: sourcelink-tfvc-test <folder> [tfs-collection-url]");

            return;
        }

        string? directory = args.ElementAt(0);
        string? tfsCollectionUrl = args.ElementAtOrDefault(1);

        try
        {
            // Enable the TFS cache
            Workstation.CacheEnabled = true;

            LocateRepository locateRepository = new LocateRepository
            {
                TfsCollectionUrl = tfsCollectionUrl,
                Directory = directory
            };

            locateRepository.Execute();

            string? result = locateRepository.Id;

            Console.WriteLine(result is null
                ? @$"Could not determine repository workspace!"
                : @$"Workspace found: {result}"
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine();
            Console.WriteLine(ex.StackTrace);
        }
    }
}
