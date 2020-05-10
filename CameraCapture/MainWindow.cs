using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using CameraCapture.Extensions;
using CameraCapture.Utilities;
using Emgu.CV;
using Emgu;
using Emgu.CV.CvEnum;
using Emgu.CV.Dnn;
using Emgu.Util;
using Emgu.CV.Structure;

namespace CameraCapture
{
    public partial class MainWindow : Form
    {
        #region Fields

        private int _camNumber = -1;
        private CascadeClassifier cascadeClassifer;
        private string _chosenAlgorithm = "Algorithms//haarcascade_frontalface_alt_tree.xml";

        private HOGDescriptor hogDescriptor;

        #endregion

        #region Properties

        private ImageProcessing TestCapture { get; }
        public CancellableExecutor Source { get; set; }

        public string FacesNum
        {
            get => facesNumLabel.Text;
            set => this.InvokeIfRequired(_ => facesNumLabel.Text = $@"Status: {value}");
        }
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            Source = new CancellableExecutor();
            LoadSettings();
            TestCapture = new ImageProcessing();
            cascadeClassifer = new CascadeClassifier(_chosenAlgorithm);
            hogDescriptor = new HOGDescriptor();
            
        }
        
        private Task ProcessFrame()
        {
            try
            {
                if (!TestCapture.TryCascadeRecognition(cascadeClassifer, Settings))
                    MessageBox.Show(@"TryCascadeRecognition - false");
                FacesNum = $"Faces: {TestCapture.NumberOfFaces}";
                
                camImageBox.Image = TestCapture.ResultFrame;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
            return Task.FromResult(true);

        }

        private async void btnStart_Click(object sender, EventArgs e)
        {
            if (TestCapture.Capture == null) return;

            if (btnStart.Text == @"Pause")
            {
                btnStart.Text = @"Resume";
                Source.Cancel();
            }
            else
            {
                btnStart.Text = @"Pause";
                await Task.Run(async () =>
                {
                    do
                    {
                        await ProcessFrame();
                        if (!Source.IsCancellationRequested()) continue;
                        Source.Restart();
                        break;
                    } while (true);

                    return true;
                }, Source.Token);
            }
            TestCapture.CaptureInProgress = !TestCapture.CaptureInProgress;
        }

        private void ReleaseData()
        {
            TestCapture.Capture?.Dispose();
        }

        private async void cbCamIndex_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!int.TryParse(cbCamIndex.Text, out _camNumber))
            {
                MessageBox.Show(@"Only numbers!");
                return;
            }

            if (TestCapture.Capture == null)
            {
                try
                {
                    await Task.Run(() => TestCapture.CreateVideoCapture(_camNumber));
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

        public void TestDnn()
        {
            //DnnInvoke.
        }
    }

    
}
