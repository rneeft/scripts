#: package Spectre.Console@0.49.1

using Spectre.Console;
using System.Xml.Linq;
using System.Xml;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

if (args.Length != 1)
{
    AnsiConsole.MarkupLine("[red]Usage: dotnet script cpm-create.csx <directory>[/]");
    return;
}

var rootDir = args[0];
if (!Directory.Exists(rootDir))
{
    AnsiConsole.MarkupLine($"[red]Directory not found: {rootDir}[/]");
    return;
}

var packageDict = new Dictionary<string, string>();
var csprojFiles = Directory.GetFiles(rootDir, "*.csproj", SearchOption.AllDirectories);

foreach (var csproj in csprojFiles)
{
    var content = File.ReadAllText(csproj);
    var originalContent = content;
    
    // First pass: collect package versions using XML parsing
    var doc = XDocument.Parse(content);
    foreach (var pr in doc.Descendants("PackageReference"))
    {
        var name = pr.Attribute("Include")?.Value;
        var versionAttr = pr.Attribute("Version");
        if (name != null && versionAttr != null)
        {
            if (!packageDict.ContainsKey(name))
                packageDict[name] = versionAttr.Value;
        }
    }
    
    // Second pass: remove Version attributes using regex to preserve formatting
    var versionPattern = @"(<PackageReference[^>]*?)(\s+Version\s*=\s*""[^""]*"")([^>]*>)";
    content = Regex.Replace(content, versionPattern, "$1$3");
    
    // Only save if content changed
    if (content != originalContent)
    {
        File.WriteAllText(csproj, content);
    }
}

// Create Directory.Packages.props
var propsPath = Path.Combine(rootDir, "Directory.Packages.props");
var propsDoc = new XDocument(
    new XElement("Project",
        new XElement("PropertyGroup",
            new XElement("ManagePackageVersionsCentrally", "true")
        ),
        new XElement("ItemGroup",
            packageDict.OrderBy(kv => kv.Key).Select(kv =>
                new XElement("PackageVersion",
                    new XAttribute("Include", kv.Key),
                    new XAttribute("Version", kv.Value)
                ))
        )
    )
);
var propsSettings = new XmlWriterSettings
{
    OmitXmlDeclaration = true,
    Indent = true
};
using var propsWriter = XmlWriter.Create(propsPath, propsSettings);
propsDoc.Save(propsWriter);

// Display table
var table = new Table().Title("[yellow]Central Package Versions[/]")
    .AddColumn("[green]Package Name[/]")
    .AddColumn("[blue]Version[/]");
foreach (var kv in packageDict)
    table.AddRow(kv.Key, kv.Value);
AnsiConsole.Write(table);

AnsiConsole.MarkupLine($"[bold green]Created:[/] {propsPath}");
