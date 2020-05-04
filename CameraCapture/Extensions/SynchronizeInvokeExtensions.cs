using System;
using System.ComponentModel;

namespace CameraCapture.Extensions
{
    public static class SynchronizeInvokeExtensions
    {
        public static void InvokeIfRequired<T>(this T value, Action<T> action)
            where T : ISynchronizeInvoke
        {
            try
            {
                if (value.InvokeRequired)
                {
                    value.Invoke(action, new object[] { value });
                }
                else
                {
                    action(value);
                }
            }
            catch (ObjectDisposedException)
            {
                // ignored
            }
        }

        public static TOut InvokeIfRequired<TIn, TOut>(this TIn value, Func<TIn, TOut> func)
            where TIn : ISynchronizeInvoke
        {
            try
            {
                return value.InvokeRequired
                    ? (TOut)value.Invoke(func, new object[] { value })
                    : func(value);
            }
            catch (ObjectDisposedException)
            {
                return default;
            }
        }
    }
}