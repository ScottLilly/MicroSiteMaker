using MicroSiteMaker.Core;

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
        Console.WriteLine($"  {Constants.Parameters.URL} <URL of website>");
        Console.WriteLine($"  {Constants.Parameters.PATH} <root directory above input and output directories>");
        Console.WriteLine("");
        Console.WriteLine("Parameters (optional)");
        Console.WriteLine($"  {Constants.Parameters.NOFOLLOW}              Add \"nofollow\" to robots meta tag on all pages");
        Console.WriteLine($"  {Constants.Parameters.NOINDEX}               Add \"noindex\" to robots meta tag on all pages");
        Console.WriteLine($"  {Constants.Parameters.NOCOMPRESS}            Do not compress images by lowering quality");
        Console.WriteLine($"  {Constants.Parameters.COMPRESS_PERCENT} <n>   Set the image quality compression percentage");
        Console.WriteLine($"  {Constants.Parameters.MAX_IMAGE_WIDTH} <n>     Rescale images in output/images to have a maximum width of <n> pixels");
        Console.WriteLine("");
        Console.WriteLine("Commands");
        Console.WriteLine($"  {Constants.Commands.CREATE}                Builds /input directory, with default Markdown files and templates");
        Console.WriteLine($"  {Constants.Commands.BUILD}                 Build HTML files in /output directory");
        Console.WriteLine("");
        Console.WriteLine("Support available at: https://github.com/ScottLilly/MicroSiteMaker");
    }
}