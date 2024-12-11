
CREATE TABLE USER_VERIFICATION (
    MAKH VARCHAR2(10),                  -- Khóa liên k?t gi? ð?nh ð?n MAKH
    MANV VARCHAR2(10),                  -- Khóa liên k?t gi? ð?nh ð?n MANV
    EMAIL VARCHAR2(255) NOT NULL,       -- Email c?a ngý?i dùng
    VERIFY_TOKEN VARCHAR2(255),         -- M? token xác th?c
    IS_VERIFIED NUMBER(1) DEFAULT 0,    -- Tr?ng thái xác th?c (0 = chýa xác th?c, 1 = ð? xác th?c)
    TOKEN_CREATED_AT DATE,              -- Th?i gian t?o token
    CONSTRAINT PK_USER_VERIFICATION PRIMARY KEY (MAKH, MANV)
);


select * from USER_VERIFICATION