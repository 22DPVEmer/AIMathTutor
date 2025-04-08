namespace MathTutor.Core.Models;

public class MathCategoryModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<MathTopicModel> Topics { get; set; } = new List<MathTopicModel>();
} 