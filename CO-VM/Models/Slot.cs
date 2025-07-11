﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CO_VM.Models;

public partial class Slot
{
    public int SlotId { get; set; }

    public int VaccineId { get; set; }

    public int CentreId { get; set; }

    [Required(ErrorMessage = "Slot date is required")]
    [DataType(DataType.Date)]
    public DateOnly SlotDate { get; set; }

    [Required(ErrorMessage = "Slot time is required")]
    [DataType(DataType.Time)]
    public TimeOnly SlotTime { get; set; }

    public int? UserId { get; set; }

    public int? FamilyId { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual Centre Centre { get; set; }

    public virtual Family Family { get; set; }

    public virtual User User { get; set; }

    public virtual Vaccine Vaccine { get; set; }
}