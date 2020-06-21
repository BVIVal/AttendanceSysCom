using System;
using System.Collections.Generic;
using System.Drawing;

namespace GraphicLibrary
{
    public class IntersectionUtilities
    {
        public const int Outside = -1;
        public const int Inside = 1;
        public const int OnBorder = 0;

        public static bool IsInZone(IReadOnlyList<Point> checkedZone, IReadOnlyList<Point> faceZone)
        {
            for (var i = 0; i < checkedZone.Count; i++)
            {
                var j = i + 1;
                if (i == checkedZone.Count - 1) j = 0;

                var seg1Start = checkedZone[i];
                var seg1End = checkedZone[j];

                for (var i2 = 0; i2 < faceZone.Count; i2++)
                {
                    var j2 = i2 + 1;
                    if (i2 == faceZone.Count - 1) j2 = 0;

                    var seg2Start = faceZone[i2];
                    var seg2End = faceZone[j2];

                    if (SegmentIntersectionPoint(seg1Start, seg1End, seg2Start, seg2End))
                        return true;
                }
            }
            return false;
        }

        public static bool SegmentIntersectionPoint(Point start1, Point end1, Point start2, Point end2)
        {
            Point dir1 = new Point(end1.X - start1.X, end1.Y - start1.Y);
            Point dir2 = new Point(end2.X - start2.X, end2.Y - start2.Y);

            //считаем уравнения прямых, проходящих через отрезки
            double a1 = -dir1.Y;
            double b1 = +dir1.X;
            double d1 = -(a1 * start1.X + b1 * start1.Y);

            double a2 = -dir2.Y;
            double b2 = +dir2.X;
            double d2 = -(a2 * start2.X + b2 * start2.Y);

            //подставляем концы отрезков для выяснения в каких полуплоскоcтях они
            //подставляем координаты концов  первого отрезка в уравнение прямой второго отрезка 
            //либо начало, либо конец должен принадлежать второй линии
            double seg1_line2_start = a2 * start1.X + b2 * start1.Y + d2;
            double seg1_line2_end = a2 * end1.X + b2 * end1.Y + d2;

            double seg2_line1_start = a1 * start2.X + b1 * start2.Y + d1;
            double seg2_line1_end = a1 * end2.X + b1 * end2.Y + d1;

            //если концы одного отрезка имеют один знак, значит он в одной полуплоскости и пересечения нет.
            if (seg1_line2_start * seg1_line2_end >= 0 || seg2_line1_start * seg2_line1_end >= 0)
                return false;

            double u = seg1_line2_start / (seg1_line2_start - seg1_line2_end);

            return true;
        }
        
    }
}