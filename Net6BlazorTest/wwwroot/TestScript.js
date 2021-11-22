// .NET6

export function Hoge(arg) {
    Blazor.platform.readInt32Field(arg, 4);
}

/**
 * Tests .NET6 Byte Array Marshaling Update.
 * @param {Uint8Array} arg In .NET 5 or before, this arg will be string(base64 encorded).
 */
export function ByteArrayTest(arg) {
    console.log(arg);
}

/**
 * @param {number} addr
 * @param {number} length
 */
export function StringTest(addr, length) {
    const array = new Uint8Array(wasmMemory.buffer);
    const slice = array.slice(addr, addr + length);

    const data = slice.slice(12, length);
    data[0] = 'f';
    data[2] = 'g';
    data[4] = 'h';
}