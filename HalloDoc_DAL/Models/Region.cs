using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HalloDoc_DAL.Models;

[Table("region")]
public partial class Region
{
    [Key]
    [Column("regionid")]
    public int Regionid { get; set; }

    [Column("name")]
    [StringLength(50)]
    public string Name { get; set; } = null!;

    [Column("abbreviation")]
    [StringLength(50)]
    public string? Abbreviation { get; set; }

    [InverseProperty("Region")]
    public virtual ICollection<Adminregion> Adminregions { get; set; } = new List<Adminregion>();

    [InverseProperty("Region")]
    public virtual ICollection<Physicianregion> Physicianregions { get; set; } = new List<Physicianregion>();

    [InverseProperty("Region")]
    public virtual ICollection<RBusinessdatum> RBusinessdata { get; set; } = new List<RBusinessdatum>();

    [InverseProperty("Region")]
    public virtual ICollection<RConcierge> RConcierges { get; set; } = new List<RConcierge>();

    [InverseProperty("Region")]
    public virtual ICollection<Requestclient> Requestclients { get; set; } = new List<Requestclient>();

    [InverseProperty("Region")]
    public virtual ICollection<Shiftdetailregion> Shiftdetailregions { get; set; } = new List<Shiftdetailregion>();

    [InverseProperty("Region")]
    public virtual ICollection<Shiftdetail> Shiftdetails { get; set; } = new List<Shiftdetail>();

    [InverseProperty("Region")]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
