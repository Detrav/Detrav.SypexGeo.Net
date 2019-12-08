using System;
using System.Collections.Generic;
using System.Net;

namespace Detrav.SypexGeo.Net
{
    public partial class SxGeo
    {
        #region Public Methods

        /// <summary>
        /// Returns information about database
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, object> About()
        {
            var charset = new string[] { "utf-8", "latin1", "cp1251" };
            var types = new string[] { "n/a", "SxGeo Country", "SxGeo City RU", "SxGeo City EN", "SxGeo City", "SxGeo City Max RU", "SxGeo City Max EN", "SxGeo City Max" };
            return new Dictionary<string, object>() {
                { "Created" , DateTimeOffset.FromUnixTimeSeconds(time) },
                { "Timestamp" , time },
                { "Charset",  charset[this.charset] },
                { "Type", types[parserType] },
                { "Byte Index" , b_idx_len },
                {  "Main Index", m_idx_len },
                {  "Blocks In Index Item" , range },
                {  "IP Blocks" , db_items },
                {  "Block Size" , block_len },
                { "City", new Dictionary<string, object> () {
                    { "Max Length", max_city },
                    { "Total Size" , info["city_size"] }
                } },
                { "Region", new Dictionary<string, object>() {
                    { "Max Length", max_region },
                    { "Total Size", info["region_size"] },
                } },
                { "Country", new Dictionary<string, object>() {
                    { "Max Length", max_country },
                    { "Total Size" , info["country_size"] }
                } }
            };
        }

        /// <summary>
        /// Returns key value object or ISO for ip address
        /// </summary>
        /// <param name="ip">ip4</param>
        /// <returns></returns>
        public object Get(IPAddress ip)
        {
            if (max_city > 0)
                return GetCity(ip);
            return GetCountry(ip);
        }

        /// <summary>
        /// Returns key value object or ISO for ip address
        /// </summary>
        /// <param name="ip">ip4</param>
        /// <returns></returns>
        public object Get(string ip)
        {
            return Get(IPAddress.Parse(ip));
        }

        /// <summary>
        /// Returns key value object with city description
        /// </summary>
        /// <param name="ip">ip4</param>
        /// <returns></returns>
        public Dictionary<string, object> GetCity(IPAddress ip)
        {
            long seek = GetNumber(ip.GetAddressBytes().AsSpan());
            return seek > 0 ? ParseCity(seek) : null;
        }

        /// <summary>
        /// Returns key value object with city description
        /// </summary>
        /// <param name="ip">ip4</param>
        /// <returns></returns>
        public Dictionary<string, object> GetCity(string ip)
        {
            return GetCity(IPAddress.Parse(ip));
        }

        /// <summary>
        /// Returns key value object with full city description
        /// </summary>
        /// <param name="ip">ip4</param>
        /// <returns></returns>
        public Dictionary<string, object> GetCityFull(IPAddress ip)
        {
            long seek = GetNumber(ip.GetAddressBytes().AsSpan());
            return seek > 0 ? ParseCity(seek, true) : null;
        }

        /// <summary>
        /// Returns key value object with full city description
        /// </summary>
        /// <param name="ip">ip4</param>
        /// <returns></returns>
        public Dictionary<string, object> GetCityFull(string ip)
        {
            return GetCityFull(IPAddress.Parse(ip));
        }

        /// <summary>
        /// Returns country ISO for ip address
        /// </summary>
        /// <param name="ip">ip4</param>
        /// <returns></returns>
        public string GetCountry(IPAddress ip)
        {
            if (max_city > 0)
            {
                var tmp = ParseCity(GetNumber(ip.GetAddressBytes().AsSpan()));
                return (tmp["country"] as Dictionary<string, object>)["iso"].ToString();
            }
            else
                return ID_2_ISO[GetNumber(ip.GetAddressBytes().AsSpan())];
        }

        /// <summary>
        /// Returns country ISO for ip address
        /// </summary>
        /// <param name="ip">ip4</param>
        /// <returns></returns>
        public string GetCountry(string ip)
        {
            return GetCountry(IPAddress.Parse(ip));
        }

        /// <summary>
        /// Returns Country identitifier
        /// </summary>
        /// <param name="ip">ip4</param>
        /// <returns></returns>
        public long GetCountryId(IPAddress ip)
        {
            if (max_city > 0)
            {
                var tmp = ParseCity(GetNumber(ip.GetAddressBytes().AsSpan()));
                return Convert.ToInt64((tmp["country"] as Dictionary<string, object>)["id"]);
            }
            else
                return GetNumber(ip.GetAddressBytes().AsSpan());
        }

        /// <summary>
        /// Returns Country identitifier
        /// </summary>
        /// <param name="ip">ip4</param>
        /// <returns></returns>
        public long GetCountryId(string ip)
        {
            return GetCountryId(IPAddress.Parse(ip));
        }

        /// <summary>
        /// Returns index in the data base. This function is public? Why?
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public long GetNumber(IPAddress ip)
        {
            return GetNumber(ip.GetAddressBytes().AsSpan());
        }

        #endregion Public Methods
    }
}