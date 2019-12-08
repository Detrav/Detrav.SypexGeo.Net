# Detrav.SypexGeo.Net

Unofficial Sypex Geo for `.NET Core` and `ASP.NET Core`.
The library for receiving information by ip address.

The version works only with:

- SypexGeo 2.2!

## How to use

### For `.NET Core`:

Install the packages using dotnet.

```bash
dotnet add package Detrav.SypexGeo.Net
```

```csharp
// Download database to ./Resources/SxGeo.dat
SxGeoDownloader loader = new SxGeoDownloader();
loader.Download();

// Parse database from ./Resources/SxGeo.dat
SxGeo sxGeo = new SxGeo();

// Get country iso
string iso1 = sxGeo.GetCountry("76.83.228.248"); 
string iso2 = sxGeo.GetCountry("169.150.187.100");

// iso1 = "US"; iso2 == "US";
```

### For `ASP.NET Core`:

Install the packages using dotnet.

```bash
dotnet add package Detrav.SypexGeo.Net
dotnet add package Detrav.SypexGeo.Net.AspNet
```

The package `Detrav.SypexGeo.Net` is optional for `ASP.NET Core`.

Add the following lines to `StartUp.cs`.

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // ...
    services.AddSypexGeo();
    // ...
}
```

Now you is able to get iso.

```csharp
// iso1 is the country from the request
string iso1 = HttpContext.GetCountry();
```

Another way.

```csharp
public class HomeController : Controller
{
    private readonly ISxGeoProvider sxGeoProvider;
    
    public HomeController(ISxGeoProvider sxGeoProvider)
    {
        this.sxGeoProvider = sxGeoProvider;
    }

    public IActionResult Index()
    {
        var IpAddress = Context.Connection.RemoteIpAddress;

        // call provider method
        string iso1 = sxGeoProvider.GetCountry(IpAddress);
        // or call database
        string iso2 = sxGeoProvider.SxGeo.GetCountry(IpAddress);
        
        return View();
    }
}
```

If your application is behind Reverse Proxy, you have to use `app.UseForwardedHeaders()` middleware.

```csharp
public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
    // ....
    services.Configure<ForwardedHeadersOptions>(options =>
    {
        // some times is needed to add KnownProxies
        // options.KnownProxies.Add(IPAddress.Parse("172.16.0.1"));
        options.ForwardedHeaders =
            ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    });
    // ....
}

public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
    app.UseForwardedHeaders();
}
```

## Demo

No live demo, see source: <https://github.com/Detrav/Detrav.SypexGeo.Net/tree/master/src/Demo>.

## Notes

* This is not an official datasheet library for <https://sypexgeo.net>.
* I accept pull requests, so feel free to make.

## License

Copyright by Detrav / Witaly Ezepchuk / Vitaliy Ezepchuk.

Licensed under the MIT license, see license text in LICENSE file.
