using Microsoft.AspNetCore.Mvc;
using ClaimCommander.Services;

namespace ClaimCommander.Controllers
{
    public class CoordinatorController : Controller
    {
        private readonly IClaimStorageService _storage;
        private readonly IFileEncryptionService _encryption;

        public CoordinatorController(IClaimStorageService storage, IFileEncryptionService encryption)
        {
            _storage = storage;
            _encryption = encryption;
        }

        [HttpGet]
        public IActionResult Dashboard()
        {
            var pendingClaims = _storage.GetClaimsByStatus("Pending");
            return View(pendingClaims);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult VerifyClaim(int claimId)
        {
            try
            {
                var claim = _storage.GetClaim(claimId);
                if (claim == null)
                {
                    TempData["ErrorMessage"] = "Claim not found.";
                    return RedirectToAction(nameof(Dashboard));
                }

                claim.Status = "CoordinatorApproved";
                _storage.UpdateClaim(claim);

                TempData["SuccessMessage"] = $"Claim {claimId} verified and forwarded to Academic Manager.";
                return RedirectToAction(nameof(Dashboard));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error verifying claim: {ex.Message}";
                return RedirectToAction(nameof(Dashboard));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RejectClaim(int claimId, string? reason)
        {
            try
            {
                var claim = _storage.GetClaim(claimId);
                if (claim == null)
                {
                    TempData["ErrorMessage"] = "Claim not found.";
                    return RedirectToAction(nameof(Dashboard));
                }

                claim.Status = "Rejected";
                claim.RejectionReason = reason ?? "Rejected by Programme Coordinator";
                _storage.UpdateClaim(claim);

                TempData["SuccessMessage"] = $"Claim {claimId} has been rejected.";
                return RedirectToAction(nameof(Dashboard));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error rejecting claim: {ex.Message}";
                return RedirectToAction(nameof(Dashboard));
            }
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
                return RedirectToAction(nameof(Dashboard));
            }
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