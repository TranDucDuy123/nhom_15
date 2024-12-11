
CREATE TABLE USER_VERIFICATION (
    MAKH VARCHAR2(10),                  -- Kh�a li�n k?t gi? �?nh �?n MAKH
    MANV VARCHAR2(10),                  -- Kh�a li�n k?t gi? �?nh �?n MANV
    EMAIL VARCHAR2(255) NOT NULL,       -- Email c?a ng�?i d�ng
    VERIFY_TOKEN VARCHAR2(255),         -- M? token x�c th?c
    IS_VERIFIED NUMBER(1) DEFAULT 0,    -- Tr?ng th�i x�c th?c (0 = ch�a x�c th?c, 1 = �? x�c th?c)
    TOKEN_CREATED_AT DATE,              -- Th?i gian t?o token
    CONSTRAINT PK_USER_VERIFICATION PRIMARY KEY (MAKH, MANV)
);


select * from USER_VERIFICATION