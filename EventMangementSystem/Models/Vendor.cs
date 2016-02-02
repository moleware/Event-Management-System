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
    
    public partial class Vendor
    {
        public Vendor()
        {
            this.Consumables = new HashSet<Consumable>();
            this.VendorContacts = new HashSet<VendorContact>();
            this.VendorVendorCategories = new HashSet<VendorVendorCategory>();
        }
    
        public int vendorId { get; set; }
        public string name { get; set; }
    
        public virtual ICollection<Consumable> Consumables { get; set; }
        public virtual ICollection<VendorContact> VendorContacts { get; set; }
        public virtual ICollection<VendorVendorCategory> VendorVendorCategories { get; set; }
    }
}
