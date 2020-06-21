using CameraCapture.Modules;
using Emgu.CV;
using Emgu.CV.Structure;

namespace CameraCapture.Common
{
    public class DetectorResult
    {
        public Image<Bgr, byte> Image;
        public ZoneModuleEnum Zone;
        public string ImageProcessingId;
    }
}