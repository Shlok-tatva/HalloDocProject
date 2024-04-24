using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TMC_DAL.Models;

[Table("tasks")]
public partial class Task
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("taskname")]
    [StringLength(150)]
    public string Taskname { get; set; } = null!;

    [Column("assignee")]
    [StringLength(150)]
    public string Assignee { get; set; } = null!;

    [Column("categoryid")]
    public int? Categoryid { get; set; }

    [Column("discription", TypeName = "character varying")]
    public string? Discription { get; set; }

    [Column("duedate", TypeName = "timestamp without time zone")]
    public DateTime Duedate { get; set; }

    [Column("category")]
    [StringLength(150)]
    public string? Category { get; set; }

    [Column("city")]
    [StringLength(150)]
    public string City { get; set; } = null!;

    [ForeignKey("Categoryid")]
    [InverseProperty("Tasks")]
    public virtual Category? CategoryNavigation { get; set; }
}
