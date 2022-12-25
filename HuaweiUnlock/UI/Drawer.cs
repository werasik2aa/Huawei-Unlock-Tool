using System.Drawing;
using System.Drawing.Drawing2D;

namespace HuaweiUnlocker.UI
{
    public static class Drawer
    {
        public static GraphicsPath RoundedRectangle(Rectangle rect, float RoundSize)
        {
            GraphicsPath gp = new GraphicsPath();

            gp.AddArc(rect.X, rect.Y, RoundSize, RoundSize, 180, 90);
            gp.AddArc(rect.X + rect.Width - RoundSize, rect.Y, RoundSize, RoundSize, 270, 90);
            gp.AddArc(rect.X + rect.Width - RoundSize, rect.Y + rect.Height - RoundSize, RoundSize, RoundSize, 0, 90);
            gp.AddArc(rect.X, rect.Y + rect.Height - RoundSize, RoundSize, RoundSize, 90, 90);

            gp.CloseFigure();

            return gp;
        }
        
        public static void DrawBlurredLine(Graphics graph, Color lineColor, Point p1, Point p2, int maxAlpha, int penWidth)
        {
            float stepAlpha = (float)maxAlpha / penWidth;

            float actualAlpha = stepAlpha;
            for (int pWidth = penWidth; pWidth > 0; pWidth--)
            {
                Color BlurredColor = Color.FromArgb((int)actualAlpha, lineColor);
                Pen BlurredPen = new Pen(BlurredColor, pWidth);
                BlurredPen.StartCap = LineCap.Round;
                BlurredPen.EndCap = LineCap.Round;
                
                graph.DrawLine(BlurredPen, p1, p2);

                actualAlpha += stepAlpha;
            }
        }

        public static void DrawBlurredRectangle(Graphics graph, Color lineColor, Rectangle rect, int maxAlpha, int penWidth)
        {
            float stepAlpha = (float)maxAlpha / penWidth;

            float actualAlpha = stepAlpha;
            for (int pWidth = penWidth; pWidth > 0; pWidth--)
            {
                Color BlurredColor = Color.FromArgb((int)actualAlpha, lineColor);
                Pen BlurredPen = new Pen(BlurredColor, pWidth);
                BlurredPen.StartCap = LineCap.Round;
                BlurredPen.EndCap = LineCap.Round;
                
                graph.DrawRectangle(BlurredPen, rect);

                actualAlpha += stepAlpha;
            }
        }
    }
}
