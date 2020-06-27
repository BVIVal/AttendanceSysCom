using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CameraCapture.Common;
using CameraCapture.Forms;

namespace CameraCapture
{
    public partial class MainWindow
    {
        public async void ShowSchedule(List<DetectorResult> data)
        {
            try
            {
                await Task.Run(() =>
                {
                    using (var form = new ScheduleForm(data))
                    {
                        if (form.ShowDialog() != DialogResult.OK)
                        {
                            return;
                        }
                    }
                });
            }
            catch (Exception exception)
            {
                MessageBox.Show($@"{nameof(ShowSettings)}. {exception.Message}");
            }
        }
    }
}
