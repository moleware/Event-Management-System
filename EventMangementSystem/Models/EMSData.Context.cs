﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class EventManagementSystemEntities : DbContext
    {
        public EventManagementSystemEntities()
            : base("name=EventManagementSystemEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Consumable> Consumables { get; set; }
        public virtual DbSet<Contact> Contacts { get; set; }
        public virtual DbSet<DietaryRestriction> DietaryRestrictions { get; set; }
        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<EventIrregularRepeat> EventIrregularRepeats { get; set; }
        public virtual DbSet<EventStatusChoice> EventStatusChoices { get; set; }
        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<LocationConfiguration> LocationConfigurations { get; set; }
        public virtual DbSet<Log> Logs { get; set; }
        public virtual DbSet<RepeatType> RepeatTypes { get; set; }
        public virtual DbSet<resdataACN> resdataACNs { get; set; }
        public virtual DbSet<resdataACT> resdataACTs { get; set; }
        public virtual DbSet<resdataDAT> resdataDATs { get; set; }
        public virtual DbSet<resdataRAC> resdataRACs { get; set; }
        public virtual DbSet<resdataREP> resdataREPs { get; set; }
        public virtual DbSet<Reservation> Reservations { get; set; }
        public virtual DbSet<Resource> Resources { get; set; }
        public virtual DbSet<ResourceType> ResourceTypes { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserFoodProfile> UserFoodProfiles { get; set; }
        public virtual DbSet<UserFoodProfileDietaryRestriction> UserFoodProfileDietaryRestrictions { get; set; }
        public virtual DbSet<Vendor> Vendors { get; set; }
        public virtual DbSet<VendorCategory> VendorCategories { get; set; }
        public virtual DbSet<VendorContact> VendorContacts { get; set; }
        public virtual DbSet<VendorVendorCategory> VendorVendorCategories { get; set; }
        public virtual DbSet<vw_ReservationLocation> vw_ReservationLocation { get; set; }
        public virtual DbSet<ReservationNote> ReservationNotes { get; set; }
        public virtual DbSet<Option> Options { get; set; }
        public virtual DbSet<ReservationConsumable> ReservationConsumables { get; set; }
    }
}