using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GraphicLibrary.Tests
{
    [TestClass]
    public class IntersectionTests
    {
        [TestMethod]
        public void IntersectionEasyTest()
        {
            IntersectionBaseTest(new List<Point>{new Point(345, 156),new Point(449, 156),new Point(345, 27),new Point(449, 285)},
                new List<Point>{new Point(457, 69),new Point(603,64),new Point(605, 409),new Point(441, 418)}, 
                true);
            IntersectionBaseTest(new List<Point> { new Point(463,141), new Point(605,141), new Point(601,260), new Point(477, 247) },
                new List<Point> { new Point(462,203), new Point(577, 203), new Point(462, 49), new Point(577,357) },
                true);
        }

        [TestMethod]
        public void IntersectionTrueMissTest()
        {
            IntersectionBaseTest(new List<Point> { new Point(475, 164), new Point(595, 160), new Point(600, 215), new Point(499, 223) },
                new List<Point> { new Point(391, 129), new Point(509, 129),  new Point(509, 267), new Point(391, 267) },
                true);
            IntersectionBaseTest(new List<Point> { new Point(475, 164), new Point(595, 160), new Point(600, 215), new Point(499, 223) },
                new List<Point> { new Point(391, 129), new Point(509, 129), new Point(391, 267), new Point(509, 267) },
                true);
        }   

        public void IntersectionBaseTest(List<Point> listZone, List<Point> listFace, bool expected)
        {
            Assert.AreEqual(expected, IntersectionUtilities.IsInZone(listZone, listFace));
        }
    }
}
