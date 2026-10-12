using System.Text.Json;
using BarangayManagementSystem.API.DTOs.Response;
using BarangayManagementSystem.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace BarangayManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentsController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public DocumentsController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Gets all stored document requests, newest first.</summary>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<DocumentRequestResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<DocumentRequestResponse>>> GetAllDocuments()
        {
            var documents = await _dbContext.DocumentRequests
                .AsNoTracking()
                .OrderByDescending(request => request.RequestId)
                .Select(request => new DocumentRequestResponse(
                    request.RequestId,
                    request.RequestDocumentType,
                    request.RequestPurpose,
                    request.RequesterEmail,
                    request.RequestDocumentContent))
                .ToListAsync();

            return Ok(documents);
        }

        [HttpPost("generateBarangayID")]
        [RequestSizeLimit(6 * 1024 * 1024)]
        public async Task<IActionResult> GenerateBarangayID([FromForm] BarangayIdRequest request)
        {
            if (request.Photo is null || request.Photo.Length == 0)
            {
                return BadRequest("An ID photo is required.");
            }

            if (request.Photo.Length > 5 * 1024 * 1024)
            {
                return BadRequest("The ID photo must be 5 MB or smaller.");
            }

            await using var photoStream = new MemoryStream();
            await request.Photo.CopyToAsync(photoStream);
            var photoBytes = photoStream.ToArray();
            var photoContentType = GetPhotoContentType(photoBytes);

            if (photoContentType is null)
            {
                return BadRequest("The ID photo must be a valid PNG or JPEG image.");
            }

            var requestContent = JsonSerializer.SerializeToUtf8Bytes(new
            {
                request.Name,
                request.CivilStatus,
                request.Gender,
                request.DateOfBirth,
                request.HomeAddress,
                request.ContactPerson,
                request.ContactNumber,
                PhotoContentType = photoContentType,
                PhotoBytes = photoBytes
            });

            var documentRequest = new DocumentRequest(
                "Barangay ID",
                "Barangay ID application",
                request.RequesterEmail ?? string.Empty,
                requestContent);

            _dbContext.DocumentRequests.Add(documentRequest);
            await _dbContext.SaveChangesAsync();

            return StatusCode(StatusCodes.Status201Created, new
            {
                requestId = documentRequest.RequestId,
                documentType = documentRequest.RequestDocumentType
            });
        }

        [HttpPost("generateIndigency")]
        public async Task<IActionResult> GenerateIndigency([FromForm] IndigencyRequest request)
        {
            var requestContent = JsonSerializer.SerializeToUtf8Bytes(new
            {
                request.FullName,
                request.Address,
                request.Purpose
            });
            var documentRequest = new DocumentRequest(
                "Certificate of Indigency",
                request.Purpose,
                request.RequesterEmail ?? string.Empty,
                requestContent);

            _dbContext.DocumentRequests.Add(documentRequest);
            await _dbContext.SaveChangesAsync();

            return StatusCode(StatusCodes.Status201Created, new
            {
                requestId = documentRequest.RequestId,
                documentType = documentRequest.RequestDocumentType
            });
        }

        private static string? GetPhotoContentType(byte[] imageBytes)
        {
            if (imageBytes.AsSpan().StartsWith(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }))
            {
                return "image/png";
            }

            if (imageBytes.Length >= 3 && imageBytes[0] == 0xFF && imageBytes[1] == 0xD8 && imageBytes[2] == 0xFF)
            {
                return "image/jpeg";
            }

            return null;
        }
    }
}