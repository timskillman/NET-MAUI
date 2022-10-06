# .NET MAUI Zoom Pan demo


Demonstrates a simple Zoom Pan class that zooms and pans the scene;

- Windows - Click/drag left mouse button to pan. Mousewheel to zoom.
- Android/iOS - Single touch/drag to pan. Two-finger drag to zoom and pan.  (option to turn off single drag pan)

Mouse and Touch screen supported. 

![alt text](https://github.com/timskillman/NET-MAUI/blob/main/Zoom.Pan.Demo/ZoomPanDemo/Images/Screenshot.jpg "Screenshot")

# Example code

```
protected override void OnSizeAllocated(double width, double height)
    {
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
        switch (e.DeviceType)
        {
            case SKTouchDeviceType.Touch:
                System.Diagnostics.Debug.WriteLine(e.ActionType);
                switch (e.ActionType)
                {
                    case SKTouchAction.Pressed:
                        zoomPan.TouchPressed(e);
                        break;
                    case SKTouchAction.Moved:
                        zoomPan.TouchZoomDrag(e, singlePointDrag: true);
                        break;
                    case SKTouchAction.Released:
                        zoomPan.TouchReset(e);
                        break;
                }
                break;
            case SKTouchDeviceType.Mouse:
                switch (e.ActionType)
                {
                    case SKTouchAction.Moved:
                        zoomPan.MousePan(e.Location, e.MouseButton == SKMouseButton.Left);
                        e.Handled = true;
                        break;

                    case SKTouchAction.WheelChanged:
                        zoomPan.MouseZoom(e.Location, e.WheelDelta / 500f);
                        e.Handled = true;
                        break;
                }
                break;
        }
        if (e.Handled) canvasView.InvalidateSurface();
    }
```
