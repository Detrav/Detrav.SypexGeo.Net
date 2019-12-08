using System.IO;
using Xunit;

namespace Detrav.SypexGeo.Net.Tests
{
    public class DownloaderTest
    {
        #region Public Methods

        [Fact]
        public void TryToDownloadAllFiles()
        {
            foreach (var file in SxGeoDownloader.SypexGeoDatabases)
            {
                SxGeoDownloader downloader = new SxGeoDownloader(file, "./Resources/SxGeo_tmp.dat", true);
                downloader.Download();
            }
            File.Delete("./Resources/SxGeo_tmp.dat");
        }

        #endregion Public Methods
    }
}