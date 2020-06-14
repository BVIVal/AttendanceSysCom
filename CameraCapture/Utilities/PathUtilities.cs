using System.Collections.Generic;
using System.IO;
using CameraCapture.FileStorage;

namespace CameraCapture.Utilities
{
    public static class PathUtilities
    {
        //ToDo: reason?!
        public static string AppsPath(string path)
        {
            var folder = Path.GetDirectoryName(path);
            if (folder != null)
            {
                Directory.CreateDirectory(folder);
            }

            return path;
        }
    }
}