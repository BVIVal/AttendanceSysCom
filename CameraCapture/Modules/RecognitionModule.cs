using System;
using System.Collections.Generic;
using System.Linq;
using Emgu.CV;
using Emgu.CV.Face;

namespace CameraCapture.Modules
{
    public class RecognitionModule
    {
        private FaceRecognizer Recognizer { get; set; }

        public RecognitionModule(RecognitionModuleEnum chosenMethod)
        {
            switch (chosenMethod)
            {
                case RecognitionModuleEnum.EigenFaceRecognizer:
                    Recognizer = new EigenFaceRecognizer();
                    break;
                case RecognitionModuleEnum.FisherFaceRecognizer:
                    Recognizer = new FisherFaceRecognizer();
                    break;
                case RecognitionModuleEnum.LbphFaceRecognizer:
                    Recognizer = new LBPHFaceRecognizer();
                    break;
                case RecognitionModuleEnum.OpenFaceRecognizer:
                default:
                    throw new ArgumentOutOfRangeException(nameof(chosenMethod), chosenMethod, null);
            }
        }

        public void Train(IList<(int, Mat, DateTime)> labeledFaces, string path)
        {
            Recognizer.Train(labeledFaces.Select(f => f.Item2).ToArray(), labeledFaces.Select(f => f.Item1).ToArray());
            Recognizer.Write(path);
        }

        public FaceRecognizer.PredictionResult Predict(Mat image)
        {
            return Recognizer.Predict(image);
        }

        public void Load(string path)
        {
            Recognizer.Read(path);
        }
    }
}