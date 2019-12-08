using Detrav.SypexGeo.Net.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Detrav.SypexGeo.Net
{
    /// <summary>
    /// Unofficial Sypex Geo for .Net CORE and ASP.Net Core.
    /// </summary>
    public partial class SxGeo
    {
        #region Private Fields

        private readonly byte b_idx_len;
        private readonly byte[] b_idx_str;
        private readonly int block_len;
        private readonly byte charset;
        private readonly long cities_begin;
        private readonly byte[] cities_db;
        private readonly uint country_size;
        private readonly byte[] db;
        private readonly long db_begin;
        private readonly uint db_items;
        private readonly string[] ID_2_ISO = new string[] {
        "", "AP", "EU", "AD", "AE", "AF", "AG", "AI", "AL", "AM", "CW", "AO", "AQ", "AR", "AS", "AT", "AU",
        "AW", "AZ", "BA", "BB", "BD", "BE", "BF", "BG", "BH", "BI", "BJ", "BM", "BN", "BO", "BR", "BS",
        "BT", "BV", "BW", "BY", "BZ", "CA", "CC", "CD", "CF", "CG", "CH", "CI", "CK", "CL", "CM", "CN",
        "CO", "CR", "CU", "CV", "CX", "CY", "CZ", "DE", "DJ", "DK", "DM", "DO", "DZ", "EC", "EE", "EG",
        "EH", "ER", "ES", "ET", "FI", "FJ", "FK", "FM", "FO", "FR", "SX", "GA", "GB", "GD", "GE", "GF",
        "GH", "GI", "GL", "GM", "GN", "GP", "GQ", "GR", "GS", "GT", "GU", "GW", "GY", "HK", "HM", "HN",
        "HR", "HT", "HU", "ID", "IE", "IL", "IN", "IO", "IQ", "IR", "IS", "IT", "JM", "JO", "JP", "KE",
        "KG", "KH", "KI", "KM", "KN", "KP", "KR", "KW", "KY", "KZ", "LA", "LB", "LC", "LI", "LK", "LR",
        "LS", "LT", "LU", "LV", "LY", "MA", "MC", "MD", "MG", "MH", "MK", "ML", "MM", "MN", "MO", "MP",
        "MQ", "MR", "MS", "MT", "MU", "MV", "MW", "MX", "MY", "MZ", "NA", "NC", "NE", "NF", "NG", "NI",
        "NL", "NO", "NP", "NR", "NU", "NZ", "OM", "PA", "PE", "PF", "PG", "PH", "PK", "PL", "PM", "PN",
        "PR", "PS", "PT", "PW", "PY", "QA", "RE", "RO", "RU", "RW", "SA", "SB", "SC", "SD", "SE", "SG",
        "SH", "SI", "SJ", "SK", "SL", "SM", "SN", "SO", "SR", "ST", "SV", "SY", "SZ", "TC", "TD", "TF",
        "TG", "TH", "TJ", "TK", "TM", "TN", "TO", "TL", "TR", "TT", "TV", "TW", "TZ", "UA", "UG", "UM",
        "US", "UY", "UZ", "VA", "VC", "VE", "VG", "VI", "VN", "VU", "WF", "WS", "YE", "YT", "RS", "ZA",
        "ZM", "ME", "ZW", "A1", "XK", "O1", "AX", "GG", "IM", "JE", "BL", "MF", "BQ", "SS"
        };
        private readonly byte id_len;
        private readonly Dictionary<string, object> info;
        private readonly ushort m_idx_len;
        private readonly byte[] m_idx_str;
        private readonly ushort max_city;
        private readonly ushort max_country;
        private readonly ushort max_region;
        private readonly byte[][] pack;
        private readonly byte parserType;
        private readonly uint range;
        private readonly long regions_begin;
        private readonly uint time;
        private readonly byte version;
        private byte[] regions_db;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initialize a new instance
        /// </summary>
        /// <param name="dbFile">path to the database file</param>
        public SxGeo(string dbFile = "./Resources/SxGeo.dat")
        {
            using (Stream fh = File.OpenRead(dbFile))

            using (BinaryReader reader = new BigEndianBinaryReader(fh))
            {
                var header = reader.ReadBytes(40);

                if (!header.AsSpan(0, 3).SequenceEqual(Encoding.UTF8.GetBytes("SxG").AsSpan()))
                    throw new FormatException("Can't parse {$db_file}");
                ReadOnlySpan<byte> slice = header.AsSpan().Slice(3);
                this.info = slice.Unpack("Cver/Ntime/Ctype/Ccharset/Cb_idx_len/nm_idx_len/nrange/Ndb_items/Cid_len/nmax_region/nmax_city/Nregion_size/Ncity_size/nmax_country/Ncountry_size/npack_size");

                this.version = Convert.ToByte(info["ver"]);
                if (version != 22)
                    throw new NotSupportedException("Version " + version + " is not supported");

                this.range = Convert.ToUInt32(info["range"]);
                this.b_idx_len = Convert.ToByte(info["b_idx_len"]);
                this.m_idx_len = Convert.ToUInt16(info["m_idx_len"]);
                this.db_items = Convert.ToUInt32(info["db_items"]);
                this.id_len = Convert.ToByte(info["id_len"]);
                this.block_len = 3 + this.id_len;
                this.max_region = Convert.ToUInt16(info["max_region"]);
                this.max_city = Convert.ToUInt16(info["max_city"]);
                this.max_country = Convert.ToUInt16(info["max_country"]);
                this.country_size = Convert.ToUInt32(info["country_size"]);
                this.charset = Convert.ToByte(info["charset"]);
                switch (charset)
                {
                    case 0:
                        PhpExtentions.CuurentEncoding = Encoding.UTF8;
                        break;

                    case 1:
                        PhpExtentions.CuurentEncoding = CodePagesEncodingProvider.Instance.GetEncoding(1252);
                        break;

                    case 2:
                        PhpExtentions.CuurentEncoding = CodePagesEncodingProvider.Instance.GetEncoding(1251);
                        break;
                }

                this.parserType = Convert.ToByte(info["type"]);

                //this.batch_mode = type == SxGeoType.SXGEO_BATCH;
                //this.memory_mode = type == SxGeoType.SXGEO_MEMORY;
                this.time = Convert.ToUInt32(info["time"]);
                int pack_size = Convert.ToInt32(info["pack_size"]);

                if ((b_idx_len * m_idx_len * range * db_items * time * id_len) == 0)
                    throw new FormatException("Wrong file format " + dbFile);

                if (pack_size > 0)
                    this.pack = reader.ReadBytes(pack_size).Split(byte.MinValue);
                this.b_idx_str = reader.ReadBytes(b_idx_len * 4);
                this.m_idx_str = reader.ReadBytes(m_idx_len * 4);

                this.db_begin = fh.Position;

                this.db = reader.ReadBytes((int)(db_items * block_len));
                if (info.ContainsKey("region_size"))
                    this.regions_db = reader.ReadBytes(Convert.ToInt32(info["region_size"]));
                else this.regions_db = new byte[0];

                if (info.ContainsKey("city_size"))
                    this.cities_db = reader.ReadBytes(Convert.ToInt32(info["city_size"]));
                else this.cities_db = new byte[0];

                info["regions_begin"] = this.regions_begin = db_begin + db_items * block_len;
                info["cities_begin"] = this.cities_begin = regions_begin + regions_db.Length;
            }
        }

        #endregion Public Constructors

        #region Protected Methods

        protected Dictionary<string, object> ParseCity(long seek, bool full = false)
        {
            Dictionary<string, object> country, city, result;
            if (pack == null || pack.Length == 0)
                return null;
            bool only_country = false;
            if (seek < country_size)
            {
                country = ReadData(seek, max_country, 0);
                city = UnpackObject(PhpExtentions.CuurentEncoding.GetString(pack[2]), new ReadOnlySpan<byte>());
                city["lat"] = country["lat"];
                city["lon"] = country["lon"];
                only_country = true;
            }
            else
            {
                city = ReadData(seek, max_city, 2);
                country = new Dictionary<string, object>();
                country["id"] = city["country_id"];
                country["iso"] = ID_2_ISO[Convert.ToInt32(city["country_id"])];
            }
            if (full)
            {
                var region = ReadData(Convert.ToInt32(city["region_seek"]), max_region, 1);
                if (!only_country)
                    country = ReadData(Convert.ToInt32(region["country_seek"]), max_country, 0);
                result = new Dictionary<string, object>();
                result["city"] = city;
                result["region"] = region;
                result["country"] = country;
                return result;
            }
            else
            {
                result = new Dictionary<string, object>();
                result["city"] = city;
                result["country"] = country;
                return result;
            }
        }

        protected Dictionary<string, object> ReadData(long seek, long max, int type)
        {
            ReadOnlySpan<byte> raw;
            if (seek > 0 && max > 0)
            {
                if (type == 1)
                {
                    if (max > regions_db.Length - seek)
                        max = regions_db.Length - seek;
                    raw = regions_db.AsSpan((int)seek, (int)(max));
                    //raw = regions_db.AsSpan();
                }
                else
                {
                    if (max > cities_db.Length - seek)
                        max = cities_db.Length - seek;
                    raw = cities_db.AsSpan((int)seek, (int)(max));
                    //raw = cities_db.AsSpan();
                }
                return UnpackObject(PhpExtentions.CuurentEncoding.GetString(pack[type]), raw);
            }
            return null;
        }

        protected long SearchDb(ReadOnlySpan<byte> str, ReadOnlySpan<byte> ipn, long min, long max)
        {
            if (max - min > 1)
            {
                ipn = ipn.Slice(1);
                while (max - min > 8)
                {
                    int offset = (int)((min + max) >> 1);
                    if (ipn.Compare(str.Slice(offset * block_len, 3)) > 0) min = offset;
                    else max = offset;
                }
                while (ipn.Compare(str.Slice((int)min * block_len, 3)) >= 0 && ++min < max) ;
            }
            else
            {
                min++;
            }

            byte[] array = str.Slice((int)min * block_len - id_len, id_len).ToArray();
            StringBuilder sb = new StringBuilder();
            foreach (byte b in array)
            {
                sb.Append(b.ToString("X2"));
            }
            return Convert.ToUInt32(sb.ToString(), 16);
        }

        protected long SearchIdx(ReadOnlySpan<byte> ipn, long min, long max)
        {
            while (max - min > 8)
            {
                int offset = (int)((min + max) >> 1);
                if (ipn.Compare(m_idx_str.AsSpan(offset * 4, 4)) > 0) min = offset;
                else max = offset;
            }
            while (ipn.Compare(m_idx_str.AsSpan((int)min * 4, 4)) > 0 && min++ < max) ;
            return min;
        }

        #endregion Protected Methods

        #region Private Methods

        private long GetNumber(ReadOnlySpan<byte> ipn)
        {
            byte ip1n = ipn[0]; // Первый байт
            if (ip1n == 0 || ip1n == 10 || ip1n == 127 || ip1n >= b_idx_len)
                return 0;

            // Находим блок данных в индексе первых байт
            ReadOnlySpan<byte> blockSpan = b_idx_str.AsSpan((ip1n - 1) * 4, 8);
            var blocks = blockSpan.Unpack("Nmin/Nmax");
            long blockMax = Convert.ToInt64(blocks["max"]);
            long blockMin = Convert.ToInt64(blocks["min"]);
            long min, max;
            if (blockMax - blockMin > range)
            {
                // Ищем блок в основном индексе
                var part = SearchIdx(ipn, (long)Math.Floor(blockMin * 1.0d / range), (long)Math.Floor(blockMax * 1.0d / range) - 1);
                // Нашли номер блока в котором нужно искать IP, теперь находим нужный блок в БД
                min = part > 0 ? part * range : 0;
                max = part > m_idx_len ? db_items : (part + 1) * range;
                // Нужно проверить чтобы блок не выходил за пределы блока первого байта
                if (min < blockMin) min = blockMin;
                if (max > blockMax) max = blockMax;
            }
            else
            {
                min = blockMin;
                max = blockMax;
            }
            var len = max - min;
            // Находим нужный диапазон в БД

            return SearchDb(db.AsSpan(), ipn, min, max);
        }

        private Dictionary<string, object> UnpackObject(string aPack, ReadOnlySpan<byte> aitem)
        {
            Dictionary<string, object> unpacked = new Dictionary<string, object>();
            bool empty = aitem.Length == 0;
            int pos = 0;
            string[] pack = aPack.Split('/');
            foreach (var p in pack)
            {
                ReadOnlySpan<byte> item = aitem.Slice(pos);
                string[] tmp = p.Split(':');
                string type = tmp[0];
                string name = tmp.Length > 1 ? tmp[1] : "";
                char type0 = type[0];
                if (empty)
                {
                    if (type0 == 'b' || type0 == 'c')
                        unpacked[name] = "";
                    else unpacked[name] = 0;
                    continue;
                }
                int l = 0;
                switch (type0)
                {
                    case 't':
                    case 'T': l = 1; break;
                    case 's':
                    case 'n':
                    case 'S': l = 2; break;
                    case 'm':
                    case 'M': l = 3; break;
                    case 'd': l = 8; break;
                    case 'c': l = int.Parse(type.Substring(1)); break;
                    case 'b': l = item.IndexOf(byte.MinValue); break;
                    default: l = 4; break;
                }
                var val = item.Slice(0, l);

                object v = null;

                switch (type0)
                {
                    case 't': v = val.unpack('c'); break;
                    case 'T': v = val.unpack('C'); break;
                    case 's': v = val.unpack('s'); break;
                    case 'S': v = val.unpack('S'); break;
                    case 'm':
                        val = val.ToArray().Union(new[] {
                          (val[2] >> 7) > 0 ? byte.MaxValue : byte.MinValue
                        }).ToArray().AsSpan();
                        v = val.unpack('l');
                        break;

                    case 'M':
                        {
                            var arr = val.ToArray();
                            arr = arr.Union(new[] { byte.MinValue }).ToArray();
                            val = arr.AsSpan();

                            v = val.unpack('L');
                        }
                        break;

                    case 'i': v = val.unpack('l'); break;
                    case 'I': v = val.unpack('L'); break;
                    case 'f': v = val.unpack('f'); break;
                    case 'd': v = val.unpack('d'); break;

                    case 'n': v = Convert.ToInt16(val.unpack('s')) / Math.Pow(10, type[1] - '0'); break;
                    case 'N': v = Convert.ToInt32(val.unpack('l')) / Math.Pow(10, type[1] - '0'); break;

                    case 'c': v = PhpExtentions.CuurentEncoding.GetString(val.RTrim((byte)' ').ToArray()); break;
                    case 'b': v = PhpExtentions.CuurentEncoding.GetString(val.ToArray()); l++; break;
                }

                pos += l;
                unpacked[name] = v;
            }
            return unpacked;
        }

        #endregion Private Methods
    }
}