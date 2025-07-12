using System;

namespace OnTime.API.Models.Requests;

public class ServiceRequest(string title, string description, TimeSpan duration)
{
    public string Title { get; set; } = title;
    public string Description { get; set; } = description;
    public TimeSpan Duration { get; set; } = duration;
}
