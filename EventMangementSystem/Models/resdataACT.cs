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
    
    public partial class resdataACT
    {
        public int Rec_No { get; set; }
        public int Actnumber { get; set; }
        public string Name { get; set; }
        public string Client { get; set; }
        public string Prime { get; set; }
        public Nullable<int> Units { get; set; }
        public string Username { get; set; }
        public Nullable<System.DateTime> Changedate { get; set; }
        public Nullable<System.DateTime> Changetime { get; set; }
        public Nullable<int> Lock { get; set; }
        public Nullable<int> Repeattype { get; set; }
        public Nullable<int> Tentative { get; set; }
        public Nullable<int> Private { get; set; }
        public string Project { get; set; }
        public Nullable<int> Inactive { get; set; }
        public string Misc { get; set; }
        public Nullable<int> Forecolour { get; set; }
        public Nullable<int> Backcolour { get; set; }
    }
}