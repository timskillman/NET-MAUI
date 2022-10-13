# .NET MAUI Zoom Pan demo using SkiaSharp SKCanvas


Demonstrates a simple Zoom Pan class that zooms and pans the scene with just one-line zoom/pan function;

- **Windows** - Click/drag left mouse button to pan. **Mousewheel** to zoom.
- **Android/iOS** - Single touch/drag to pan. **Two-finger drag** to zoom and pan.  (option to turn off single drag pan)

Snapshot from Android phone (free SVG world map from simple maps) ...

![alt text](https://github.com/timskillman/NET-MAUI/blob/main/Zoom.Pan.Demo/ZoomPanDemo/Images/WorldMap.jpg "World map taken from simplemaps.com")

# Example code

Example 'MainPage.xaml'

```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:skia="clr-namespace:SkiaSharp.Views.Maui.Controls;assembly=SkiaSharp.Views.Maui.Controls"
             x:Class="ZoomPanDemo.MainPage">

    <Grid RowDefinitions="*" >
        <skia:SKCanvasView Grid.Row="0" x:Name="canvasView" 
                PaintSurface="OnPaintSurface"
                Touch="OnTouch" 
                EnableTouchEvents="True">
        </skia:SKCanvasView>
    </Grid>

</ContentPage>
```

Example code in 'MainPage.xaml.cs' using the ZoomPan.cs class (in example project)

```c#
using SkiaSharp;
using SkiaSharp.Views.Maui;

namespace ZoomPanDemo;

public partial class MainPage : ContentPage
{

    private ZoomPan zoomPan = new ZoomPan();
    private SKPath path = new SKPath();
    
    public MainPage()
    {
        InitializeComponent();
        string svg = "M 100,100, .... z" //Some SVG path data goes here
        path = SKPath.ParseSvgPathData(svg);
    }
    
    protected override void OnSizeAllocated(double width, double height)
    {
        //Zooms and pans the SKPath object to fit inside the full width of the canvas on startup
        base.OnSizeAllocated(width, height);
        zoomPan.ZoomToRect(new SKRect(0, 0, (float)canvasView.Width, (float)canvasView.Height), path.Bounds); //Zooms SKPath to canvasView (see MainPage.xaml)
    }

    private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        SKCanvas canvas = e.Surface.Canvas;
        canvas.SetMatrix(zoomPan.GetMatrix());   //Get the resultant matrix from ZoomPan class and set to Canvas
        SKPaint paint = new SKPaint { Color = SKColors.PaleVioletRed, Style = SKPaintStyle.Stroke, IsAntialias = true };
        canvas.Clear();
        canvas.DrawPath(path, paint);
    }

    private void OnTouch(object sender, SKTouchEventArgs e)
    {
        //one-line zoom-pan function ... e.handled is set if function is used ...
        if (zoomPan.DoZoomPan(e,SKMouseButton.Middle, 500f) {
            canvasView.InvalidateSurface();
        }
    }
}
```


# Dependencies

- SkiaSharp
- SkiaSharp.Views.Maui.Controls

