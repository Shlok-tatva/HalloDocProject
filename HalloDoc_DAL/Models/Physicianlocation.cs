﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HalloDoc_DAL.Models;

[Table("physicianlocation")]
public partial class Physicianlocation
{
    [Key]
    [Column("locationid")]
    public int Locationid { get; set; }

    [Column("physicianid")]
    public int? Physicianid { get; set; }

    [Column("latitude")]
    public float? Latitude { get; set; }

    [Column("longtitude")]
    public float? Longtitude { get; set; }

    [Column("createddate", TypeName = "timestamp without time zone")]
    public DateTime? Createddate { get; set; }

    [Column("physicianname")]
    [StringLength(50)]
    public string? Physicianname { get; set; }

    [Column("address")]
    [StringLength(500)]
    public string? Address { get; set; }

    [ForeignKey("Physicianid")]
    [InverseProperty("Physicianlocations")]
    public virtual Physician? Physician { get; set; }
}
