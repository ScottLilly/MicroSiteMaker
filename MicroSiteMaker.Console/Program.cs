using MicroSiteMaker.Services;

namespace MicroSiteMaker.Console;

internal class Program
{
    static void Main(string[] args)
    {
        var argDictionary = ArgParser.GetArgumentDictionary(args);

        if (argDictionary.ContainsKey("--create"))
        {
            if (string.IsNullOrWhiteSpace(argDictionary["--create"]))
            {
                throw new ArgumentException("Missing path for '--create' parameter");
            }

            SiteBuilderService.BuildDirectories(argDictionary["--create"]);
        }
    }
}