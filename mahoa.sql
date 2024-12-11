
CREATE OR REPLACE FUNCTION ENCRYPT_AES(p_data IN VARCHAR2, p_key IN RAW, p_iv IN RAW)
RETURN RAW
AS
    v_encrypted_data RAW(2000);
BEGIN
    v_encrypted_data := DBMS_CRYPTO.ENCRYPT(
        src => UTL_I18N.STRING_TO_RAW(p_data, 'AL32UTF8'),
        typ => DBMS_CRYPTO.ENCRYPT_AES + DBMS_CRYPTO.CHAIN_CBC + DBMS_CRYPTO.PAD_PKCS5,
        key => p_key,
        iv  => p_iv
    );
    RETURN v_encrypted_data;
END;
/


CREATE OR REPLACE FUNCTION DECRYPT_AES(p_encrypted_data IN RAW, p_key IN RAW, p_iv IN RAW)
RETURN VARCHAR2
AS
    v_decrypted_data RAW(2000);
BEGIN
    v_decrypted_data := DBMS_CRYPTO.DECRYPT(
        src => p_encrypted_data,
        typ => DBMS_CRYPTO.ENCRYPT_AES + DBMS_CRYPTO.CHAIN_CBC + DBMS_CRYPTO.PAD_PKCS5,
        key => p_key,
        iv  => p_iv
    );
    RETURN UTL_I18N.RAW_TO_CHAR(v_decrypted_data, 'AL32UTF8');
END;
/




DECLARE
    v_plaintext        VARCHAR2(100) := 'This is a test message.';
    v_key              RAW(32) := UTL_RAW.CAST_TO_RAW('12345678901234567890123456789012'); -- 32 bytes (256-bit key)
    v_iv               RAW(16) := UTL_RAW.CAST_TO_RAW('1234567890123456'); -- 16 bytes IV
    v_encrypted_data   RAW(2000);
    v_decrypted_data   VARCHAR2(100);
BEGIN
    -- Test encryption
    v_encrypted_data := ENCRYPT_AES(v_plaintext, v_key, v_iv);
    DBMS_OUTPUT.PUT_LINE('Encrypted Data: ' || RAWTOHEX(v_encrypted_data));

    -- Test decryption
    v_decrypted_data := DECRYPT_AES(v_encrypted_data, v_key, v_iv);
    DBMS_OUTPUT.PUT_LINE('Decrypted Data: ' || v_decrypted_data);

    -- Check if the original plaintext matches the decrypted text
    IF v_plaintext = v_decrypted_data THEN
        DBMS_OUTPUT.PUT_LINE('Test Passed: Decrypted data matches the original plaintext.');
    ELSE
        DBMS_OUTPUT.PUT_LINE('Test Failed: Decrypted data does not match the original plaintext.');
    END IF;
END;
/
--S? d?ng truy v?n sau ð? ki?m tra tr?ng thái c?a hàm ENCRYPT_AES:
SELECT * FROM all_objects WHERE object_name = 'DBMS_CRYPTO';
--Ki?m tra user ðang dùng:
SELECT USER FROM dual;

--ghi log sql
SET SERVEROUTPUT ON


DECLARE
    v_plaintext        VARCHAR2(100) := 'Test message';
    v_key              RAW(32) := UTL_RAW.CAST_TO_RAW('12345678901234567890123456789012'); -- 32 byte key
    v_iv               RAW(16) := UTL_RAW.CAST_TO_RAW('1234567890123456'); -- 16 byte IV
    v_encrypted_data   RAW(2000);
BEGIN
    v_encrypted_data := ENCRYPT_AES(v_plaintext, v_key, v_iv);
    DBMS_OUTPUT.PUT_LINE('Encrypted data: ' || RAWTOHEX(v_encrypted_data));
END;



DECLARE
    v_data VARCHAR2(100) := 'Test encryption data';
    v_key RAW(32) := UTL_RAW.CAST_TO_RAW('12345678901234567890123456789012'); -- 32-byte key
    v_iv RAW(16) := UTL_RAW.CAST_TO_RAW('initialvector123'); -- 16-byte IV
    v_encrypted RAW(2000);
    v_decrypted VARCHAR2(100);
BEGIN
    -- G?i hàm mã hóa
    v_encrypted := ENCRYPT_AES(v_data, v_key, v_iv);
    
    -- Hi?n th? d? li?u mã hóa
    DBMS_OUTPUT.PUT_LINE('Encrypted Data: ' || RAWTOHEX(v_encrypted));
    
    -- G?i hàm gi?i mã
    v_decrypted := DECRYPT_AES(v_encrypted, v_key, v_iv);
    
    -- Hi?n th? d? li?u gi?i mã
    DBMS_OUTPUT.PUT_LINE('Decrypted Data: ' || v_decrypted);
END;
/


