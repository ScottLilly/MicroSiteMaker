using MicroSiteMaker.Models;
using MicroSiteMaker.Services;

namespace MicroSiteMaker.Console;

internal class Program
{
    static void Main(string[] args)
    {
        var argDictionary = ArgParser.GetArgumentDictionary(args);

        // If user passes in --help parameter, display help message and stop running
        if (argDictionary.ContainsKey("--help"))
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
        if (website.Parameters.ContainsKey("--nocompress"))
        {
            SiteBuilderService.SetCompressImages(false);
        }

        if (website.Parameters.ContainsKey("--compresspercent"))
        {
            SiteBuilderService.SetCompressPercent(Convert.ToInt64(website.Parameters["--compresspercent"]));
        }

        if (website.Parameters.ContainsKey("--maximagewidth"))
        {
            SiteBuilderService.SetMaxImageWidth(Convert.ToInt32(website.Parameters["--maximagewidth"]));
        }

        // Run commands
        if (website.Parameters.ContainsKey("--create"))
        {
            SiteBuilderService.CreateInputDirectoriesAndDefaultFiles(website);
        }

        if (website.Parameters.ContainsKey("--build"))
        {
            SiteBuilderService.CreateOutputDirectoriesAndFiles(website);
        }
    }
}