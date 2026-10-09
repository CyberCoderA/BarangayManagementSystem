using System.Globalization;
using System.Net;
using BarangayManagementSystem.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Playwright;

namespace BarangayManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentsController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;

        public DocumentsController(IWebHostEnvironment environment)
        {
            _environment = environment;
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

            var photoDataUri = $"data:{photoContentType};base64,{Convert.ToBase64String(photoBytes)}";
            var templateDirectory = Path.Combine(_environment.ContentRootPath, "templates");
            var templatePath = Path.Combine(templateDirectory, "brgy_id.html");
            var html = await System.IO.File.ReadAllTextAsync(templatePath);

            html = html
                .Replace("{{PHOTO_DATA_URI}}", photoDataUri, StringComparison.Ordinal)
                .Replace("{{BRGY_LOGO}}", await ReadImageDataUriAsync(templateDirectory, "brgy_84_logo.png"), StringComparison.Ordinal)
                .Replace("{{PASAY_LOGO}}", await ReadImageDataUriAsync(templateDirectory, "pasay-logo.png"), StringComparison.Ordinal)
                .Replace("{{NAME}}", WebUtility.HtmlEncode(request.Name.ToUpperInvariant()), StringComparison.Ordinal)
                .Replace("{{CIVIL_STATUS}}", WebUtility.HtmlEncode(request.CivilStatus), StringComparison.Ordinal)
                .Replace("{{GENDER}}", WebUtility.HtmlEncode(request.Gender), StringComparison.Ordinal)
                .Replace("{{DATE_OF_BIRTH}}", request.DateOfBirth!.Value.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture), StringComparison.Ordinal)
                .Replace("{{VALID_UNTIL}}", DateTime.Today.AddYears(1).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture), StringComparison.Ordinal)
                .Replace("{{HOME_ADDRESS}}", WebUtility.HtmlEncode(request.HomeAddress.ToUpperInvariant()), StringComparison.Ordinal)
                .Replace("{{CONTACT_PERSON}}", WebUtility.HtmlEncode(request.ContactPerson), StringComparison.Ordinal)
                .Replace("{{CONTACT_NUMBER}}", WebUtility.HtmlEncode(request.ContactNumber), StringComparison.Ordinal);

            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new() { Headless = true });
            var page = await browser.NewPageAsync();
            await page.SetContentAsync(html, new() { WaitUntil = WaitUntilState.NetworkIdle });

            var pdf = await page.PdfAsync(new()
            {
                Width = "230mm",
                Height = "335mm",
                PrintBackground = true,
                Margin = new() { Top = "0mm", Right = "0mm", Bottom = "0mm", Left = "0mm" }
            });

            return File(pdf, "application/pdf", "barangay-id.pdf");
        }

        [HttpPost("generateIndigency")]
        public async Task<IActionResult> GenerateIndigency([FromForm] IndigencyRequest request)
        {
            var templateDirectory = Path.Combine(_environment.ContentRootPath, "templates");
            var templatePath = Path.Combine(templateDirectory, "indigency.html");
            var html = await System.IO.File.ReadAllTextAsync(templatePath);
            var barangayLogoDataUri = await ReadImageDataUriAsync(templateDirectory, "brgy_84_logo.png");
            var pasayLogoDataUri = await ReadImageDataUriAsync(templateDirectory, "pasay-logo.png");
            var issueDate = DateTime.Today;
            var dayOfMonth = issueDate.Day;
            var daySuffix = dayOfMonth % 100 is 11 or 12 or 13
                ? "th"
                : (dayOfMonth % 10) switch
                {
                    1 => "st",
                    2 => "nd",
                    3 => "rd",
                    _ => "th"
                };

            html = html
                .Replace("{{BRGY_LOGO}}", barangayLogoDataUri, StringComparison.Ordinal)
                .Replace("{{PASAY_LOGO}}", pasayLogoDataUri, StringComparison.Ordinal)
                .Replace("{{FULL_NAME}}", WebUtility.HtmlEncode(request.FullName), StringComparison.Ordinal)
                .Replace("{{ADDRESS}}", WebUtility.HtmlEncode(request.Address), StringComparison.Ordinal)
                .Replace("{{PURPOSE}}", WebUtility.HtmlEncode(request.Purpose), StringComparison.Ordinal)
                .Replace("{{day}}", $"{dayOfMonth}{daySuffix}", StringComparison.Ordinal)
                .Replace("{{month}}", issueDate.ToString("MMMM", CultureInfo.InvariantCulture), StringComparison.Ordinal)
                .Replace("{{year}}", issueDate.Year.ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal);

            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new() { Headless = true });
            var page = await browser.NewPageAsync();
            await page.SetContentAsync(html, new() { WaitUntil = WaitUntilState.NetworkIdle });

            var pdf = await page.PdfAsync(new()
            {
                Width = "210mm",
                Height = "297mm",
                PrintBackground = true,
                Margin = new() { Top = "0mm", Right = "0mm", Bottom = "0mm", Left = "0mm" }
            });

            return File(pdf, "application/pdf", "indigency.pdf");
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

        private static async Task<string> ReadImageDataUriAsync(string templateDirectory, string fileName)
        {
            var imagePath = Path.Combine(templateDirectory, "assets", fileName);
            var imageData = Convert.ToBase64String(await System.IO.File.ReadAllBytesAsync(imagePath));
            return $"data:image/png;base64,{imageData}";
        }
    }
}