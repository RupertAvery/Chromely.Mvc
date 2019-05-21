# Chromely.Mvc

Chromely.Mvc is an add-on to [Chromely](https://github.com/chromelyapps/Chromely) that allows you to create controllers using ASP.NET MVC conventions.

# Nuget

```
Install-Package Chromely.Mvc
```

# Known Issues

Versions 4.0.0.1 and below have an issue where the returned value from the `boundControllerAsync` response callback is a string instead of an object. See the [Javascript](#Javascript) section.


# Features
* Use MVC's convention-based approach to writing and wiring up controllers.
* Use .NET Core's built-in `IServiceCollection` for dependency injection.
* Constructor and property injection on controller classes.
* Controller classes are transient services - created for each request.
* Annotate Controller action methods with `HttpGet`, `HttpPost`, etc.
* Model binding, so you can create your action methods like this:

```csharp
public IEnumerable<WeatherData> GetWeatherForecast(DateTime date, string location)
```

# Comparison

## Chromely
```csharp
[ControllerProperty(Name = "WeatherController", Route = "weathercontroller")]
public class WeatherController : ChromelyController
{
	public WeatherController(){
            this.RegisterGetRequest("/weathercontroller/movies", this.GetMovies);
	}

	public ChromelyReponse GetWeatherForecast(ChromelyRequest request)
	{
		var parametersJsom = request.Parameters.EnsureJson();
		// parse json into arguments..
		...			
		var weatherForecast = new List<WeatherData>();
		... 
		ChromelyResponse response = new ChromelyResponse();
		response.Data = weatherForecast; 
		return response;
	}
}
```

## Chromely.Mvc

```csharp
public class WeatherController : Controller
{
	[HttpGet]
	public IEnumerable<WeatherData> GetWeatherForecast(DateTime date, string location)
	{
		var weatherForecast = new List<WeatherData>();
		... 
		return weatherForecast;
	}
}

```


# Usage

In your `Program.cs`, create your Chromely window using `ChromelyConfiguration` as usual, except for registering a JsHandler.

```csharp
var config = ChromelyConfiguration
	.Create()
	.WithAppArgs(args)
	.WithHostMode(WindowState.Normal, true)
	.WithHostTitle("delphi")
	.WithHostIconFile("chromely.ico")
	.WithStartUrl(startUrl)
	.WithLogFile("logs\\chromely.cef_new.log")
	.WithLogSeverity(LogSeverity.Info)
	.UseDefaultLogger()
	.UseDefaultResourceSchemeHandler("local", string.Empty)
	.UseDefaultHttpSchemeHandler("http", "chromely.com");
```

Then use `MvcConfigurationBuilder` to initialize the `IServiceCollection` and setup the MVC Services. 

```csharp
MvcConfigurationBuilder
	.Create()
	.AddChromelyMvcWithDefaultRoutes()
	.UseControllersFromAssembly(Assembly.GetExecutingAssembly())
	// Optional, register any additional services fluently
	.RegisterServices((serviceCollection) =>
	{
		serviceCollection.AddTransient<ITimesheetBL, TimesheetBL>();
		serviceCollection.AddSingleton<ILocalDataBL, LocalDataBL>();
		serviceCollection.AddSingleton<IOracleService, OracleService>();
	})
	.UseDefaultMvcBoundObject(config);


using (var window = ChromelyWindow.Create(config))
{
	// nothing else needed here, UseControllersFromAssembly registers controllers and 
	// UseDefaultMvcBoundObject() sets up the entry point for the controllers when requests come in
	return window.Run(args);
}
```

# Configuration Details

`MvcConfigurationBuilder.Create()` internally creates an `IServiceCollection`.

## Adding MVC Services and Routing 

`AddMvc` must be called and supplied with a route pattern, similar to mvc. This allows you to tailor your URLs.  This is very basic as of now and only supports `{controller}` and `{action}` tokens. 

```csharp
.AddChromelyMvc((routes) =>
{
	routes.MapRoute("default", "/{controller}/{action}");
})
```

Alternatively, you can use the provided shortcut:

```csharp
.AddChromelyMvcWithDefaultRoutes()
```

## Controller Registration 

`UseControllersFromAssembly` crawls the supplied assembly and registers any classes that inherit from `Chromwly.Mvc.Controller` on the ServiceCollection.

```csharp
.UseControllersFromAssembly(Assembly.GetExecutingAssembly())
```

The default controller factory uses MVC convention when resolving controllers, i.e. if the class name ends with `Controller`, the controller factory will use the class name without the `Controller` suffix.

e.g.

`WeatherForecastController` will resolve to `/weatherforecast`

## MVC ChromelyJSHandler

`UseDefaultMvcBoundObject` registers `MvcCefSharpBoundObject` as a `ChromelyJsHandler` with the supplied `ChromelyConfiguration`. It defaults to `boundControllerAsync` as the object name to bind, and async to `true`.

```csharp
.UseDefaultMvcBoundObject(config);
```

# Javascript

There are some slight differences over Chromely in the javascript you should use. The response is no longer wrapped in a `CallbackResponseStruct`, so just parse the object firectly.

Assuming you used the defaults of `boundControllerAsync` and async `true`, your http services script could look like this:


```js
function boundObjectGetJson(url, parameters, response) {
    boundControllerAsync.getJson(url, parameters, response);
}

function boundObjectPostJson(url, parameters, postData, response) {
    boundControllerAsync.postJson(url, parameters, postData, response);
}


export function boundObjectGet(url, parameters, callback) {
	boundObjectGetJson(url, parameters, response => {
		// version 4.0.0.1 returns a string
		// this is not needed in 4.0.0.2
		if (typeof response === 'string') {
			response = JSON.parse(response);
		}
		if (response.ReadyState == 4 && response.Status == 200) {
			callback(response.Data);
		} else {
			console.log("An error occurs during message routing. With ur:" + url + ". Response received:" + response);
		}
	});
}

export function boundObjectPost(url, parameters, postData, callback) {
	boundObjectPostJson(url, parameters, postData, response => {
		// version 4.0.0.1 returns a string
		// this is not needed in 4.0.0.2
		if (typeof response === 'string') {
			response = JSON.parse(response);
		}
		if (response.ReadyState == 4 && response.Status == 200) {
			callback(response.Data);
		} else {
			console.log("An error occurs during message routing. With ur:" + url + ". Response received:" + response);
		}
	});
}
```

# License

Chromely.Mvc is available under the **MIT License**. See Chromely.Mvc/LICENSE.txt