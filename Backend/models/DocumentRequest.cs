using Microsoft.EntityFrameworkCore;

public class DocumentRequest
{
    public int RequestId { get; set; }
    public string RequestDocumentType { get; set; } = string.Empty;
    public string RequestPurpose { get; set; } = string.Empty;
    public string RequesterEmail { get; set; } = string.Empty;
    public byte[]? RequestDocumentContent { get; set; }

    private DocumentRequest() { } // Private constructor for EF Core

    // Public constructor to initialize the DocumentRequest object with required properties
    public DocumentRequest(string requestDocumentType, string requestPurpose, string requesterEmail, byte[]? requestDocumentContent)
    {
        if (string.IsNullOrWhiteSpace(requestDocumentType))
            throw new ArgumentException("Request document type cannot be null or empty.", nameof(requestDocumentType));

        if (string.IsNullOrWhiteSpace(requestPurpose))
            throw new ArgumentException("Request purpose cannot be null or empty.", nameof(requestPurpose));

        RequestDocumentType = requestDocumentType;
        RequestPurpose = requestPurpose;
        RequesterEmail = requesterEmail;
        RequestDocumentContent = requestDocumentContent;
    }

    // Model behavior methods
}