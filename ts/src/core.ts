export enum MethodCallArg_BuiltDataType {
    Unknown = 0,
    Null = -2,
    Buffer = -3,
    Float = -4,
    String = -5,
    Callback = -6,
    StringDictionary = -7,
};

export type MethodCallArgData = {
    name: string;
    index: number;
    data_type: number;
    value: unknown;
};

export type MethodCallData = {
    rid: string;
    method_name: string;
    args: MethodCallArgData[];
};

export type MethodCallDataArray = {
    list: MethodCallData[];
};

type RPCTCallback = (...args: any[]) => void;

export class RPCT {
    methods: Record<string, RPCTCallback> = {};
    sendToRemote: ((callData: MethodCallData) => void) | undefined;


    getDataTypeFromValue = (value: any) => {
        if (value == null) return MethodCallArg_BuiltDataType.Null;

        const type = typeof value;
        if (type == "number") return MethodCallArg_BuiltDataType.Float;
        if (type == "string") return MethodCallArg_BuiltDataType.String;
        if (value instanceof Uint8Array) return MethodCallArg_BuiltDataType.Buffer;
        if (type === "function") return MethodCallArg_BuiltDataType.Callback;

        return MethodCallArg_BuiltDataType.Unknown;
    };

    fromArgToArgData = (name: string, index: number, value: any): MethodCallArgData => {
        const data_type = this.getDataTypeFromValue(value);
        let out_value = null;

        switch (data_type) {
            case MethodCallArg_BuiltDataType.Unknown:
            throw new Error("unknown value type");
            // break;

            case MethodCallArg_BuiltDataType.Callback:
            var cb_name = "";
            cb_name = this._addCallback((...args: any[]) => {
                var original_func = value as RPCTCallback;
                original_func(...args);
                delete this.methods[cb_name];
            });
            out_value = cb_name;
            break;

            default:
            out_value = value;
            break;
        }

        return {
            name: name,
            index: index,
            data_type: data_type,
            value: out_value,
        };
    };

    addMethod = (methodName: string, func: RPCTCallback) => {
        this.methods[methodName] = func;
    };

    _addCallback = (func: RPCTCallback): string => {
        var rndName = Math.random().toString() + '_' + Math.random().toString();
        this.addMethod(rndName, func);
        return rndName;
    };

    fromArgDataToArg = (arg: MethodCallArgData) => {
        switch (arg.data_type) {
            case MethodCallArg_BuiltDataType.Unknown:
            throw new Error("unknown value type");
            // break;

            case MethodCallArg_BuiltDataType.Callback:
            return ((...args: any[]) => {
                var remote_func_name = arg.value as string;
                this.call(remote_func_name, ...args);
            });
            // break;

            default:
            return arg.value;
        }
    };

    call = (methodName: string, ...args: any[]) => {
        const rid = Math.random().toString() + '_' + Math.random().toString();

        const conv_args: MethodCallArgData[] = new Array(args.length);

        for (let i = 0; i < args.length; ++i) {
            const val = this.fromArgToArgData("", i, args[i]);
            conv_args[i] = val;
        }

        const method_call_data: MethodCallData = {
            rid: rid,
            method_name: methodName,
            args: conv_args,
        };

        if (this.sendToRemote) {
            this.sendToRemote(method_call_data);
        }
    }

    receiveFromRemote = (method_call_data: MethodCallData) => {
        const method = this.methods[method_call_data.method_name];
        if (!method) {
            return;
        }

        const args = new Array(method_call_data.args.length);

        for (let i = 0; i < method_call_data.args.length; ++i) {
            const val = this.fromArgDataToArg(method_call_data.args[i]);
            args[i] = val;
        }

        method.apply(undefined, args);
    }
}
