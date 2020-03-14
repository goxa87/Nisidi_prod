using EventB.Auth;
using EventB.Data;
using EventB.Models;
using EventB.ViewModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventB.Services
{
    public class ViewModelFactory : IViewModelFactory
    {
        private IDataProvider _data { get; set; }
        /// <summary>
        /// генерирует EventDetailsViewModel из id
        /// </summary>
        /// <param name="user">менеджер аккаунта</param>
        /// <param name="eventId">id события для которого нужны детали</param>
        /// <param name="db">Dataprovider для доступа к данным</param>
        /// <returns></returns>
        public EventDetailsViewModel GetEventDetailsViewModel(SignInManager<User> user, int eventId, IDataProvider db)
        {
            _data = db;
            // событие
            
            Event _event = _data.GetEvents().Where(e => e.EventId == eventId).FirstOrDefault();
            
            // имя создателя

            //string _name = db.GetPersons().Where(e => e.PersonId == _event.Creator).FirstOrDefault().Name;
            string _name = "db";
            //посетители в виде (id - имя)
            Dictionary<int, string> _vizitors = new Dictionary<int, string>();
            //из БД

            var p = db.GetPersons();
            var v = db.GetVizitors().Where(e=>e.EventId == eventId);


            if (v.Any())
            {
                var viz = p.Join(
                        v
                        , p => p.PersonId
                        , v => v.PersonId
                        , (p, v) => new { personName = p.Name, personId = v.PersonId }
                    );
                foreach (var e in viz)
                {
                    _vizitors.Add(e.personId, e.personName);
                }
            }
            //var viz = db.GetVizitors().Where(e => e.EventId == eventId)
            //    .Join(db.GetPersons(), e => e.PersonId, p=>p.PersonId , (e, p) =>
            //        new {
            //            personId = e.PersonId,
            //            personName = p.Name
            //        }).ToList();  
            

            //приведение к виду словаря
           
            //выбор сообщений если соответствующий чат работает
            var _messages = new List<Message>();

            if (_event.ChatId != null)
            {
                _messages = db.GetMessage().Where(e => e.ChatId == _event.ChatId.Value).OrderByDescending(e=>e.PostDate).ToList();
            }

            // модель
            var model = new EventDetailsViewModel {
                Event = _event,
                CreatorName = _name,
                Messages = _messages,
                Vizitors = _vizitors
            };

            return model;
        }
    }
}
