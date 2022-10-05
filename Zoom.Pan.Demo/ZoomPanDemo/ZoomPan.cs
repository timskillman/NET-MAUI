using SkiaSharp;


namespace ZoomPanDemo
{
    public class ZoomPan
    {

        public ZoomPan(float scale = 1f, float offsetX = 0, float offsetY = 0) { Scale = scale; pan = new SKPoint(offsetX, offsetY); }

        public float Scale { get; set; }

        public SKPoint Pan { get => pan; set => pan = value; }

        public SKMatrix GetMatrix()
        {
            return SKMatrix.CreateScaleTranslation(Scale, Scale, origin.X - pan.X * Scale, origin.Y - pan.Y * Scale);
        }

        // NOTE! Pan must always be called in the mouse/touch Move event (even if pan is not used)
        public void MovePan(SKPoint point, bool doPan)
        {
            SKPoint prevTouch = touchPoint;
            touchPoint = ZoomInv(point);
            if (doPan) pan -= touchPoint - prevTouch;
            touchPoint = ZoomInv(point);
        }

        public void Zoom(SKPoint point, float delta)
        {
            Scale += delta;
            origin = point;
            pan = touchPoint;
            touchPoint = ZoomInv(point);
        }

        public void ZoomToRect(SKRect viewRect, SKRect zoomRect)
        {
            Scale = viewRect.Width / zoomRect.Width;
            SKPoint centerPoint = new SKPoint(-zoomRect.Left * Scale, -zoomRect.Top * Scale);
            origin = centerPoint;
            pan = ZoomInv(centerPoint);
        }

        private SKPoint origin = new SKPoint(0, 0);
        private SKPoint pan = new SKPoint(0, 0);
        private SKPoint touchPoint = new SKPoint(0, 0);

        private SKPoint ZoomInv(SKPoint point)
        {
            return new SKPoint((point.X - origin.X) / Scale, (point.Y - origin.Y) / Scale) + pan;
        }
    }
}
