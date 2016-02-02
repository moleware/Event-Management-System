using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EventManagementSystem.Models
{
    public class CalendarViewModel
    {
        public DateTime StartTime { get; set; }
        public int HoursToDisplay { get; set; }
        public int MinuteIncrements { get; set; }
        public List<Location> Locations = new List<Location>();
        public List<Reservation> Reservations = new List<Reservation>();
    }
}