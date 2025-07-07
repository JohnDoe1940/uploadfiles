using Microsoft.AspNetCore.StaticFiles;
using Microsoft.JSInterop;

namespace Blazorserver.Services;

public interface IFileDownload
{
    Task<List<string>> GetUploadedFiles();
    Task DownloadFile(string url);
}

public class FileDownload : IFileDownload
{
    private readonly IJSRuntime _js;
    private readonly ILogger<FileDownload> _logger;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public FileDownload(IWebHostEnvironment webHostEnvironment, IJSRuntime js, ILogger<FileDownload> logger)
    {
        _webHostEnvironment = webHostEnvironment;
        _js = js;
        _logger = logger;
    }

    public async Task<List<string>> GetUploadedFiles()
    {
        var base64Urls = new List<string>();
        try
        {
            var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadPath))
            {
                return base64Urls;
            }

            var files = Directory.GetFiles(uploadPath);

            foreach (var file in files)
            {
                using var fileInput = new FileStream(file, FileMode.Open, FileAccess.Read);
                using var memoryStream = new MemoryStream();
                await fileInput.CopyToAsync(memoryStream);

                var buffer = memoryStream.ToArray();
                var fileType = GetMimeTypeForFileExtension(file);
                var fileName = Path.GetFileName(file);
                base64Urls.Add($"data:{fileType};name:{fileName};base64,{Convert.ToBase64String(buffer)}");
            }
        }
        catch (Exception)
        {
            throw new Exception("Error getting uploaded files");
        }

        return base64Urls;
    }

    public async Task DownloadFile(string url)
    {
        await _js.InvokeVoidAsync("downloadFile", url);
    }

    private string GetMimeTypeForFileExtension(string filePath)
    {
        const string DefaultContentType = "application/octet-stream";
        var provider = new FileExtensionContentTypeProvider();
        if (!provider.TryGetContentType(filePath, out var contentType))
            contentType = DefaultContentType;
        return contentType;
    }
}