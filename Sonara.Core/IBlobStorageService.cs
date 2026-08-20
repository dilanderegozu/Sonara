using System;
using System.Collections.Generic;
using System.Text;

namespace Sonara.CoreLayer
{
    public interface IBlobStorageService
    {
        Task<string> UploadFileAsync(Stream fileStream, string fileName, string containerName, string? contentType = null);
    }
}
