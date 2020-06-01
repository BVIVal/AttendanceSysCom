using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using CameraCapture.Extensions;
using CameraCapture.Modules;
using CameraCapture.Utilities;
using Emgu.CV;

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

        private List<CascadeClassifier> CascadeClassifierList { get; set; }
        private string _chosenAlgorithm0 = "Algorithms//haarcascade_frontalface_alt_tree.xml";
        private string _chosenAlgorithm1 = "Algorithms//haarcascade_profileface.xml";

        private HOGDescriptor hogDescriptor;

        
        #endregion

        #region Properties

        private ImageProcessing CameraImage { get; }
        public CancellableExecutor Source { get; set; }
        private DetectionModule Detector { get; set; }

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
                
                var resultImage = Detector.GetDetectedFacesDnn(CameraImage.GetRetrieveImage());
                camImageBox.Image = resultImage;

                //var testDetector = DetectionModule.GetDetectedFaceBox()

                //if (!CameraImage.TryCascadeRecognition(originalFrames, CascadeClassifierList[0], Settings, Color.Blue))
                //    MessageBox.Show(@"TryCascadeRecognition 0- false");
                //if (!CameraImage.TryCascadeRecognition(CameraImage.ResultFrame.Mat, CascadeClassifierList[1], Settings, Color.Red))
                //    MessageBox.Show(@"TryCascadeRecognition 1- false");

                FacesNum = $"Faces: {CameraImage.NumberOfFaces}";

                Fps = $@"{sw.Elapsed}";
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }

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





    }
}
