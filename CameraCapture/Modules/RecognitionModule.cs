using System;
using System.Collections.Generic;
using System.Linq;
using CameraCapture.Utilities;
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
                    Recognizer = new EigenFaceRecognizer(80, double.MaxValue/2);
                    break;
                case RecognitionModuleEnum.FisherFaceRecognizer:
                    Recognizer = new FisherFaceRecognizer(0, double.MaxValue);
                    break;
                case RecognitionModuleEnum.LbphFaceRecognizer:
                    Recognizer = new LBPHFaceRecognizer(1, 8,8,9, 100);
                    break;
                case RecognitionModuleEnum.OpenFaceRecognizer:
                default:
                    throw new ArgumentOutOfRangeException(nameof(chosenMethod), chosenMethod, null);
            }
        }

        public void Train(IList<(int, Mat)> labeledFaces, string path)
        {
            Recognizer.Train(labeledFaces.Select(f => ImageUtilities.ToGrayScale(f.Item2)).ToArray(), labeledFaces.Select(f => f.Item1).ToArray());
            Recognizer.Write(path);
        }

        public FaceRecognizer.PredictionResult Predict(Mat mat)
        {
            return Recognizer.Predict(mat);
        }

        public void Load(string path)
        {
            Recognizer.Read(path);
        }
    }
}