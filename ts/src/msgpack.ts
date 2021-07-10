import { encode, decode } from "@msgpack/msgpack";
import { MethodCallDataArray, RPCT } from "./core";
import { IBinarySerializer, ISerializationChainPart, SerializationChain } from "./io";

export abstract class RpctMsgpack
{
    static toBytes(rpct: RPCT, callData: MethodCallDataArray): Uint8Array
    {
        return encode(callData);
    }

    static fromBytes(rpct: RPCT, buf: Uint8Array): MethodCallDataArray
    {
        return decode(buf) as MethodCallDataArray;
    }

    static serializationChain(chain: ISerializationChainPart[] = []): SerializationChain
    {
        return new SerializationChain(new RpctMsgpackBinarySerializer(), chain);
    }
}

export class RpctMsgpackBinarySerializer implements IBinarySerializer
{
    toBytes(rpct: RPCT, callData: MethodCallDataArray): Uint8Array
    {
        return RpctMsgpack.toBytes(rpct, callData);
    }

    fromBytes(rpct: RPCT, buf: Uint8Array): MethodCallDataArray
    {
        return RpctMsgpack.fromBytes(rpct, buf);
    }
}