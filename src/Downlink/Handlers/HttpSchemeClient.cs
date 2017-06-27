using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Downlink.Core;
using Microsoft.AspNetCore.Mvc;

namespace Downlink.Handlers
{
    public class HttpSchemeClient : SchemeClient
    {
        public HttpSchemeClient() : base(new[] { "http", "https" })
        {
            DownloadLocation = System.IO.Path.GetTempPath();
        }

        public string DownloadLocation { get; private set; }

        public override async Task<IActionResult> GetContentAsync(IFileSource file)
        {
            if (!Schemes.Contains(file.FileUri.Scheme))
            {
                throw new InvalidOperationException();
            }
            var fileStream = await DownloadRemoteFileAsync(file.FileUri);
            return new FileStreamResult(fileStream, "application/octet-stream");
        }

        private async System.Threading.Tasks.Task<Stream> DownloadRemoteFileAsync(Uri requestUri)
        {
            using (var client = new HttpClient())
            {
                var ms = new MemoryStream();
                var stream = (await client.GetStreamAsync(requestUri)).CopyToAsync(ms);
                return ms;
            }
        }
    }
}