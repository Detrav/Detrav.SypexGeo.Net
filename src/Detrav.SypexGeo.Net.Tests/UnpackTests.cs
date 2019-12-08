using Detrav.SypexGeo.Net.Helpers;
using System;
using System.Linq;
using Xunit;

namespace Detrav.SypexGeo.Net.Tests
{
    public class UnpackTests
    {
        #region Public Methods

        [Theory]
        [InlineData('d', "abcdabcd", 3.8354610812683E+175d)]
        [InlineData('e', "abcdabcd", 3.8354610812683E+175d)]
        [InlineData('E', "abcdabcd", 1.2926117739473E+161d)]
        public void UnpackDouble(char format, string array, double value)
        {
            ReadOnlySpan<byte> span = array.Select(m => (byte)m).ToArray().AsSpan();
            Assert.True(Math.Abs((Convert.ToDouble(span.unpack(format)) / value) - 1) < 0.001);
        }

        [Theory]
        [InlineData('f', "abcd", 1.6777999408082E+22f)]
        [InlineData('g', "abcd", 1.6777999408082E+22f)]
        [InlineData('G', "abcd", 2.6100787562286E+20f)]
        public void UnpackFloat(char format, string array, float value)
        {
            ReadOnlySpan<byte> span = array.Select(m => (byte)m).ToArray().AsSpan();
            Assert.True(Math.Abs((Convert.ToSingle(span.unpack(format)) / value) - 1) < 0.001);
            //Assert.Equal(Convert.ToSingle(span.unpack(format)), value, 5);
        }

        [Theory]
        [InlineData('s', "ab", (short)25185)]
        [InlineData('S', "ab", (ushort)25185)]
        [InlineData('n', "ab", (ushort)24930)]
        [InlineData('v', "ab", (ushort)25185)]
        [InlineData('s', "\xff\xff", (short)-1)]
        [InlineData('S', "\xff\xff", (ushort)ushort.MaxValue)]
        [InlineData('n', "\xff\xff", (ushort)ushort.MaxValue)]
        [InlineData('v', "\xff\xff", (ushort)ushort.MaxValue)]
        [InlineData('i', "abcd", (int)1684234849)]
        [InlineData('I', "abcd", (uint)1684234849U)]
        [InlineData('i', "\xff\xff\xff\xff", (int)-1)]
        [InlineData('I', "\xff\xff\xff\xff", (uint)uint.MaxValue)]
        [InlineData('l', "abcd", (int)1684234849)]
        [InlineData('L', "abcd", (uint)1684234849U)]
        [InlineData('N', "abcd", (uint)1633837924U)]
        [InlineData('V', "abcd", (uint)1684234849U)]
        [InlineData('l', "\xff\xff\xff\xff", (int)-1)]
        [InlineData('L', "\xff\xff\xff\xff", (uint)uint.MaxValue)]
        [InlineData('N', "\xff\xff\xff\xff", (uint)uint.MaxValue)]
        [InlineData('V', "\xff\xff\xff\xff", (uint)uint.MaxValue)]
        [InlineData('q', "abcdabcd", (long)7233733596922733153L)]
        [InlineData('Q', "abcdabcd", (ulong)7233733596922733153UL)]
        [InlineData('J', "abcdabcd", (ulong)7017280452178371428UL)]
        [InlineData('P', "abcdabcd", (ulong)7233733596922733153UL)]
        [InlineData('q', "\xff\xff\xff\xff\xff\xff\xff\xff", (long)-1L)]
        [InlineData('Q', "\xff\xff\xff\xff\xff\xff\xff\xff", (ulong)ulong.MaxValue)]
        [InlineData('J', "\xff\xff\xff\xff\xff\xff\xff\xff", (ulong)ulong.MaxValue)]
        [InlineData('P', "\xff\xff\xff\xff\xff\xff\xff\xff", (ulong)ulong.MaxValue)]
        public void UnpackTest(char format, string array, object value)
        {
            ReadOnlySpan<byte> span = array.Select(m => (byte)m).ToArray().AsSpan();
            Assert.Equal(span.unpack(format), value);
        }

        #endregion Public Methods
    }
}