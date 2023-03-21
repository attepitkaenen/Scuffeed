
public class Post 
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public Flair? Flair { get; set; }
    // public DateTime createdAt { get; } = DateTime.Now;
}