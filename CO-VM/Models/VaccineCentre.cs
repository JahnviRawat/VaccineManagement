﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace CO_VM.Models;

public partial class VaccineCentre
{
    public int VaccineId { get; set; }

    public int CentreId { get; set; }

    public string DoctorName { get; set; }

    public virtual Centre Centre { get; set; }

    public virtual Vaccine Vaccine { get; set; }
}