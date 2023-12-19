.CODE
FiltrAsm PROC

applyPrewitt:
    ; Parametry wejœciowe:
    ;   rcx - wskaŸnik na obraz wejœciowy (GBR24)
    ;   rdx - wskaŸnik na obraz wyjœciowy (GBR24)
    ;   r8  - szerokoœæ obrazu (width)
    ;   r9  - wysokoœæ obrazu (height)
            
    push r12
    push r13
    push r14
    push r15
    push rdi
    push rsi
    ;r10 iteratot i
    ;r11 iterator j

    imul r8, 3         ; R8 = width * 3 (bo 3 bajty gbr)

    mov r12, r8
    sub r12, 3 ; do porowniania zeby pominac ostatni wiersz 
    mov r13, r9
    dec r13 ; -||- ostatnia kolumne

    xor r10, r10       ; R10 = iterator wierszy (i)
    inc r10 ; zeby pominac 1 wiersz
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
    xorps xmm3, xmm3
    xorps xmm4, xmm4
    xorps xmm5, xmm5
    xorps xmm6, xmm6
    ;dodawnie 1 wartosc z filtru x (lewy gorny rog)
    xor rax, rax
    mov r14, r11 ;zamiast ciagle przenosic r11 do r14 uzyc inngeo rejestru?
    sub r14, r8
    sub r14, 3 
    mov r15b, byte PTR[rsi + r14]
    pinsrw xmm3, r15, 0
    mov r15b, byte PTR[rsi + r14 + 1]
    pinsrw xmm3, r15, 4
    ;dodawnie 2 wartosc z filtru x (lewy srodkowy)
    add r14, r8                    ;przejscie do srodkowego
    mov r15b, byte PTR[rsi + r14] 
    pinsrw xmm3, r15, 1
    mov r15b, byte PTR[rsi + r14 + 1]
    pinsrw xmm3, r15, 5
    ;dodawnie 3 wartosc z filtru x (lewy dolny rog)
    add r14, r8
    mov r15b, byte PTR[rsi + r14]
    pinsrw xmm3, r15, 2
    mov r15b, byte PTR[rsi + r14 + 1]
    pinsrw xmm3, r15, 6
    ;dodanie x1+x2+x3 (1) x1+x2+x3(2)
    phaddw  xmm3, xmm3
    phaddw  xmm3, xmm3

     ;odjecie 4 wartosc z filtru x (prawy gorny rog)
    ;xorps xmm0, xmm0
    xor rax, rax
    mov r14, r11 
    sub r14, r8
    add r14, 3 
    mov r15b, byte PTR[rsi + r14]
    pinsrw xmm4, r15, 0
    mov r15b, byte PTR[rsi + r14 + 1]
    pinsrw xmm4, r15, 4
    ;odjecie 5 wartosc z filtru x (prawy srodkowy)
    add r14, r8
    mov r15b, byte PTR[rsi + r14]
    pinsrw xmm4, r15, 1
    mov r15b, byte PTR[rsi + r14 + 1]
    pinsrw xmm4, r15, 5
    ;odjecie 6 wartosc z filtru x (prawy dolny rog)
    add r14, r8
    mov r15b, byte PTR[rsi + r14]
    pinsrw xmm4, r15, 2
    mov r15b, byte PTR[rsi + r14 + 1]
    pinsrw xmm4, r15, 6
    ;dodanie x4+x5+x6 (1) x4+x5+x6(2)
    phaddw  xmm4, xmm4
    phaddw  xmm4, xmm4
    
    ;dodawnie 1 wartosc z filtru y (lewy gorny rog)
    xor rax, rax
    mov r14, r11 
    sub r14, r8
    sub r14, 3
    mov r15b, byte PTR[rsi + r14]
    pinsrw xmm5, r15d, 0
    mov r15b, byte PTR[rsi + r14 + 1]
    pinsrw xmm5, r15d, 4
    ;dodawnie 2 wartosc z filtru y (gorny srodkowy)
    add r14, 3
    mov r15b, byte PTR[rsi + r14]
    pinsrw xmm5, r15d, 1
    mov r15b, byte PTR[rsi + r14 + 1]
    pinsrw xmm5, r15d, 5
    ;dodawnie 3 wartosc z filtru y (prawy gorny rog)
    add r14, 3
    mov r15b, byte PTR[rsi + r14]
    pinsrw xmm5, r15d, 2
    mov r15b, byte PTR[rsi + r14 + 1]
    pinsrw xmm5, r15d, 6
    ;dodanie y1+y2+y3 (1) y1+y2+y3 (2)
    phaddw xmm5, xmm5
    phaddw xmm5, xmm5

    ;odjecie 4 wartosc z filtru y (lewy dolny rog)
    xor rax, rax
    mov r14, r11 
    add r14, r8
    sub r14, 3
    mov r15b, byte PTR[rsi + r14]
    pinsrw xmm6, r15d, 0
    mov r15b, byte PTR[rsi + r14 + 1]
    pinsrw xmm6, r15d, 4
    ;odjecie 5 wartosc z filtru y (dolny srodkowy)
    add r14, 3
    mov r15b, byte PTR[rsi + r14]
    pinsrw xmm6, r15d, 1
    mov r15b, byte PTR[rsi + r14 + 1]
    pinsrw xmm6, r15d, 5
    ;odjecie 6 wartosc z filtru y (prawy dolny rog)
    add r14, 3
    mov r15b, byte PTR[rsi + r14]
    pinsrw xmm6, r15d, 2
    mov r15b, byte PTR[rsi + r14 + 1]
    pinsrw xmm6, r15d, 6
    ;dodanie y4+y5+y6 (1) y4+y5+y6 (2)
    phaddw xmm6, xmm6
    phaddw xmm6, xmm6

    ; To raczej da sie zoptymalziwac jescze
    ;Wyciaganie do xmm1 xmm0
    ;xmm1 ---   y123(2) - x123(2) - y123(1) - x123(1)
    ;xmm0 ---   y456(2) - x456(2) - y456(1) - x456(1)
    pextrw  r15d, xmm3, 0
    pinsrd xmm1, r15d, 0 ;x123 (1)
    pextrw  r15d, xmm3, 1
    pinsrd xmm1, r15d, 2 ;x123 (2)
    pextrw r15d, xmm4, 0
    pinsrd xmm0, r15d, 0 ;x456 (1)
    pextrw r15d, xmm4, 1
    pinsrd xmm0, r15d, 2 ;x456 (2)

    pextrw  r15d, xmm5, 0
    pinsrd xmm1, r15d, 1 ;y123 (1)
    pextrw  r15d, xmm5, 1
    pinsrd xmm1, r15d, 3 ;y123 (2)
    pextrw r15d, xmm6, 0
    pinsrd xmm0, r15d, 1 ;y456 (1)
    pextrw r15d, xmm6, 1
    pinsrd xmm0, r15d, 3 ;y456 (2)


    subps xmm1, xmm0 ;operacja x123-x456, y123-y456 na (1) i (2)
    
    ;gradX (1) - gradY (1) - gradX (2) - gradY (2)
    ;Dodac sprawdzenie czy wartosc nie przekrqacz 255??? TODO

     ;Tutah kwadrat, dodwaniae, zmiana na double, pierwiastkowanie, zmiana na int (chyba niepotrzebnie xmm, mozna zmiejszyc)
    pmulld xmm1, xmm1   ; podnosimy wartoœci do kwadratu
    phaddd xmm1, xmm1   ; dodajemy wartoœci
    cvtdq2pd    xmm1, xmm1 ; konwertujemy na dp
    sqrtpd  xmm1, xmm1   ; pierwiastek dp
    cvtpd2dq  xmm1 ,xmm1    ;zmiana z dp do int
    movd eax, xmm1 ; przeniesienie do eax (1)
    mov byte PTR[rdi + r11 ], al  ;
    pextrd  eax, xmm1, 1 ;przniesienie do eax (2)
    mov byte PTR[rdi + r11 + 1], al  ;

    ; Inkrementacja iteratora kolumn (j)
    add r11, 2 ; Dodanie 2 bo w kazdej petli obliczmy 2 wartosci
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

    ; Koniec
    pop rsi
    pop rdi
    pop r15
    pop r14
    pop r13
    pop r12
    ret

FiltrAsm ENDP

END