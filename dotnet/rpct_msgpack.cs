namespace rpct_dotnet
{
    public abstract class RpctMsgpack
    {
        public static byte[] ToBytes(RPCT rpct, MethodCallDataArray callData)
        {
            return MessagePack.MessagePackSerializer.Serialize(callData);
        }

        public static MethodCallDataArray FromBytes(RPCT rpct, System.Buffers.ReadOnlySequence<byte> buf)
        {
            return MessagePack.MessagePackSerializer.Deserialize<MethodCallDataArray>(buf);
        }

        public static SerializationChain SerializationChain(ISerializationChainPart[] chain = null)
        {
            return new SerializationChain(new RpctMsgpackBinarySerializer(), chain);
        }
    }

    public class RpctMsgpackBinarySerializer : IBinarySerializer
    {
        public byte[] ToBytes(RPCT rpct, MethodCallDataArray callData)
        {
            return RpctMsgpack.ToBytes(rpct, callData);
        }

        public MethodCallDataArray FromBytes(RPCT rpct, byte[] buf)
        {
            return RpctMsgpack.FromBytes(rpct, new System.Buffers.ReadOnlySequence<byte>(buf));
        }
    }
}