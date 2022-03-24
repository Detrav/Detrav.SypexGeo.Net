using System.IO;
using System.Net;
using Xunit;

namespace Detrav.SypexGeo.Net.Tests
{
    public class ReadDBTestPhp
    {
        #region Public Constructors

        public ReadDBTestPhp()
        {
            SxGeoDownloader downloader = new SxGeoDownloader("https://sypexgeo.net/files/SxGeoCountry.zip",
                "./Resources/SxGeo.dat");
            downloader.Download();
            downloader = new SxGeoDownloader("https://sypexgeo.net/files/SxGeoCity_utf8.zip",
                   "./Resources/SxGeoCity.dat");
            downloader.Download();
        }

        #endregion Public Constructors

        #region Public Methods

        [Fact]
        public void GetIpTest()
        {
            SxGeo sxGeo = new SxGeo();
            Assert.Equal("RU", sxGeo.GetCountry(IPAddress.Parse("95.24.17.92")));
            var countryid = sxGeo.GetCountryId(IPAddress.Parse("95.24.17.92"));
            var city = sxGeo.GetCity(IPAddress.Parse("95.24.17.92"));
            var cityFull = sxGeo.GetCityFull(IPAddress.Parse("95.24.17.92"));
            var num = sxGeo.GetNumber(IPAddress.Parse("95.24.17.92"));
            var about = sxGeo.About();
        }

        [Fact]
        public void GetIpTestFull()
        {
            SxGeo sxGeo = new SxGeo("./Resources/SxGeoCity.dat");
            Assert.Equal("RU", sxGeo.GetCountry(IPAddress.Parse("95.24.17.92")));
            var countryid = sxGeo.GetCountryId(IPAddress.Parse("95.24.17.92"));
            var city = sxGeo.GetCity(IPAddress.Parse("95.24.17.92"));
            var cityFull = sxGeo.GetCityFull(IPAddress.Parse("95.24.17.92"));
            var num = sxGeo.GetNumber(IPAddress.Parse("95.24.17.92"));
            var about = sxGeo.About();
        }

        [Fact]
        public void ReadHeaderCityTest()
        {
            SxGeo sxGeo = new SxGeo("./Resources/SxGeoCity.dat");

            //Assert.Equal("SxG", sxGeo.Header.Identifier);
        }

        [Fact]
        public void ReadHeaderTest()
        {
            SxGeo sxGeo = new SxGeo();

            //Assert.Equal("SxH", sxGeo.Header.Identifier);
        }

        [Fact]
        public void ReadFromStream()
        {
            var stream = File.OpenRead("./Resources/SxGeoCity.dat");
            SxGeo sxGeo = new SxGeo(stream);

            Assert.Equal("RU", sxGeo.GetCountry(IPAddress.Parse("95.24.17.92")));
            Assert.False(stream.CanRead);
        }

        [Fact]
        public void ReadFromStreamAndLeaveOpen()
        {
            var stream = File.OpenRead("./Resources/SxGeoCity.dat");
            SxGeo sxGeo = new SxGeo(stream, true);

            Assert.Equal("RU", sxGeo.GetCountry(IPAddress.Parse("95.24.17.92")));
            Assert.True(stream.CanRead);
        }

        #endregion Public Methods
    }
}