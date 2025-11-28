using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Ds.Core.Entities;

[Table("customers")]
[Index(nameof(Email), IsUnique = true)]
public class Customer
{
    [Column("id"), Key] public int Id { get; set; }
    [Column("email"), MaxLength(255)] public required string Email { get; set; }
    [Column("active_key_id")] public int? ActiveKeyId { get; set; }

    [ForeignKey(nameof(ActiveKeyId))] public virtual CustomerKey? ActiveKey { get; set; }
}