using System;

namespace OnTime.API.Models.Domain;

public class Session
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public TimeSpan Duration { get; set; }

    public Session(string title, string description, TimeSpan duration)
    {
        Title = title;
        Description = description;
        Duration = duration;
    }
}
