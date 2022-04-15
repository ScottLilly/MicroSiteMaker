using System.Reflection.PortableExecutable;

namespace MicroSiteMaker.Core;

public static class Constants
{
    public static class Parameters
    {
        public const string HELP = "--help";
        public const string URL = "--url";
        public const string PATH = "--path";
        public const string NOFOLLOW = "--nofollow";
        public const string NOINDEX = "--noindex";
        public const string NOCOMPRESS = "--nocompress";
        public const string COMPRESS_PERCENT = "--compresspercent";
        public const string MAX_IMAGE_WIDTH = "--maximagewidth";
        public const string STYLESHEET = "--stylesheet";
        public const string TEMPLATE = "--template";
    }

    public static class Commands
    {
        public const string CREATE = "--create";
        public const string BUILD = "--build";
    }

    public static class Regexes
    {
        public const string CATEGORIES = @"{{Categories:(.*?)}}";
        public const string META_TAG_DESCRIPTION = @"{{Meta-Description:(.*?)}}";
        public const string TITLE_OVERRIDE = @"{{Title:(.*?)}}";
    }

    public static class SpecialCategories
    {
        public const string UNCATEGORIZED = "Uncategorized";
        public const string EXCLUDE = "Exclude";
    }

    public static class Directories
    {
        public static class Input
        {
            public const string ROOT = "input";
            public const string PAGES = "pages";
            public const string TEMPLATES = "templates";
            public const string IMAGES = "images";
        }

        public static class Output
        {
            public const string ROOT = "output";
            public const string CSS = "css";
            public const string IMAGES = "images";

        }
    }
}