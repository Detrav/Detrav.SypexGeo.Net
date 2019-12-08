using System;
using System.Collections.Generic;
using System.Text;

namespace Detrav.SypexGeo.Net.Helpers
{
    public static class SpanExtentions
    {
        public static ReadOnlySpan<T> RTrim<T>(this ReadOnlySpan<T> span, T value)
        {
            int len = span.Length ;
            for (; len > 0 && value.Equals(span[len-1]); len--) ;
            if (len > 0)
                return span.Slice(0, len);
            return new ReadOnlySpan<T>();
        }

        public static byte[][] Split(this byte[] bytes, byte ch)
        {
            List<byte[]> result = new List<byte[]>();
            int lastPos = 0;

            for(int i = 0; i< bytes.Length; i++)
            {
                if (bytes[i] == ch)
                {
                    if (lastPos == i)
                        result.Add(new byte[0]);
                    else
                        result.Add(bytes.AsSpan(lastPos,   i - lastPos).ToArray());
                    lastPos = i + 1;
                }
            }
            if(lastPos< bytes.Length)
            {
                result.Add(bytes.AsSpan(lastPos).ToArray());
            }

            return result.ToArray();
        }

        public static int Compare(this ReadOnlySpan<byte> str1, ReadOnlySpan<byte> str2)
        {
            int l1 = str1.Length;
            int l2 = str2.Length;
            int lmin = Math.Min(l1, l2);

            for (int i = 0; i < lmin; i++)
            {
                byte str1_ch = str1[i];
                byte str2_ch = str2[i];

                if (str1_ch != str2_ch)
                {
                    return str1_ch - str2_ch;
                }
            }

            if (l1 != l2)
            {
                return l1 - l2;
            }

            else
            {
                return 0;
            }
        }
    }
}
