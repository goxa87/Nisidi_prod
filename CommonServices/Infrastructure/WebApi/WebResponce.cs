using System;
using System.Collections.Generic;
using System.Text;

namespace CommonServices.Infrastructure.WebApi
{
    /// <summary>
    /// Класс содержимого ответа с ошибкой
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WebResponce<T>
    {
        /// <summary>
        /// Содержимое успешного ответа.
        /// </summary>
        public T Content { get; set; }

        /// <summary>
        /// Признак того, что запрос выполнился успешно и содержит корректный контент
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Сообщение об ошибке, если обработка запроса прошла неуспешно
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Экземпляр ответа с содержимым и описанием
        /// </summary>
        /// <param name="content"></param>
        public WebResponce(T content)
        {
            Content = content;
            IsSuccess = true;
            ErrorMessage = "";
        }

        /// <summary>
        /// Экземпляр ответа с содержимым и описанием
        /// </summary>
        /// <param name="content"></param>
        /// <param name="isSuccess"></param>
        /// <param name="errorMessage"></param>
        public WebResponce(T content, bool isSuccess, string errorMessage)
        {
            Content = content;
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
        }
    }
}
