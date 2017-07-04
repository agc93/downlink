using System;
using System.Threading.Tasks;
using Downlink.Core;
using Microsoft.AspNetCore.Mvc;

namespace Downlink.Handlers
{
    public class FileSchemeClient : SchemeClient
    {
        public FileSchemeClient() : base("file") { }
        public override Task<IActionResult> GetContentAsync(IFileSource file)
        {
            return Task.FromResult(new FileStreamResult(System.IO.File.OpenRead(file.FileUri.AbsolutePath), "application/octet-stream") as IActionResult);
        }
    }
}