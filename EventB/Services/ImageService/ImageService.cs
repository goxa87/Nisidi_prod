using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SkiaSharp;
using System.IO;
using EventB.Services.Logger;
using Microsoft.AspNetCore.Hosting;

namespace EventB.Services.ImageService
{
    public class ImageService : IImageService
    {
        const int RESIZE_COMPRESS_QUALITY = 90;
        const string DEFAULT_IMAGE_PATH = "/images/defaultimg.jpg";

        private readonly ILogger logger;
        private readonly IWebHostEnvironment environment;

        public ImageService(ILogger logger,
            IWebHostEnvironment _environment)
        {
            this.logger = logger;
            environment = _environment;
        }
        
        public async Task<Dictionary<int, string>> SaveOriginAndResizedImagesByInputedSizes(IFormFile originFile, string suffix, Dictionary<int, string> requiredSizesWithPaths)
        {
            var result = new Dictionary<int, string>();
            //Сначала надо сохранить оригиал и отнего ресайзить
            var inputHasOriginPath = requiredSizesWithPaths.TryGetValue(0, out string originPath);
            if(!inputHasOriginPath)
            {
                originPath = $"{environment.WebRootPath}/images/tempimg{DateTime.Now.Millisecond}";
            }
            try
            {
                await SaveImageWithoutResizing(originFile, environment.WebRootPath + originPath, suffix);
                if (inputHasOriginPath)
                {
                    result.Add(0, originPath + suffix);
                }
            }
            catch(Exception ex)
            {
                await logger.LogStringToFile($"Ошибка создания картинок для события : {ex.Message}");
            }

            // Сохраняем остальные взяв за основу исходник
            foreach (var image in requiredSizesWithPaths)
            {
                try
                {
                    if(image.Key != 0)
                    {
                        await SaveResizedImage(environment.WebRootPath + originPath + suffix, environment.WebRootPath + image.Value , image.Key, suffix);
                        result.Add(image.Key, requiredSizesWithPaths[image.Key] + suffix);
                    }
                }
                catch (Exception ex)
                {
                    await logger.LogStringToFile($"Ошибка создания картинок для события : {ex.Message}");
                    await SaveResizedImage(environment.WebRootPath + DEFAULT_IMAGE_PATH + suffix, environment.WebRootPath + image.Value, image.Key, suffix);
                    result.Add(image.Key, requiredSizesWithPaths[image.Key] + suffix);
                }
            }
            if (!inputHasOriginPath)
            {
                await DeleteImage(originPath);
            }
            return result;
        }

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
