using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventB.Services.ImageService
{
    public interface IImageService
    {
        Task<bool> SaveResizedImage(string originPath, string outputPath, int newSize);
        Task<bool> SaveImageWithoutResizing(IFormFile file, string path);
    }
}
