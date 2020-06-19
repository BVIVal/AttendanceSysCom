using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace CameraCapture.Modules
{
    public interface IZoneModule
    {
        ZoneModule AreaModule { get; set; }
    }
}