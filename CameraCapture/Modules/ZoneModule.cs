using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace CameraCapture.Modules
{
    public class ZoneModule
    {
        public event EventHandler<PaintEventArgs> PaintShapeEnter;
        public event EventHandler<PaintEventArgs> PaintShapeExit;

        public bool IsEnterPointsNumMax { get; private set; }
        public bool IsExitPointsNumMax { get; private set; }
        public int MaxPointsNum { get; }
        private List<Point> ShapeEnter { get; }
        private List<Point> ShapeExit { get; }

        public ZoneModule(int maxPointsNum = 4)
        {
            ShapeEnter = new List<Point>();
            ShapeExit = new List<Point>();
            MaxPointsNum = maxPointsNum;

            ClearShapeEnter();
            ClearShapeExit();
        }

        public void AddEnterPoint(Point point)
        {
            if (ShapeEnter.Count == MaxPointsNum) ClearShapeEnter();
            ShapeEnter.Add(point);
            if (ShapeEnter.Count == MaxPointsNum) IsEnterPointsNumMax = true;
        }

        public void AddExitPoint(Point point)
        {
            if (ShapeExit.Count == MaxPointsNum) ClearShapeExit();
            ShapeExit.Add(point);
            if (ShapeExit.Count == MaxPointsNum) IsExitPointsNumMax = true;
        }

        public void ClearShapeEnter()
        {
            IsEnterPointsNumMax = false;
            ShapeEnter.Clear();
        }

        public void ClearShapeExit()
        {
            IsExitPointsNumMax = false;
            ShapeExit.Clear();
        }

        public void OnPaintShapeEnter(Emgu.CV.UI.ImageBox sender)
        {
            if (!IsEnterPointsNumMax) return;
            var graphics = sender.CreateGraphics();
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.FillPolygon(new SolidBrush(Color2Transparent(Color.DarkGreen)), ShapeEnter.ToArray());
        }

        private void OnPaintShapeEnter(object sender, PaintEventArgs e)
        {
            if (!IsExitPointsNumMax) return;
            var graphics = e.Graphics;
            graphics.FillPolygon(new SolidBrush(Color2Transparent(Color.DarkGreen)),ShapeEnter.ToArray());
        }

        public void OnPaintShapeExit(Emgu.CV.UI.ImageBox sender)
        {
            if (!IsExitPointsNumMax) return;
            var graphics = sender.CreateGraphics();
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.FillPolygon(new SolidBrush(Color2Transparent(Color.DarkRed)), ShapeExit.ToArray());
        }

        private void OnPaintShapeExit(object sender, PaintEventArgs e)
        {
            var graphics = e.Graphics;
            graphics.FillPolygon(new SolidBrush(Color2Transparent(Color.DarkRed)), ShapeExit.ToArray());
        }

        private static Color Color2Transparent(Color c)
        {
            return Color.FromArgb(128, c.R, c.G, c.B);
        }
    }
}
