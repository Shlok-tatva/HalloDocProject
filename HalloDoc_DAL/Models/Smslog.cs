﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HalloDoc_DAL.Models;

[Table("smslog")]
public partial class Smslog
{
    [Key]
    [Column("smslogid")]
    [Precision(9, 0)]
    public decimal Smslogid { get; set; }

    [Column("smstemplate")]
    [StringLength(1)]
    public string Smstemplate { get; set; } = null!;

    [Column("mobilenumber")]
    [StringLength(50)]
    public string Mobilenumber { get; set; } = null!;

    [Column("confirmationnumber")]
    [StringLength(200)]
    public string? Confirmationnumber { get; set; }

    [Column("roleid")]
    public int? Roleid { get; set; }

    [Column("adminid")]
    public int? Adminid { get; set; }

    [Column("requestid")]
    public int? Requestid { get; set; }

    [Column("physicianid")]
    public int? Physicianid { get; set; }

    [Column("createdate", TypeName = "timestamp without time zone")]
    public DateTime Createdate { get; set; }

    [Column("sentdate", TypeName = "timestamp without time zone")]
    public DateTime? Sentdate { get; set; }

    [Column("issmssent")]
    public bool? Issmssent { get; set; }

    [Column("senttries")]
    public int Senttries { get; set; }

    [Column("action")]
    [StringLength(500)]
    public string? Action { get; set; }

    [Column("receivername", TypeName = "character varying")]
    public string? Receivername { get; set; }
}
