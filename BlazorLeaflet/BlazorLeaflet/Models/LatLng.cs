using System.Drawing;

namespace BlazorLeaflet.Models;

public record LatLng
{
    public LatLng()
    {
    }

    public LatLng(PointF position) : this(position.X, position.Y)
    {
    }

    public LatLng(float lat, float lng)
    {
        Lat = lat;
        Lng = lng;
    }

    public LatLng(float lat, float lng, float alt) : this(lat, lng)
    {
        Alt = alt;
    }

    public float Lat { get; set; }

    public float Lng { get; set; }

    public float Alt { get; set; }

    public PointF ToPointF() => new(Lat, Lng);
}