using System;
using System.Collections.Generic;
using System.Text;

namespace Detrav.SypexGeo.Net.Helpers
{
    public static class PhpExtentions
    {
        private static readonly bool isReverseBL = !BitConverter.IsLittleEndian;
        private static readonly bool machine_little_endian = false;
        
        //private static readonly int[] machine_endian_short_map = new int[2];
        //private static readonly int[] big_endian_short_map = new int[2];
        //private static readonly int[] little_endian_short_map = new int[2];
        //private static readonly int[] machine_endian_long_map = new int[4];
        //private static readonly int[] big_endian_long_map = new int[4];
        //private static readonly int[] little_endian_long_map = new int[4];
        //private static readonly int[] machine_endian_longlong_map = new int[8];
        //private static readonly int[] big_endian_longlong_map = new int[8];
        //private static readonly int[] little_endian_longlong_map = new int[8];

        static PhpExtentions()
        {
            //machine_endian_short_map[0] = 0;
            //machine_endian_short_map[1] = 1;
            //big_endian_short_map[0] = 1;
            //big_endian_short_map[1] = 0;
            //little_endian_short_map[0] = 0;
            //little_endian_short_map[1] = 1;

            //machine_endian_long_map[0] = 0;
            //machine_endian_long_map[1] = 1;
            //machine_endian_long_map[2] = 2;
            //machine_endian_long_map[3] = 3;
            //big_endian_long_map[0] = 3;
            //big_endian_long_map[1] = 2;
            //big_endian_long_map[2] = 1;
            //big_endian_long_map[3] = 0;
            //little_endian_long_map[0] = 0;
            //little_endian_long_map[1] = 1;
            //little_endian_long_map[2] = 2;
            //little_endian_long_map[3] = 3;

            //machine_endian_longlong_map[0] = 0;
            //machine_endian_longlong_map[1] = 1;
            //machine_endian_longlong_map[2] = 2;
            //machine_endian_longlong_map[3] = 3;
            //machine_endian_longlong_map[4] = 4;
            //machine_endian_longlong_map[5] = 5;
            //machine_endian_longlong_map[6] = 6;
            //machine_endian_longlong_map[7] = 7;
            //big_endian_longlong_map[0] = 7;
            //big_endian_longlong_map[1] = 6;
            //big_endian_longlong_map[2] = 5;
            //big_endian_longlong_map[3] = 4;
            //big_endian_longlong_map[4] = 3;
            //big_endian_longlong_map[5] = 2;
            //big_endian_longlong_map[6] = 1;
            //big_endian_longlong_map[7] = 0;
            //little_endian_longlong_map[0] = 0;
            //little_endian_longlong_map[1] = 1;
            //little_endian_longlong_map[2] = 2;
            //little_endian_longlong_map[3] = 3;
            //little_endian_longlong_map[4] = 4;
            //little_endian_longlong_map[5] = 5;
            //little_endian_longlong_map[6] = 6;
            //little_endian_longlong_map[7] = 7;
        }


        public static Encoding CuurentEncoding { get; set; } = Encoding.UTF8;
        public static Dictionary<string, object> Unpack(this ReadOnlySpan<byte> input, string format, int offset = 0)
        {
            var result = new Dictionary<string, object>();
            int formatPtr = 0;
            int inputPtr = 0;
            int formatlen = format.Length;
            int inputlen = input.Length;
            int inputpos = 0;

            if(offset < 0 || offset > inputlen)
            {
                return null;
            };

            inputPtr += offset;
            inputlen -= offset;

            while (formatlen-- > 0)
            {
                char type = format[formatPtr];
                formatPtr++;
                int arg = 1, argb;
                char c;
                string name;
                int namelen;
                int size = 0;

                /* Handle format arguments if any */
                if (formatlen > 0)
                {
                    c = format[formatPtr];

                    if (c >= '0' && c <= '9')
                    {
                        arg = format[formatPtr] - '0';
                        formatPtr++;
                        formatlen--;

                        while (formatlen > 0 && format[formatPtr] >= '0' && format[formatPtr] <= '9')
                        {
                            arg = arg * 10 + (format[formatPtr] - '0');
                            formatPtr++;
                            formatlen--;
                        }
                    }
                    else if (c == '*')
                    {
                        arg = -1;
                        formatPtr++;
                        formatlen--;
                    }
                }

                int namePtr = formatPtr;
                argb = arg;

                while (formatlen > 0 && format[formatPtr] != '/')
                {
                    formatlen--;
                    formatPtr++;
                }

                namelen = formatPtr - namePtr;

                //name = format.Substring(namePtr, namelen);

                if (namelen > 200)
                    namelen = 200;

               

                /* Do actual unpacking */
                for (int i = 0; i < arg; i++)
                {
                    /* Space for name + number, safe as namelen is ensured <= 200 */
                    string n;



                    if (arg != 1 || namelen == 0)
                    {
                        /* Need to add element number to name */
                        n = format.Substring(namePtr, namelen) + (i + 1);
                    }
                    else
                    {
                        /* Truncate name to next format code or end of string */
                        n = format.Substring(namePtr, namelen);
                    }

                    object v = input.Slice(inputPtr + inputpos).Unpack(type, ref arg, ref argb, ref inputpos, ref i, ref inputlen);
                    if (v != null)
                        result[n] = v;

                    else if (arg < 0)
                    {
                        /* Reached end of input for '*' repeater */
                        break;
                    }
                    else
                    {
                        throw new FormatException("Type " + type + ": not enough input, need " + size + ", have " + (inputlen - inputpos));

                    }
                }
                if (formatlen > 0)
                {
                    formatlen--;    /* Skip '/' separator, does no harm if inputlen == 0 */
                    formatPtr++;
                }
            }

            return result;
        }

        public static object unpack(this ReadOnlySpan<byte> span, char type)
        {
            int arg = 1;
            int argb = 1;
            int inputpos = 0;
            int i = 0;
            int inputlen = span.Length;
            return Unpack(span, type, ref arg, ref argb, ref inputpos, ref i, ref inputlen);
        }



        public static object Unpack(this ReadOnlySpan<byte> span, char type, ref int arg, ref int argb, ref int inputpos, ref int i, ref int inputlen)
        {
            object result = null;
            int size = 0;
            switch (type)
            {
                /* Never use any input */
                case 'X':
                    size = -1;
                    if (arg < 0)
                    {
                        arg = 1;
                    }
                    break;

                case '@':
                    size = 0;
                    break;

                case 'a':
                case 'A':
                case 'Z':
                    size = arg;
                    arg = 1;
                    break;

                case 'h':
                case 'H':
                    size = (arg > 0) ? (arg + (arg % 2)) / 2 : arg;
                    arg = 1;
                    break;

                /* Use 1 byte of input */
                case 'c':
                case 'C':
                case 'x':
                    size = 1;
                    break;

                /* Use 2 bytes of input */
                case 's':
                case 'S':
                case 'n':
                case 'v':
                    size = 2;
                    break;

                /* Use sizeof(int) bytes of input */
                case 'i':
                case 'I':
                    size = sizeof(int);
                    break;

                /* Use 4 bytes of input */
                case 'l':
                case 'L':
                case 'N':
                case 'V':
                    size = 4;
                    break;

                /* Use 8 bytes of input */
                case 'q':
                case 'Q':
                case 'J':
                case 'P':
                    size = 8;
                    break;

                /* Use sizeof(float) bytes of input */
                case 'f':
                case 'g':
                case 'G':
                    size = sizeof(float);
                    break;

                /* Use sizeof(double) bytes of input */
                case 'd':
                case 'e':
                case 'E':
                    size = sizeof(double);
                    break;

                default:
                    throw new FormatException("Invalid format type " + type);

            }

            if (size != 0 && size != -1 && size < 0)
            {
                throw new FormatException("Type " + type + " : integer overflow");
            }

            if (size <= span.Length)
            {

                switch (type)
                {
                    case 'a':
                        {
                            /* a will not strip any trailing whitespace or null padding */
                            int len = span.Length;    /* Remaining string */

                            /* If size was given take minimum of len and size */
                            if ((size >= 0) && (len > size))
                            {
                                len = size;
                            }

                            size = len;

                            result = CuurentEncoding.GetString(span.Slice(0, len));
                            break;
                        }
                    case 'A':
                        {
                            /* A will strip any trailing whitespace */
                            char padn = '\0'; char pads = ' '; char padt = '\t'; char padc = '\r'; char padl = '\n';
                            int len = span.Length;    /* Remaining string */

                            /* If size was given take minimum of len and size */
                            if ((size >= 0) && (len > size))
                            {
                                len = size;
                            }

                            size = len;

                            /* Remove trailing white space and nulls chars from unpacked data */
                            while (--len >= 0)
                            {
                                if (span[ len] != padn
                                    && span[len] != pads
                                    && span[ len] != padt
                                    && span[len] != padc
                                    && span[len] != padl
                                )
                                    break;
                            }

                            result = CuurentEncoding.GetString(span.Slice(0, len + 1));
                            break;
                        }
                    /* New option added for Z to remain in-line with the Perl implementation */
                    case 'Z':
                        {
                            /* Z will strip everything after the first null character */
                            char pad = '\0';
                            int s, len = span.Length; /* Remaining string */

                            /* If size was given take minimum of len and size */
                            if ((size >= 0) && (len > size))
                            {
                                len = size;
                            }

                            size = len;

                            /* Remove everything after the first null */
                            for (s = 0; s < len; s++)
                            {
                                if (span[ s] == pad)
                                    break;
                            }
                            len = s;

                            result = CuurentEncoding.GetString(span.Slice(0, len));
                            break;
                        }

                    case 'h':
                    case 'H':
                        {
                            int len = (span.Length) * 2;  /* Remaining */
                            int nibbleshift = (type == 'h') ? 0 : 4;
                            int first = 1;
                            //zend_string* buf;
                            //zend_long ipos, opos;

                            /* If size was given take minimum of len and size */
                            if (size >= 0 && len > (size * 2))
                            {
                                len = size * 2;
                            }

                            if (len > 0 && argb > 0)
                            {
                                len -= argb % 2;
                            }

                            char[] buf = new char[len];
                            int ipos, opos;

                            for (ipos = opos = 0; opos < len; opos++)
                            {
                                char cc = (char)((span[ipos] >> nibbleshift) & 0xf);

                                if (cc < 10)
                                {
                                    cc += '0';
                                }
                                else
                                {
                                    cc += (char)('a' - 10);
                                }

                                buf[opos] = cc;
                                nibbleshift = (nibbleshift + 4) & 7;

                                if (first-- == 0)
                                {
                                    ipos++;
                                    first = 1;
                                }
                            }

                            //buf[len] = '\0';
                            result = new string(buf);
                            break;
                        }

                    case 'c':
                    case 'C':
                        {
                            int issigned = (type == 'c') ? (span[0] & 0x80) : 0;

                            if (issigned > 0)
                            {
                                result = (sbyte)span[0];
                            }
                            else
                            {
                                result = span[0];
                            }
                            break;
                        }
                    case 's':
                    case 'S':
                    case 'n':
                    case 'v':
                        {

                            int issigned = 0;
                            bool inBigEndian = isReverseBL;

                            if (type == 's')
                            {
                                issigned = 1;
                               // issigned = span[ (machine_little_endian ? 1 : 0)] & 0x80;
                            }
                            else if (type == 'n')
                            {
                                inBigEndian = !isReverseBL;
                            }
                            else if (type == 'v')
                            {
                                inBigEndian = isReverseBL;
                            }

                            byte[] bytes = span.Slice(0, size).ToArray();
                            if (inBigEndian)
                                Array.Reverse(bytes);

                            object v;
                            if (issigned > 0)
                            {
                                v = BitConverter.ToInt16(bytes);
                            }
                            else
                            {
                                v = BitConverter.ToUInt16(bytes);
                            }
                            result = v;
                            break;
                        }
                    case 'i':
                    case 'I':
                        {
                            object v;
                            int issigned = 0;

                            bool inBigEndian = isReverseBL;

                            if (type == 'i')
                            {
                                issigned = 1;
                               // issigned = span[(machine_little_endian ? (sizeof(int) - 1) : 0)] & 0x80;
                            }

                            byte[] bytes = span.Slice(0, size).ToArray();
                            if (inBigEndian)
                                Array.Reverse(bytes);


                            if (issigned > 0)
                            {
                                v = BitConverter.ToInt32(bytes);
                            }
                            else
                            {
                                v = BitConverter.ToUInt32(bytes);
                            }
                            result = v;

                            break;

                        }
                    case 'l':
                    case 'L':
                    case 'N':
                    case 'V':
                        {
                            int issigned = 0;
                            bool inBigEndian = isReverseBL;
                            object v = 0;

                            if (type == 'l')
                            {
                                issigned = 1;
                                //issigned = span[(machine_little_endian ? 3 : 0)] & 0x80;
                            }
                            else if (type == 'N')
                            {
                                //issigned = span[0] & 0x80;
                                inBigEndian = !isReverseBL;

                            }
                            else if (type == 'V')
                            {
                                //issigned = span[3] & 0x80;
                                inBigEndian = isReverseBL;

                            }


                            byte[] bytes = span.Slice(0, size).ToArray();
                            if (inBigEndian)
                                Array.Reverse(bytes);


                            if (issigned > 0)
                            {
                                v = BitConverter.ToInt32(bytes);
                            }
                            else
                            {
                                v = BitConverter.ToUInt32(bytes);
                            }
                            result = v;

                            break;
                        }

                    case 'q':
                    case 'Q':
                    case 'J':
                    case 'P':
                        {
                            int issigned = 0;
                            bool inBigEndian = isReverseBL;
                            object v = 0;

                            if (type == 'q' /*|| type == 'Q'*/)
                            {
                                issigned = 1;
                               // issigned = span[ (machine_little_endian ? 7 : 0)] & 0x80;
                            }
                            else if (type == 'J')
                            {
                                //issigned = span[0] & 0x80;
                                inBigEndian = !isReverseBL;
                            }
                            else if (type == 'P')
                            {
                                //issigned = span[ 7] & 0x80;
                                inBigEndian = isReverseBL;
                            }

                            byte[] bytes = span.Slice(0, size).ToArray();
                            if (inBigEndian)
                                Array.Reverse(bytes);


                            if (issigned > 0)
                            {
                                v = BitConverter.ToInt64(bytes);
                            }
                            else
                            {
                                v = BitConverter.ToUInt64(bytes);
                            }
                            result = v;


                            break;
                        }

                    case 'f': /* float */
                    case 'g': /* little endian float*/
                    case 'G': /* big endian float*/
                        {
                            float v;
                            bool inBigEndian = isReverseBL;


                            if (type == 'g')
                            {
                                inBigEndian = isReverseBL;
                            }
                            else if (type == 'G')
                            {
                                inBigEndian = !isReverseBL;
                            }

                            byte[] bytes = span.Slice(0, size).ToArray();
                            if (inBigEndian)
                                Array.Reverse(bytes);



                            v = BitConverter.ToSingle(bytes);

                            result = v;

                            break;
                        }

                    case 'd': /* double */
                    case 'e': /* little endian float */
                    case 'E': /* big endian float */
                        {
                            double v;
                            bool inBigEndian = isReverseBL;

                            if (type == 'e')
                            {
                                inBigEndian = isReverseBL;
                            }
                            else if (type == 'E')
                            {
                                inBigEndian = !isReverseBL;
                            }
                            byte[] bytes = span.Slice(0, size).ToArray();
                            if (inBigEndian)
                                Array.Reverse(bytes);



                            v = BitConverter.ToDouble(bytes);

                            result = v;
                            break;
                        }

                    case 'x':
                        /* Do nothing with input, just skip it */
                        break;
                    case 'X':
                        if (inputpos < size)
                        {
                            inputpos = -size;
                            i = arg - 1;        /* Break out of for loop */

                            if (arg >= 0)
                            {
                                //php_error_docref(NULL, E_WARNING, "Type %c: outside of string", type);
                            }
                        }
                        break;

                    case '@':
                        if (arg <= inputlen)
                        {
                            inputpos = arg;
                        }
                        else
                        {
                            //php_error_docref(NULL, E_WARNING, "Type %c: outside of string", type);
                        }

                        i = arg - 1;    /* Done, break out of for loop */
                        break;

                }

                inputpos += size;
                if (inputpos < 0)
                {
                    if (size != -1)
                    { /* only print warning if not working with * */
                      //php_error_docref(NULL, E_WARNING, "Type %c: outside of string", type);
                    }
                    inputpos = 0;
                }

                
            }

            return result;
        }
    }
}
