.CODE
FiltrAsm PROC

applyPrewitt:
    ; Input parameters:
    ;   rcx - pointer to input image (GBR24)
    ;   rdx - pointer to output image (GBR24)
    ;   r8  - image width (width)
    ;   r9  - image height (height)
    mov eax, DWORD PTR[rsp + 40] ; start
    mov ebx, DWORD PTR[rsp + 48] ; end

    push r12
    push r13
    push r14
    push r15
    push rdi
    push rsi

        ;r10 - iterator i
        ;r11 - iterator j
    imul r8, 3      ; R8 = width * 3 (because 3 bytes per pixel, GBR)

    mov r12, r8
    sub r12, 3      ; for comparison to skip the last column 
    mov r13, rbx    ; setting the last processed row for comparison at the end of the loop

    xor r10, r10    ; R10 = row iterator (i)
    mov r10, rax    ; setting the row iterator to the first row to process
   
    imul rax, r8    ; start row * width
    add rcx, rax    ; move pointer to input image to the start row
    add rdx, rax    ; move pointer to output image to the start row
outer_loop:
        ; Prepare inner loop (column iteration)
    mov rdi, rcx    ; RDI = pointer to input image (restore initial address)
    mov rsi, rdx    ; RSI = pointer to output image (restore initial address)

    xor r11, r11    ; R11 = column iterator (j)
    add r11, 3      ; to skip the first column

inner_loop:
    
    xor r15, r15
    xorps xmm0, xmm0 
    xorps xmm1, xmm1
    xorps xmm3, xmm3
    xorps xmm4, xmm4
    xorps xmm5, xmm5
    xorps xmm6, xmm6
        ;adding 1st value from x filter (top left)
    xor rax, rax
    mov r14, r11    ;to avoid repeatedly moving r11 to r14, use another register?
    sub r14, r8
    sub r14, 3 
    mov r15b, byte PTR[rsi + r14]
    pinsrw xmm3, r15, 0
    mov r15b, byte PTR[rsi + r14 + 1]
    pinsrw xmm3, r15, 4
        ;adding 2nd value from x filter (top middle)
    add r14, r8      ;move to the middle
    mov r15b, byte PTR[rsi + r14] 
    pinsrw xmm3, r15, 1
    mov r15b, byte PTR[rsi + r14 + 1]
    pinsrw xmm3, r15, 5
        ;adding 3rd value from x filter (top right)
    add r14, r8
    mov r15b, byte PTR[rsi + r14]
    pinsrw xmm3, r15, 2
    mov r15b, byte PTR[rsi + r14 + 1]
    pinsrw xmm3, r15, 6
        ;adding x1+x2+x3 (1) x1+x2+x3(2)
    phaddw  xmm3, xmm3
    phaddw  xmm3, xmm3

        ;subtracting 4th value from x filter (bottom left)
    xor rax, rax
    mov r14, r11 
    sub r14, r8
    add r14, 3 
    mov r15b, byte PTR[rsi + r14]
    pinsrw xmm4, r15, 0
    mov r15b, byte PTR[rsi + r14 + 1]
    pinsrw xmm4, r15, 4
        ;subtracting 5th value from x filter (bottom middle)
    add r14, r8
    mov r15b, byte PTR[rsi + r14]
    pinsrw xmm4, r15, 1
    mov r15b, byte PTR[rsi + r14 + 1]
    pinsrw xmm4, r15, 5
        ;subtracting 6th value from x filter (bottom right)
    add r14, r8
    mov r15b, byte PTR[rsi + r14]
    pinsrw xmm4, r15, 2
    mov r15b, byte PTR[rsi + r14 + 1]
    pinsrw xmm4, r15, 6
        ;adding x4+x5+x6 (1) x4+x5+x6(2)
    phaddw  xmm4, xmm4
    phaddw  xmm4, xmm4
    
        ;adding 1st value from y filter (top left)
    xor rax, rax
    mov r14, r11 
    sub r14, r8
    sub r14, 3
    mov r15b, byte PTR[rsi + r14]
    pinsrw xmm5, r15d, 0
    mov r15b, byte PTR[rsi + r14 + 1]
    pinsrw xmm5, r15d, 4
        ;adding 2nd value from y filter (top middle)
    add r14, 3
    mov r15b, byte PTR[rsi + r14]
    pinsrw xmm5, r15d, 1
    mov r15b, byte PTR[rsi + r14 + 1]
    pinsrw xmm5, r15d, 5
        ;adding 3rd value from y filter (top right)
    add r14, 3
    mov r15b, byte PTR[rsi + r14]
    pinsrw xmm5, r15d, 2
    mov r15b, byte PTR[rsi + r14 + 1]
    pinsrw xmm5, r15d, 6
        ;adding y1+y2+y3 (1) y1+y2+y3 (2)
    phaddw xmm5, xmm5
    phaddw xmm5, xmm5

        ;subtracting 4th value from y filter (bottom left)
    xor rax, rax
    mov r14, r11 
    add r14, r8
    sub r14, 3
    mov r15b, byte PTR[rsi + r14]
    pinsrw xmm6, r15d, 0
    mov r15b, byte PTR[rsi + r14 + 1]
    pinsrw xmm6, r15d, 4
        ;subtracting 5th value from y filter (bottom middle)
    add r14, 3
    mov r15b, byte PTR[rsi + r14]
    pinsrw xmm6, r15d, 1
    mov r15b, byte PTR[rsi + r14 + 1]
    pinsrw xmm6, r15d, 5
        ;subtracting 6th value from y filter (bottom right)
    add r14, 3
    mov r15b, byte PTR[rsi + r14]
    pinsrw xmm6, r15d, 2
    mov r15b, byte PTR[rsi + r14 + 1]
    pinsrw xmm6, r15d, 6
        ;adding y4+y5+y6 (1) y4+y5+y6 (2)
    phaddw xmm6, xmm6
    phaddw xmm6, xmm6

        ;Extracting into xmm1 and xmm0
        ;This section could likely be rewritten for better optimization.
        ;xmm1 ---   y123(2) - x123(2) - y123(1) - x123(1)
        ;xmm0 ---   y456(2) - x456(2) - y456(1) - x456(1)
    pextrw  r15d, xmm3, 0
    pinsrd xmm1, r15d, 0    ;x123 (1)
    pextrw  r15d, xmm3, 1
    pinsrd xmm1, r15d, 2    ;x123 (2)
    pextrw r15d, xmm4, 0
    pinsrd xmm0, r15d, 0    ;x456 (1)
    pextrw r15d, xmm4, 1
    pinsrd xmm0, r15d, 2    ;x456 (2)

    pextrw  r15d, xmm5, 0
    pinsrd xmm1, r15d, 1    ;y123 (1)
    pextrw  r15d, xmm5, 1
    pinsrd xmm1, r15d, 3    ;y123 (2)
    pextrw r15d, xmm6, 0
    pinsrd xmm0, r15d, 1    ;y456 (1)
    pextrw r15d, xmm6, 1
    pinsrd xmm0, r15d, 3    ;y456 (2)


    subps xmm1, xmm0    ;operation x123-x456, y123-y456 on (1) and (2)
    
        ;gradX (1) - gradY (1) - gradX (2) - gradY (2)
        ;Here we square, add, change to double, take square root, change to int 
    pmulld xmm1, xmm1       ; square the values
    phaddd xmm1, xmm1       ; add the values
    cvtdq2pd    xmm1, xmm1  ; convert to double precision
    sqrtpd  xmm1, xmm1      ; take square root of dp
    cvtpd2dq  xmm1 ,xmm1    ; convert from dp to int
    movd eax, xmm1          ; move to eax (1)
    mov byte PTR[rdi + r11 ], al  
    pextrd  eax, xmm1, 1    ;move to eax (2)
    mov byte PTR[rdi + r11 + 1], al 

        ; Increment column iterator (j)
    add r11, 2      ; Add 2 because we compute 2 values in each loop
        ;inc rsi then we won't need to add r11 before each addressing (TO DO)
    cmp r11, r12    ; check if end of row (second last column in row)
    jl inner_loop

        ; Move pointers to the next row
    add     rcx, r8       ; pointer + width * 3
    add     rdx, r8       ; pointer + width * 3

    ; Increment row iterator (i)
    inc     r10
    cmp     r10, r13        ; Check if last row of the image
    jl      outer_loop

    ; End
    pop rsi
    pop rdi
    pop r15
    pop r14
    pop r13
    pop r12
    ret

FiltrAsm ENDP

END
