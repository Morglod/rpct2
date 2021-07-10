namespace rpct_dotnet
{
    public interface IBinarySerializer
    {
        byte[] ToBytes(RPCT rpct, MethodCallDataArray data);
        MethodCallDataArray FromBytes(RPCT rpct, byte[] data);
    }

    public interface ISerializationChainPart
    {
        byte[] To(RPCT rpct, byte[] data);
        byte[] From(RPCT rpct, byte[] data);
    }

    public class SerializationChain : IBinarySerializer
    {
        public IBinarySerializer Serializer;
        public ISerializationChainPart[] Chain;

        public SerializationChain()
        {
            this.Serializer = null;
            this.Chain = new ISerializationChainPart[0];
        }

        public SerializationChain(IBinarySerializer serializer, ISerializationChainPart[] chain = null)
        {
            this.Serializer = serializer;
            this.Chain = chain == null ? new ISerializationChainPart[0] : chain;
        }

        public byte[] ToBytes(RPCT rpct, MethodCallDataArray data)
        {
            var buf = this.Serializer.ToBytes(rpct, data);
            foreach (var chainPart in this.Chain) {
                buf = chainPart.To(rpct, buf);
            }
            return buf;
        }

        public MethodCallDataArray FromBytes(RPCT rpct, byte[] data)
        {
            var buf = data;
            for (var i = this.Chain.Length -1; i >= 0; --i) {
                buf = this.Chain[i].From(rpct, buf);
            }
            return this.Serializer.FromBytes(rpct, buf);
        }
    }
}