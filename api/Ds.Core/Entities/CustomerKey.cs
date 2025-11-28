using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Ds.Core.Entities;

[Table("customer_keys")]
[Index(nameof(CustomerId))]
public class CustomerKey
{
    [Column("id"), Key] public int Id { get; set; }
    [Column("customer_id")] public int CustomerId { get; set; }
    [Column("public_key")] public required byte[] PublicKey { get; set; }
    [Column("encrypted_private_key")] public required byte[] EncryptedPrivateKey { get; set; }
    [Column("private_key_salt")] public required byte[] PrivateKeySalt { get; set; }
    [Column("iv")] public required byte[] Iv { get; set; }
    [Column("created_at")] public long CreatedAt { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    [Column("superseded_at")] public long? SupersededAt { get; set; }

    [ForeignKey(nameof(CustomerId))] public virtual Customer? Customer { get; set; }

    [NotMapped] public bool IsActive => SupersededAt == null;
}