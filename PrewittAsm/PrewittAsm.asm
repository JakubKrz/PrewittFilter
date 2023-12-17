.CODE
Myproc PROC

applyPrewitt:
    ; Parametry wejœciowe:
    ;   rcx - wskaŸnik na obraz wejœciowy (GBR24)
    ;   rdx - wskaŸnik na obraz wyjœciowy (GBR24)
    ;   r8  - szerokoœæ obrazu (width)
    ;   r9  - wysokoœæ obrazu (height)
            
    ;r10 iteratot i
    ;r11 iterator j

    imul    r8, 3         ; R8 = width * 3 (bo 3 bajty gbr)

    mov r12, r8
    dec r12 ; do porowniania zeby pominac ostatni wiersz i klomne ( tu chyba ma byc odjac 3?)
    mov r13, r9
    dec r13 ; -||-

    mov r10, 1; zeby pominac 1 wiersz

outer_loop:
    ; Przygotowanie pêtli wewnêtrznej (iteracja po kolumnach)
    mov     rdi, rcx       ; RDI = wskaŸnik na obraz wejœciowy (przywrócenie pocz¹tkowego adresu)
    mov     rsi, rdx       ; RSI = wskaŸnik na obraz wyjœciowy (przywrócenie pocz¹tkowego adresu)

    xor r11, r11     ; R13 = iterator kolumn (j)
   ; zeby pominac 1 kolumne

inner_loop:
    ;Tutaj ma byc docelowo filtr prwitta

    xorps xmm0, xmm0
    xorps xmm1, xmm1

    ;dodawnie 1 wartosc z filtru x (lewy gorny rog)
    xor rax, rax
    mov r14, r11 
    sub r14, r8
    sub r14, 3
    add al, byte PTR[rsi + r14]
    pinsrd xmm0, eax, 0
    addps xmm1, xmm0

    ;dodawnie 2 wartosc z filtru x (lewy srodkowy)
    xorps xmm0, xmm0
    xor rax, rax
    mov r14, r11
    sub r14, 3
    add al, byte PTR[rsi + r14]
    pinsrd xmm0, eax, 0
    addps xmm1, xmm0

    ;dodawnie 3 wartosc z filtru x (lewy dolny rog)
    xorps xmm0, xmm0
    xor rax, rax
    mov r14, r11
    add r14, r8
    sub r14, 3
    add al, byte PTR[rsi + r14]
    pinsrd xmm0, eax, 0
    addps xmm1, xmm0

    ;odjecie 4 wartosc z filtru x (prawy gorny rog)
    xorps xmm0, xmm0
    xor rax, rax
    mov r14, r11 
    sub r14, r8
    add r14, 3
    add al, byte PTR[rsi + r14]
    pinsrd xmm0, eax, 0
    subps xmm1, xmm0

    ;odjecie 5 wartosc z filtru x (prawy srodkowy)
    xorps xmm0, xmm0
    xor rax, rax
    mov r14, r11
    add r14, 3
    add al, byte PTR[rsi + r14]
    pinsrd xmm0, eax, 0
    subps xmm1, xmm0

    ;odjecie 6 wartosc z filtru x (prawy dolny rog)
    xorps xmm0, xmm0
    xor rax, rax
    mov r14, r11
    add r14, r8
    add r14, 3
    add al, byte PTR[rsi + r14]
    pinsrd xmm0, eax, 0
    subps xmm1, xmm0
    
    ;Trzeba dodac filt w kierunku y i 2 pixel naraz zeby 2 na raz przetwarzac i wykorzystac caly rejestr xmm
    ;gradX (1) - gradY (1) - gradX (2) - gradY (2)
    ;caly rejestr ^2
    ;wyciagnac  dodac zrobic pierwiastek i zapisac
    ;Dodac sprawdzenie czy wartosc nie przekrqacz 255???

    ;tu dodac wartosc gradientx^2 + gradinty^2
    ;na razei tylko grandient x
    pmulld xmm1, xmm1   ; gradient * gradient
    pextrd	eax, xmm1, 0    ; wyciagniecie wartosci do eax
    ; tu powinno sie to dodac
    
    xorps xmm3, xmm3
    cvtsi2sd    xmm3, eax ; z eax do smm3 zeby zrobi pierwiastek
    sqrtsd xmm3, xmm3   ;pierwistek z gradienta
    
    xor rax, rax
    cvtsd2si   eax, xmm3


    mov byte PTR[rdi + r11 + 2], al ; red
    mov byte PTR[rdi + r11 + 1], al ; green
    mov byte PTR[rdi + r11 ], al  ;blue



    ;test eax, eax     ; Test the sign of EAX
    ;jns  skip_negate  ; Jump if not signed (negative)
    ;neg  eax          ; Negate EAX if signed (negative)

    ;skip_negate:
    ; Inkrementacja iteratora kolumn (j)
    add r11, 3
    cmp     r11, r12        ; sprawdzanie czy koniec wiersza (przedostatnia kolumna w wierszu)
    jl      inner_loop     ; 

    ; Przesuniêcie wskaŸników na kolejny wiersz
    add     rcx, r8       ; wskaznik + width * 3
    add     rdx, r8       ; wskaznik + width * 3

    ; Inkrementacja iteratora wierszy (i)
    inc     r10
    cmp     r10, r13        ; Sprawdzanie czy przedostatni wiersz koniec obrazu
    jl      outer_loop     ; 

    ; Koniec
    ret

Myproc ENDP

END