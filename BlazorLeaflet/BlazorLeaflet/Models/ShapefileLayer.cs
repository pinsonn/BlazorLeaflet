namespace BlazorLeaflet.Models;

/// <summary>
///     Shapefile layer - Requires Leaflet.Shapefile plugin
/// </summary>
public record ShapefileLayer : Layer
{
    /// <summary>
    ///     Instantiates a tile layer object given a URL template.
    /// </summary>
    public string? UrlTemplate { get; set; }
}