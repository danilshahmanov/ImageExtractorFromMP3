
namespace TagExtractorsLib.Extensions
{
    internal static class StreamExtensions
    {
        public static byte[] ReadAsByteArray(this Stream stream, long count, int offset = 0)
        {
            var buffer = new byte[count];
            stream.Read(buffer, offset, ((int)count));
            return buffer;
        }


        public static List<byte> ReadTillTerminator(this Stream stream, int maxLength, byte terminator)
        {
            var bytes = new List<byte>();
            var buffer = new byte[64];
            int totalBytesRead = 1;
            while (true)
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                for (int i = 0; i < bytesRead; i++, totalBytesRead++)
                {
                    if (totalBytesRead == maxLength)
                        break;
                    if (buffer[i] == terminator)
                    {
                        bytes.Add(buffer[i]);
                        stream.Position -= 64 - totalBytesRead;
                        return bytes;
                    }
                    else
                        bytes.Add(buffer[i]);
                }
            }
        }
    }
}
