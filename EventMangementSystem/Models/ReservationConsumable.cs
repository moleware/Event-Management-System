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
    
    public partial class ReservationConsumable
    {
        public int reservationId { get; set; }
        public int consumableId { get; set; }
        public int quantity { get; set; }
        public Nullable<int> reservationParticipantId { get; set; }
    
        public virtual Consumable Consumable { get; set; }
        public virtual Reservation Reservation { get; set; }
    }
}