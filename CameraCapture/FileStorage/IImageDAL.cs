using System.Collections.Generic;

namespace CameraCapture.FileStorage
{
    // ReSharper disable once InconsistentNaming
    public interface IImageDAL
    {
        ImageLabel Get(int id);

        void Add(IEnumerable<ImageLabel> images);

        void Add(ImageLabel image);

        void Delete(int id);

        void SetLabel(int imageLabelId, string label);
    }
}