// .NET6
import JSTextDecode from "./TextDecoder.js";

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

const dotnetArrayOffset = 16;
const nativeLen = 256; // おおよそこれくらいのサイズまではJS実装のほうが高速
const nativeDecoder = new TextDecoder();

/**
 * @type Uint8Array
 * */
let array = null;

/**
 * Parse Json encorded as UTF-8 Text
 * @param {number} arg 配列オブジェクトのアドレス
 * @param {any} len 配列長
 */
export function UTF8JsonTest(arg, len) {
    if (array == null) {
        array = new Uint8Array(wasmMemory.buffer);
    }
    const target = array.subarray(arg + dotnetArrayOffset, arg + dotnetArrayOffset + len);
    const str = len > nativeLen ? nativeDecoder.decode(target) : JSTextDecode(target);
    const obj = JSON.parse(str);
    console.log(obj.id);
}

/**
 * 
 * @param {string} arg
 */
export function JsonTest(arg) {
    console.log(arg.id);
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