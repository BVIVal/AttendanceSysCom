using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using CameraCapture.Algorithms;
using CameraCapture.Utilities;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Dnn;
using Emgu.CV.Structure;

namespace CameraCapture.Modules
{
    public class DetectionModule
    {
        #region Fields  

        /// <summary>
        /// Resolution X.
        /// </summary>
        private int resolutionX;

        /// <summary>
        /// Resolution Y.
        /// </summary>
        private int resolutionY;

        #endregion

        #region AI section:

        /// <summary>
        /// Image size for detection.
        /// </summary>
        private int detectionSize = 300;

        /// <summary>
        /// resolutionX/detectionSize.
        /// </summary>
        private float xRate = 1.0f;

        /// <summary>
        /// resolutionY/detectionSize.
        /// </summary>
        private float yRate = 1.0f;

        private readonly Net detector;
        private readonly double minConfidence;

        #endregion

        public DetectionModule(int resolutionX, int resolutionY, double minConfidence)
        {
            if (resolutionX <= 0) throw new ArgumentOutOfRangeException(nameof(resolutionX));
            if (resolutionY <= 0) throw new ArgumentOutOfRangeException(nameof(resolutionY));

            this.resolutionX = resolutionX;
            this.resolutionY = resolutionY;

            xRate = resolutionX / (float) detectionSize;
            yRate = resolutionY / (float) detectionSize;

            detector = GetDetectorDnn();
            this.minConfidence = minConfidence;
        }

        private static Net GetDetectorDnn()
        {
            var proto = ResourcesUtilities.GetResourceBytes("deploy.prototxt");
            var model = ResourcesUtilities.GetResourceBytes("res10_300x300_ssd_iter_140000.caffemodel");
            return DnnInvoke.ReadNetFromCaffe(proto, model);
        }

        public Image<Bgr, byte> GetDetectedFacesDnn(Image<Bgr, byte> originalImage)
        {
            if (!IsResolutionCorrect(originalImage))
                throw new ArgumentException($"Not equal resolution exception: {nameof(originalImage)}");
            var resultImage = originalImage.Clone();

            var blobs = DnnInvoke.BlobFromImage(resultImage, 1.0, new Size(detectionSize, detectionSize));
            detector.SetInput(blobs);
            var detectedRectangles = GetDetectedRectangles(detector.Forward());

            foreach (var detectedRectangle in detectedRectangles)
            {
                resultImage.Draw(detectedRectangle, new Bgr(Color.GreenYellow));
            }

            return resultImage;
        }

        private IEnumerable<Rectangle> GetDetectedRectangles(Mat detections)
        {
            //ToDo: checking of correct usage.
            if (!(detections.GetData() is float[,,,] detectionsArrayInFloats))
                throw new ArgumentNullException(nameof(detectionsArrayInFloats));

            var detectedRectangles = new List<Rectangle>();
            for (var i = 0; i < detectionsArrayInFloats.GetLength(2); i++)
            {
                if (!(Convert.ToSingle(detectionsArrayInFloats[0, 0, i, 2], CultureInfo.InvariantCulture) >
                      0.4)) continue;

                //ToDo: make it easier
                var Xstart = Convert.ToSingle(detectionsArrayInFloats[0, 0, i, 3],
                                 CultureInfo.InvariantCulture) * detectionSize * xRate;
                var Ystart = Convert.ToSingle(detectionsArrayInFloats[0, 0, i, 4],
                                 CultureInfo.InvariantCulture) * detectionSize * yRate;
                var Xend = Convert.ToSingle(detectionsArrayInFloats[0, 0, i, 5],
                               CultureInfo.InvariantCulture) * detectionSize * xRate;
                var Yend = Convert.ToSingle(detectionsArrayInFloats[0, 0, i, 6],
                               CultureInfo.InvariantCulture) * detectionSize * yRate;

                var rect = new Rectangle
                {
                    X = (int) Xstart,
                    Y = (int) Ystart,
                    Height = (int) (Yend - Ystart),
                    Width = (int) (Xend - Xstart)
                };
                detectedRectangles.Add(rect);
            }

            return detectedRectangles;
        }

        //ToDo: rework settings & return type
        public Tuple<Image<Bgr, byte>, int> TryCascadeRecognition(Mat originalFrames,
            CascadeClassifier cascadeClassifier, Settings settings, Color color)
        {
            try
            {
                var resultFrame = originalFrames.ToImage<Bgr, byte>();
                var faces = cascadeClassifier.DetectMultiScale(
                    originalFrames.ToImage<Gray, byte>(),
                    settings.ScaleRate, settings.MinNeighbors,
                    new Size(settings.MinWindowSize, settings.MinWindowSize)
                );

                var numberOfFaces = faces.Length;

                foreach (var face in faces)
                {
                    resultFrame.Draw(face, new Bgr(color), 4, LineType.FourConnected);
                }

                return new Tuple<Image<Bgr, byte>, int>(resultFrame, numberOfFaces);
            }
            catch
            {
                //ignore
                return null;
            }

        }

        //ToDo: Not Implemented
        public Image<Bgr, byte> TryHogDescriptor(Mat originalFrames, HOGDescriptor hogDescriptor)
        {
            try
            {
                var resultFrame = new Image<Bgr,byte>(resolutionX, resolutionY);
                hogDescriptor.SetSVMDetector(HOGDescriptor.GetDefaultPeopleDetector());
                var regions = hogDescriptor.DetectMultiScale(originalFrames);
                foreach (var pedestrain in regions)
                {
                    resultFrame.Draw(pedestrain.Rect, new Bgr(Color.Red), 1);
                }
                return resultFrame;
            }
            catch
            {
                //ignore
                return null;
            }
            
        }

        private bool IsResolutionCorrect(Image<Bgr, byte> originalImage)
        {
            return originalImage.Width == resolutionX && originalImage.Height == resolutionY;
        }

    }
}