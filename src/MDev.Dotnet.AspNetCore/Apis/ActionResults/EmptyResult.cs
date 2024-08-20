
namespace Modules.API.Core.MDev.Dotnet.AspNetCore.Apis.ActionResults
{
    public class EmptyResult : IAsyncResult
    {
        public EmptyResult()
        {
        }

        public bool IsCompleted
        {
            get { return true; }
        }

        public WaitHandle AsyncWaitHandle
        {
            get { throw new NotImplementedException(); }
        }

        public object AsyncState
        {
            get { return true; }
        }

        public bool CompletedSynchronously
        {
            get { return true; }
        }
    }
}
