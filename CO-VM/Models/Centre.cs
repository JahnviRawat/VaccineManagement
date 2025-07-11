﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace CO_VM.Models;

public partial class Centre
{
    public int CentreId { get; set; }

    public string CentreName { get; set; }

    public string Address { get; set; }

    public string City { get; set; }

    public string State { get; set; }

    public string ContactNumber { get; set; }

    public virtual ICollection<Slot> Slots { get; set; } = new List<Slot>();

    public virtual ICollection<VaccineCentre> VaccineCentres { get; set; } = new List<VaccineCentre>();
}