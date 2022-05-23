using System.Drawing;

namespace BlazorLeaflet.Models;

/// <summary>
///     A class for drawing rectangle overlays on a map. Extends Polygon.
///     <example>
///         Example:
///         <code>
/// [[54.559322, -5.767822], [56.1210604, -3.021240]]
/// </code>
///     </example>
/// </summary>
public record Rectangle : Polyline<RectangleF>;