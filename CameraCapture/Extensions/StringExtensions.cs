using System.Globalization;
using System.Linq;

namespace CameraCapture.Extensions
{
    public static class StringExtensions
    {
        public static double ToDouble(this string text, double defaultValue = default)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return defaultValue;
            }

            var normalizedText = new string(text.Where(c => char.IsDigit(c) || c == '.' || c == ',' || c == '-').ToArray());
            const NumberStyles styles = NumberStyles.AllowLeadingWhite |
                                        NumberStyles.AllowTrailingWhite |
                                        NumberStyles.AllowDecimalPoint |
                                        NumberStyles.AllowLeadingSign;

            return double.TryParse(normalizedText.Replace(',', '.'), styles, CultureInfo.InvariantCulture, out var result)
                ? result : defaultValue;
        }

        public static int ToInteger(this string text, int defaultValue = default)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return defaultValue;
            }

            return int.TryParse(text, out var result)
                ? result
                : defaultValue;
        }
    }
}