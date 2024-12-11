SELECT USER FROM DUAL;
--m? hóa AES
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
--hi?n log
SET SERVEROUTPUT ON;

--test m? hóa
DECLARE
    v_data VARCHAR2(100) := 'This is a test string';
    v_key RAW(32) := UTL_RAW.CAST_TO_RAW('0123456789ABCDEF0123456789ABCDEF'); -- Khóa 256-bit
    v_iv RAW(16) := UTL_RAW.CAST_TO_RAW('ABCDEF1234567890'); -- Initialization Vector
    v_encrypted RAW(2000);
BEGIN
    v_encrypted := ENCRYPT_AES(v_data, v_key, v_iv);
    DBMS_OUTPUT.PUT_LINE('Encrypted Data: ' || RAWTOHEX(v_encrypted));
END;
/
--gi?i m? AES
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
select * from users
