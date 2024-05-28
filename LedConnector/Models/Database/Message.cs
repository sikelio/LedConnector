using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LedConnector.Models.Database
{
    [Table("messages")]
    public class Message
    {
        [Key]
        [Required]
        [Column("id_message")]
        public int Id { get; set; }

        [Column("raw_message")]
        public string RawMessage { get; set; }

        [Column("binary_message")]
        public string BinaryMessage { get; set; }

        public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
    }
}
