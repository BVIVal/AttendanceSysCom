using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using CameraCapture.Algorithms;
using CameraCapture.Forms;

namespace CameraCapture
{
    public partial class MainWindow 
    {
        public const string SettingsPath = "settings/settings.json";

        public async void ShowSettings()
        {
            try
            {
                await Task.Run(() =>
                {
                    //ToDo: non-optimal;
                    LoadSettings();
                    using (var form = new SettingsForm(Settings))
                    {
                        if (form.ShowDialog() != DialogResult.OK)
                        {
                            LoadSettings();
                            return;
                        }

                        SaveSettings();
                    }
                });
            }
            catch (Exception exception)
            {
                MessageBox.Show($@"{nameof(ShowSettings)}. {exception.Message}");
            }
        }

        public Settings Settings = new Settings();

        public double ScaleRate
        {
            get => Settings.ScaleRate;
            set => Settings.ScaleRate = value;
        }

        public int MinNeighbors
        {
            get => Settings.MinNeighbors;
            set => Settings.MinNeighbors = value;
        }

        public int MinWindowSize
        {
            get => Settings.MinWindowSize;
            set => Settings.MinWindowSize = value;
        }

        public void LoadSettings()
        {
            Settings = LoadFromJson<Settings>(SettingsPath) ?? Settings;
        }

        public void SaveSettings()
        {
            SaveAsJson(SettingsPath, Settings, false);
        }
    }
}
