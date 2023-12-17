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
extern "C" __declspec(dllexport) void ExampleFunction(unsigned char* resultArray, unsigned char* byteArray, int width, int height)
{

    int horizontalFilter[3][3] = {
        {-1, 0, 1},
        {-1, 0, 1},
        {-1, 0, 1}
    };

    int verticalFilter[3][3] = {
        {-1, -1, -1},
        { 0,  0,  0},
        { 1,  1,  1}
    };

    for (int y = 1; y < height - 1; ++y) {
        for (int x = 1; x < width - 1; ++x) {
            for (int c = 0; c < 1; ++c) { //zmienic na 1 jesli ma byc szare i odkomentowac resultarray
                int horizontalSum = 0;
                int verticalSum = 0;

                for (int i = -1; i <= 1; ++i) {
                    for (int j = -1; j <= 1; ++j) {
                        horizontalSum += byteArray[((y + i) * width + (x + j)) * 3 + c] * horizontalFilter[i + 1][j + 1];
                        verticalSum += byteArray[((y + i) * width + (x + j)) * 3 + c] * verticalFilter[i + 1][j + 1];
                    }
                }
                int gradient = static_cast<int>(std::sqrt(horizontalSum * horizontalSum + verticalSum * verticalSum));

                resultArray[(y * width + x) * 3 + c] = static_cast<unsigned char>(std::sqrt(horizontalSum * horizontalSum + verticalSum * verticalSum));
                resultArray[(y * width + x) * 3 + 1] = static_cast<unsigned char>(std::sqrt(horizontalSum * horizontalSum + verticalSum * verticalSum));
                resultArray[(y * width + x) * 3 + 2] = static_cast<unsigned char>(std::sqrt(horizontalSum * horizontalSum + verticalSum * verticalSum));
            }
        }
    }
}

