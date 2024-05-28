using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LedConnector.Models.Database
{
    [Table("message_tags")]
    public class MessageTag
    {
        [Key]
        [Required]
        [Column("id_message")]
        public int MessageId { get; set; }

        [Key]
        [Required]
        [Column("id_tag")]
        public int TagId { get; set; }
    }
}
