namespace MicroSiteMaker.Models;

public class Page
{
    private readonly FileInfo _fileInfo;
    private readonly string[] _lines;

    public string FullName => _fileInfo.FullName;
    public string FileName => _fileInfo.Name;
    public string FileNameWithoutExtension => Path.GetFileNameWithoutExtension(_fileInfo.Name);
    public List<string> InputFileLines => _lines.ToList();
    public List<string> OutputLines { get; } = new List<string>();

    public Page(FileInfo fileInfo, string[] lines)
    {
        _fileInfo = fileInfo;
        _lines = lines;
    }
}