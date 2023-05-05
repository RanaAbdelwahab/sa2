namespace StudentAPI.Model
{
    public class CourseDto
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public byte Updated { get; set; }
        public byte Deleted { get; set; }
    }
}
