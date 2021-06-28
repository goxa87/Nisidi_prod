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
        Task<bool> SaveResizedImage(string originPath, string outputPath, int newSize, string suffix = ".jpeg", bool verticalSizing = false);
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

        /// <summary>
        /// Запишет в тот же словарь с путями к файлам с ресайзингом и оигиналом. Конвертирует в заданный формат.
        /// в ответе ключ 0 это путь к оригинальному изображению с заданным суффиксм
        /// цифровые ключи не 0 - это пути к ресайженым изображениям
        /// </summary>
        /// <param name="originFile"></param>
        /// <param name="suffix"></param>
        /// <param name="requiredSizesWithPaths">Словарь: размер - путь куда сохранять картинки</param>
        /// <returns> 0 путь с оригиналом другие не с оригиналом</returns>
        Task<Dictionary<int, string>> SaveOriginAndResizedImagesByInputedSizes(IFormFile originFile, string suffix, Dictionary<int, string> requiredSizesWithPaths, int? verticatlSizeForDefaualtImage);
    }
}
