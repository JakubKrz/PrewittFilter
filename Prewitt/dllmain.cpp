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
extern "C" __declspec(dllexport) void ExampleFunction(unsigned char* byteArray, int width, int height)
{
    // Filtr Prewitta dla kierunku poziomego
    int horizontalFilter[3][3] = {
        {-1, 0, 1},
        {-1, 0, 1},
        {-1, 0, 1}
    };

    // Filtr Prewitta dla kierunku pionowego
    int verticalFilter[3][3] = {
        {-1, -1, -1},
        { 0,  0,  0},
        { 1,  1,  1}
    };

    // Utwórz tymczasow¹ tablicê do przechowywania wyników
    unsigned char* resultArray = new unsigned char[width * height * 3];

    // Iteruj przez piksele (z pominiêciem ramki 1-pikselowej)
    for (int y = 1; y < height - 1; ++y) {
        for (int x = 1; x < width - 1; ++x) {
            for (int c = 0; c < 3; ++c) { // Iteruj przez sk³adowe koloru (R, G, B)
                int horizontalSum = 0;
                int verticalSum = 0;

                // Zastosuj filtr Prewitta
                for (int i = -1; i <= 1; ++i) {
                    for (int j = -1; j <= 1; ++j) {
                        horizontalSum += byteArray[((y + i) * width + (x + j)) * 3 + c] * horizontalFilter[i + 1][j + 1];
                        verticalSum += byteArray[((y + i) * width + (x + j)) * 3 + c] * verticalFilter[i + 1][j + 1];
                    }
                }

                // Przypisz wynik do nowego obrazu
                resultArray[(y * width + x) * 3 + c] = static_cast<unsigned char>(std::sqrt(horizontalSum * horizontalSum + verticalSum * verticalSum));
            }
        }
    }

    // Skopiuj wyniki z tablicy tymczasowej z powrotem do oryginalnej tablicy
    std::copy(resultArray, resultArray + width * height * 3, byteArray);

    // Zwolnij pamiêæ po tablicy tymczasowej
    delete[] resultArray;
}

