sostituire WasiApp.Native.targets in C:\Program Files\dotnet\packs\Microsoft.NET.Runtime.WebAssembly.Wasi.Sdk\8.0.0\Sdk
sostituire main.c in C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Runtime.Mono.wasi-wasm\8.0.0\runtimes\wasi-wasm\native\src

esecuzione
wasmtime .\WasiConsole.wasm --dir .

chiamata funzione
wasmtime .\WasiConsole.wasm --dir . --invoke fibonacci 10

