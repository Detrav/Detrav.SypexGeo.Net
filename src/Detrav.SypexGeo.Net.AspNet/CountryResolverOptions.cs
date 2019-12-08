namespace Detrav.SypexGeo.Net.AspNet
{
    public interface ICountryResolverOptions
    {
        #region Public Properties

        /// <summary>
        /// Download when application is started
        /// </summary>
        bool DownloadOnStart { get; set; }

        /// <summary>
        /// Path to local database file
        /// </summary>
        string Path { get; set; }

        /// <summary>
        /// Url to database file
        /// </summary>
        string URL { get; set; }

        #endregion Public Properties
    }

    public class CountryResolverOptions : ICountryResolverOptions
    {
        #region Public Properties

        /// <summary>
        /// Download when application is started
        /// </summary>
        public bool DownloadOnStart { get; set; } = true;

        /// <summary>
        /// Path to local database file
        /// </summary>
        public string Path { get; set; } = "./Resources/SxGeo.dat";

        /// <summary>
        /// Url to database file
        /// </summary>
        public string URL { get; set; } = "https://sypexgeo.net/files/SxGeoCountry.zip";

        #endregion Public Properties
    }
}