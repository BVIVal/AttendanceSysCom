using System;
using System.Collections.Generic;
using CameraCapture.Modules;
using Emgu.CV;
using Emgu.CV.Structure;

namespace CameraCapture.Common
{
    public class DetectorResult : Person, ICloneable, IEqualityComparer<DetectorResult>
    {
        public Image<Bgr, byte> Image { get; set; }
        public ZoneModuleEnum Zone { get; set; }
        public string ImageProcessingId { get; set; }

        public object Clone()
        {
            return new DetectorResult {Zone = Zone, ImageProcessingId = ImageProcessingId, Time = Time, Name = Name};
        }

        public bool Equals(DetectorResult x, DetectorResult y)
            => x?.Name == y?.Name && x?.Zone == y?.Zone && x?.ImageProcessingId == y?.ImageProcessingId;

        //public bool Equals(DetectorResult x, DetectorResult y)
        //    => y != null && (x != null && GetHashCode(x) == GetHashCode(y));

        public int GetHashCode(DetectorResult obj)
        {
            if (Time != null)
                return Time.Value.GetHashCode() * 3 + Name.GetHashCode() + ImageProcessingId.GetHashCode();
            return 0;
        }
    }
}