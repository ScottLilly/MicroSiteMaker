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
        Console.WriteLine("  --url <URL of website>");
        Console.WriteLine("  --path <root directory above input and output directories>");
        Console.WriteLine("");
        Console.WriteLine("Parameters (optional)");
        Console.WriteLine("  --nofollow              Add \"nofollow\" to robots meta tag on all pages");
        Console.WriteLine("  --noindex               Add \"noindex\" to robots meta tag on all pages");
        Console.WriteLine("  --nocompress            Do not compress images by lowering quality");
        Console.WriteLine("  --compresspercent <n>   Set the image quality compression percentage");
        Console.WriteLine("  --maximagewidth <n>     Rescale images in output/images to have a maximum width of <n> pixels");
        Console.WriteLine("");
        Console.WriteLine("Commands");
        Console.WriteLine("  --create                Builds /input directory, with default Markdown files and templates");
        Console.WriteLine("  --build                 Build HTML files in /output directory");
        Console.WriteLine("");
        Console.WriteLine("Support available at: https://github.com/ScottLilly/MicroSiteMaker");
    }
}