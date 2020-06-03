using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using CameraCapture.Extensions;
using CameraCapture.Modules;
using CameraCapture.Utilities;
using Emgu.CV;
using Emgu.CV.Structure;
using DateTime = System.DateTime;

namespace CameraCapture
{
    public partial class MainWindow : Form
    {
        #region Fields

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
        
        #endregion

        #region Properties

        private ImageProcessing CameraImage { get; }
        public CancellableExecutor Source { get; set; }
        private DetectionModule Detector { get; set; }
        private RecognitionModule Recognizer { get; set; }

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

        #endregion

        public MainWindow()
        {
            InitializeComponent();
            Source = new CancellableExecutor();
            LoadSettings();
            CameraImage = new ImageProcessing(_resolutionX, _resolutionY);

            Detector = new DetectionModule(_resolutionX, _resolutionY, 0.5);

            CascadeClassifierList = new List<CascadeClassifier>();
            CascadeClassifierList.Add(new CascadeClassifier(_chosenAlgorithm0));
            CascadeClassifierList.Add(new CascadeClassifier(_chosenAlgorithm1));
        }

        
        private void ProcessFrameEventTask(object sender, EventArgs e)
        {
            ProcessFrame();
        }

        private void ProcessFrame()
        {
            try
            {
                var sw = new Stopwatch();
                sw.Start();
                //skip 2/3 of the frames, due to too much work on CPU
                //if (_counter++ % 3 != 0) return Task.FromResult(true);
                //var originalFrames = CameraImage.GetFrames();
                
                var resultImageDetector = Detector.GetDetectedFacesDnn(CameraImage.GetRetrieveImage());
                if(IsSnapshotRequested)
                {
                    IsSnapshotRequested = false;
                    SaveRoiToFileJpg(_archivePath + $@"0000_{DateTime.Now:dd_MM_yyyy__HH_mm_ss}.jpg", new[] {resultImageDetector});
                    SaveRoiToFileJpg(_archivePath + $@"0001_{DateTime.Now:dd_MM_yyyy__HH_mm_ss}", Detector.RoiList);

                }

                camImageBox.Image = resultImageDetector;
                FacesNum = $"Faces: {Detector.NumberOfFaces}";
                Fps = $@"{sw.Elapsed}";
            }
            catch (Exception exception)
            {
                //ToDo: make logs
                Console.WriteLine(exception);
            }

        }

        public void Train()
        {
            CameraImage.Capture.Stop();
            //ToDo: add Console.WriteLine("[INFO] Model exists, loading");
            //Recognizer.Train();
        }

        private void SaveRoiToFileJpg(string path, IEnumerable<Image<Bgr, byte>> roiList, bool onlyWritable = true)
        {
            foreach (var roi in roiList.Select((value, i) => new {i, value}))
            {
                var normalizedImage = HistogramEqualization(roi.value);
                SaveBytesToFile(path + $"_{roi.i}.jpg", normalizedImage.ToJpegData());
            }
        }

        //ToDo: check if cloning is necessary; Think about BinaryWriter + FileStream
        private void SaveBytesToFile(string path, object value, bool onlyWritable = true)
        {
            try
            {
                var folder = Path.GetDirectoryName(path);
                if (folder != null)
                {
                    Directory.CreateDirectory(folder);
                }

                File.WriteAllBytes(path, (byte[])value);
            }
            catch (Exception exception)
            {
                MessageBox.Show($@"SaveSnapshotToFile - exception. {exception.Message}");
            }
        }

        private Image<Bgr, byte> HistogramEqualization(Image<Bgr, byte> image)
        {
            var imageYcc = image.Convert<Ycc, byte>();

            var channelY = imageYcc[0];
            channelY._EqualizeHist();
            imageYcc[0] = channelY;

            return imageYcc.Convert<Bgr, byte>();
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
