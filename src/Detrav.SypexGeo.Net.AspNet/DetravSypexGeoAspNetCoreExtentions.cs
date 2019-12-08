using Detrav.SypexGeo.Net.AspNet;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

public static class DetravSypexGeoAspNetCoreExtentions
{
    #region Public Methods

    public static void AddSypexGeo(this IServiceCollection services, Action<CountryResolverOptions> configureOptions = null)
    {
        services.AddHttpContextAccessor();
        services.AddSingleton<ICountryResolverOptions, CountryResolverOptions>((serviceProvider) =>
        {
            CountryResolverOptions result = new CountryResolverOptions();
            configureOptions?.Invoke(result);
            return result;
        });
        services.AddSingleton<ISxGeoProvider, SxGeoProvider>();
    }

    public static string GetCountry(this HttpRequest request)
    {
        return request.HttpContext.GetCountry();
    }

    public static string GetCountry(this HttpContext context)
    {
        ISxGeoProvider provider = context.RequestServices.GetService<ISxGeoProvider>();
        ILogger log = context.RequestServices.GetService<ILogger>();
        try
        {
            return provider.GetCountry(context.Connection.RemoteIpAddress);
        }
        catch (Exception e)
        {
            if (log != null)
                log.LogError(e, "Can't resolve provider or ip address");
        }
        return "";
    }

    #endregion Public Methods
}