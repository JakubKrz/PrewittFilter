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
extern "C" __declspec(dllexport) void FiltrCpp(unsigned char* resultArray, unsigned char* byteArray, int width, int height)
{

    for (int y = 1; y < height - 1; ++y) {
        for (int x = 1; x < width - 1; ++x) {
            for (int c = 0; c < 3; ++c) {
                int horizontalSum =
                    - byteArray[((y - 1) * width + (x - 1)) * 3 + c]
                    - byteArray[(y * width + (x - 1)) * 3 + c]
                    - byteArray[((y + 1) * width + (x - 1)) * 3 + c]
                    + byteArray[((y - 1) * width + (x + 1)) * 3 + c]
                    + byteArray[(y * width + (x + 1)) * 3 + c]
                    + byteArray[((y + 1) * width + (x + 1)) * 3 + c];

                int verticalSum =
                    - byteArray[((y - 1) * width + (x - 1)) * 3 + c]
                    - byteArray[((y - 1) * width + x) * 3 + c]
                    - byteArray[((y - 1) * width + (x + 1)) * 3 + c]
                    + byteArray[((y + 1) * width + (x - 1)) * 3 + c]
                    + byteArray[((y + 1) * width + x) * 3 + c]
                    + byteArray[((y + 1) * width + (x + 1)) * 3 + c];

                int gradient = static_cast<int>(std::sqrt(horizontalSum * horizontalSum + verticalSum * verticalSum));

                resultArray[(y * width + x) * 3 + c] = static_cast<unsigned char>(gradient);
            }
        }
    }
}

