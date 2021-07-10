using System;
using System.Text.Json;

namespace rpct_dotnet
{
    public abstract class RpctJson
    {
        public static string ToString(RPCT rpct, MethodCallDataArray callData)
        {
            return JsonSerializer.Serialize(callData);
        }
        public static byte[] ToUtf8Bytes(RPCT rpct, MethodCallDataArray callData)
        {
            return JsonSerializer.SerializeToUtf8Bytes(callData);
        }

        public static MethodCallDataArray FromString(RPCT rpct, string str)
        {
            return JsonSerializer.Deserialize<MethodCallDataArray>(str);
        }

        public static MethodCallDataArray FromUtf8Bytes(RPCT rpct, byte[] buf)
        {
            var dataArr = JsonSerializer.Deserialize<MethodCallDataArray>(buf, new JsonSerializerOptions {
                IncludeFields = true
            });
            
            NormalizeAfterDeser(rpct, dataArr);

            return dataArr;
        }

        public static MethodCallDataArray FromUtf8Bytes(RPCT rpct, ReadOnlySpan<byte> buf)
        {
            var dataArr = JsonSerializer.Deserialize<MethodCallDataArray>(buf, new JsonSerializerOptions {
                IncludeFields = true
            });

            NormalizeAfterDeser(rpct, dataArr);

            return dataArr;
        }

        static void NormalizeAfterDeser(RPCT rpct, MethodCallDataArray dataArr)
        {
            foreach (var item in dataArr.list) {
                for (var i = 0; i < item.args.Length; ++i) {
                    if (item.args[i].value.GetType() != typeof(System.Text.Json.JsonElement)) {
                        continue;
                    }

                    var val = (System.Text.Json.JsonElement)item.args[i].value;

                    // deserialize dataArr.list[].args[].value by looking on data_type
                    // JsonSerializer dont convert it, because .value is typeof object

                    switch(item.args[i].data_type) {
                        case (int)MethodCallArg_BuiltDataType.Unknown:
                        item.args[i].value = null;
                        break;

                        case (int)MethodCallArg_BuiltDataType.String:
                        item.args[i].value = val.GetString();
                        break;

                        case (int)MethodCallArg_BuiltDataType.Float:
                        item.args[i].value = (float)val.GetDouble();
                        break;

                        case (int)MethodCallArg_BuiltDataType.Null:
                        item.args[i].value = null;
                        break;

                        case (int)MethodCallArg_BuiltDataType.Callback:
                        item.args[i].value = val.GetString();
                        break;

                        case (int)MethodCallArg_BuiltDataType.Buffer:
                        if (val.ValueKind == JsonValueKind.Array)
                        {
                            var buf_len = val.GetArrayLength();
                            var buf_val = new byte[buf_len];
                            for (var it = 0; it < buf_len; ++it) {
                                buf_val[it] = val[it].GetByte();
                            }
                            item.args[i].value = buf_val;
                        } else if (val.ValueKind == JsonValueKind.String)
                        {
                            var base64 = val.GetString();
                            item.args[i].value = System.Convert.FromBase64String(base64);
                        }
                        break;
                    }
                }
            }
        }

        public static SerializationChain SerializationChain(RPCT rpct, ISerializationChainPart[] chain = null)
        {
            return new SerializationChain(new RcptJsonBinarySerializer(), chain);
        }
    }

    public class RcptJsonBinarySerializer : IBinarySerializer
    {
        public byte[] ToBytes(RPCT rpct, MethodCallDataArray callData)
        {
            return RpctJson.ToUtf8Bytes(rpct, callData);
        }

        public MethodCallDataArray FromBytes(RPCT rpct, byte[] buf)
        {
            return RpctJson.FromUtf8Bytes(rpct, buf);
        }
    }
}