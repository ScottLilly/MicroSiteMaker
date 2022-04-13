namespace MicroSiteMaker.Services;

public static  class ArgParser
{
    public static Dictionary<string, string> GetArgumentDictionary(IEnumerable<string> args)
    {
        Dictionary<string, string> argDictionary =
            new Dictionary<string, string>();

        string lastCommand = "";

        foreach (var arg in args)
        {
            if (arg.StartsWith("--"))
            {
                argDictionary.Add(arg.Replace(" ", "").ToLower(), "");
                lastCommand = arg;
            }
            else
            {
                if (argDictionary.Count == 0)
                {
                    Console.WriteLine("Invalid arguments");
                }
                else
                {
                    argDictionary[lastCommand] =
                        (argDictionary[lastCommand] + " " + arg).Trim();
                }
            }
        }

        return argDictionary;
    }

    public static void DisplayHelp()
    {
        Console.WriteLine("Parameters (required)");
        Console.WriteLine("===================================");
        Console.WriteLine("--site <URL of website> (required)");
        Console.WriteLine("--path <root directory of files used by, or created by, MicroSiteMaker> (required)");
        Console.WriteLine("");
        Console.WriteLine("Parameters (optional)");
        Console.WriteLine("===================================");
        Console.WriteLine("--nofollow adds \"nofollow\" to robots meta tag on all pages");
        Console.WriteLine("--noindex adds \"noindex\" to robots meta tag on all pages");
        Console.WriteLine("--nocompress does not compress images (default is to compress images to 80% of quality)");
        Console.WriteLine("--compresspercent <n> sets the images compression percentage");
        Console.WriteLine("--maximagewidth <n> rescales images to have a maximum width of <n> pixels");
        Console.WriteLine("");
        Console.WriteLine("Commands");
        Console.WriteLine("===================================");
        Console.WriteLine("--create builds input directories and creates default input Markdown files and templates");
        Console.WriteLine("--build builds output directories and creates the HTML files");
        Console.WriteLine("");
        Console.WriteLine("Support available at: https://github.com/ScottLilly/MicroSiteMaker");
    }
}