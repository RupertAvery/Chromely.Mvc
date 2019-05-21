using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using global::CefSharp;

namespace Chromely.Mvc
{
    public class DefaultRequestHandler : IRequestHandler
    {
        private IActionBuilder _actionBuilder;
        private IModelBinder _modelBinder;

        public DefaultRequestHandler(IActionBuilder actionBuilder, IModelBinder modelBinder)
        {
            _actionBuilder = actionBuilder;
            _modelBinder = modelBinder;
        }

        public Task<object> RunAsync(string method, string url, object parameters, object postData)
        {
            return ExcuteRouteAsync(method, url, parameters, postData);
        }

        public object Run(string method, string url, object parameters, object postData)
        {
            return ExcuteRoute(method, url, parameters, postData);
        }

        private RequestContext GetRequestContext(string method, string requestUrl)
        {
            var querySplitIndex = requestUrl.IndexOf('?');
            var url = "";
            List<KeyValuePair<string, string>> queryParameters = null;

            if (querySplitIndex == -1)
            {
                url = requestUrl;
            }
            else
            {
                url = requestUrl.Substring(0, querySplitIndex);
                queryParameters = requestUrl
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
            };
        }

        private object ExcuteRoute(string method, string url, object parameters, object postData)
        {
            var requestContext = GetRequestContext(method, url);
            var action = _actionBuilder.BuildAction(requestContext);

            var arguments = BindParameters(action, requestContext, parameters, postData);

            return action.Invoke(arguments);
        }

        private async Task<object> ExcuteRouteAsync(string method, string url, object parameters, object postData)
        {
            var requestContext = GetRequestContext(method, url);
            var action = _actionBuilder.BuildAction(requestContext);

            var arguments = BindParameters(action, requestContext, parameters, postData);

            object result;

            if (action.IsAsync)
            {
                var asyncObject = (Task)action.Invoke(arguments);
                var taskType = asyncObject.GetType();
                await asyncObject.ConfigureAwait(false);
                var resultProperty = taskType.GetProperty("Result");
                result = resultProperty.GetValue(asyncObject);
            }
            else
            {
                result = action.Invoke(arguments);
            }

            return result;
        }

        public object[] BindParameters(MvcAction action, RequestContext requestContext, object parameters, object postData)
        {
            var methodInfo = action.ActionContext.ActionMethod;
            var arguments = new List<object>();

            var actionParameters = methodInfo.GetParameters();

            var isPost = action.ActionContext.Request.Method == Methods.Post;

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

            if (isPost)
            {
                if (postData is ExpandoObject || postData is List<ExpandoObject> || postData is List<Object>)
                {
                    if (actionParameters.Length == 1)
                    {
                        arguments.Add(_modelBinder.BindToModel(actionParameters[0].ParameterType, postData));
                    }
                    else
                    {
                        throw new NotImplementedException("Does not support POST with multiple parameters");
                    }
                }
                else
                {
                    throw new NotImplementedException("Unsupported type");
                }
            }
            else
            {


                if (parameters is ExpandoObject)
                {

                    var paramlookup = (IDictionary<string, object>)parameters;


                    foreach (var parameter in actionParameters.OrderBy(x => x.Position))
                    {
                        var boundValue = _modelBinder.GetBoundValue(parameter.ParameterType, parameter.Name, paramlookup);
                        arguments.Add(boundValue);
                    }
                }
                else
                {
                    if (parameters != null)
                    {
                        throw new NotImplementedException("Unsupported type");
                    }
                }
            }

            return arguments.ToArray();
        }

        private static string GetPostData(IRequest request)
        {
            var elements = request?.PostData?.Elements;
            if (elements == null || (elements.Count == 0))
            {
                return string.Empty;
            }

            var dataElement = elements[0];
            return dataElement.GetBody();
        }
    }
}