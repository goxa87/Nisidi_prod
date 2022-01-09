using EventBLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Services.SupportService
{
    public interface ISupportService
    {

        Task<List<SupportTicket>> GetAllTickets();
        ///// <summary>
        ///// Получить все чаты, которые не были прочитаны техподдержкой
        ///// </summary>
        ///// <returns></returns>
        //Task<List<SupportChat>> GetUnreadSupportChats();

        ///// <summary>
        ///// Отправить сообщение в чат техподдержки (с созданием чата если его нет.)
        ///// </summary>
        ///// <param name="clientId">id клиента, которому пишем</param>
        ///// <param name="message">сообщение</param>
        ///// <param name="supportName">отображаемое имя техподдержки</param>
        ///// <returns></returns>
        //Task SendMessageToSupportChat(string clientId, string message, string supportPersonId, string supportName = "NISIDI.RU");
    }
}
