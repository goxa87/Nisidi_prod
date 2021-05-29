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

        public async Task<bool> DeleteImage(string filePath)
        {
            File.Delete(filePath);
            return true;
        }

        public async Task<bool> SaveImageWithoutResizing(IFormFile sourceFile, string outputPath, string suffix = ".jpeg")
        {
            using (var FS = new FileStream(outputPath, FileMode.Create))
            {
                await sourceFile.CopyToAsync(FS);
                FS.Close();
            }
            using (var input = File.OpenRead(outputPath))
            {
                using (var inputStream = new SKManagedStream(input))
                {
                    using (var original = SKBitmap.Decode(inputStream))
                    {
                        using (var image = SKImage.FromBitmap(original))
                        {
                            using (var output = File.OpenWrite(outputPath + suffix))
                            {
                                image.Encode(SKEncodedImageFormat.Jpeg, 100).SaveTo(output);
                            }
                            
                        }
                    }
                }
            }
            File.Delete(outputPath);
            return true;
        }

        public async Task<bool> SaveResizedImage(string originPath, string outputPath, int newSize, string suffix = ".jpeg")
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
                                using (var output = File.OpenWrite(outputPath + suffix))
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
