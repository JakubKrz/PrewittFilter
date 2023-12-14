.CODE
Myproc PROC

applyPrewitt:
    ; Parametry wej�ciowe:
    ;   rcx - wska�nik na obraz wej�ciowy (GBR24)
    ;   rdx - wska�nik na obraz wyj�ciowy (GBR24)
    ;   r8  - szeroko�� obrazu (width)
    ;   r9  - wysoko�� obrazu (height)

    ;r10 iteratot i
    ;r11 iterator j

    imul    r8, 3         ; R8 = width * 3 (bo 3 bajty gbr)
    xor     r10, r10       ; R10 = iterator wierszy (i)

outer_loop:
    ; Przygotowanie p�tli wewn�trznej (iteracja po kolumnach)
    mov     rdi, rcx       ; RDI = wska�nik na obraz wej�ciowy (przywr�cenie pocz�tkowego adresu)
    mov     rsi, rdx       ; RSI = wska�nik na obraz wyj�ciowy (przywr�cenie pocz�tkowego adresu)

    xor     r11, r11       ; R13 = iterator kolumn (j)

inner_loop:
    ;Tutaj ma byc docelowo filtr prwitta
    mov   byte PTR[rdi + r11 + 2], 255      ; ustawianie Red na 255

    ; Inkrementacja iteratora kolumn (j)
    add r11, 3
    cmp     r11, r8        ; sprawdzanie czy koniec wiersza (ostatnia kolumna w wierszu)
    jl      inner_loop     ; 

    ; Przesuni�cie wska�nik�w na kolejny wiersz
    add     rcx, r8       ; wskaznik + width * 3
    add     rdx, r8       ; wskaznik + width * 3

    ; Inkrementacja iteratora wierszy (i)
    inc     r10
    cmp     r10, r9        ; Sprawdzanie czy koniec obrazu
    jl      outer_loop     ; 

    ; Koniec
    ret

Myproc ENDP

END