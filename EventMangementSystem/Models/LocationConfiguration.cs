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
    
    public partial class LocationConfiguration
    {
        public int locationConfigurationId { get; set; }
        public int locationId { get; set; }
        public string name { get; set; }
    
        public virtual Location Location { get; set; }
    }
}
