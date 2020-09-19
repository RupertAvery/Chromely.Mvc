using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Chromely.Core.Network;
using global::CefSharp;

namespace Chromely.Mvc
{
    /// <summary>
    /// A request task runner that performs ASP.NET MVC-style model binding, so the Controller Action arguments do not need to be of type <see cref="ChromelyRequest"/>
    /// </summary>
    public class DefaultMvcRequestTaskRunner : IChromelyRequestTaskRunner
    {
        private readonly IActionBuilder _actionBuilder;
        private readonly IModelBinder _modelBinder;

        public DefaultMvcRequestTaskRunner(IActionBuilder actionBuilder, IModelBinder modelBinder)
        {
            _actionBuilder = actionBuilder;
            _modelBinder = modelBinder;
        }

        public ChromelyResponse Run(ChromelyRequest request)
        {
            var parameters = request.Parameters ?? request.RoutePath.Path.GetParameters()?.ToObjectDictionary();
            var postData = request.PostData;

            return ExcuteRoute(request.RoutePath, parameters, postData);
        }

        public Task<ChromelyResponse> RunAsync(ChromelyRequest request)
        {
            var parameters = request.Parameters ?? request.RoutePath.Path.GetParameters()?.ToObjectDictionary();
            var postData = request.PostData;

            return ExcuteRouteAsync(request.RoutePath, parameters, postData);
        }

        public ChromelyResponse Run(string requestId, RoutePath routePath, IDictionary<string, string> parameters, object postData, string requestData)
        {
            if (routePath == null || string.IsNullOrWhiteSpace(routePath?.Path))
            {
                return GetBadRequestResponse(null);
            }

            return ExcuteRoute(routePath, parameters, postData, requestId);
        }


        public Task<ChromelyResponse> RunAsync(string requestId, RoutePath routePath, IDictionary<string, string> parameters, object postData, string requestData)
        {
            if (routePath == null || string.IsNullOrWhiteSpace(routePath?.Path))
            {
                return Task.FromResult(GetBadRequestResponse(null));
            }

            return ExcuteRouteAsync(routePath, parameters, postData, requestId);
        }

        public Task<ChromelyResponse> RunAsync(string method, string path, IDictionary<string, string> parameters, object postData)
        {
            var routePath = new RoutePath(method, path);
            if (string.IsNullOrWhiteSpace(routePath?.Path))
            {
                return Task.FromResult(GetBadRequestResponse(null));
            }

            return ExcuteRouteAsync(routePath, parameters, postData);
        }

        public ChromelyResponse Run(string method, string path, IDictionary<string, string> parameters, object postData)
        {
            var routePath = new RoutePath(method, path);
            if (string.IsNullOrWhiteSpace(routePath?.Path))
            {
                return GetBadRequestResponse(null);
            }

            return ExcuteRoute(routePath, parameters, postData);
        }

        private RequestContext GetRequestContext(Method method, string path, string requestId = null)
        {
            var querySplitIndex = path.IndexOf('?');
            var url = "";
            List<KeyValuePair<string, string>> queryParameters = null;

            if (querySplitIndex == -1)
            {
                url = path;
            }
            else
            {
                url = path.Substring(0, querySplitIndex);
                queryParameters = path
                    .Substring(querySplitIndex + 1)
                    .Split(new[] { '&' })
                    .Select(x =>
                    {
                        var pairSplitIndex = x.IndexOf('=');
                        var key = "";
                        var value = "";

                        if (pairSplitIndex > 0)
                        {
                            key = x.Substring(0, pairSplitIndex);
                            value = x.Substring(pairSplitIndex + 1);
                            value = value.Replace("+", " ");
                            //TODO: UrlDecode
                        }
                        else if (pairSplitIndex < 0)
                        {
                            key = x;
                        }
                        else
                        {
                            key = x.Substring(0, pairSplitIndex);
                        }

                        return new KeyValuePair<string, string>(key, value);
                    })
                    .ToList();
            }

            return new RequestContext()
            {
                Method = method,
                Url = url,
                QueryParameters = queryParameters,
                RequestId = requestId
            };
        }

        private ChromelyResponse ExcuteRoute(RoutePath routePath, IDictionary<string, string> parameters, object postData, string requestId = null)
        {

            object result = null;
            var status = 200;
            var statusText = "OK";

            try
            {
                var requestContext = GetRequestContext(routePath.Method, routePath.Path, requestId);
                var action = _actionBuilder.BuildAction(requestContext);

                var arguments = BindParameters(action, requestContext, parameters, postData);

                result = action.Invoke(arguments);
            }
            catch (RouteException re)
            {
                result = re.Message;
                status = 404;
                statusText = "Not Found";
            }
            catch (Exception e)
            {
                result = e.Message;
                status = 500;
                statusText = "Server Error";
            }

            return new ChromelyResponse()
            {
                ReadyState = (int)ReadyState.ResponseIsReady,
                Status = status,
                StatusText = (string.IsNullOrWhiteSpace(statusText) && (status == (int)HttpStatusCode.OK))
                    ? "OK"
                    : statusText,
                Data = result
            };
        }

        private async Task<ChromelyResponse> ExcuteRouteAsync(RoutePath routePath, object parameters, object postData, string requestId = null)
        {
            object result = null;
            var status = 200;
            var statusText = "OK";

            try
            {
                var requestContext = GetRequestContext(routePath.Method, routePath.Path, requestId);
                var action = _actionBuilder.BuildAction(requestContext);

                var arguments = BindParameters(action, requestContext, parameters, postData);

                if (action.IsAsync)
                {
                    result = await action.InvokeAsync(arguments);
                    if (result != null)
                    {
                        var resultType = result.GetType();
                        if (resultType.Name == "VoidTaskResult")
                        {
                            result = null;
                        }
                    }
                }
                else
                {
                    result = action.Invoke(arguments);
                }
            }
            catch (Exception e)
            {
                result = e.Message;
                status = 500;
                statusText = "Server Error";
            }

            return new ChromelyResponse()
            {
                ReadyState = (int)ReadyState.ResponseIsReady,
                Status = status,
                StatusText = (string.IsNullOrWhiteSpace(statusText) && (status == (int)HttpStatusCode.OK))
                    ? "OK"
                    : statusText,
                Data = result
            };
        }

        /// <summary>
        /// Returns a list of objects mapped to the values in the parameters or postdata in the order of the matched actions's arguments.
        /// Objects will be parsed into the types declared by the method arguments
        /// </summary>
        /// <param name="action">The controller action that matches the route</param>
        /// <param name="requestContext">The request context</param>
        /// <param name="parameters">A collection of key value pairs</param>
        /// <param name="postData">The data posted by Cef from the browser</param>
        /// <returns></returns>
        public object[] BindParameters(MvcAction action, RequestContext requestContext, object parameters, object postData)
        {
            var methodInfo = action.ActionContext.ActionMethod;
            var arguments = new List<object>();

            var actionParameters = methodInfo.GetParameters();

            // TODO: Handle case where same parameter appears in the querystring and post data
            
            if (requestContext.QueryParameters != null)
            {
                // TODO: handle multiple values for one key
                var queryParameters = requestContext.QueryParameters.ToDictionary(x => x.Key, x => (object)x.Value);
                foreach (var parameter in actionParameters.OrderBy(x => x.Position))
                {
                    var boundValue = _modelBinder.GetBoundValue(parameter.ParameterType, parameter.Name, queryParameters);
                    arguments.Add(boundValue);
                }
            }

            if (parameters != null)
            {
                var paramlookup = (IDictionary<string, object>)((IDictionary<string, string>)parameters).ToDictionary(x => x.Key, x => (object)x.Value);


                foreach (var parameter in actionParameters.OrderBy(x => x.Position))
                {
                    var boundValue = _modelBinder.GetBoundValue(parameter.ParameterType, parameter.Name, paramlookup);
                    arguments.Add(boundValue);
                }
            }

            if (postData != null)
            {
                if (actionParameters.Length == 1)
                {
                    switch (postData)
                    {
                        case JsonElement data:
                            arguments.Add(_modelBinder.BindToModel(actionParameters[0].ParameterType, data));
                            break;
                        case ExpandoObject _:
                        case List<ExpandoObject> _:
                        case List<object> _:
                            arguments.Add(_modelBinder.BindToModel(actionParameters[0].ParameterType, postData));
                            break;
                        default:
                            throw new Exception($"Unsupported type: {postData.GetType()}");
                    }
                }
                else
                {
                    throw new Exception("POST with zero or multiple parameters is not supported");
                }
            }


            return arguments.ToArray();
        }

        private ChromelyResponse GetBadRequestResponse(string requestId)
        {
            return new ChromelyResponse
            {
                RequestId = requestId,
                ReadyState = (int)ReadyState.ResponseIsReady,
                Status = (int)System.Net.HttpStatusCode.BadRequest,
                StatusText = "Bad Request"
            };
        }

    }
}