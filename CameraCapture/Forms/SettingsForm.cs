using System;
using System.Windows.Forms;
using CameraCapture.Algorithms;
using CameraCapture.Extensions;

namespace CameraCapture.Forms
{
    public partial class SettingsForm : Form
    {
        public Settings Settings { get; }

        public SettingsForm(Settings settings)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            InitializeComponent();
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            try
            {
                ScaleComboBox.Text = "" + Settings.ScaleRate;
                MinNeighborsComboBox.Text = "" + Settings.MinNeighbors;
                MinWindowSizeComboBox.Text = "" + Settings.MinWindowSize;
            }
            catch (Exception exception)
            {
                MessageBox.Show($@"{nameof(SettingsForm_Load)}. {exception.Message}");
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            try
            {
                Settings.ScaleRate = ScaleComboBox.Text.ToDouble();
                Settings.MinNeighbors = MinNeighborsComboBox.Text.ToInteger();
                Settings.MinWindowSize = MinWindowSizeComboBox.Text.ToInteger();

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception exception)
            {
                MessageBox.Show($@"{nameof(SaveButton_Click)}. {exception}");
            }
            

        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            try
            {
                Close();
            }
            catch (Exception exception)
            {
                MessageBox.Show($@"{nameof(SaveButton_Click)}. {exception}");
            }
        }
    }
}
