using System.Globalization;
using System.Net;
using System.Text.Json;
using BarangayManagementSystem.API.Models;
using Microsoft.Playwright;

namespace BarangayManagementSystem.API.Services
{
    public class DocumentGenerationService
    {
        private readonly IWebHostEnvironment _environment;

        public DocumentGenerationService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<byte[]> GenerateBarangayIdAsync(DocumentRequest request)
        {
            var content = DeserializeContent<BarangayIdContent>(request);
            var templateDirectory = Path.Combine(_environment.ContentRootPath, "templates");
            var templatePath = Path.Combine(templateDirectory, "brgy_id.html");
            var html = await File.ReadAllTextAsync(templatePath);
            var photoDataUri = $"data:{content.PhotoContentType};base64,{Convert.ToBase64String(content.PhotoBytes)}";

            html = html
                .Replace("{{PHOTO_DATA_URI}}", photoDataUri, StringComparison.Ordinal)
                .Replace("{{BRGY_LOGO}}", await ReadImageDataUriAsync(templateDirectory, "brgy_84_logo.png"), StringComparison.Ordinal)
                .Replace("{{PASAY_LOGO}}", await ReadImageDataUriAsync(templateDirectory, "pasay-logo.png"), StringComparison.Ordinal)
                .Replace("{{NAME}}", WebUtility.HtmlEncode(content.Name.ToUpperInvariant()), StringComparison.Ordinal)
                .Replace("{{CIVIL_STATUS}}", WebUtility.HtmlEncode(content.CivilStatus), StringComparison.Ordinal)
                .Replace("{{GENDER}}", WebUtility.HtmlEncode(content.Gender), StringComparison.Ordinal)
                .Replace("{{DATE_OF_BIRTH}}", content.DateOfBirth.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture), StringComparison.Ordinal)
                .Replace("{{VALID_UNTIL}}", DateTime.Today.AddYears(1).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture), StringComparison.Ordinal)
                .Replace("{{HOME_ADDRESS}}", WebUtility.HtmlEncode(content.HomeAddress.ToUpperInvariant()), StringComparison.Ordinal)
                .Replace("{{CONTACT_PERSON}}", WebUtility.HtmlEncode(content.ContactPerson), StringComparison.Ordinal)
                .Replace("{{CONTACT_NUMBER}}", WebUtility.HtmlEncode(content.ContactNumber), StringComparison.Ordinal);

            return await RenderPdfAsync(html, "230mm", "335mm");
        }

        public async Task<byte[]> GenerateIndigencyAsync(DocumentRequest request)
        {
            var content = DeserializeContent<IndigencyContent>(request);
            var templateDirectory = Path.Combine(_environment.ContentRootPath, "templates");
            var templatePath = Path.Combine(templateDirectory, "indigency.html");
            var html = await File.ReadAllTextAsync(templatePath);
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
                .Replace("{{BRGY_LOGO}}", await ReadImageDataUriAsync(templateDirectory, "brgy_84_logo.png"), StringComparison.Ordinal)
                .Replace("{{PASAY_LOGO}}", await ReadImageDataUriAsync(templateDirectory, "pasay-logo.png"), StringComparison.Ordinal)
                .Replace("{{FULL_NAME}}", WebUtility.HtmlEncode(content.FullName), StringComparison.Ordinal)
                .Replace("{{ADDRESS}}", WebUtility.HtmlEncode(content.Address), StringComparison.Ordinal)
                .Replace("{{PURPOSE}}", WebUtility.HtmlEncode(content.Purpose), StringComparison.Ordinal)
                .Replace("{{day}}", $"{dayOfMonth}{daySuffix}", StringComparison.Ordinal)
                .Replace("{{month}}", issueDate.ToString("MMMM", CultureInfo.InvariantCulture), StringComparison.Ordinal)
                .Replace("{{year}}", issueDate.Year.ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal);

            return await RenderPdfAsync(html, "210mm", "297mm");
        }

        private static TContent DeserializeContent<TContent>(DocumentRequest request)
        {
            if (request.RequestDocumentContent is null)
            {
                throw new InvalidOperationException("The document request does not contain saved form data.");
            }

            return JsonSerializer.Deserialize<TContent>(request.RequestDocumentContent)
                ?? throw new InvalidOperationException("The document request form data is invalid.");
        }

        private static async Task<byte[]> RenderPdfAsync(string html, string width, string height)
        {
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new() { Headless = true });
            var page = await browser.NewPageAsync();
            await page.SetContentAsync(html, new() { WaitUntil = WaitUntilState.NetworkIdle });

            return await page.PdfAsync(new()
            {
                Width = width,
                Height = height,
                PrintBackground = true,
                Margin = new() { Top = "0mm", Right = "0mm", Bottom = "0mm", Left = "0mm" }
            });
        }

        private static async Task<string> ReadImageDataUriAsync(string templateDirectory, string fileName)
        {
            var imagePath = Path.Combine(templateDirectory, "assets", fileName);
            var imageData = Convert.ToBase64String(await File.ReadAllBytesAsync(imagePath));
            return $"data:image/png;base64,{imageData}";
        }

        private sealed class BarangayIdContent
        {
            public string Name { get; init; } = string.Empty;
            public string CivilStatus { get; init; } = string.Empty;
            public string Gender { get; init; } = string.Empty;
            public DateOnly DateOfBirth { get; init; }
            public string HomeAddress { get; init; } = string.Empty;
            public string ContactPerson { get; init; } = string.Empty;
            public string ContactNumber { get; init; } = string.Empty;
            public string PhotoContentType { get; init; } = string.Empty;
            public byte[] PhotoBytes { get; init; } = [];
        }

        private sealed class IndigencyContent
        {
            public string FullName { get; init; } = string.Empty;
            public string Address { get; init; } = string.Empty;
            public string Purpose { get; init; } = string.Empty;
        }
    }
}