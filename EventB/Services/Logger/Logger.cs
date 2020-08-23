using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventB.Services.Logger
{
    public class Logger : ILogger
    {
        private readonly string logPath;
        private static object locker;

        public Logger(string path)
        {
            logPath = path;
            locker = new object();
            // Проверяем директорию.

            var isDirectory = Directory.Exists(logPath);
            if(!isDirectory)
            {
                Directory.CreateDirectory(logPath);
            }
        }

        public async Task LogObjectToFile(string notaition, object data)
        {
            var logDate = DateTime.Now.ToString("dd_MM_yyyy");
            var currentFilePath = $"{logPath}/EBLog_{logDate}.txt";
            var isFileExist = File.Exists(currentFilePath);
            if (!isFileExist)
            {
                using (StreamWriter stream = File.CreateText(currentFilePath))
                {
                    await stream.WriteLineAsync($"Лог за {logDate}");
                }
            }

            var objProps = data.GetType().GetProperties();

            var SB = new StringBuilder();
            var n = "\n";
            SB.Append(notaition + n);
            SB.Append($"Передан объект для логгирования: {n}");
            foreach (var element in objProps)
            {
                SB.Append($"\t{element.Name} : {element.GetValue(data)}");
                SB.Append(n);
            }
            
            using (var SW = File.AppendText(currentFilePath))
            {               
                await SW.WriteLineAsync($"{DateTime.Now:hh.mm.sss}:\n {SB}");
            }
        }

        /// <summary>
        /// Логгирует строку в файл
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task LogStringToFile(string message)
        {
            var logDate = DateTime.Now.ToString("dd_MM_yyyy");
            var currentFilePath = $"{logPath}/EBLog_{logDate}.txt";
            var isFileExist = File.Exists(currentFilePath);
            if(!isFileExist)
            {
                using (StreamWriter stream = File.CreateText(currentFilePath))
                {
                    await stream.WriteLineAsync($"Лог за {logDate}");
                }
            }
            using (var SW = File.AppendText(currentFilePath))
            {
                await SW.WriteLineAsync($"{DateTime.Now.ToString("hh.mm.sss")}: {message}");
            }
        }

        public Task LogToDB(object data)
        {
            throw new NotImplementedException();
        }
    }
}
