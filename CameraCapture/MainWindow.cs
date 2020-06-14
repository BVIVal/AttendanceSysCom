using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using CameraCapture.Extensions;
using CameraCapture.FileStorage;
using CameraCapture.Modules;
using CameraCapture.Utilities;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Newtonsoft.Json.Linq;
using DateTime = System.DateTime;

namespace CameraCapture
{
    public partial class MainWindow : Form
    {
        #region Fields

        private const string trainedModelPath = "Embeddings.trained";
        private bool hasTrainedModel = false;
        private readonly double minConfidence;


        private int _camNumber = -1;
        private int _counter = 0;

        /// <summary>
        /// Resolution X.
        /// </summary>
        private int _resolutionX = 640;

        /// <summary>
        /// Resolution Y.
        /// </summary>
        private int _resolutionY = 480;

        //ToDo: create ConfigClass
        private List<CascadeClassifier> CascadeClassifierList { get; set; }
        private string _chosenAlgorithm0 = "Algorithms//haarcascade_frontalface_alt_tree.xml";
        private string _chosenAlgorithm1 = "Algorithms//haarcascade_profileface.xml";
        private string _trainedModelPath = "Embeddings_";
        public const string _archivePath = "Archive/";

        private HOGDescriptor hogDescriptor;
        private bool IsSnapshotRequested = false;
        //private bool IsRecognitionEnabled = false;
        private LabelMap labelMap;

        private Size RoiSize { get; set; }

        //private readonly ITrainDataDAL trainDataDAL;

        #endregion

        #region Properties

        private ImageProcessing CameraImage { get; }
        public CancellableExecutor Source { get; set; }
        private DetectionModule Detector { get; set; }
        private RecognitionModule Recognizer { get; set; }
        private ITrainDataDAL TrainDataDAL { get; set; }

        private event EventHandler<Mat> PersonDetected; 

        public string FacesNum
        {
            get => facesNumLabel.Text;
            set => this.InvokeIfRequired(_ => facesNumLabel.Text = $@"Status: {value}");
        }

        public string Fps
        {
            get => fpsLabel.Text;
            set => this.InvokeIfRequired(_ => fpsLabel.Text = $@"FPS: {value}");
        }

        public string Log
        {
            set => this.InvokeIfRequired(_ => LogListBox.Items.Add($@"{value}"));
        }

        public bool IsRecognitionEnabled => EnableRecognitionCheckBox.Checked;

        #endregion

        public MainWindow()
        {
            InitializeComponent();
            Source = new CancellableExecutor();
            LoadSettings();
            CameraImage = new ImageProcessing(_resolutionX, _resolutionY);

            Detector = new DetectionModule(_resolutionX, _resolutionY);
            Recognizer = new RecognitionModule(RecognitionModuleEnum.LbphFaceRecognizer);
            TrainDataDAL = new FileSystemDAL(_archivePath);
            labelMap = new LabelMap(TrainDataDAL.GetLabelMap());
            minConfidence = 100; // 100 - LBPH

            CascadeClassifierList = new List<CascadeClassifier>();
            CascadeClassifierList.Add(new CascadeClassifier(_chosenAlgorithm0));
            CascadeClassifierList.Add(new CascadeClassifier(_chosenAlgorithm1));

            PersonDetected += OnPersonDetected;
        }

        
        private void ProcessFrameEventTask(object sender, EventArgs e)
        {
            ProcessFrame();
        }

        //ToDo: separate AddToDb from ProcessFrame
        private void ProcessFrame()
        {
            try
            {
                var sw = new Stopwatch();
                sw.Start();
                //skip 2/3 of the frames, due to too much work on CPU
                //if (_counter++ % 3 != 0) return Task.FromResult(true);
                //var originalFrames = CameraImage.GetFrames();
                
                var imageWithDetections = Detector.GetDetectedFacesDnn(CameraImage.GetRetrieveImage());
                if(IsSnapshotRequested)
                {
                    IsSnapshotRequested = false;
                    AddToDB(Detector.RoiList, Detector.RoiResizeValue, imageWithDetections);
                }

                if (IsRecognitionEnabled)
                { 
                    foreach (var image in Detector.RoiList)
                    {
                        PersonDetected?.Invoke(this, image.Mat);
                    }
                }

                camImageBox.Image = imageWithDetections;
                FacesNum = $"Faces: {Detector.NumberOfFaces}";
                Fps = $@"{sw.Elapsed}";
            }
            catch (Exception exception)
            {
                //ToDo: make logs
                Console.WriteLine(exception);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="mat"> Cropped image. Send DetectorModule.ListOfRois</param>
        private void OnPersonDetected(object sender, Mat mat)
        {
            if (!IsRecognitionEnabled) return;
            var predictionInfo = Predict(ImageUtilities.ResizeMat(mat, Detector.RoiResizeValue));
            if (predictionInfo.distance < minConfidence)
            {
                Debug.WriteLine($@"Label: {predictionInfo.label}. Confidence: {predictionInfo.distance}");
                Log = $@"Label: {predictionInfo.label}. Confidence: {predictionInfo.distance}. Time: {DateTime.Now.ToLongTimeString()}.";
            }
            else Debug.WriteLine($@"LabelFailed: {predictionInfo.label}. Confidence: {predictionInfo.distance}");
        }

        private (double distance, string label) Predict(Mat mat)
        {
            if (!hasTrainedModel) EnsureTrained(true);

            var prediction = Recognizer.Predict(ImageUtilities.ToGrayScale(mat));
            return (prediction.Distance, labelMap.ReverseMap[prediction.Label]);
        }

        //ToDo: Check if model already Trained and trigger on ForceRetrainingRequested
        public void EnsureTrained(bool isForceRetraining = false)
        {
            if (hasTrainedModel && !isForceRetraining)
            {
                Console.WriteLine(@"[INFO] Model has been trained already");
                return;
            }
            if (File.Exists(trainedModelPath))
            {
                Console.WriteLine(@"[INFO] Model exists, loading");
                Recognizer.Load(trainedModelPath);
            }
            else
            {
                Console.WriteLine(@"[INFO] Model doesn't exist, started training");
                Train();
            }

            hasTrainedModel = true;
        }

        public void Train()
        {
            CameraImage.Capture.Pause();
            var images = TrainDataDAL.GetImages().ToList();
            labelMap = new LabelMap(TrainDataDAL.GetLabelMap());
            var faceEmbeddings = images
                .Select(img => (labelMap.Map[img.Label], ImageUtilities.BytesToBgrMat(img.Image)))
                .Where(tuple => tuple.Item2 != null)
                .ToList();
            if (faceEmbeddings.Any())
            {
                Recognizer.Train(faceEmbeddings, trainedModelPath);
            }

            //ToDo: encapsulation of start/stop VideoCapture
            if(btnStart.Text == @"Pause") CameraImage.Capture.Start();
            hasTrainedModel = false;
        }

        //ToDo: How to make it Async!!! ConfigureAwait(false)?
        // ReSharper disable once InconsistentNaming
        private static void AddToDB(IEnumerable<Image<Bgr, byte>> roiList, Size imageSize, Image<Bgr, byte> originalResultImage = null)
        {
            try
            {
                if (originalResultImage != null)
                {
                    ImageUtilities.SaveRoiToFileJpg(_archivePath + $@"0000_{DateTime.Now:dd_MM_yyyy__HH_mm_ss}",
                        new[] { originalResultImage });
                }

                NormalizeAndSafe(roiList, imageSize);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //throw;
            }
        }

        //ToDo: Replace to ImageUtilities
        private static void NormalizeAndSafe(IEnumerable<Image<Bgr, byte>> roiList, Size imageSize)
        {
            var localRoiList = roiList.ToList();
            ImageUtilities.SaveRoiToFileJpg(_archivePath + $@"0001_{DateTime.Now:dd_MM_yyyy__HH_mm_ss}", localRoiList, imageSize);
            ImageUtilities.SaveRoiToFileJpg(_archivePath + $@"0002_{DateTime.Now:dd_MM_yyyy__HH_mm_ss}", ImageUtilities.FlipImagesHorizontal(localRoiList), imageSize);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (CameraImage.Capture == null) return;

            if (btnStart.Text == @"Pause")
            {
                btnStart.Text = @"Resume";
                CameraImage.Capture.ImageGrabbed -= ProcessFrameEventTask;
                CameraImage.Capture.Stop();
            }
            else
            {
                btnStart.Text = @"Pause";
                CameraImage.Capture.Start();
                CameraImage.Capture.ImageGrabbed += ProcessFrameEventTask;
            }
            CameraImage.CaptureInProgress = !CameraImage.CaptureInProgress;
        }

        private void ReleaseData()
        {
            CameraImage.Capture?.Dispose();
        }

        private async void cbCamIndex_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!int.TryParse(cbCamIndex.Text, out _camNumber))
            {
                MessageBox.Show(@"Only numbers!");
                return;
            }

            if (CameraImage.Capture == null)
            {
                try
                {
                    await Task.Run(() => CameraImage.CreateVideoCapture(_camNumber));
                }
                catch (NullReferenceException exception)
                {
                    MessageBox.Show(exception.Message);
                }
            }

            btnStart_Click(sender, e);
            btnStart.Enabled = true;
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {

        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowSettings();
        }

        public T LoadFromJson<T>(string path) where T : class, new()
        {
            if (!File.Exists(path))
            {
                return null;
            }

            try
            {
                var text = File.ReadAllText(path);
                return JsonUtilities.Deserialize<T>(text);
            }
            catch (Exception exception)
            {
                MessageBox.Show($@"LoadFromJson - exception. {exception}");

                return null;
            }
        }

        public void SaveAsJson(string path, object value, bool onlyWritable = true)
        {
            try
            {
                var folder = Path.GetDirectoryName(path);
                if (folder != null)
                {
                    Directory.CreateDirectory(folder);
                }

                var json = JsonUtilities.Serialize(value, true, onlyWritable);

                File.WriteAllText(path, json);
            }
            catch (Exception exception)
            {
                MessageBox.Show($@"SaveAsJson - exception. {exception.Message}");
            }
        }

        private void BtnGetSnapshot_Click(object sender, EventArgs e)
        {
            IsSnapshotRequested = true;
        }
    }
}
