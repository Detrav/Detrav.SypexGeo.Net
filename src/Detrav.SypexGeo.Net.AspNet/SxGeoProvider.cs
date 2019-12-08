using System;
using System.IO;
using System.Net;

namespace Detrav.SypexGeo.Net.AspNet
{
    public interface ISxGeoProvider
    {
        #region Public Methods

        /// <summary>
        /// Returns country by ip4
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        string GetCountry(string ip);

        /// <summary>
        /// Returns country by ip4
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        string GetCountry(IPAddress ip);

        #endregion Public Methods
    }

    public class SxGeoProvider : ISxGeoProvider
    {
        #region Private Fields

        private readonly SxGeo sxGeo;

        #endregion Private Fields

        #region Public Properties

        public SxGeo SxGeo => sxGeo;

        #endregion Public Properties

        #region Public Constructors

        public SxGeoProvider(ICountryResolverOptions options)
        {
            if (options.DownloadOnStart || !File.Exists(options.Path))
            {
                SxGeoDownloader downloader = new SxGeoDownloader(options.URL, options.Path, true);
                if (!downloader.Download())
                {
                    throw new FileNotFoundException("Can't download database!");
                }
            }

            sxGeo = new SxGeo(options.Path);
        }

        #endregion Public Constructors

        #region Public Methods

        public string GetCountry(string ip)
        {
            return GetCountry(IPAddress.Parse(ip));
        }

        public string GetCountry(IPAddress ip)
        {
            if (ip.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork)
                throw new NotSupportedException("" + ip.AddressFamily + " is not supported!");
            return sxGeo.GetCountry(ip);
        }

        #endregion Public Methods
    }
}