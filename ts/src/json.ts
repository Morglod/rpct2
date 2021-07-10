import { MethodCallDataArray, RPCT } from "./core";
import { IBinarySerializer, ISerializationChainPart, SerializationChain } from "./io";

export abstract class RpctJson {
    static toString(rpct: RPCT, callData: MethodCallDataArray): string {
        return JSON.stringify(callData);
    }

    static toUtf8Bytes(rpct: RPCT, callData: MethodCallDataArray): Uint8Array {
        const str = JSON.stringify(callData);
        const buf = new Uint8Array(str.length);

        for (let i = 0; i < str.length; ++i) {
            buf[i] = str.charCodeAt(i);
        }
        
        return buf;
    }

    static fromString(rpct: RPCT, str: string): MethodCallDataArray {
        return JSON.parse(str);
    }

    static fromUtf8Bytes(rpct: RPCT, buf: Uint8Array): MethodCallDataArray
    {
        let str = String.fromCharCode.apply(String, Array.from(buf));
        return JSON.parse(str);
    }

    static serializationChain(chain: ISerializationChainPart[] = []): SerializationChain
    {
        return new SerializationChain(new RcptJsonBinarySerializer(), chain);
    }
}

export class RcptJsonBinarySerializer implements IBinarySerializer
{
    toBytes(rpct: RPCT, callData: MethodCallDataArray): Uint8Array
    {
        return RpctJson.toUtf8Bytes(rpct, callData);
    }

    fromBytes(rpct: RPCT, buf: Uint8Array): MethodCallDataArray
    {
        return RpctJson.fromUtf8Bytes(rpct, buf);
    }
}