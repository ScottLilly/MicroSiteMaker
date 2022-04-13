namespace MicroSiteMaker.Models;

public interface IHtmlPageSource
{
    public string HtmlFileName { get; }
    public List<string> InputFileLines { get; }
    public List<string> OutputLines { get; }
    public string Title { get; }
    public DateTime FileDateTime { get; }
}