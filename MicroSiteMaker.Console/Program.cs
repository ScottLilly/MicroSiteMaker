using MicroSiteMaker.Models;
using MicroSiteMaker.Services;

namespace MicroSiteMaker.Console;

internal class Program
{
    static void Main(string[] args)
    {
        var argDictionary = ArgParser.GetArgumentDictionary(args);

        // If user passes in --help parameter, display help message and stop running
        if (argDictionary.ContainsKey("--help") ||
            argDictionary.ContainsKey("--h") ||
            argDictionary.Count == 0)
        {
            ArgParser.DisplayHelp();
            return;
        }

        // Build the Website object and check for errors with the parameters.
        // Stop running if there are invalid, or missing required, parameters.
        var website = new Website(argDictionary);

        if (website.HasErrors)
        {
            foreach (string error in website.ErrorMessages)
            {
                System.Console.WriteLine(error);
            }

            return;
        }

        // Set values from parameters
        if (website.Parameters.ContainsKey("--nofollow"))
        {
            SiteBuilderService.SetFollow(false);
        }

        if (website.Parameters.ContainsKey("--noindex"))
        {
            SiteBuilderService.SetIndex(false);
        }
        
        if (website.Parameters.ContainsKey("--nocompress"))
        {
            FileService.SetCompressImages(false);
        }

        if (website.Parameters.ContainsKey("--compresspercent"))
        {
            FileService.SetCompressPercent(Convert.ToInt64(website.Parameters["--compresspercent"]));
        }

        if (website.Parameters.ContainsKey("--maximagewidth"))
        {
            FileService.SetMaxImageWidth(Convert.ToInt32(website.Parameters["--maximagewidth"]));
        }

        // Run commands
        if (website.Parameters.ContainsKey("--create") ||
            (!website.Parameters.ContainsKey("--create") &&
             !website.Parameters.ContainsKey("--build")))
        {
            FileService.CreateInputDirectoriesAndDefaultFiles(website);
        }

        if (website.Parameters.ContainsKey("--build"))
        {
            if (!Directory.Exists(website.InputRootDirectory) ||
                !Directory.Exists(website.InputTemplatesDirectory) ||
                !Directory.Exists(website.InputPagesDirectory) ||
                !Directory.Exists(website.InputImagesDirectory))
            {
                System.Console.WriteLine("You must run MicroSiteMaker with '--create' parameter and add/edit input markdown files");
            }
            else
            {
                SiteBuilderService.CreateOutputDirectoriesAndFiles(website);
            }
        }
    }
}