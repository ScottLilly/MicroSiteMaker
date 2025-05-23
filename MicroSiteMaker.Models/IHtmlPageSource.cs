﻿namespace MicroSiteMaker.Models;

public interface IHtmlPageSource
{
    public bool IsSiteInformationPage { get; }
    public bool IncludeInCategories { get; }
    public string HtmlFileName { get; }
    public string MetaTagDescription { get; }
    public List<string> InputFileLines { get; }
    public List<string> OutputLines { get; }
    public string Title { get; }
    public DateTime FileDateTime { get; }
}