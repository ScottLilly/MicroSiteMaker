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
        Console.WriteLine("Parameters");
        Console.WriteLine("===================================");
        Console.WriteLine("--site <URL of website> (required)");
        Console.WriteLine("--path <root directory of files used by, or created by, MicroSiteMaker> (required)");
        Console.WriteLine("");
        Console.WriteLine("Commands");
        Console.WriteLine("===================================");
        Console.WriteLine("--create (builds directories and creates default files)");
    }
}