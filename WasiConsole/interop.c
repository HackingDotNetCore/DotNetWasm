#include <driver.h>
#include <assert.h>
#include <string.h>
//﻿#include <mono/metadata/object.h>
//#include <mono/metadata/exception.h>
//#include <driver.h>


MonoObject* invoke_hello(MonoObject* target_instance, void* method_params[]) {
    static MonoMethod* method_hello = NULL;

    if (!method_hello) {
        method_hello = lookup_dotnet_method("WasiConsole.dll", "WasiConsole", "Program", "Hello", -1);
        assert(method_hello);
    }

    MonoObject* exception = NULL;
    MonoObject* res = mono_runtime_invoke(method_hello, target_instance, method_params, &exception);
    //assert(!exception);

    return res;
}

static int runtime_initialized = 0;

extern void _start(void);

__attribute__((export_name("fibonacci")))
int fibonacci(int len) {

    if (runtime_initialized == 0) {
        _start();
        runtime_initialized = 1;
    }

    static MonoMethod* method = NULL;

    if (!method) {
        method = lookup_dotnet_method("WasiConsole.dll", "WasiConsole", "Program", "Fibonacci", -1);
        assert(method);
    }

    void* method_params[] = { &len };
    MonoObject* exception = NULL;
    MonoObject* res = mono_runtime_invoke(method, NULL, method_params, &exception);

    return *(int*)mono_object_unbox(res);
}

__attribute__((export_name("hello")))
void hello(const char* value, int value_len) {

    if (runtime_initialized == 0) {
        _start();
        runtime_initialized = 1;
    }
    MonoString* text_string = mono_string_new_len(0, value, value_len);

    void* params[] = { text_string };
    invoke_hello(NULL, params);

    free(text_string);
}

__attribute__((import_name("print")))
void print(const char* value, int value_len);

void internal_print(MonoString* message) {
    char* message_utf8 = mono_string_to_utf8(message);
    print(message_utf8, strlen(message_utf8));
    free(message_utf8);
}

void attach_internal_calls()
{
    mono_add_internal_call("WasiConsole.Native::Print", internal_print);
}