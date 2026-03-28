using System.Drawing;
using WinFormsApp2.Models;

namespace WinFormsApp2.UI
{
    public class AnimatedMessage
    {
        public Mensaje Mensaje { get; set; } = new Mensaje();
        public float Progress { get; set; } = 0f;
        public PointF Start { get; set; }
        public PointF End { get; set; }

        public PointF GetCurrentPosition()
        {
            float x = Start.X + (End.X - Start.X) * Progress;
            float y = Start.Y + (End.Y - Start.Y) * Progress;
            return new PointF(x, y);
        }
    }
}