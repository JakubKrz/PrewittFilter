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
    push r15
    imul    r8, 3         ; R8 = width * 3 (bo 3 bajty gbr)

    mov r12, r8
    dec r12 ; do porowniania zeby pominac ostatni wiersz i klomne
    mov r13, r9
    dec r13 ; -||-

    xor     r10, r10       ; R10 = iterator wierszy (i)
    add r11, 3 ; zeby pominac 1 wiersz

outer_loop:
    ; Przygotowanie pêtli wewnêtrznej (iteracja po kolumnach)
    mov     rdi, rcx       ; RDI = wskaŸnik na obraz wejœciowy (przywrócenie pocz¹tkowego adresu)
    mov     rsi, rdx       ; RSI = wskaŸnik na obraz wyjœciowy (przywrócenie pocz¹tkowego adresu)

    xor     r11, r11       ; R13 = iterator kolumn (j)
    add r11, 3 ; zeby pominac 1 kolumne

inner_loop:
    ;Tutaj ma byc docelowo filtr prwitta
    
    xor r15, r15
    xorps xmm0, xmm0 
    xorps xmm1, xmm1

    ;dodawnie 1 wartosc z filtru x (lewy gorny rog)
    xor rax, rax
    mov r14, r11 ;zamiast ciagle przenosic r11 do r14 uzyc inngeo rejestru?
    sub r14, r8
    sub r14, 3 
    ;add al, byte PTR[rsi + r14]
    mov r15b, byte PTR[rsi + r14]
    add eax, r15d
    ;dodawnie 2 wartosc z filtru x (lewy srodkowy)
    add r14, r8                    ;przejscie do srodkowego
    mov r15b, byte PTR[rsi + r14] 
    add eax, r15d
    ;dodawnie 3 wartosc z filtru x (lewy dolny rog)
    add r14, r8
    mov r15b, byte PTR[rsi + r14]
    add eax, r15d
    ;Zapisanie wyniku dodawania w xmm1 (0)
    pinsrd xmm1, eax, 0 ; zmienic na mm?

     ;odjecie 4 wartosc z filtru x (prawy gorny rog)
    ;xorps xmm0, xmm0
    xor rax, rax
    mov r14, r11 
    sub r14, r8
    add r14, 3 
    mov r15b, byte PTR[rsi + r14]
    add eax, r15d
    ;odjecie 5 wartosc z filtru x (prawy srodkowy)
    add r14, r8
    mov r15b, byte PTR[rsi + r14]
    add eax, r15d
    ;odjecie 6 wartosc z filtru x (prawy dolny rog)
    add r14, r8
    mov r15b, byte PTR[rsi + r14]
    add eax, r15d
    ;zapisanie wyniku do xmm1
    pinsrd xmm0, eax, 0


    ;dodawnie 1 wartosc z filtru y (lewy gorny rog)
    xor rax, rax
    mov r14, r11 
    sub r14, r8
    sub r14, 3
    mov r15b, byte PTR[rsi + r14]
    add eax, r15d
    ;dodawnie 2 wartosc z filtru y (gorny srodkowy)
    add r14, 3
    mov r15b, byte PTR[rsi + r14]
    add eax, r15d
    ;dodawnie 3 wartosc z filtru y (prawy gorny rog)
    add r14, 3
    mov r15b, byte PTR[rsi + r14]
    add eax, r15d
    ;Zapisanie wyniku dodawania w xmm1 (1)
    pinsrd xmm1, eax, 1

    ;odjecie 4 wartosc z filtru y (lewy dolny rog)
    xor rax, rax
    mov r14, r11 
    add r14, r8
    sub r14, 3
    mov r15b, byte PTR[rsi + r14]
    add eax, r15d
    ;odjecie 5 wartosc z filtru y (dolny srodkowy)
    add r14, 3
    mov r15b, byte PTR[rsi + r14]
    add eax, r15d
    ;odjecie 6 wartosc z filtru y (prawy dolny rog)
    add r14, 3
    mov r15b, byte PTR[rsi + r14]
    add eax, r15d
    ;Zapisanie do xmm0 (1)
    pinsrd xmm0, eax, 1

    subps xmm1, xmm0 ;operacja x123-x456, y123-y456
    
    ;gradX (1) - gradY (1) - gradX (2) - gradY (2)
    ;Dodac sprawdzenie czy wartosc nie przekrqacz 255???
    ;mozna to robic w mm zamiast xmm

     ;Tutah kwadra, dodwaniae, zmiana na double, pierwiastkowanie, zmiana na int (chyba niepotrzebnie xmm, mozna zmiejszyc)
    pmulld xmm1, xmm1   ; podnosimy wartoœci do kwadratu
    phaddd xmm1, xmm1   ; dodajemy wartoœci
    cvtdq2pd    xmm1, xmm1 ; konwertujemy na dp
    sqrtpd  xmm1, xmm1   ; pierwiastej dp
    cvtpd2dq  xmm1 ,xmm1    ;zmiana z dp do int
    movd eax, xmm1 ; przeniesienie do eax 


    ;mov byte PTR[rdi + r11 + 2], al ; red
    ;mov byte PTR[rdi + r11 + 1], al ; green 
    mov byte PTR[rdi + r11 ], al  ;blue

    ; Inkrementacja iteratora kolumn (j)
    add r11, 1  ;zmienic na 3 jesli tylko 1 kolor
    ;inc rsi wtedy nie bedzie trzeba dodawac r11 przed kazdym adresowaniem 
    cmp r11, r12        ; sprawdzanie czy koniec wiersza (przedostatnia kolumna w wierszu)
    jl inner_loop     ; 

    ; Przesuniêcie wskaŸników na kolejny wiersz
    add     rcx, r8       ; wskaznik + width * 3
    add     rdx, r8       ; wskaznik + width * 3

    ; Inkrementacja iteratora wierszy (i)
    inc     r10
    cmp     r10, r13        ; Sprawdzanie czy przedostatni wiersz koniec obrazu
    jl      outer_loop     ; 

    pop r15
    ; Koniec
    ret

Myproc ENDP

END