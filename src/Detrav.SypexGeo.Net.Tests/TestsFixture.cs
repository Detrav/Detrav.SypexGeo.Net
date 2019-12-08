using System;
using System.IO;

namespace Detrav.SypexGeo.Net.Tests
{
    public class TestsFixture : IDisposable
    {
        #region Public Properties

        public SxGeo SxGeo { get; private set; }

        #endregion Public Properties

        #region Public Constructors

        public TestsFixture()
        {
            var dir = Path.GetFullPath(Environment.CurrentDirectory);
            this.SxGeo = new SxGeo(Path.Combine(ScanDir(dir), "SxGeo.dat"));
        }

        #endregion Public Constructors

        #region Public Methods

        public void Dispose()
        {
        }

        #endregion Public Methods

        #region Private Methods

        private string ScanDir(string dir, int count = 7)
        {
            string assets = Path.Combine(dir, "assets");
            if (Directory.Exists(assets))
                return assets;

            if (count < 0)
                throw new Exception("Cant find assets");
            return ScanDir(Path.Combine(dir, ".."), count - 1);
        }

        #endregion Private Methods
    }
}