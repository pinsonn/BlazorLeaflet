using System.Drawing;

namespace BlazorLeaflet.Demo.Data;

public class City
{
    public string? CoatOfArmsImageUrl { get; init; }

    public string? Country { get; init; }

    public string? Name { get; init; }

    public string? Description { get; init; }

    public PointF Coordinates { get; init; }
}