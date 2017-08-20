using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Downlink.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Downlink.Handlers
{
    public class HttpDownloadClient : SchemeClient
    {
        private readonly ILogger<HttpDownloadClient> _logger;

        public HttpDownloadClient(ILogger<HttpDownloadClient> logger) : base(new[] { "http", "https" })
        {
            _logger = logger;
            DownloadLocation = System.IO.Path.GetTempPath();
        }

        public string DownloadLocation { get; private set; }

        public override async Task<IActionResult> GetContentAsync(IFileSource file)
        {
            if (!Schemes.Contains(file.FileUri.Scheme))
            {
                _logger.LogError(405, $"Scheme mismatch! '{file.FileUri.Scheme}' not supported..");
                throw new InvalidOperationException();
            }
            _logger.LogInformation("Downloading remote file from {0}", file.FileUri.ToString());
            var fileStream = await DownloadRemoteFileAsync(file.FileUri);
            //_logger.LogDebug("Downloaded remote file to stream (length {0})", fileStream.Length);
            _logger.LogDebug("File downloaded, streaming {0} to browser...", file.Metadata.FileName);
            return new FileStreamResult(fileStream, "application/octet-stream") {
                FileDownloadName = file.Metadata.FileName
            };
        }

        private async System.Threading.Tasks.Task<Stream> DownloadRemoteFileAsync(Uri requestUri)
        {
            using (var client = new HttpClient())
            {
                return await client.GetStreamAsync(requestUri);
            }
        }
    }
}