using SQLite;

namespace Korean_Vocabulary_new.Models
{
    [Table("Categories")]
    [Preserve(AllMembers = true)]
    public class Category
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(7)]
        public string Color { get; set; } = "#512BD4"; // Hex color

        public int DisplayOrder { get; set; } = 0; // For sorting/reordering

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}


