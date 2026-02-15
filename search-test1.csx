#r "nuget: OpenSearch.Client, 1.8.0"

using OpenSearch.Client;
using OpenSearch.Net;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// Define metadata structure
public class MetadataItem
{
    public string key { get; set; } = string.Empty;
    public string value { get; set; } = string.Empty;
}

// Define document structure
public class IndexedDocument
{
    public int id { get; set; }
    public string fileName { get; set; } = string.Empty;
    public string filePath { get; set; } = string.Empty;
    public DateTime? uploadDate { get; set; }
    public string uploadedBy { get; set; } = string.Empty;
    public List<MetadataItem> metadata { get; set; } = new();
}

// Connect to OpenSearch
var settings = new ConnectionSettings(new Uri("http://localhost:9200"))
    .DefaultIndex("documentsindex");

var client = new OpenSearchClient(settings);

// Build nested search query
var query = new QueryContainerDescriptor<IndexedDocument>()
    .Nested(n => n
        .Path("metadata")
        .Query(nq => nq
            .Bool(b => b
                .Filter(
                    f => f.Term(t => t.Field("metadata.key").Value("InvoiceNumber")),
                    f => f.Term(t => t.Field("metadata.value").Value("INV123"))
                )
            )
        )
    );

// Execute search
Console.WriteLine("Running search...");
var response = await client.SearchAsync<IndexedDocument>(s => s
    .Index("documentsindex")
    .Query(q => query)
);

if (!response.IsValid)
{
    Console.WriteLine("Search failed:");
    Console.WriteLine($"Server Error: {response.ServerError?.Error?.Reason ?? "No reason provided"}");
    Console.WriteLine($"Debug Info: {response.DebugInformation}");
    return;
}

var docs = response.Documents;
Console.WriteLine($"Found {docs?.Count ?? 0} results:");
if (docs != null)
{
    foreach (var doc in docs)
    {
        Console.WriteLine($"- {doc.fileName ?? "(no name)"} ({doc.filePath ?? "(no path)"}) uploaded by {doc.uploadedBy ?? "(unknown)"}");

        if (doc.metadata != null)
        {
            foreach (var meta in doc.metadata)
            {
                Console.WriteLine($"  Metadata: {meta.key} = {meta.value}");
            }
        }
        else
        {
            Console.WriteLine("  No metadata found.");
        }
    }
}
else
{
    Console.WriteLine("No documents returned.");
}
