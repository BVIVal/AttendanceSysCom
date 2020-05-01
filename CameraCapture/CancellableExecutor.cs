using System.Threading;

namespace CameraCapture
{
    public class CancellableExecutor
    {
        #region Fields

        CancellationTokenSource _cts = new CancellationTokenSource();
        #endregion

        #region Properties

        public CancellationToken Token { get; private set; }
        #endregion

        public CancellableExecutor()
        {
            Token = _cts.Token;
        }

        //public Task Execute(Action<CancellationToken> action)
        //{
        //    return Task.Run(() => action(Token), Token);
        //}

        public void Cancel()
        {
            _cts.Cancel();
            _cts.Dispose();
            
        }

        public void Restart()
        {
            _cts = new CancellationTokenSource();
            Token = _cts.Token;
        }

        public bool IsCancellationRequested()
        {
            return Token.IsCancellationRequested;
        }
        
    }
}