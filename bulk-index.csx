#r "nuget: OpenSearch.Client, 1.8.0"

using OpenSearch.Client;
using OpenSearch.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// Raw metadata structure
public class MetadataItemRaw
{
    public int DocumentId { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

// Raw document structure
public class DocumentRaw
{
    public int DocumentId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public DateTime UploadDate { get; set; }
    public string UploadedBy { get; set; } = string.Empty;
}

// Final indexed structure
public class MetadataItem
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

public class IndexedDocument
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public DateTime? UploadDate { get; set; }
    public string UploadedBy { get; set; } = string.Empty;
    public List<MetadataItem> Metadata { get; set; } = new();
}

// Connect to OpenSearch
var settings = new ConnectionSettings(new Uri("http://localhost:9200"))
    .DefaultIndex("documentsindex");

var client = new OpenSearchClient(settings);

// Sample metadata
var metadataList = new List<MetadataItemRaw>
{
    new MetadataItemRaw { DocumentId = 1, Key = "InvoiceNumber", Value = "INV123" },
    new MetadataItemRaw { DocumentId = 1, Key = "InvoiceNumber", Value = "INV001" },
    new MetadataItemRaw { DocumentId = 1, Key = "Date", Value = "2025-08-23" },
    new MetadataItemRaw { DocumentId = 1, Key = "Vendor", Value = "ABC Corp" },
    new MetadataItemRaw { DocumentId = 1, Key = "TotalAmount", Value = "500.00" },
    new MetadataItemRaw { DocumentId = 1, Key = "TestKey", Value = "invoice" },
    new MetadataItemRaw { DocumentId = 2, Key = "DeliveryNumber", Value = "DEL001" },
    new MetadataItemRaw { DocumentId = 2, Key = "Date", Value = "2025-08-23" },
    new MetadataItemRaw { DocumentId = 2, Key = "Supplier", Value = "XYZ Ltd" },
    new MetadataItemRaw { DocumentId = 2, Key = "ItemCount", Value = "3" },
    new MetadataItemRaw { DocumentId = 3, Key = "ReceiptNumber", Value = "REC001" },
    new MetadataItemRaw { DocumentId = 3, Key = "Date", Value = "2025-08-23" },
    new MetadataItemRaw { DocumentId = 3, Key = "StoreName", Value = "Shop A" },
    new MetadataItemRaw { DocumentId = 3, Key = "TotalAmount", Value = "150.00" }
};

// Sample documents
var documentList = new List<DocumentRaw>
{
    new DocumentRaw { DocumentId = 1, FileName = "invoice.pdf", FilePath = "/Users/tp/cloud9.2/documents/invoice.pdf", UploadDate = DateTime.Parse("2025-08-21T18:25:39.303"), UploadedBy = "tp" },
    new DocumentRaw { DocumentId = 2, FileName = "delivery-note-001.pdf", FilePath = "/Users/tp/Projects/Cloud9.2/documents/delivery-note-001.pdf", UploadDate = DateTime.Parse("2025-08-23T09:05:00"), UploadedBy = "tp" },
    new DocumentRaw { DocumentId = 3, FileName = "receipt-001.pdf", FilePath = "/Users/tp/Projects/Cloud9.2/documents/receipt-001.pdf", UploadDate = DateTime.Parse("2025-08-23T09:10:00"), UploadedBy = "tp" }
};

// Group metadata by DocumentId
var groupedMetadata = metadataList
    .GroupBy(m => m.DocumentId)
    .ToDictionary(g => g.Key, g => g.Select(m => new MetadataItem { Key = m.Key, Value = m.Value }).ToList());

// Index each document
foreach (var doc in documentList)
{
    var indexedDoc = new IndexedDocument
    {
        Id = doc.DocumentId,
        FileName = doc.FileName,
        FilePath = doc.FilePath,
        UploadDate = doc.UploadDate,
        UploadedBy = doc.UploadedBy,
        Metadata = groupedMetadata.ContainsKey(doc.DocumentId) ? groupedMetadata[doc.DocumentId] : new List<MetadataItem>()
    };

    var indexResponse = await client.IndexAsync(indexedDoc, i => i
        .Index("documentsindex")
        .Id(doc.DocumentId.ToString())
        .Refresh(Refresh.True));

    Console.WriteLine($"Indexed DocumentId {doc.DocumentId}: {indexResponse.IsValid}");
}
