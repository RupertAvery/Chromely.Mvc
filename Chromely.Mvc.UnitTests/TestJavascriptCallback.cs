using System.Threading.Tasks;
using CefSharp;

namespace Tests
{
    public class TestJavascriptCallback : IJavascriptCallback
    {
        private TaskCompletionSource<object[]> _responsePromise;

        public TestJavascriptCallback()
        {
            _responsePromise = new TaskCompletionSource<object[]>();
        }

        public Task<object[]> ResultTask
        {
            get { return _responsePromise.Task; }
        }

        public void Dispose()
        {
        }

        public Task<JavascriptResponse> ExecuteAsync(params object[] parms)
        {
            _responsePromise.TrySetResult(parms);
            return new Task<JavascriptResponse>(() => new JavascriptResponse());
        }

        public long Id { get; }
        public bool CanExecute { get; }
        public bool IsDisposed { get; }
    }
}