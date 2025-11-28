using System.Text;

namespace Ds.Api.Extensions;

public static class StringExtensions
{
    extension(string value)
    {
        public byte[] ToUTF8Bytes() => Encoding.UTF8.GetBytes(value);
    }
}