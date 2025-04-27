using legallead.permissions.api.Models;
using legallead.permissions.api.Services;
using Microsoft.AspNetCore.Mvc;

namespace legallead.permissions.api.Controllers
{
    [Route("/downloads")]
    public class DownloadController : Controller
    {
        private static readonly object _lock = new();
        private static DownloadContentResponse? _current = null;
        private readonly DownloadContentService _downloadService = new();
        // private static 
        [HttpGet("")]
        public async Task<IActionResult> IndexAsync()
        {
            if (_current != null && DateTime.UtcNow.Subtract(_current.CreationDate).TotalMinutes < 30)
            {
                return GetAction(_current);
            }

            var newContent = await _downloadService.GetDownloadsAsync("Home");
            lock (_lock)
            {
                _current = newContent;
            }
            return GetAction(_current);
        }

        [HttpGet("release/{id}")]
        public IActionResult GetReleaseDetails(long id)
        {
            if (_current == null || !_current.Models.Exists(x => x.Id == id)) return RedirectToAction("");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var item = _current.Models.Find(x => x.Id == id);
            var content = _downloadService.GetContent("Details", item);
            return new ContentResult
            {
                Content = content,
                ContentType = "text/html",
                StatusCode = 200
            };
        }


        [HttpGet("release/{id}/{assetId}")]
        public async Task<IActionResult> GetAssetAsync(long id, long assetId)
        {
            if (!ModelState.IsValid) return BadRequest();
            var bo = _downloadService.GetAssetDetail(id, assetId);
            if (bo == null) return BadRequest();
            var assetName = bo.Assets.Find(x => x.Id == assetId)?.Name;
            if (string.IsNullOrEmpty(assetName)) return BadRequest();
            var assetContent = await _downloadService.GetAssetsAsync(id, assetName);
            if (assetContent == null) return NotFound();

            return File(assetContent, "application/octet-stream", assetName);
        }
        private static ContentResult GetAction(DownloadContentResponse current)
        {
            return new ContentResult
            {
                Content = current.Content,
                ContentType = "text/html",
                StatusCode = 200
            };
        }
    }
}
