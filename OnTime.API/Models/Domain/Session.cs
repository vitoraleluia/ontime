namespace OnTime.API.Models.Domain;

public class Session : BaseEntity
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int DurationInMinutes { get; set; }

    public Session(string title, string description, int durationInMinutes)
    {
        Title = title;
        Description = description;
        DurationInMinutes = durationInMinutes;
    }
}
