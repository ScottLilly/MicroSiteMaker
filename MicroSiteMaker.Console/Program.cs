using MicroSiteMaker.Models;
using MicroSiteMaker.Services;

namespace MicroSiteMaker.Console;

internal class Program
{
    static void Main(string[] args)
    {
        // Parse arguments and show error messages, if any
        var argDictionary = ArgParser.GetArgumentDictionary(args);

        var website = new WebSite(argDictionary);

        if (website.HasErrors)
        {
            foreach (string error in website.ErrorMessages)
            {
                System.Console.WriteLine(error);
            }

            return;
        }

        // Run commands
        if (website.Parameters.ContainsKey("--create"))
        {
            SiteBuilderService.BuildDirectories(website);
        }
    }
}