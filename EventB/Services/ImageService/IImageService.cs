using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventB.Services.ImageService
{
    public interface IImageService
    {
        /// <summary>
        /// возмет оригинал картинки и сохранит ее с ресайзингом
        /// </summary>
        /// <param name="originPath">Путь к оригиналу</param>
        /// <param name="outputPath">выходной путь</param>
        /// <param name="newSize">новый размер картинки</param>
        /// <param name="suffix">формат новой картинки</param>
        /// <returns></returns>
        Task<bool> SaveResizedImage(string originPath, string outputPath, int newSize, string suffix = ".jpeg");
        /// <summary>
        /// Сохранит оригинал картинки и оригинал в формате суффикса
        /// </summary>
        /// <param name="sourceFile">Оригинал источник</param>
        /// <param name="outputPath">Выходной путь сохранения</param>
        /// <param name="suffix">расширение файла</param>
        /// <returns></returns>
        Task<bool> SaveImageWithoutResizing(IFormFile sourceFile, string outputPath, string suffix = ".jpeg");
        /// <summary>
        /// Удалит файл
        /// </summary>
        /// <param name="filePath">путь к файлу</param>
        /// <returns></returns>
        Task<bool> DeleteImage(string filePath);
    }
}
