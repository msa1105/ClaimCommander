using Microsoft.AspNetCore.Mvc;
using ClaimCommander.Models;
using ClaimCommander.Services;

namespace ClaimCommander.Controllers
{
    public class LecturerController : Controller
    {
        private readonly IClaimStorageService _storage;
        private readonly IFileEncryptionService _encryption;
        private readonly IWebHostEnvironment _environment;
        private const long MaxFileSize = 5 * 1024 * 1024; // 5MB
        private readonly string[] AllowedExtensions = { ".pdf", ".docx", ".xlsx" };

        public LecturerController(
            IClaimStorageService storage,
            IFileEncryptionService encryption,
            IWebHostEnvironment environment)
        {
            _storage = storage;
            _encryption = encryption;
            _environment = environment;
        }

        [HttpGet]
        public IActionResult SubmitClaim()
        {
            return View(new Claim());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitClaim(Claim claim, IFormFile? documentFile)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["ErrorMessage"] = "Please correct the errors in the form.";
                    return View(claim);
                }

                // Handle file upload if provided
                if (documentFile != null)
                {
                    var validationError = ValidateFile(documentFile);
                    if (validationError != null)
                    {
                        TempData["ErrorMessage"] = validationError;
                        return View(claim);
                    }

                    var uploadPath = Path.Combine(_environment.WebRootPath, "uploads");
                    var encryptedPath = await _encryption.EncryptAndSaveFileAsync(documentFile, uploadPath);

                    claim.Documents.Add(new DocumentInfo
                    {
                        FileName = documentFile.FileName,
                        EncryptedFilePath = encryptedPath,
                        FileSize = documentFile.Length
                    });
                }

                var claimId = _storage.AddClaim(claim);
                TempData["SuccessMessage"] = $"Claim submitted successfully! Claim ID: {claimId}";
                return RedirectToAction(nameof(ViewClaims));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error submitting claim: {ex.Message}";
                return View(claim);
            }
        }

        [HttpGet]
        public IActionResult ViewClaims()
        {
            var claims = _storage.GetAllClaims();
            return View(claims);
        }

        [HttpGet]
        public async Task<IActionResult> DownloadDocument(int claimId, int documentIndex)
        {
            try
            {
                var claim = _storage.GetClaim(claimId);
                if (claim == null || documentIndex >= claim.Documents.Count)
                {
                    return NotFound("Document not found");
                }

                var document = claim.Documents[documentIndex];
                var decryptedBytes = await _encryption.DecryptFileAsync(document.EncryptedFilePath);

                var contentType = GetContentType(document.FileName);
                return File(decryptedBytes, contentType, document.FileName);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error downloading document: {ex.Message}";
                return RedirectToAction(nameof(ViewClaims));
            }
        }

        private string? ValidateFile(IFormFile file)
        {
            if (file.Length > MaxFileSize)
                return $"File size exceeds the maximum limit of {MaxFileSize / 1024 / 1024}MB.";

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
                return $"Invalid file type. Only {string.Join(", ", AllowedExtensions)} files are allowed.";

            return null;
        }

        private string GetContentType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return extension switch
            {
                ".pdf" => "application/pdf",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                _ => "application/octet-stream"
            };
        }
    }
}