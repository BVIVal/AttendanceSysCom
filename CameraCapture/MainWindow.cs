using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CameraCapture
{
    public partial class MainWindow : Form
    {
        #region Fields

        private int _camNumber = -1;
        #endregion

        #region Properties

        private ImageProcessing TestCapture { get; }
        public CancellableExecutor Source { get; set; }
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            Source = new CancellableExecutor();
            TestCapture = new ImageProcessing();
        }

        private Task ProcessFrame()
        {
            try
            {
                camImageBox.Image = TestCapture.GetFrames();
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
    }
}
