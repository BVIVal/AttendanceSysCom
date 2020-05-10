﻿using System.Drawing;
using System.Threading.Tasks;
using CameraCapture.Algorithms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace CameraCapture
{
    public class ImageProcessing
    {
        public VideoCapture Capture { get; private set; } 
        public bool CaptureInProgress { get; set; }
        public int NumberOfFaces { get; set; }
        public Image<Bgr, byte> ResultFrame { get; set; }

        public ImageProcessing()
        {
            NumberOfFaces = -1;
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

        #region Algorithms

        public bool TryCascadeRecognition(Mat originalFrames, CascadeClassifier cascadeClassifier, Settings settings, Color color)
        {
            try
            {
                //var testOriginalFrames = GetFrames();
                ResultFrame = originalFrames.ToImage<Bgr, byte>();
                var faces = cascadeClassifier.DetectMultiScale(
                    originalFrames.ToImage<Gray, byte>(),
                    settings.ScaleRate, settings.MinNeighbors,
                    new Size(settings.MinWindowSize, settings.MinWindowSize)
                );

                NumberOfFaces = faces.Length;

                foreach (var face in faces)
                {
                    ResultFrame.Draw(face, new Bgr(color), 4, LineType.FourConnected);
                }
            }
            catch 
            {
                //ignore
                return false;
            }
            return true;
        }

        public bool TryHogDescriptor(Mat originalFrames, HOGDescriptor hogDescriptor)
        {
            try
            {
                hogDescriptor.SetSVMDetector(HOGDescriptor.GetDefaultPeopleDetector());
                var regions = hogDescriptor.DetectMultiScale(originalFrames);
                foreach (var pedestrain in regions)
                {
                    ResultFrame.Draw(pedestrain.Rect, new Bgr(Color.Red), 1);
                }
            }
            catch
            {
                //ignore
                return false;
            }
            return true;
        }
        #endregion
    }
}