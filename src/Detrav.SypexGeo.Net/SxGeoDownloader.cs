using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;

namespace Detrav.SypexGeo.Net
{
    public class SxGeoDownloader
    {
        #region Public Properties

        public static List<string> SypexGeoDatabases { get; } = new List<string>()
        {
            "https://sypexgeo.net/files/SxGeoCountry.zip",
            "https://sypexgeo.net/files/SxGeoCity_utf8.zip",
            "https://raw.githubusercontent.com/Detrav/Detrav.SypexGeo.Net/master/assets/SxGeo.dat"
        };

        /// <summary>
        /// Indicates that need to replace target file
        /// </summary>
        public bool ForceUpade { get; set; }

        /// <summary>
        /// Local path
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Throw on get error
        /// </summary>
        public bool ThrowOnError { get; set; }

        /// <summary>
        /// The url to zip or dat file with database, if url is null, try to download from SypexGeoDatabases list
        /// </summary>
        public string Url { get; set; }

        #endregion Public Properties

        #region Public Constructors

        /// <summary>
        /// Initialize a new instance
        /// </summary>
        /// <param name="url">The url to zip or dat file with database, if url is null, try to download from SypexGeoDatabases list</param>
        /// <param name="path">Local path</param>
        /// <param name="forceUpdate">Indicates that need to replace target file</param>
        /// <param name="throwOnError">throw on get error</param>
        public SxGeoDownloader(string url = null, string path = "./Resources/SxGeo.dat", bool forceUpdate = false,
            bool throwOnError = false
            )
        {
            this.Url = url;
            this.Path = path;
            this.ForceUpade = forceUpdate;
            this.ThrowOnError = throwOnError;
        }

        #endregion Public Constructors

        #region Public Methods

        public bool Download()
        {
            if (Url != null)
                return Download(Url);
            else
            {
                foreach (var url in SypexGeoDatabases)
                {
                    bool result = Download(url);
                    if (result)
                        return true;
                }
            }

            if (ThrowOnError)
                throw new Exception("Can't download data file");
            return false;
        }

        #endregion Public Methods

        #region Private Methods

        private bool Download(string url)
        {
            try
            {
                string fullPath = System.IO.Path.GetFullPath(Path);
                string directory = System.IO.Path.GetDirectoryName(fullPath);
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);
                using (WebClient client = new WebClient())
                {
                    byte[] dataFile = client.DownloadData(url);

                    if (dataFile[0] == 'P' && dataFile[1] == 'K')
                    {
                        using (MemoryStream ms = new MemoryStream(dataFile))
                        using (var zip = new ZipArchive(ms))
                        {
                            using (Stream s = zip.Entries[0].Open())
                            {
                                if (File.Exists(fullPath))
                                    File.Delete(fullPath);
                                using (var file = File.Open(fullPath, FileMode.Create))
                                {
                                    s.CopyTo(file);
                                }
                            }
                        }
                    }
                    else if (dataFile[0] == 'S' && dataFile[0] == 'x' && dataFile[0] == 'G')
                    {
                        if (File.Exists(fullPath))
                            File.Delete(fullPath);
                        File.WriteAllBytes(Path, dataFile);
                    }

                    return true;
                }
            }
            catch (Exception e)
            {
                if (ThrowOnError)
                    throw new Exception("Can't download data file", e);
                return false;
            }
        }

        #endregion Private Methods
    }
}