using System;
using System.Threading.Tasks;
using global::CefSharp;
using Newtonsoft.Json;

namespace Chromely.Mvc
{

    /// <summary>
    /// The class that is bound to a javascript object through Cef
    /// </summary>
    public class MvcCefSharpBoundObject
    {
        private readonly IRequestHandler requestTaskRunner;

        public MvcCefSharpBoundObject(IRequestHandler requestTaskRunner)
        {
            this.requestTaskRunner = requestTaskRunner;
        }

        public void GetJson(string url, object parameters, IJavascriptCallback javascriptCallback)
        {
            Task.Run(async () =>
            {
                using (javascriptCallback)
                {
                    try
                    {
                        var data = await requestTaskRunner.RunAsync(Methods.Get, url, parameters, null);

                        var response = new Response()
                        {
                            Data = data,
                            ReadyState = 4,
                            Status = 200,
                        };

                        var jsonResponse = JsonConvert.SerializeObject(response);

                        await javascriptCallback.ExecuteAsync(jsonResponse);
                    }
                    catch (Exception ex)
                    {
                        await HandleExceptionAsync(ex, javascriptCallback);
                    }
                }
            });
        }

        public string GetJson(string url, object parameters)
        {
            try
            {
                var data = requestTaskRunner.Run(Methods.Get, url, parameters, null);

                var response = new Response()
                {
                    Data = data,
                    ReadyState = 4,
                    Status = 200,
                };

                var jsonResponse = JsonConvert.SerializeObject(response);

                return jsonResponse;
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        public void PostJson(string url, object parameters, object postData, IJavascriptCallback javascriptCallback)
        {
            Task.Run(async () =>
            {
                using (javascriptCallback)
                {
                    try
                    {
                        var data = await requestTaskRunner.RunAsync(Methods.Post, url, parameters, postData);

                        var response = new Response()
                        {
                            Data = data,
                            ReadyState = 4,
                            Status = 200,
                        };

                        var jsonResponse = JsonConvert.SerializeObject(response);

                        await javascriptCallback.ExecuteAsync(jsonResponse);
                    }
                    catch (Exception ex)
                    {
                        await HandleExceptionAsync(ex, javascriptCallback);
                    }
                }
            });
        }

        public string PostJson(string url, object parameters, object postData)
        {
            try
            {
                var data = requestTaskRunner.Run(Methods.Post, url, parameters, postData);

                var response = new Response()
                {
                    Data = data,
                    ReadyState = 4,
                    Status = 200,
                };

                var jsonResponse = JsonConvert.SerializeObject(response);

                return jsonResponse;
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }



        private async Task HandleExceptionAsync(Exception ex, IJavascriptCallback javascriptCallback)
        {
            var response = new Response()
            {
                Data = ex.Message,
                ReadyState = 4,
                Status = 500,
            };
            var jsonResponse = JsonConvert.SerializeObject(response);

            await javascriptCallback.ExecuteAsync(jsonResponse);
        }

        private string HandleException(Exception ex)
        {
            var response = new Response()
            {
                Data = ex.Message,
                ReadyState = 4,
                Status = 500,
            };

            if (ex is RouteException)
            {
                response.Status = 404;
            }

            var jsonResponse = JsonConvert.SerializeObject(response);

            return jsonResponse;
        }

    }
}
