namespace BlazorLeaflet.Models;

public record GeoJsonDataLayer : InteractiveLayer
{
    public string? GeoJsonData { get; set; }
}