using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CameraCapture.Tests.Utilities
{
    public static class ResourcesUtilities
    {
        public static string ReadFile(string name, Assembly assembly = null)
        {
            assembly = assembly ?? Assembly.GetExecutingAssembly();
            var resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith(name));

            using (var stream = assembly.GetManifestResourceStream(resourceName) ?? throw new ArgumentException($"\"{name}\" is not found in embedded resources"))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public static byte[] GetResourceBytes(string name, Assembly assembly = null)
        {
            assembly = assembly ?? Assembly.GetExecutingAssembly();
            var resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith(name));

            using (var stream = assembly.GetManifestResourceStream(resourceName) ?? throw new ArgumentException($"\"{name}\" is not found in embedded resources"))
            {
                var bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                return bytes;
            }
        }
    }
}