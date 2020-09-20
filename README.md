# Chromely.Mvc

Chromely.Mvc is an add-on to [Chromely](https://github.com/chromelyapps/Chromely) that allows you to write controllers using ASP.NET MVC convention.

# Demos 

https://github.com/RupertAvery/Chromely.Mvc.Demos

# Nuget

```
Install-Package Chromely.Mvc -version 5.0.0.6
```

# Known Issues

Routing is very basic as of now, and only supports simple routing without defaults.  This may change if there is demand. Also, pull requests are welcome!

# What's new

Updated to Chromely.Core 5.0.0.6 and Chromely 5.0.83

# Features
* Use MVC's convention-based approach to writing and wiring up controllers.
* Conventional routing using attributes `HttpGet`, `HttpPost`, etc.
* Constructor and property injection on controller classes.
* Return `Task<T>` from your action methods.
* Controller classes are transient services - created for each request.
* Use .NET Core's built-in `IServiceCollection` for dependency injection.
* Model binding, so you can create your action methods like this:

```csharp
public IEnumerable<WeatherData> GetWeatherForecast(DateTime date, string location)
```

instead of this:

```csharp
public ChromelyResponse GetWeatherForecast(ChromelyRequest request)
```

# Comparison

## Chromely

Chromely requires you to registers routes to specific methods. The method argument and return value _must_ be `ChromelyRequest` and `ChromelyReponse`.. This requires a lot of bolierplate code, and requires you to parse the request yourself.

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

Create a class that inherits from `ChromelyMvcBasicApp`. Override the `Configure` method, and register your services. You _must_ call `base.Configure(container)`. Call `container.AddControllers()` to register the controllers in your assembly. The controllers must inherit from `Chromely.Mvc.Controller`.

Controller actions will be added to the route as `{controller}/{action}`, where `{controller}` by convention is the controller class name with the word "Controller" removed.

So if you have a `DemoController` class with a `GetMovies` method, you can access it via the route `demo/getmovies`

```csharp
public class DemoChromelyApp : ChromelyMvcBasicApp
{
	public override void Configure(IServiceCollection container)
	{
		base.Configure(container);
		// Register any services that will be injected into the controllers, or other services
		container.AddTransient<IInfoService, InfoService>();
		container.AddTransient<IMovieService, MovieService>();
		// Register the controllers in the calling assembly
		container.AddControllers();
	}
}
```

In your `Program.cs` `Main` method, build and run your application using `MvcAppBuilder`.

```csharp
static void Main(string[] args)
{
	MvcAppBuilder
		.Create()
		.UseApp<DemoChromelyApp>()
		.Build()
		.Run(args);
}
```

# Configuration

You will need to add a `chromelyconfig.json` file in your project, and make sure it gets copied during build.

The most important setting is the `StartUrl.url`. The url in the example below tells chromely to load the `dist/index.html` file relative to the application path.  Set this to where your compiled website is located. If not specified, it will default to `app/chromely.html`

```json
{
  "appName": "chromely_demo",
  "startUrl": {
    "url": "local://dist/index.html",
    "loadType": "localResource"
  },
  "loadCefBinariesIfNotFound": true,
  "silentCefBinariesLoading": false,
  "windowLeft": 0,
  "windowTop": 0,
  "windowWidth": 1200,
  "windowHeight": 900,
  "windowNoResize": false,
  "windowNoMinMaxBoxes": false,
  "windowFrameless": false,
  "windowCenterScreen": true,
  "windowKioskMode": false,
  "windowState": "normal",
  "windowTitle": "chromely",
  "windowIconFile": "chromely.ico",
  "debuggingMode": true,
  "windowCustomCreation": {
    "windowStyles": 0,
    "windowExStyles": 0,
    "useCustomtyle": false
  },
  "urlSchemes": [
    {
      "name": "default-resource",
      "baseUrl": "",
      "scheme": "local",
      "host": "",
      "urlSchemeType": "resource",
      "baseUrlStrict": false
    },
    {
      "name": "default-custom-http",
      "baseUrl": "",
      "scheme": "http",
      "host": "chromely.com",
      "urlSchemeType": "custom",
      "baseUrlStrict": false
    },
    {
      "name": "default-command-http",
      "baseUrl": "",
      "scheme": "http",
      "host": "command.com",
      "urlSchemeType": "command",
      "baseUrlStrict": false
    },
    {
      "name": "chromely-site",
      "baseUrl": "https://github.com/chromelyapps/Chromely",
      "scheme": "",
      "host": "",
      "urlSchemeType": "external",
      "baseUrlStrict": true
    }
  ],
  //"controllerAssemblies": [
  //  "Chromely.External.Controllers.dll"
  //],
  "customSettings": [
    {
      "name": "cefLogFile",
      "value": "logs\\chromely.cef.log"
    },
    {
      "name": "logSeverity",
      "value": "info"
    },
    {
      "name": "locale",
      "value": "en-US"
    }
  ],
  "commandLineArgs": [
  ],
  "commandLineOptions": [
  ]
}
```


# Javascript

A `cefQuery` method will be attached to the `window` object.  You can use the following code as a service that you can call from your code using Promises.

```js
// chromely.service.js

export function get(url, parameters) {
    return new Promise(function (resolve, reject) {
        var request = {
            "method": "GET",
            "url": url,
            "parameters": parameters,
            "postData": null
        };

        messageRouterQuery(request, resolve, reject);
    });
}

export function post(url, parameters, postData) {
    return new Promise(function (resolve, reject) {
        var request = {
            "method": "POST",
            "url": url,
            "parameters": parameters,
            "postData": postData
        };

        messageRouterQuery(request, resolve, reject);
    });
}

function messageRouterQuery(request, success, error) {
    window.cefQuery({
        request: JSON.stringify(request),
        onSuccess: (response) => {
            var jsonData = JSON.parse(response);
            if (jsonData.ReadyState == 4 && jsonData.Status == 200) {
                if (success) success(jsonData.Data);
            } else {
                if (error) error(jsonData);
                console.log("Error" + jsonData);
            }
        },
        onFailure: (err, msg) => {
            if (error) error({ err, msg });
            console.log(err, msg);
        }
    });
}

```

then in your code:

```js
var chromelyService = require('./services/chromely.service');

...

chromelyService.get('/demo/getmovies', null)
	.then((data) => {
		this.movies = data;
	});
```

## RxJs/TypeScript



```ts
import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

// These cef definitions can be placed in a separate d.ts file
// cefQuery request
interface CefRequest {
  request: string;
  onSuccess: (response?: any) => void;
  onFailure: (error?: any, msg?: any) => void;
}

// add the cefQuery method to the global Window definition
declare global {
  interface Window {
    cefQuery(request: CefRequest);
  }
}

@Injectable()
export class ChromelyService {

  get<T>(url: string, parameters: any) {
    var request = {
      "method": "GET",
      "url": url,
      "parameters": parameters,
      "postData": null
    };

    return this.messageRouterQuery<T>(request);
  }

  post<T>(url: string, parameters: any, postData: any) {
    var request = {
      "method": "POST",
      "url": url,
      "parameters": parameters,
      "postData": postData
    };

    return this.messageRouterQuery<T>(request);
  }

  private messageRouterQuery<T>(request): Subject<T> {
    var subject = new Subject<T>();

    // window.cefQuery is injected by Chromely/Cef.
    window.cefQuery({
      request: JSON.stringify(request),
      onSuccess: (response) => {
        var jsonData = JSON.parse(response);
        if (jsonData.ReadyState === 4 && jsonData.Status === 200) {
          subject.next(jsonData.Data);
        } else {
          subject.error(jsonData);
          console.log("Error:" + jsonData);
        }
      },
      onFailure: (err, msg) => {
        subject.error({ err, msg });
        console.log(err, msg);
      }
    });

    return subject;
  }


}
```

# License

Chromely.Mvc is available under the **MIT License**. See Chromely.Mvc/LICENSE.txt
