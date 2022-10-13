using SkiaSharp;
using SkiaSharp.Views.Maui;

namespace ZoomPanDemo
{
    public class ZoomPan
    {
        public ZoomPan(float scale = 1f, float offsetX = 0, float offsetY = 0) { Scale = scale; centerPix = new SKPoint(offsetX, offsetY); }

        public float Scale { get; set; }

        public SKPoint Pan { get => centerPix; set => centerPix = value; }

        public SKMatrix GetMatrix()
        {
            return SKMatrix.CreateScaleTranslation(Scale, Scale, centerVec.X - centerPix.X * Scale, centerVec.Y - centerPix.Y * Scale);
        }

        // NOTE! MousePan must always be called in a mouse/touch Move event
        // Otherwise canvas will appear to 'jump' from one place to another.
        // This is because 'delta' should be a relative move and not absolute.
        public void MousePan(SKPoint delta, bool doPan)
        {
            if (doPan) centerPix -= VecToPix(delta) - vecPoint;
            vecPoint = VecToPix(delta);
        }
        public void MouseZoom(SKPoint point, float delta)
        {
            Scale += delta;
            centerVec = point;
            centerPix = vecPoint;
            vecPoint = VecToPix(point);
        }
        public void ZoomToRect(SKRect viewRect, SKRect zoomRect)
        {
            Scale = viewRect.Width / zoomRect.Width;
            SKPoint centerPoint = new SKPoint(-zoomRect.Left * Scale, -zoomRect.Top * Scale);
            centerVec = centerPoint;
            vecPoint = VecToPix(centerPoint);
            centerPix = vecPoint;
        }

        public void TouchPressed(SKTouchEventArgs e)
        {
            if (!fingerStrokes.ContainsKey(e.Id)) fingerStrokes.Add(e.Id, e.Location);
            vecPoint = VecToPix(e.Location);
            e.Handled = true;
        }
        public void TouchZoomDrag(SKTouchEventArgs e, bool singlePointDrag = true)
        {
            if (!fingerStrokes.ContainsKey(e.Id)) return;

            fingerStrokes[e.Id] = e.Location;

            switch (fingerStrokes.Count)
            {
                case 1:
                    MousePan(e.Location, singlePointDrag);
                    break;
                case 2:
                    if (e.Id != firstFingerID && firstFingerID != -1)
                    {
                        SKPoint newFinger1 = fingerStrokes[firstFingerID];
                        SKPoint newFinger2 = fingerStrokes[e.Id];
                        SKPoint midpoint1 = new SKPoint((newFinger1.X + newFinger2.X) / 2, (newFinger1.Y + newFinger2.Y) / 2);  //mid point between each touch
                        SKPoint midpoint2 = (newpress) ? midpoint1 : new SKPoint((oldFinger1.X + oldFinger2.X) / 2, (oldFinger1.Y + oldFinger2.Y) / 2);
                        SKPoint centreTouch = new SKPoint((midpoint1.X + midpoint2.X) / 2, (midpoint1.Y + midpoint2.Y) / 2);

                        if (newpress)
                        {
                            vecPoint = VecToPix(centreTouch);
                            newpress = false;
                        }
                        else
                        {
                            //DRAG, PINCH ZOOM ...
                            float dist1 = (fingerStrokes[firstFingerID] - fingerStrokes[e.Id]).Length;  //Vector distance moved on finger 1
                            float dist2 = (oldFinger2 - oldFinger1).Length;                            //Vector distance moved on finger 2
                            float scaleDelta = dist2 / dist1;
                            if (!float.IsNaN(scaleDelta))
                            {
                                scaleDelta = (1f - scaleDelta) * Scale;
                                MouseZoom(centreTouch, scaleDelta);
                            }
                        }

                        oldFinger1 = fingerStrokes[firstFingerID];
                        oldFinger2 = fingerStrokes[e.Id];

                    }
                    else
                    {
                        firstFingerID = e.Id;
                    }
                    break;
            }
            e.Handled = true;
        }

        public void TouchReset(SKTouchEventArgs e)
        {
            vecPoint = VecToPix(e.Location);
            fingerStrokes.Clear();
            newpress = true;
            firstFingerID = -1;
            e.Handled = true;
        }

        public bool DoZoomPan(SKTouchEventArgs e, SKMouseButton button, float zoomFactor, bool singlePointDrag = true)
        {
            switch (e.DeviceType)
            {
                case SKTouchDeviceType.Touch:
                    System.Diagnostics.Debug.WriteLine(e.ActionType);
                    switch (e.ActionType)
                    {
                        case SKTouchAction.Pressed:
                            TouchPressed(e);
                            break;
                        case SKTouchAction.Moved:
                            TouchZoomDrag(e, singlePointDrag);
                            break;
                        case SKTouchAction.Released:
                            TouchReset(e);
                            break;
                    }
                    break;
                case SKTouchDeviceType.Mouse:
                    switch (e.ActionType)
                    {
                        case SKTouchAction.Moved:
                            MousePan(e.Location, e.MouseButton == button);
                            e.Handled = true;
                            break;

                        case SKTouchAction.WheelChanged:
                            MouseZoom(e.Location, e.WheelDelta / zoomFactor);
                            e.Handled = true;
                            break;
                    }
                    break;
            }
            return e.Handled;
        }

        private SKPoint VecToPix(SKPoint point)
        {
            return new SKPoint((point.X - centerVec.X) / Scale, (point.Y - centerVec.Y) / Scale) + centerPix;
        }


        private SKPoint centerVec = new SKPoint(0, 0);
        private SKPoint centerPix = new SKPoint(0, 0);
        private SKPoint vecPoint = new SKPoint(0, 0);

        private Dictionary<long, SKPoint> fingerStrokes = new Dictionary<long, SKPoint>();
        private long firstFingerID = -1;
        SKPoint oldFinger1 = new();
        SKPoint oldFinger2 = new();
        private bool newpress = false;
    }
}