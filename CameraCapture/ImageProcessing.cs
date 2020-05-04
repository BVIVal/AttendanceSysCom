using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;

namespace CameraCapture
{
    public class ImageProcessing
    {
        public VideoCapture Capture { get; private set; } 
        public bool CaptureInProgress { get; set; } 


        public Task CreateVideoCapture(int camNumber = 0)
        {
            Capture = new VideoCapture(camNumber);
            return Task.FromResult(Capture != null);
        }
        public Mat GetFrames()
        {
            return Capture.QueryFrame();

        }
    }
}