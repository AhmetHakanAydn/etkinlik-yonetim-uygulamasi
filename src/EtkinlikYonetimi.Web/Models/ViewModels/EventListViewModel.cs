using EtkinlikYonetimi.Business.DTOs;
using System.Collections.Generic;

namespace EtkinlikYonetimi.Web.Models.ViewModels
{
    public class EventListViewModel
    {
        public IEnumerable<EventDto> Events { get; set; } = new List<EventDto>();
        public string? PageTitle { get; set; }
        public string? PageDescription { get; set; }
    }
}