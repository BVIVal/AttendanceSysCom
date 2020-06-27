using System;
using System.Threading.Tasks;
using CameraCapture.Common;

namespace CameraCapture.Modules
{
    public interface IOverwatchModule
    {
        Task SendAsync(DetectorResult result);
        Task GetScheduleAsync();

    }
}