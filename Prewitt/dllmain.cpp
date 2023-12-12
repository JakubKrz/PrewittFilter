// dllmain.cpp : Defines the entry point for the DLL application.
#include "pch.h"
#include <iostream>

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
                     )
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;

}
extern "C" __declspec(dllexport) void ExampleFunction(unsigned char* byteArray, int length)
{
    std::cout << "Przetwarzanie tablicy bajtow: ";
    for (int i = 0; i < 20; ++i)
    {
        std::cout << static_cast<int>(byteArray[i]) << " ";
    }
    for (int i = 0; i < length; i += 3)
    {
        // Ustawiamy wartoœæ sk³adowej Red na maksymaln¹ (255), a pozosta³e na 0
        //byteArray[i] = 0;     // Blue
        //byteArray[i + 1] = 0;   // Green
        byteArray[i + 2] = 255;   // Red
    }
    // Implementacja funkcji
}

