using System.IO;

namespace Lykke.Service.Balances.Services
{
    internal static class CacheSerializer
    {
        public static T Deserialize<T>(byte[] data)
        {
            using (var stream = new MemoryStream(data))
            {
                return ProtoBuf.Serializer.Deserialize<T>(stream);
            }
        }

        public static byte[] Serialize<T>(T data)
        {
            using (var stream = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize(stream, data);
                stream.Flush();
                return stream.ToArray();
            }
        }
    }
}
