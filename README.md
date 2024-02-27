# Prewitt Filter using C++ and ASM DLL

## Description

The goal of the project was to create an application capable of detecting edges in images using the Prewitt algorithm. The project involved implementing two dynamic libraries: one written in C++, and the other in assembly language, utilizing vector instructions and multithreading.

## Program Operation

The program allows the user to load an image and then perform edge detection using the Prewitt algorithm. The user can choose the number of threads to utilize and select the implementation library (C++ or assembly). The detection results are displayed in the graphical interface, saved to a new image file, and the processing time of the selected library is also shown.

**Supported Input File Format:** BMP (24bpp)

**Threads Selection Range:** 1-64

**Screenshots of the Application in Action:**

![image](https://github.com/JakubKrz/PrewittFilter/assets/91898433/112338b5-a6b4-4e8c-b145-db27de144977)
![image](https://github.com/JakubKrz/PrewittFilter/assets/91898433/5d2adedb-e153-4d15-8f33-5afae30912d3)

### Comparison of Execution Times
The tests were conducted in release mode with code optimization /O2 for the C++ library. Each test was repeated 5 times, excluding the first measurement.
| Number of Threads | C++ Library Time (ms) | ASM Library Time (ms) |
|-------------------|------------------------|-----------------------|
| 1                 | 1221                    | 510                    |
| 2                 | 615                     | 273                   |
| 4                 | 455                     | 193                    |
| 8                 | 331                     | 136                    |
| 16                | 363                     | 138                    |
| 32                | 322                     | 146                     |
| 64                | 336                      | 142                     |

On average, the assembly library was approximately 2.3 times faster than the C++ library
- Processor: Intel i5-11 (8 logical cores)
- Image Size: 70 316 KB
