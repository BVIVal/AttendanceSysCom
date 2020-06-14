using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace CameraCapture.Utilities
{
    public static class ImageUtilities
    {
        public static IEnumerable<Image<Bgr, byte>> FlipImagesHorizontal(IEnumerable<Image<Bgr, byte>> roiList)
        {
            return roiList.Select(image => image.Flip(FlipType.Horizontal));
        }

        //ToDo: check if cloning is necessary; Think about BinaryWriter + FileStream
        //ToDo: what to do when we have the file with the same name
        public static void SaveBytesToFile(string path, object value, bool onlyWritable = true)
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

        public static Image<Bgr, byte> Normalize(Image<Bgr, byte> image, Size resizeValue)
        {
            var resizedImage = image.Resize(resizeValue.Width, resizeValue.Height, Inter.Cubic);
            return HistogramEqualization(resizedImage);
        }

        public static Image<Bgr, byte> HistogramEqualization(Image<Bgr, byte> image)
        {
            var imageYcc = image.Convert<Ycc, byte>();

            var channelY = imageYcc[0];
            channelY._EqualizeHist();
            imageYcc[0] = channelY;

            return imageYcc.Convert<Bgr, byte>();
        }

        public static void SaveRoiToFileJpg(string path, IEnumerable<Image<Bgr, byte>> roiList, Size imageSize = default, bool onlyWritable = true)
        {
            foreach (var roi in roiList.Select((value, i) => new { i, value }))
            {
                SaveBytesToFile(path + $"_{roi.i}.jpg",
                    imageSize == default
                        ? Normalize(roi.value, roi.value.Size).ToJpegData()
                        : Normalize(roi.value, imageSize).ToJpegData());
            }
        }

        public static Mat BytesToBgrMat(byte[] bytes)
        {
            var mat = new Mat();
            CvInvoke.Imdecode(bytes, ImreadModes.AnyColor, mat);
            return mat;
        }

        public static Mat ResizeMat(Mat mat, Size size)
        {
            var resizedMat = new Mat();
            CvInvoke.Resize(mat, resizedMat, size, interpolation:Inter.Cubic);
            return resizedMat;
        }

        //ToDo: just get one of the channels?
        public static Mat ToGrayScale(Mat mat)
        {
            var greyMat = new Mat();
            CvInvoke.CvtColor(mat, greyMat, ColorConversion.Bgr2Gray);
            return greyMat;
        }

    }
}