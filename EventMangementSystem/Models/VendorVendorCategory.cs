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
    
    public partial class VendorVendorCategory
    {
        public int vendorVendorCategoryId { get; set; }
        public int vendorId { get; set; }
        public int vendorCategoryId { get; set; }
    
        public virtual Vendor Vendor { get; set; }
        public virtual VendorCategory VendorCategory { get; set; }
    }
}
