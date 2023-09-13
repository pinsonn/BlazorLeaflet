using System.Drawing;
using BlazorLeaflet.Models.Events;
using Microsoft.JSInterop;
using Rectangle = BlazorLeaflet.Models.Rectangle;

namespace BlazorLeaflet.Demo.Data;

public class DrawHandler : IDisposable
{
    private readonly Circle _circle = new();
    private readonly IJSRuntime _jsRuntime;

    private readonly Map _map;
    private readonly List<MouseEvent> _mouseClickEvents = new();
    private readonly Polygon _polygon = new();
    private readonly Rectangle _rectangle = new();
    private DrawState _drawState;

    public DrawHandler(Map map, IJSRuntime jsRuntime)
    {
        _map = map;
        _jsRuntime = jsRuntime;
        _rectangle.StrokeColor = Color.Teal;
        _rectangle.StrokeWidth = 1;
        _rectangle.Fill = true;
        _rectangle.FillColor = Color.Orange;

        _circle.StrokeColor = Color.DarkSlateBlue;
        _circle.StrokeWidth = 1;
        _circle.Fill = true;
        _circle.FillColor = Color.Navy;

        _polygon.StrokeColor = Color.Black;
        _polygon.StrokeWidth = 1;
        _polygon.Fill = true;
        _polygon.FillColor = Color.Red;
    }

    public void Dispose() => UnsubscribeFromMapEvents();

    public event EventHandler? DrawFinished;

    public void OnDrawCircleToggle(bool isToggled)
    {
        _map.RemoveLayer(_circle);
        _drawState = DrawState.DrawingCircle;
        OnDrawToggle(isToggled);
    }

    public void OnDrawRectangleToggle(bool isToggled)
    {
        _map.RemoveLayer(_rectangle);
        _drawState = DrawState.DrawingRectangle;
        OnDrawToggle(isToggled);
    }

    public void OnDrawPolygonToggle(bool isToggled)
    {
        _map.RemoveLayer(_polygon);
        _polygon.Shape = null;
        _drawState = DrawState.DrawingPolygon;
        OnDrawToggle(isToggled);
    }

    private void OnDrawToggle(bool isToggled)
    {
        _mouseClickEvents.Clear();
        if (isToggled)
        {
            _map.OnClick += OnMapClick;
            _map.OnMouseMove += OnMouseMove;
        }
        else
        {
            UnsubscribeFromMapEvents();
        }
    }

    private void OnMapClick(object sender, MouseEvent e)
    {
        if (_drawState != DrawState.DrawingPolygon)
        {
            AddClickAndUpdateShape(e);
            if (_mouseClickEvents.Count == 2)
                // untoggle button
                DrawComplete();
        }
        else
        {
            // finish a line
            if (_polygon.Shape?[0].Length > 2 &&
                Math.Abs(_mouseClickEvents[0].ContainerPoint.X - e.ContainerPoint.X) < 10 &&
                Math.Abs(_mouseClickEvents[0].ContainerPoint.Y - e.ContainerPoint.Y) < 10)
            {
                // update the polygon without the last point (mouse move point)
                // and we're finished
                UpdatePolygon(null);
                DrawComplete();
            }
            else
            {
                AddClickAndUpdateShape(e);
            }
        }
    }

    private void OnMouseMove(object sender, MouseEvent e)
    {
        if (_mouseClickEvents.Any())
            UpdateShape(e.LatLng);
    }

    private void AddClickAndUpdateShape(MouseEvent e)
    {
        _mouseClickEvents.Add(e);
        UpdateShape(e.LatLng);
    }

    private void UpdateShape(LatLng latLng)
    {
        switch (_drawState)
        {
            case DrawState.DrawingRectangle:
                UpdateRectangle(latLng);
                break;
            case DrawState.DrawingCircle:
                UpdateCircle(latLng);
                break;
            case DrawState.DrawingPolygon:
                UpdatePolygon(latLng);
                break;
            case DrawState.None:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(latLng));
        }
    }

    private void UpdateRectangle(LatLng latLng)
    {
        _rectangle.Shape = new RectangleF(
            _mouseClickEvents[0].LatLng.Lng,
            _mouseClickEvents[0].LatLng.Lat,
            latLng.Lng - _mouseClickEvents[0].LatLng.Lng,
            latLng.Lat - _mouseClickEvents[0].LatLng.Lat
        );
        AddOrUpdateShape(_rectangle);
    }

    private void UpdateCircle(LatLng latLng)
    {
        _circle.Position = _mouseClickEvents[0].LatLng;
        // get a rough approximate for now: have to convert to meters - there should be better more precise algorithms out there
        _circle.Radius = Math.Max(Math.Abs(latLng.Lng - _mouseClickEvents[0].LatLng.Lng),
            Math.Abs(latLng.Lat - _mouseClickEvents[0].LatLng.Lat)) * 111320;
        AddOrUpdateShape(_circle);
    }

    private void UpdatePolygon(LatLng? latLng)
    {
        // copy over previous points, add a new one if LatLng defined
        var size = _mouseClickEvents.Count;
        var shape = new PointF[1][];
        shape[0] = new PointF[latLng == null ? size : size + 1];
        for (var i = 0; i < size; i++)
            shape[0][i] = _mouseClickEvents[i].LatLng.ToPointF();
        if (latLng != null)
            shape[0][size] = latLng.ToPointF();
        _polygon.Shape = shape;
        AddOrUpdateShape(_polygon);
    }

    private void AddOrUpdateShape(Layer shape)
    {
        if (_map.GetLayers().Contains(shape))
            LeafletInterops.UpdateShape(_jsRuntime, _map.Id, shape);
        else
            _map.AddLayer(shape);
    }

    private void DrawComplete()
    {
        UnsubscribeFromMapEvents();
        _drawState = DrawState.None;
        DrawFinished?.Invoke(this, EventArgs.Empty);
    }

    private void UnsubscribeFromMapEvents()
    {
        _map.OnClick -= OnMapClick;
        _map.OnMouseMove -= OnMouseMove;
    }

    private enum DrawState
    {
        None,
        DrawingRectangle,
        DrawingCircle,
        DrawingPolygon
    }
}