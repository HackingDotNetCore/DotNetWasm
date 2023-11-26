using System.Text;

using Wasmtime;

// Definizione funzionalità WASI
var config = new Config()
    .WithReferenceTypes(true)
    .WithDebugInfo(true)
    .WithSIMD(true)
    .WithBulkMemory(true)
    .WithWasmThreads(true);
//.WithFuelConsumption(true)
//.WithEpochInterruption(true);

using var engine = new Engine(config);
using var store = new Store(engine);

// Definizione dell'ambiente WASI
var wasiConfiguration = new WasiConfiguration().WithInheritedStandardOutput()
    .WithInheritedStandardInput()
    .WithInheritedArgs()
    .WithInheritedStandardError()
    .WithInheritedEnvironment()
    .WithPreopenedDirectory(Environment.CurrentDirectory, ".");
store.SetWasiConfiguration(wasiConfiguration);

//store.SetLimits();
//store.SetEpochDeadline(10000);

// Carico il modulo
using var module = Module.FromFile(engine, @"..\..\..\..\WasiConsole\bin\Release\net8.0\wasi-wasm\AppBundle\WasiConsole.wasm");

using var linker = new Linker(engine);
// Definizione funzioni previste da WASI
linker.DefineWasi();
// Aggiunta funzione Print
DefinePrintFunction();

PrintFunctions();

var instance = linker.Instantiate(store, module);
var memory = instance.GetMemory("memory")!;

var fibonacci = instance.GetFunction<int, int>("fibonacci")!;
Console.WriteLine($"Fibonacci: {fibonacci(10)}");
Console.ReadLine();

var text = "DotNet Conference 2013!";

// Ricerca funzioni di memoria
var alloc = instance.GetFunction<int, int>("malloc")!;
var free = instance.GetAction<int>("free")!;

// Alloco la memory per il testo
var textPtr = alloc(text.Length);
// Scrivo il testo nella memory
memory.WriteString(textPtr, text, Encoding.UTF8);

// Chiamo la funzione con puntatore e dimensione
var action = instance.GetAction<int, int>("hello")!;
action(textPtr, text.Length);

// Libero la memoria
free(textPtr);

return;

void PrintFunctions()
{
    Console.WriteLine("Exports:");
    foreach (var export in module.Exports)
    {
        Console.WriteLine(export.Name);
    }

    Console.WriteLine("Imports:");
    foreach (var import in module.Imports)
    {
        Console.WriteLine(import.Name);
    }
}

void DefinePrintFunction()
{
    linker.Define(
        "env",
        "print",
        Function.FromCallback(store, (Caller caller, int valuePtr, int valueLen) =>
        {
            var memory = caller.GetMemory("memory");
            if (memory is null)
                throw new Exception("Missing export 'memory'");

            // Leggo tramite puntatore e dimensione
            var value = memory.ReadString(valuePtr, valueLen, Encoding.UTF8);
            Console.WriteLine(value);
        })
    );
}