#: package Spectre.Console@0.49.1

using System;
using System.Security.Cryptography.X509Certificates;
using Spectre.Console;
using System.Text.RegularExpressions;
using System.Linq;

// Query the Current User - Personal certificate store
var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);

try
{
    // Open the certificate store in read-only mode
    store.Open(OpenFlags.ReadOnly);
    
    // Get all certificates from the store
    var certificates = store.Certificates;
    
    if (certificates.Count == 0)
    {
        AnsiConsole.MarkupLine("[yellow]No certificates found in the Current User - Personal store.[/]");
        return;
    }
    
    // Create a table to display the certificates
    var table = new Table();
    table.AddColumn("[bold]Thumbprint[/]");
    table.AddColumn("[bold]Common Name (CN)[/]");
    
    // Sort certificates alphabetically by thumbprint and add to table
    foreach (X509Certificate2 cert in certificates.Cast<X509Certificate2>().OrderBy(c => c.Thumbprint))
    {
        // Extract CN from subject using regex
        var subject = cert.Subject ?? "";
        var cnMatch = Regex.Match(subject, @"CN=([^,]+)");
        var commonName = cnMatch.Success ? cnMatch.Groups[1].Value.Trim() : "N/A";
        
        // Trim CN to 40 characters
        if (commonName.Length > 40)
        {
            commonName = commonName.Substring(0, 40) + "...";
        }
        
        var thumbprint = cert.Thumbprint ?? "N/A";
        
        table.AddRow(
            $"[dim]{thumbprint}[/]",
            commonName.EscapeMarkup()
        );
    }
    
    // Display header
    AnsiConsole.MarkupLine($"[green]Found {certificates.Count} certificate(s) in Current User - Personal store:[/]");
    AnsiConsole.WriteLine();
    
    // Render the table
    AnsiConsole.Write(table);
}
catch (Exception ex)
{
    AnsiConsole.MarkupLine($"[red]Error accessing certificate store: {ex.Message}[/]");
}
finally
{
    // Always close the store
    store.Close();
}