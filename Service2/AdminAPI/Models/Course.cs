namespace AdminAPI.Models
{
    public class Course
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public byte Updated { get; set; }
        public byte Deleted { get; set; }
    }
}
