using System;
using System.Text.Json.Serialization;

namespace rpct_dotnet
{
    [Serializable]
    public enum MethodCallArg_BuiltDataType : int
    {
        Unknown = 0,
        Null = -2,
        Buffer = -3,
        Float = -4,
        String = -5,
        Callback = -6,
        StringDictionary = -7,
    }

    [Serializable]
    [MessagePack.MessagePackObject(keyAsPropertyName: true)]
    public struct MethodCallArgData
    {
        [JsonInclude]
        public string name;

        [JsonInclude]
        public int index;

        [JsonInclude]
        public int data_type;

        [JsonInclude]
        public object value;
    }

    [Serializable]
    [MessagePack.MessagePackObject(keyAsPropertyName: true)]
    public struct MethodCallData
    {
        [JsonInclude]
        public string rid;

        [JsonInclude]
        public string method_name;

        [JsonInclude]
        public MethodCallArgData[] args;
    }

    [Serializable]
    [MessagePack.MessagePackObject(keyAsPropertyName: true)]
    public struct MethodCallDataArray
    {
        [JsonInclude]
        public MethodCallData[] list;
    }

    public delegate void DSendMethodCall(MethodCallData methodCall);
    public delegate void DCallback(params object[] args);

    public delegate void VoidFunc();
    public delegate void VoidFunc<in T1>(T1 arg);
    public delegate void VoidFunc<in T1, in T2>(T1 arg, T2 arg2);
    public delegate void VoidFunc<in T1, in T2, in T3>(T1 arg, T2 arg2, T3 arg3);
    public delegate void VoidFunc<in T1, in T2, in T3, in T4>(T1 arg, T2 arg2, T3 arg3, T4 arg4);
    public delegate void VoidFunc<in T1, in T2, in T3, in T4, in T5>(T1 arg, T2 arg2, T3 arg3, T4 arg4, T5 arg5);

    public class RPCT
    {
        public static DCallback AsCallback(VoidFunc func)
        {
            return delegate (object[] _ignore) { func(); };
        }

        public static DCallback AsCallback<T1>(VoidFunc<T1> func)
        {
            return delegate (object[] args) { func((dynamic)args[0]); };
        }
        public static DCallback AsCallback<T1, T2>(VoidFunc<T1, T2> func)
        {
            return delegate (object[] args) { func((dynamic)args[0], (dynamic)args[1]); };
        }
        public static DCallback AsCallback<T1, T2, T3>(VoidFunc<T1, T2, T3> func)
        {
            return delegate (object[] args) { func((dynamic)args[0], (dynamic)args[1], (dynamic)args[2]); };
        }
        public static DCallback AsCallback<T1, T2, T3, T4>(VoidFunc<T1, T2, T3, T4> func)
        {
            return delegate (object[] args) { func((dynamic)args[0], (dynamic)args[1], (dynamic)args[2], (dynamic)args[3]); };
        }
        public static DCallback AsCallback<T1, T2, T3, T4, T5>(VoidFunc<T1, T2, T3, T4, T5> func)
        {
            return delegate (object[] args) { func((dynamic)args[0], (dynamic)args[1], (dynamic)args[2], (dynamic)args[3], (dynamic)args[4]); };
        }

        public DSendMethodCall SendToRemote = null;
        public System.Collections.Generic.Dictionary<string, DCallback> Methods = new System.Collections.Generic.Dictionary<string, DCallback>();

        public int GetDataTypeFromValue(object value)
        {
            if (value == null) return (int)MethodCallArg_BuiltDataType.Null;

            var type = value.GetType();
            if (type == ((int)0).GetType()) return (int)MethodCallArg_BuiltDataType.Float;
            if (type == ((float)0).GetType()) return (int)MethodCallArg_BuiltDataType.Float;
            if (type == ((double)0).GetType()) return (int)MethodCallArg_BuiltDataType.Float;
            if (type == ("".GetType())) return (int)MethodCallArg_BuiltDataType.String;
            if (type == ((new byte[1]).GetType())) return (int)MethodCallArg_BuiltDataType.Buffer;
            if (type == typeof(DCallback)) return (int)MethodCallArg_BuiltDataType.Callback;
            if (type == typeof(System.Collections.Generic.Dictionary<string, string>)) return (int)MethodCallArg_BuiltDataType.StringDictionary;

            return (int)MethodCallArg_BuiltDataType.Unknown;
        }

        public MethodCallArgData FromArgToArgData(string name, int index, object value)
        {
            var data_type = this.GetDataTypeFromValue(value);
            object out_value = null;

            switch (data_type) {
                case (int)MethodCallArg_BuiltDataType.Unknown:
                throw new Exception("unknown value type");
                // break;

                case (int)MethodCallArg_BuiltDataType.Callback:
                var cb_name = "";
                cb_name = this._AddCallback((object[] args) => {
                    var original_func = (DCallback)value;
                    original_func.Invoke(args);
                    this.Methods.Remove(cb_name);
                });
                out_value = cb_name;
                break;

                default:
                out_value = value;
                break;
            }

            return new MethodCallArgData {
                name = name,
                index = index,
                data_type = data_type,
                value = out_value,
            };
        }

        public void AddMethod(string methodName, DCallback func)
        {
            this.Methods.Add(methodName, func);
        }

        public string _AddCallback(DCallback func)
        {
            var rnd = new Random();
            var rndName = rnd.Next().ToString();
            this.AddMethod(rndName, func);
            return rndName;
        }
        public object FromArgDataToArg(MethodCallArgData arg)
        {
            switch (arg.data_type) {
                case (int)MethodCallArg_BuiltDataType.Unknown:
                throw new Exception("unknown value type");
                // break;

                case (int)MethodCallArg_BuiltDataType.Callback:
                return (DCallback)((object[] args) => {
                    var remote_func_name = (string)arg.value;
                    this.Call(remote_func_name, args);
                });
                // break;

                default:
                return arg.value;
            }
        }

        public void Call(string methodName, params object[] args)
        {
            var rnd = new Random();
            var rid = rnd.Next().ToString();

            var conv_args = new MethodCallArgData[args.Length];

            for (var i = 0; i < args.Length; ++i) {
                var val = this.FromArgToArgData("", i, args[i]);
                conv_args[i] = val;
            }

            var method_call_data = new MethodCallData {
                rid = rid,
                method_name = methodName,
                args = conv_args,
            };

            if (this.SendToRemote != null) {
                this.SendToRemote.Invoke(method_call_data);
            }
        }

        public void ReceiveFromRemote(MethodCallData method_call_data)
        {
            var method = this.Methods[method_call_data.method_name];
            if (method == null) {
                return;
            }

            var args = new object[method_call_data.args.Length];

            for (var i = 0; i < method_call_data.args.Length; ++i) {
                var val = this.FromArgDataToArg(method_call_data.args[i]);
                args[i] = val;
            }

            method.Invoke(args);
        }
    }
}
