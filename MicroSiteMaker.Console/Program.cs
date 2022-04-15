using MicroSiteMaker.Core;
using MicroSiteMaker.Models;
using MicroSiteMaker.Services;

namespace MicroSiteMaker.Console;

internal class Program
{
    static void Main(string[] args)
    {
        var argDictionary = ArgParser.GetArgumentDictionary(args);

        // If user passes in --help parameter, display help message and stop running
        if (argDictionary.ContainsKey(Constants.Parameters.HELP) ||
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
        if (website.Parameters.ContainsKey(Constants.Parameters.NOFOLLOW))
        {
            SiteBuilderService.SetFollow(false);
        }

        if (website.Parameters.ContainsKey(Constants.Parameters.NOINDEX))
        {
            SiteBuilderService.SetIndex(false);
        }
        
        if (website.Parameters.ContainsKey(Constants.Parameters.NOCOMPRESS))
        {
            FileService.SetCompressImages(false);
        }

        if (website.Parameters.ContainsKey(Constants.Parameters.COMPRESS_PERCENT))
        {
            FileService.SetCompressPercent(Convert.ToInt64(website.Parameters[Constants.Parameters.COMPRESS_PERCENT]));
        }

        if (website.Parameters.ContainsKey(Constants.Parameters.MAX_IMAGE_WIDTH))
        {
            FileService.SetMaxImageWidth(Convert.ToInt32(website.Parameters[Constants.Parameters.MAX_IMAGE_WIDTH]));
        }

        // Run commands
        if (website.Parameters.ContainsKey(Constants.Commands.CREATE) ||
            (!website.Parameters.ContainsKey(Constants.Commands.CREATE) &&
             !website.Parameters.ContainsKey(Constants.Commands.BUILD)))
        {
            FileService.CreateInputDirectoriesAndDefaultFiles(website);
        }

        if (website.Parameters.ContainsKey(Constants.Commands.BUILD))
        {
            if (!Directory.Exists(website.InputRootDirectory) ||
                !Directory.Exists(website.InputTemplatesDirectory) ||
                !Directory.Exists(website.InputPagesDirectory) ||
                !Directory.Exists(website.InputImagesDirectory))
            {
                System.Console.WriteLine($"You must first run MicroSiteMaker with '{Constants.Commands.CREATE}' parameter and add/edit input markdown files");
            }
            else
            {
                SiteBuilderService.CreateOutputDirectoriesAndFiles(website);
            }
        }
    }
}