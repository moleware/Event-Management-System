using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EventManagementSystem.Models
{
    public class EventViewModel
    {
        public Event Event { get; set; }
        public List<SelectListItem> LocationList { get; set; }
        public List<SelectListItem> UserList { get; set; }
        public List<SelectListItem> OptionList { get; set; }

    }
}