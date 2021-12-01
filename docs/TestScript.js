//@ts-check
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
 * Parse Json encorded as UTF-8 Text
 * @param {number} arg 配列オブジェクトのアドレス
 * @param {any} len 配列長
 */
export function UTF8JsonTest(arg, len) {
    const array = new Uint8Array(wasmMemory.buffer, arg + dotnetArrayOffset, len);
    const str = len > nativeLen ? nativeDecoder.decode(array) : JSTextDecode(array);
    const obj = JSON.parse(str);
    console.log(obj.Id);
}

/**
 * 
 * @param {any} arg
 */
export function JsonTest(arg) {
    console.log(arg.id);
}

/**
 * Parse Json encorded as UTF-8 Text
 * @param {number} arg 配列オブジェクトのアドレス
 * @param {any} len 配列長
 */
export function BinaryTest(arg, len) {
    const array = new Uint8Array(wasmMemory.buffer, arg + dotnetArrayOffset, len);
    return BINDING.js_typed_array_to_array(array);
    //mono_wasm_string_from_utf16(str: CharPtr, len: number): MonoString;も多分使える、stringのallocならそっち
}
