using Microsoft.AspNetCore.Mvc;
using ClaimCommander.Services;

namespace ClaimCommander.Controllers
{
    public class ManagerController : Controller
    {
        private readonly IClaimStorageService _storage;
        private readonly IFileEncryptionService _encryption;

        public ManagerController(IClaimStorageService storage, IFileEncryptionService encryption)
        {
            _storage = storage;
            _encryption = encryption;
        }

        [HttpGet]
        public IActionResult Dashboard()
        {
            var approvedClaims = _storage.GetClaimsByStatus("CoordinatorApproved");
            return View(approvedClaims);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ApproveClaim(int claimId)
        {
            try
            {
                var claim = _storage.GetClaim(claimId);
                if (claim == null)
                {
                    TempData["ErrorMessage"] = "Claim not found.";
                    return RedirectToAction(nameof(Dashboard));
                }

                claim.Status = "ManagerApproved";
                _storage.UpdateClaim(claim);

                TempData["SuccessMessage"] = $"Claim {claimId} has been fully approved.";
                return RedirectToAction(nameof(Dashboard));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error approving claim: {ex.Message}";
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
                claim.RejectionReason = reason ?? "Rejected by Academic Manager";
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