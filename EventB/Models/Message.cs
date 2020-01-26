using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EventB.Models
{
    /// <summary>
    /// Представляет сообщение в чате
    /// </summary>
    public class Message
    {
        /// <summary>
        /// идентификатор сообщения
        /// </summary>
        public int MessageId { get; set; }
        /// <summary>
        /// Идентификатор чата к которому относится сообщение
        /// </summary>
        [Required]
        public int ChatId { get; set; }
        /// <summary>
        /// идентификатор автора сообщения
        /// </summary>
        [Required]
        public int PersonId { get; set; }
        /// <summary>
        /// текст сообщения
        /// </summary>
        public string Body { get; set; }
        /// <summary>
        /// дата публикации сообщения
        /// </summary>
        [Required]
        [DataType(DataType.DateTime), DisplayFormat(DataFormatString ="{0:dd.MM.yy hh.mm}",ApplyFormatInEditMode =true)]
        public DateTime PostDate { get; set; }
    }
}
