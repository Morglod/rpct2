import { MethodCallDataArray, RPCT } from "./core";

export interface IBinarySerializer {
    toBytes(rpct: RPCT, data: MethodCallDataArray): Uint8Array;
    fromBytes(rpct: RPCT, data: Uint8Array): MethodCallDataArray;
};

export interface ISerializationChainPart {
    to(rpct: RPCT, data: Uint8Array): Uint8Array;
    from(rpct: RPCT, data: Uint8Array): Uint8Array;
};

export class SerializationChain implements IBinarySerializer {
    serializer!: IBinarySerializer;
    chain!: ISerializationChainPart[];

    constructor(serializer: IBinarySerializer = undefined!, chain: ISerializationChainPart[] = []) {
        this.serializer = serializer;
        this.chain = chain;
    }

    toBytes(rpct: RPCT, data: MethodCallDataArray): Uint8Array {
        let buf = this.serializer.toBytes(rpct, data);
        for (const chainPart of this.chain) {
            buf = chainPart.to(rpct, buf);
        }
        return buf;
    };

    fromBytes(rpct: RPCT, data: Uint8Array): MethodCallDataArray {
        let buf = data;
        for (let i = this.chain.length -1; i >= 0; --i) {
            buf = this.chain[i].from(rpct, buf);
        }
        return this.serializer.fromBytes(rpct, buf);
    };
}