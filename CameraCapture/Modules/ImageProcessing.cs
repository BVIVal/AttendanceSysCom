using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;

namespace CameraCapture.Modules
{
    public class ImageProcessing : IDisposable
    {
        #region Fields
        private int resolutionX;
        private int resolutionY;
        public Image<Bgr, byte> OriginalFrame;

        #endregion

        #region Properties
        public VideoCapture Capture { get; private set; }
        public bool CaptureInProgress { get; set; }

        #endregion

        #region Id
        public string ImageProcessingId;

        #endregion

        public ImageProcessing(int resolutionX, int resolutionY)
        {
            if (resolutionX <= 0) throw new ArgumentOutOfRangeException(nameof(resolutionX));
            if (resolutionY <= 0) throw new ArgumentOutOfRangeException(nameof(resolutionY));

            this.resolutionX = resolutionX;
            this.resolutionY = resolutionY;
            OriginalFrame = new Image<Bgr, byte>(this.resolutionX, this.resolutionY);
            ImageProcessingId = GenerateId();
        }

        private static string GenerateId() => $"{DateTime.UtcNow.ToLongTimeString()}_{Guid.NewGuid()}";

        public Task CreateVideoCapture(int camNumber = 0)
        {
            //ToDo: add 'setCaptureProperty' like: fps, height, width
            Capture = new VideoCapture(camNumber);
            return Task.FromResult(Capture != null);
        }
        public Mat GetFrames()
        {
            return Capture.QueryFrame();
        }

        //ToDo: use smooth filter
        public Image<Bgr,byte> GetRetrieveImage()
        {
            Capture.Retrieve(OriginalFrame);
            return OriginalFrame;
        }

        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public void Pause()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}