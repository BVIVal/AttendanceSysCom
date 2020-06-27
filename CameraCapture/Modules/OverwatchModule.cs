using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CameraCapture.Common;
using LiteDB;
using System.Linq;

namespace CameraCapture.Modules
{
    public class OverwatchModule //: IOverwatchModule
    {
        private LiteDatabase LiteDb;
        private ILiteCollection<DetectorResult> Collection { get; set; }
        public OverwatchModule(string dataBasePath)
        {
            LiteDb = new LiteDatabase(dataBasePath);
            Collection = LiteDb.GetCollection<DetectorResult>();
        }

        public async Task SendAsync(DetectorResult data) => await Task.Run(() =>
        {
            try
            {

                //var testVar = Collection.Find(x => x.ImageProcessingId == data.ImageProcessingId).LastOrDefault();
                //if (testVar == null || testVar.Equals(data) ) return;
                Collection.Insert(new DetectorResult
                {
                    ImageProcessingId = data.ImageProcessingId,
                    Name = data.Name,
                    Position = data.Position,
                    Time = data.Time,
                    Zone = data.Zone
                });
            }
            catch (Exception exception)
            {
                throw new ArgumentException("SendAsync", exception);
            }
        });

        public List<DetectorResult> GetScheduleAsync()
        {
            var items =  Collection.FindAll().ToList();
            return items;
        }
    }
}