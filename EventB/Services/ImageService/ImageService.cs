using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SkiaSharp;
using System.IO;

namespace EventB.Services.ImageService
{
    public class ImageService : IImageService
    {
        const int RESIZE_COMPRESS_QUALITY = 90;
        public async Task<bool> SaveImageWithoutResizing(IFormFile file, string path)
        {
            using (var FS = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(FS);
                FS.Close();
            }
            using (var input = File.OpenRead(path))
            {
                using (var inputStream = new SKManagedStream(input))
                {
                    using (var original = SKBitmap.Decode(inputStream))
                    {
                        using (var image = SKImage.FromBitmap(original))
                        {
                            using (var output = File.OpenWrite(path))
                            {
                                image.Encode(SKEncodedImageFormat.Jpeg, 100).SaveTo(output);
                            }
                            return true;
                        }
                    }
                }
            }
        }

        public async Task<bool> SaveResizedImage(string originPath, string outputPath, int newSize)
        {
            var size = newSize;
            const int quality = RESIZE_COMPRESS_QUALITY;
            using (var input = File.OpenRead(originPath))
            {
                using (var inputStream = new SKManagedStream(input))
                {
                    using (var original = SKBitmap.Decode(inputStream))
                    {
                        int width, height;
                        if (original.Width > original.Height)
                        {
                            width = size;
                            height = original.Height * size / original.Width;
                        }
                        else
                        {
                            width = original.Width * size / original.Height;
                            height = size;
                        }

                        using (var resized = original.Resize(new SKImageInfo(width, height), SKBitmapResizeMethod.Lanczos3))
                        {
                            if (resized == null) return false;

                            using (var image = SKImage.FromBitmap(resized))
                            {
                                using (var output = File.OpenWrite(outputPath))
                                {
                                    image.Encode(SKEncodedImageFormat.Jpeg, quality).SaveTo(output);
                                }
                                return true;
                            }
                        }
                    }
                }
            }
        }
    }
}
