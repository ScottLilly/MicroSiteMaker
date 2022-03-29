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

}