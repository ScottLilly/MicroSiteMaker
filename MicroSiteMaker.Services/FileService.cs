using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using System.Text;
using MicroSiteMaker.Models;
using Encoder = System.Drawing.Imaging.Encoder;

namespace MicroSiteMaker.Services;

public static class FileService
{
    private static bool s_shouldCompressImages = true;
    private static long s_compressPercent = 80;
    private static int s_maxImageWidth = -1;

    public static void SetCompressImages(bool shouldCompress)
    {
        s_shouldCompressImages = shouldCompress;
    }

    public static void SetCompressPercent(long compressPercent)
    {
        s_compressPercent = compressPercent;
    }

    public static void SetMaxImageWidth(int maxImageWidth)
    {
        s_maxImageWidth = maxImageWidth;
    }

    public static void CreateInputDirectoriesAndDefaultFiles(Website website)
    {
        Directory.CreateDirectory(website.ProjectDirectory);
        Directory.CreateDirectory(website.InputRootDirectory);
        Directory.CreateDirectory(website.InputPagesDirectory);
        Directory.CreateDirectory(website.InputTemplatesDirectory);
        Directory.CreateDirectory(website.InputImagesDirectory);
        Console.WriteLine("Created input directories");

        CreateFile(website.InputTemplatesDirectory, "stylesheet.css", DefaultCssStylesheet());
        CreateFile(website.InputTemplatesDirectory, "page-template.html", DefaultWebPageTemplate());

        CreateFile(website.InputPagesDirectory, "index.md", DefaultIndexPageMarkdown());
        CreateFile(website.InputPagesDirectory, "page-not-found.md", DefaultPageNotFoundMarkdown());
        CreateFile(website.InputPagesDirectory, "about.md", DefaultAboutPageMarkdown());
        CreateFile(website.InputPagesDirectory, "privacy-policy.md", DefaultPrivacyPolicyPageMarkdown());
        CreateFile(website.InputPagesDirectory, "contact.md", DefaultContactPageMarkdown());
    }

    public static void CreateOutputDirectories(Website website)
    {
        // Delete existing folders and files
        var rootDirectory = new DirectoryInfo(website.OutputRootDirectory);

        if (rootDirectory.Exists)
        {
            rootDirectory.Delete(true);
        }

        // Create the new folders and files
        Directory.CreateDirectory(website.OutputRootDirectory);
        Directory.CreateDirectory(website.OutputImagesDirectory);
        Directory.CreateDirectory(website.OutputCssDirectory);
    }

    public static void PopulateWebsiteInputFiles(Website website)
    {
        foreach (FileInfo fileInfo in GetFilesWithExtension(website.InputPagesDirectory, "md"))
        {
            website.Pages.Add(new Page(fileInfo));
        }
    }

    public static List<string> GetPageTemplateLines(Website website) =>
        File.ReadAllLines(Path.Combine(website.InputTemplatesDirectory, website.TemplateFileName))
            .ToList();

    public static void CopyCssFilesToOutputDirectory(Website website)
    {
        foreach (FileInfo fileInfo in GetFilesWithExtension(website.InputTemplatesDirectory, "css"))
        {
            File.Copy(fileInfo.FullName, Path.Combine(website.OutputCssDirectory, fileInfo.Name));
        }
    }

    public static void CopyImageFilesToOutputDirectory(Website website)
    {
        foreach (FileInfo fileInfo in Directory.GetFiles(website.InputImagesDirectory)
                     .Select(f => new FileInfo(f)))
        {
            var outputFileName = Path.Combine(website.OutputImagesDirectory, fileInfo.Name);

            var image = Image.FromFile(fileInfo.FullName);
            var encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] =
                new EncoderParameter(Encoder.Quality, s_shouldCompressImages ? s_compressPercent : 100L);

            if (s_maxImageWidth != -1 && image.Width > s_maxImageWidth)
            {
                ShrinkImageAndCopyToOutput(image, fileInfo.FullName, outputFileName, encoderParameters);
            }
            else
            {
                if (s_shouldCompressImages)
                {
                    image.Save(outputFileName, GetEncoder(ImageFormat.Jpeg), encoderParameters);
                }
                else
                {
                    File.Copy(fileInfo.FullName, outputFileName);
                }
            }
        }
    }

    public static void WriteOutputFiles(Website website)
    {
        foreach (Page page in website.Pages)
        {
            CreateFile(website.OutputRootDirectory, $"{page.HtmlFileName}", page.OutputLines);
        }

        Console.WriteLine($"Total HTML files created: {website.Pages.Count}");
    }

    public static void CreateSitemapFile(Website website)
    {
        List<string> lines = new List<string>();

        lines.Add("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        lines.Add("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");

        foreach (Page page in website.Pages)
        {
            lines.Add("   <url>");
            lines.Add($"      <loc>{page.HtmlFileName}</loc>");
            lines.Add($"      <lastmod>{page.FileDateTime:yyyy-MM-dd}</lastmod>");
            lines.Add("   </url>");
        }

        lines.Add("</urlset>");

        CreateFile(website.OutputRootDirectory, "sitemap.xml", lines);
    }

    private static void CreateFile(string path, string filename, IEnumerable<string> contents)
    {
        var nonEmptyLines = contents.Select(c => c.ReplaceLineEndings("")).Where(c => c.Length > 0).ToList();

        File.WriteAllLines(Path.Combine(path, filename), nonEmptyLines);

        Console.WriteLine($"Created: {Path.Combine(path, filename)}");
    }

    private static List<FileInfo> GetFilesWithExtension(string path, string extension) =>
        Directory.GetFiles(path)
            .Select(f => new FileInfo(f))
            .Where(f => f.Name.EndsWith(extension, StringComparison.InvariantCultureIgnoreCase))
            .ToList();

    private static IEnumerable<string> DefaultCssStylesheet() =>
        GetLinesFromResource("MicroSiteMaker.Services.Resources.DefaultCssStylesheet.txt");

    private static IEnumerable<string> DefaultWebPageTemplate() =>
        GetLinesFromResource("MicroSiteMaker.Services.Resources.DefaultWebPageTemplate.txt");

    private static IEnumerable<string> DefaultIndexPageMarkdown() =>
        GetLinesFromResource("MicroSiteMaker.Services.Resources.DefaultIndexPageMarkdown.txt");

    private static IEnumerable<string> DefaultPageNotFoundMarkdown() =>
        GetLinesFromResource("MicroSiteMaker.Services.Resources.DefaultPageNotFoundMarkdown.txt");

    private static IEnumerable<string> DefaultAboutPageMarkdown() =>
        GetLinesFromResource("MicroSiteMaker.Services.Resources.DefaultAboutPageMarkdown.txt");

    private static IEnumerable<string> DefaultPrivacyPolicyPageMarkdown() =>
        GetLinesFromResource("MicroSiteMaker.Services.Resources.DefaultPrivacyPolicyPageMarkdown.txt");

    private static IEnumerable<string> DefaultContactPageMarkdown() =>
        GetLinesFromResource("MicroSiteMaker.Services.Resources.DefaultContactPageMarkdown.txt");

    private static IEnumerable<string> GetLinesFromResource(string resourceName)
    {
        var assembly = Assembly.GetExecutingAssembly();

        using Stream stream = assembly.GetManifestResourceStream(resourceName);
        using StreamReader reader = new StreamReader(stream);

        return reader.ReadToEnd().Split(Environment.NewLine).ToList();
    }

    private static void ShrinkImageAndCopyToOutput(Image bmp, string inputFileName, string outputFileName,
        EncoderParameters encoderParameters)
    {
        double scale = Convert.ToDouble(s_maxImageWidth) / Convert.ToDouble(bmp.Width);

        Size newSize = new Size(Convert.ToInt32(bmp.Width * scale), Convert.ToInt32(bmp.Height * scale));

        var bitmap = new Bitmap(Image.FromFile(inputFileName), newSize);

        bitmap.Save(outputFileName, GetEncoder(ImageFormat.Jpeg), encoderParameters);
    }

    private static ImageCodecInfo GetEncoder(ImageFormat format) =>
        ImageCodecInfo.GetImageDecoders().FirstOrDefault(codec => codec.FormatID == format.Guid);
}