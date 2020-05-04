using System;
using System.Windows.Forms;
using CameraCapture.Algorithms;
using CameraCapture.Forms;

namespace CameraCapture
{
    public partial class MainWindow 
    {
        public const string SettingsPath = "settings/settings.json";

        public void ShowSettings()
        {
            try
            {
                using (var form = new SettingsForm(Settings))
                {
                    if(form.ShowDialog() != DialogResult.OK)
                    {
                        LoadSettings();
                        return;
                    }

                    SaveSettings();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }

        public Settings Settings { get; set; } = new Settings();

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
