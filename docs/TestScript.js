﻿// .NET6

/**
 * Tests Js Call
 * @param {any} arg 引数。コンソールに書き込まれます。
 */
export function Hoge(arg) {
    console.log(arg);
}

/**
 * Tests .NET6 Byte Array Marshaling Update.
 * @param {Uint8Array} arg In .NET 5 or before, this arg will be string(base64 encorded).
 */
export function ByteArrayTest(arg) {
    console.log(arg);
}

/**
 * Tests String Reference
 * @param {number} addr CLI Stringへのアドレス
 * @param {number} length 文字列長(UTF-16で)
 */
export function StringTest(addr, length) {
    const array = new Uint8Array(wasmMemory.buffer);
    const slice = array.slice(addr, addr + length);

    const data = slice.slice(12, length);
    data[0] = 'f';
    data[2] = 'g';
    data[4] = 'h';
}