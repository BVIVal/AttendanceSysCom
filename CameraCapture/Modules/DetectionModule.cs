using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using CameraCapture.Algorithms;
using CameraCapture.Utilities;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Dnn;
using Emgu.CV.Structure;

namespace CameraCapture.Modules
{
    public class DetectionModule : IZoneModule
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
        
        public List<Image<Bgr,byte>> RoiList { get; private set; }
        public int NumberOfFaces { get; set; }
        public ZoneModule AreaModule { get; set; }
        public Size RoiResizeValue { get; private set; }
        #endregion

        public DetectionModule(int resolutionX, int resolutionY)
        {
            if (resolutionX <= 0) throw new ArgumentOutOfRangeException(nameof(resolutionX));
            if (resolutionY <= 0) throw new ArgumentOutOfRangeException(nameof(resolutionY));

            this.resolutionX = resolutionX;
            this.resolutionY = resolutionY;

            xRate = resolutionX / (float) detectionSize;
            yRate = resolutionY / (float) detectionSize;

            NumberOfFaces = -1;
            RoiList = new List<Image<Bgr, byte>>();
            detector = GetDetectorDnn();
           
            RoiResizeValue = new Size(113, 146);
            AreaModule = new ZoneModule();
        }

        private static Net GetDetectorDnn()
        {
            var proto = ResourcesUtilities.GetResourceBytes("deploy.prototxt");
            var model = ResourcesUtilities.GetResourceBytes("res10_300x300_ssd_iter_140000.caffemodel");
            return DnnInvoke.ReadNetFromCaffe(proto, model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="originalBgrMat"> Get only Mat and Image'Bgr,byte'</param>
        /// <returns></returns>
        public Image<Bgr, byte> GetDetectedFacesDnn(IInputArray originalBgrMat) 
        {
            var mat = originalBgrMat as Mat;
            var originalImage = originalBgrMat as Image<Bgr, byte> ?? mat?.ToImage<Bgr, byte>();
            if (originalImage == null) throw new ArgumentNullException();

            if (!IsResolutionCorrect(originalImage))
                throw new ArgumentException($"Not equal resolution exception: {nameof(originalImage)}");
            var resultImage = originalImage.Clone();
            RoiList.Clear();

            var blobs = DnnInvoke.BlobFromImage(resultImage, 1.0, new Size(detectionSize, detectionSize));
            detector.SetInput(blobs);
            var detectedRectangles = GetDetectedRectangles(detector.Forward());

            foreach (var detectedRectangle in detectedRectangles)
            {
                resultImage.Draw(detectedRectangle, new Bgr(Color.GreenYellow));

                RoiList.Add(resultImage.Copy(detectedRectangle));
            }

            NumberOfFaces = RoiList.Count;
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
        public IList<(Image<Bgr,byte> resultImage, int numberOfFaces)> TryCascadeRecognition(Mat originalFrames,
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

                var localNumberOfFaces = faces.Length;

                foreach (var face in faces)
                {
                    resultFrame.Draw(face, new Bgr(color), 4, LineType.FourConnected);
                }
                return new List<(Image<Bgr, byte> resultImage, int numberOfFaces)>{(resultFrame,localNumberOfFaces)};
            }
            catch
            {
                //ignore
                return new List<(Image<Bgr, byte> resultImage, int numberOfFaces)>();
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