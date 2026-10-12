namespace BarangayManagementSystem.API.DTOs.Response;

/// <summary>Represents a stored document request returned by the API.</summary>
public sealed record DocumentRequestResponse(
    int RequestId,
    string RequestDocumentType,
    string RequestPurpose,
    string RequesterEmail,
    byte[]? RequestDocumentContent);