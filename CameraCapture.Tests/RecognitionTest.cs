using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CameraCapture.FileStorage;
using CameraCapture.Modules;
using CameraCapture.Utilities;
using Emgu.CV;
using Emgu.CV.Face;
using Emgu.CV.Structure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ResourcesUtilities = CameraCapture.Tests.Utilities.ResourcesUtilities;

namespace CameraCapture.Tests
{
    [TestClass]
    public class RecognitionTest
    {
        private const string PathTrained = "EmbeddingTest.Trained";

        [TestMethod]
        [DataRow("Archive/")]
        public void BaseRecognitionAlgoTest(string dataSubPath)
        {
            FaceRecognizerTest(dataSubPath, RecognitionModuleEnum.EigenFaceRecognizer, "Valentin_2.jpg", "Valentin", 100.0);
            FaceRecognizerTest(dataSubPath, RecognitionModuleEnum.FisherFaceRecognizer, "Valentin_2.jpg", "Valentin", 100.0); //Fail - Egor
            FaceRecognizerTest(dataSubPath, RecognitionModuleEnum.LbphFaceRecognizer, "Valentin_2.jpg", "Valentin", 100.0);
            FaceRecognizerTest(dataSubPath, RecognitionModuleEnum.LbphFaceRecognizer, "Valentin_1.jpg", "Valentin", 100.0);
            FaceRecognizerTest(dataSubPath, RecognitionModuleEnum.LbphFaceRecognizer, "Olya_1.jpg", "Olya", 100.0); 
            FaceRecognizerTest(dataSubPath, RecognitionModuleEnum.LbphFaceRecognizer, "Egor_1.png", "Egor", 100.0); // Fail - Valentin
        }


        public void FaceRecognizerTest(string dataSubPath, RecognitionModuleEnum recognitionMethod, string testImageFileName, string expected, double confidence)
        {
            var recognizer = new RecognitionModule(recognitionMethod);

            // ReSharper disable once InconsistentNaming
            var trainDataDAL = new FileSystemDAL(dataSubPath);
            var mainLabelMap = new LabelMap(trainDataDAL.GetLabelMap());

            TrainRecognizerImage(recognizer, mainLabelMap, trainDataDAL.GetImages().ToList());

            var testImageBytes = ResourcesUtilities.GetResourceBytes(testImageFileName);
            var testImageMat = ImageUtilities.BytesToBgrMat(testImageBytes);

            var detector = new DetectionModule(testImageMat.Width, testImageMat.Height);
            detector.GetDetectedFacesDnn(testImageMat, "1");
           
            foreach (var predictionInfo in detector.DetectorResults.Select(detectorResult => 
                Predict(recognizer, mainLabelMap, ImageUtilities.ResizeMat(detectorResult.Image.Mat, detector.RoiResizeValue))))
            {
                Debug.WriteLine(predictionInfo.distance >= confidence
                    ? $@"Label: {predictionInfo.label}. Confidence: {predictionInfo.distance}"
                    : $@"LabelFailed: {predictionInfo.label}. Confidence: {predictionInfo.distance}");
                Assert.AreEqual(expected, predictionInfo.label);
            }
        }

        public static void TrainRecognizerImage(RecognitionModule recognizer, LabelMap mainLabelMap, List<ImageLabel> images)
        {
            var faceEmbeddings = images
                .Select(img => (
                        mainLabelMap.Map[img.Label],
                        ImageUtilities.ToGrayScale(ImageUtilities.BytesToBgrMat(img.Image))
                        )
                ).Where(tuple => tuple.Item2 != null)
                .ToList();

            if (faceEmbeddings.Any())
            {
                recognizer.Train(faceEmbeddings, PathTrained);
            }

        }

        private (double distance, string label) Predict(RecognitionModule recognizer, LabelMap mainLabelMap, Mat mat)
        {
            var prediction = recognizer.Predict(ImageUtilities.ToGrayScale(mat));
            return (prediction.Distance, mainLabelMap.ReverseMap[prediction.Label]);
        }

        [TestMethod]
        public void EigenFaceBase()
        {
            const string folderPath = @"E:\Val-ly_PC\Desktop\SamplesQuee";
            var trainingImages = new List<Image<Bgr, byte>>
            {
                new Image<Bgr, byte>($@"{folderPath}\Olya.jpg"),
                new Image<Bgr, byte>($@"{folderPath}\Pasha.jpg"),
                new Image<Bgr, byte>($@"{folderPath}\Valentin.jpg")
            };

            var trainingImagesMat = new List<Mat>
            {
                ImageUtilities.ToGrayScale(trainingImages[0].Mat),
                ImageUtilities.ToGrayScale(trainingImages[1].Mat),
                ImageUtilities.ToGrayScale(trainingImages[2].Mat)
            };

            var labels = new[] { 0, 1, 2};
            var testImage = new Image<Bgr, byte>($@"{folderPath}\Pasha.jpg");

            var recognizer = new EigenFaceRecognizer(0, 0.2);
            recognizer.Train(trainingImagesMat.ToArray(), labels);

            var result = recognizer.Predict(ImageUtilities.ToGrayScale(testImage.Mat));

            Debug.WriteLine(result.Label);
            Assert.AreEqual(1, result.Label);
        }


    }

    //if (!CameraImage.TryCascadeRecognition(originalFrames, CascadeClassifierList[0], Settings, Color.Blue))
    //    MessageBox.Show(@"TryCascadeRecognition 0- false");
    //if (!CameraImage.TryCascadeRecognition(CameraImage.ResultFrame.Mat, CascadeClassifierList[1], Settings, Color.Red))
    //    MessageBox.Show(@"TryCascadeRecognition 1- false");
}
