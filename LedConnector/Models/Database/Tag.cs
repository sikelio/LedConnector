using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LedConnector.Models.Database
{
    [Table("tags")]
    public class Tag
    {
        [Key]
        [Required]
        [Column("id_tag")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        public virtual ICollection<Message> Messages { get; set; }
    }
}
