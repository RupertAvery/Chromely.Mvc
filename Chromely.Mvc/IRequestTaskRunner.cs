using System.Threading.Tasks;
using CefSharp;
using Chromely.Core.RestfulService;

namespace Chromely.Mvc
{
    public interface IRequestHandler
    {
        object Run(string method, string url, object parameters, object postData);
        Task<object> RunAsync(string method, string url, object parameters, object postData);
    }
}