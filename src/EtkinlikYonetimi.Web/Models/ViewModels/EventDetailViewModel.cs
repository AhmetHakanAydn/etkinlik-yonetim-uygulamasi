using EtkinlikYonetimi.Business.DTOs;
using System.Collections.Generic;

namespace EtkinlikYonetimi.Web.Models.ViewModels
{
    public class EventDetailViewModel
    {
        public EventDto Event { get; set; } = null!;
        public IEnumerable<EventDto> LatestEvents { get; set; } = new List<EventDto>();
    }
}