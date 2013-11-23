using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using s = SharpDX;
using sd = SharpDX.Direct2D1;
using sw = SharpDX.DirectWrite;
using Eto.Drawing;

namespace Eto.Platform.Direct2D.Drawing
{
    public class BrushHandler : IBrush
    {
		sd.Brush Control { get; set; }

        public void Create(PointF point1, PointF point2, Color color1, Color color2)
        {
            throw new NotImplementedException();
        }

        public void Create(Color color)
        {
			Control = CreateBrush(ref color);
        }

		public static sd.SolidColorBrush CreateBrush(ref Color color)
		{
			return new sd.SolidColorBrush(
				renderTarget: GraphicsHandler.CurrentRenderTarget, // BUGBUG: TODO
				color: color.ToSD());
		}

        public void Create(Bitmap b)
        {
            throw new NotImplementedException();
        }

        public void Create(RectangleF rectangle, Color c1, Color c2, float angle)
        {
            throw new NotImplementedException();
        }

        public void TranslateTransform(float x, float y)
        {
            Control.Transform =
                s.Matrix3x2.Multiply(
                    s.Matrix3x2.Translation(x, y),
                    Control.Transform);
        }
    }
}
