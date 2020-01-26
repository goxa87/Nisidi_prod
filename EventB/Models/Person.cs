using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EventB.Models
{
    /// <summary>
    ///  пользователь пиложения
    /// </summary>
    public class Person
    {
        /// <summary>
        /// Идентификатор пользователя 
        /// </summary>
        public int PersonId { get; set; }
        /// <summary>
        /// псевдоним пользователя
        /// </summary>
        [Required]
        [StringLength(100,MinimumLength =4)]
        public string Name { get; set; }
        /// <summary>
        /// Адрес электронной почты
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// роль в системе
        /// </summary>
        [Required]
        public string Role { get; set; }
        /// <summary>
        /// город привязки для поиска
        /// </summary>
        [Required]
        public string Sity { get; set; }
        /// <summary>
        /// строка интересов для поиска событий
        /// </summary>
        public string Interest { get; set; }
        /// <summary>
        /// аксессор , который возвращает List<string> интересов пользователя 
        /// </summary>
        public List<string> InterestList 
        {
            get
            {
                // логика получения списка интересов из строки БД Interest

                return new List<string>();            
            }
        }
        /// <summary>
        /// список друзей или подписок пользователя для представление в БД
        /// </summary>
        public string Friends { get; set; }
        /// <summary>
        /// список друзей пользователя для отображения и обработки внутри приложения
        /// </summary>
        public List<int> FriendsList
        {
            get
            {
                //логика получения id пользователей из строки Friends
                return new List<int>();
            }
        }
        /// <summary>
        /// список чатов, в которых участвует пользователь
        /// </summary>
        public List<UserChat> UserChat { get; set; } 

        /// <summary>
        /// связанный список событий на которые пойдет пользователь
        /// </summary>
        public List<Vizitors> Vizitors { get; set; }
    }
}
