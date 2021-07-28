using DLS.Model.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLS.Model.Models
{
    public interface ILabelingData
    {
        double ID { get; }
        string Name { get; }
        LabelingType LabelingType { get; }

        bool IsContains(PointF pos);
        PointF GetCenterPoint();
        Rectangle GetBoundingRect();
    }

    public abstract class LabelingData : ILabelingData
    {
        public double ID { get; private set; }
        public string Name { get; private set; }
        public LabelingType LabelingType { get; private set; }

        public abstract bool IsContains(PointF pos);
        public abstract PointF GetCenterPoint();
        public abstract Rectangle GetBoundingRect();

        public LabelingData(LabelingType labelingType, Class @class)
        {
            ID = @class.ID;
            Name = @class.Name;
            LabelingType = labelingType;
        }
    }

    public class RectLabelingData : LabelingData
    {
        public RectangleF Rect { get; private set; }

        public RectLabelingData(Class @class, RectangleF rect) : base(LabelingType.Rect, @class)
        {
            Rect = rect;
        }

        public override bool IsContains(PointF pos)
        {
            return Rect.Contains(pos);
        }

        public override PointF GetCenterPoint()
        {
            return new PointF(Rect.X + Rect.Width / 2, Rect.Y + Rect.Height / 2);
        }

        public void Update(float x, float y, float width, float height)
        {
            Rect = new RectangleF(x, y, width, height);
        }

        public override Rectangle GetBoundingRect()
        {
            return Rectangle.Ceiling(Rect);
        }
    }

    public class PointLabelingData : LabelingData
    {
        public List<PointF> Points { get; private set; }

        public PointLabelingData(Class @class, IEnumerable<PointF> points) : base(LabelingType.Polygon, @class)
        {
            Points = new List<PointF>(points);
        }

        public override bool IsContains(PointF pos)
        {
            if (Points.Count <= 0)
                return false;

            double minX = Points[0].X;
            double maxX = Points[0].X;
            double minY = Points[0].Y;
            double maxY = Points[0].Y;
            for (int i = 1; i < Points.Count; i++)
            {
                var q = Points[i];
                minX = Math.Min(q.X, minX);
                maxX = Math.Max(q.X, maxX);
                minY = Math.Min(q.Y, minY);
                maxY = Math.Max(q.Y, maxY);
            }

            if (pos.X < minX || pos.X > maxX || pos.Y < minY || pos.Y > maxY)
                return false;
        
            bool inside = false;
            for (int i = 0, j = Points.Count - 1; i < Points.Count; j = i++)
            {
                if ((Points[i].Y > pos.Y) != (Points[j].Y > pos.Y) && 
                    pos.X < (Points[j].X - Points[i].X) * (pos.Y - Points[i].Y) / (Points[j].Y - Points[i].Y) + Points[i].X)
                {
                    inside = !inside;
                }
            }

            return inside;
        }

        public override PointF GetCenterPoint()
        {
            if (Points.Count <= 0)
                return PointF.Empty;

            var minX = Points.Min(p => p.X);
            var minY = Points.Min(p => p.Y);
            var maxX = Points.Max(p => p.X);
            var maxY = Points.Max(p => p.Y);

            return new PointF(minX + (maxX - minX) / 2, minY + (maxY - minY) / 2);
        }

        public override Rectangle GetBoundingRect()
        {
            throw new NotImplementedException();
        }
    }
}
