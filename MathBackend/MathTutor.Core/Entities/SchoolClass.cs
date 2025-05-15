namespace MathTutor.Core.Entities;

public class SchoolClass
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    // Navigation properties
    public virtual ICollection<MathTopic> Topics { get; set; } = new List<MathTopic>();
} 