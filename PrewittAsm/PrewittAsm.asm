.CODE
Myproc PROC

    mov rsi, rcx
    mov rdi, rdx
    imul rdi, r8     

    mov rcx, 0

loop_start:
    cmp rcx, rdi
    je  loop_end

    mov rax, rcx
    imul rax, 3
    add rax, rsi
    mov byte ptr [rax + 2], 255

    inc rcx
    jmp loop_start

loop_end:
    ret

Myproc ENDP

END