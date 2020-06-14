using System.Collections.Generic;

namespace CameraCapture.FileStorage
{
    public interface ITrainDataDAL
    {
        IEnumerable<ImageLabel> GetImages();

        Dictionary<string, int> GetLabelMap();
    }
}