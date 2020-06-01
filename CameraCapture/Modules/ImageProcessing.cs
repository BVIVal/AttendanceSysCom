using System;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;

namespace CameraCapture.Modules
{
    public class ImageProcessing
    {
        /// <summary>
        /// Resolution X.
        /// </summary>
        private int resolutionX;

        /// <summary>
        /// Resolution Y.
        /// </summary>
        private int resolutionY;
        public VideoCapture Capture { get; private set; } 
        public bool CaptureInProgress { get; set; }
        public int NumberOfFaces { get; set; }
        public Image<Bgr, byte> ResultFrame { get; set; }

        public Image<Bgr, byte> OriginalFrame;

        public ImageProcessing(int resolutionX, int resolutionY)
        {
            if (resolutionX <= 0) throw new ArgumentOutOfRangeException(nameof(resolutionX));
            if (resolutionY <= 0) throw new ArgumentOutOfRangeException(nameof(resolutionY));

            NumberOfFaces = -1;
            this.resolutionX = resolutionX;
            this.resolutionY = resolutionY;
            OriginalFrame = new Image<Bgr, byte>(this.resolutionX, this.resolutionY);
        }

        public Task CreateVideoCapture(int camNumber = 0)
        {
            Capture = new VideoCapture(camNumber);
            return Task.FromResult(Capture != null);
        }
        public Mat GetFrames()
        {
            return Capture.QueryFrame();
        }

        public Image<Bgr,byte> GetRetrieveImage()
        {
            Capture.Retrieve(OriginalFrame);
            return OriginalFrame;
        }

    }
}