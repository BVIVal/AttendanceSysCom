using System;
using System.Collections.Generic;
using System.Windows.Forms;
using CameraCapture.Common;

namespace CameraCapture.Forms
{
    public partial class ScheduleForm : Form
    {
        public List<DetectorResult> Data { get; }
        public ScheduleForm(List<DetectorResult> data)
        {
            Data = data ?? throw new ArgumentNullException(nameof(data));
            InitializeComponent();
        }

        private void ScheduleForm_Load(object sender, EventArgs e)
        {
            try
            {
                var result = TimeCalculation.CalculateSpan(Data);
                
                foreach (var person in result)
                {
                    View.Rows.Add(person.name, person.absentTime);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            try
            {
                Close();
            }
            catch (Exception exception)
            {
                MessageBox.Show($@"{nameof(ExitButton_Click)}. {exception.Message}");
            }
        }
    }
}
