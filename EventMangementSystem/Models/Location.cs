//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EventManagementSystem.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Location
    {
        public Location()
        {
            this.LocationConfigurations = new HashSet<LocationConfiguration>();
            this.Reservations = new HashSet<Reservation>();
            this.Events = new HashSet<Event>();
        }
    
        public int locationId { get; set; }
        public string name { get; set; }
        public int maxOccupancy { get; set; }
    
        public virtual ICollection<LocationConfiguration> LocationConfigurations { get; set; }
        public virtual ICollection<Reservation> Reservations { get; set; }
        public virtual ICollection<Event> Events { get; set; }
    }
}
