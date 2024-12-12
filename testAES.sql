-- kich ban test AES
DECLARE
    v_plaintext        VARCHAR2(100) := 'This is a test message.'; -- Chu?i ??u v�o
    v_key              RAW(32) := UTL_RAW.CAST_TO_RAW('12345678901234567890123456789012'); -- Kh�a m� h�a 32 byte
    v_iv               RAW(16) := UTL_RAW.CAST_TO_RAW('1234567890123456'); -- IV 16 byte
    v_encrypted_data   RAW(2000); -- D? li?u m� h�a
    v_decrypted_data   VARCHAR2(100); -- D? li?u gi?i m�
    v_encrypted_text   VARCHAR2(2000); -- Chu?i m� h�a sau khi chuy?n ??i
BEGIN
    -- Log th�ng tin ??u v�o
    DBMS_OUTPUT.PUT_LINE('Input Plaintext: ' || v_plaintext);
    DBMS_OUTPUT.PUT_LINE('Encryption Key (HEX): ' || RAWTOHEX(v_key));
    DBMS_OUTPUT.PUT_LINE('Initialization Vector (IV) (HEX): ' || RAWTOHEX(v_iv));

    -- M� h�a chu?i
    v_encrypted_data := ENCRYPT_AES(v_plaintext, v_key, v_iv);

    -- Chuy?n ??i chu?i m� h�a (RAW) sang chu?i ??c ???c (HEX ho?c BASE64 n?u c?n)
    v_encrypted_text := RAWTOHEX(v_encrypted_data);
    DBMS_OUTPUT.PUT_LINE('Encrypted Data (HEX): ' || v_encrypted_text);

    -- Gi?i m� chu?i
    v_decrypted_data := DECRYPT_AES(v_encrypted_data, v_key, v_iv);
    DBMS_OUTPUT.PUT_LINE('Decrypted Data: ' || v_decrypted_data);

    -- Ki?m tra t�nh ch�nh x�c
    IF v_plaintext = v_decrypted_data THEN
        DBMS_OUTPUT.PUT_LINE('Test Passed: Decrypted data matches the original plaintext.');
    ELSE
        DBMS_OUTPUT.PUT_LINE('Test Failed: Decrypted data does not match the original plaintext.');
    END IF;
END;
/





--ma hoa RSA
CREATE OR REPLACE FUNCTION ENCRYPT_RSA(p_data IN VARCHAR2, p_public_key IN CLOB)
RETURN CLOB
AS
    v_encrypted_data RAW(2000);
BEGIN
    -- M� h�a d? li?u RSA b?ng DBMS_CRYPTO (l?u �: Oracle kh�ng h? tr? tr?c ti?p)
    v_encrypted_data := DBMS_CRYPTO.ENCRYPT(
        UTL_RAW.CAST_TO_RAW(p_data), 
        DBMS_CRYPTO.ENCRYPT_RSA_PKCS1_PADDING,
        UTL_RAW.CAST_TO_RAW(p_public_key)
    );

    -- M� h�a d? li?u th�nh Base64 s? d?ng UTL_ENCODE
    RETURN UTL_ENCODE.BASE64_ENCODE(v_encrypted_data);
END;
/







--giai ma RSA
CREATE OR REPLACE FUNCTION DECRYPT_RSA(p_encrypted_data IN CLOB, p_private_key IN CLOB)
RETURN VARCHAR2
AS
    v_decrypted_data RAW(2000);
BEGIN
    v_decrypted_data := DBMS_CRYPTO.DECRYPT(
        UTL_RAW.DECODE_BASE64(UTL_RAW.CAST_TO_RAW(p_encrypted_data)),
        DBMS_CRYPTO.ENCRYPT_RSA_PKCS1_PADDING,
        UTL_RAW.CAST_TO_RAW(p_private_key)
    );
    RETURN UTL_I18N.RAW_TO_CHAR(v_decrypted_data, 'AL32UTF8');
END;
/





--test RSA
DECLARE
    v_plaintext        VARCHAR2(100) := 'This is a test message.'; -- Chu?i ban ??u
    v_encrypted_data   CLOB; -- D? li?u m� h�a
    v_decrypted_data   VARCHAR2(100); -- D? li?u gi?i m�
    v_public_key       CLOB;
    v_private_key      CLOB;
BEGIN
    -- L?y kh�a RSA t? b?ng
    SELECT PUBLIC_KEY, PRIVATE_KEY INTO v_public_key, v_private_key FROM RSA_KEYS WHERE ID = 1;

    -- M� h�a chu?i
    v_encrypted_data := ENCRYPT_RSA(v_plaintext, v_public_key);
    DBMS_OUTPUT.PUT_LINE('Encrypted Data: ' || v_encrypted_data);

    -- Gi?i m� chu?i
    v_decrypted_data := DECRYPT_RSA(v_encrypted_data, v_private_key);
    DBMS_OUTPUT.PUT_LINE('Decrypted Data: ' || v_decrypted_data);

    -- Ki?m tra t�nh ch�nh x�c
    IF v_plaintext = v_decrypted_data THEN
        DBMS_OUTPUT.PUT_LINE('Test Passed: Decrypted data matches the original plaintext.');
    ELSE
        DBMS_OUTPUT.PUT_LINE('Test Failed: Decrypted data does not match the original plaintext.');
    END IF;
END;
/





--check dinh ki
CREATE OR REPLACE PROCEDURE CHECK_PASSWORD_EXPIRATION(p_username IN VARCHAR2) IS
    v_last_changed_date DATE;
    v_current_date DATE;
    v_days_difference NUMBER;
BEGIN
    -- L?y ng�y thay ??i m?t kh?u g?n nh?t
    SELECT LASTPASSWORDCHANGED INTO v_last_changed_date
    FROM USERS
    WHERE USERNAME = p_username;
    
    v_current_date := SYSDATE;  -- Ng�y hi?n t?i
    
    -- T�nh s? kh�c bi?t gi?a ng�y hi?n t?i v� ng�y thay ??i m?t kh?u cu?i c�ng
    v_days_difference := (v_current_date - v_last_changed_date);

    -- N?u ?� ?? 30 ng�y, y�u c?u ng??i d�ng thay ??i m?t kh?u
    IF v_days_difference >= 30 THEN
        -- Y�u c?u ng??i d�ng thay ??i m?t kh?u
        DBMS_OUTPUT.PUT_LINE('Your password has expired. Please change your password.');
        -- ? ?�y, b?n c� th? th?c hi?n c�c b??c nh? redirect t?i trang thay ??i m?t kh?u ho?c g?i th�ng b�o cho ng??i d�ng
    ELSE
        DBMS_OUTPUT.PUT_LINE('Password is still valid.');
    END IF;
    
EXCEPTION
    WHEN NO_DATA_FOUND THEN
        DBMS_OUTPUT.PUT_LINE('User not found: ' || p_username);
    WHEN OTHERS THEN
        DBMS_OUTPUT.PUT_LINE('Error occurred: ' || SQLERRM);
END;
/
-- Test th? t?c v?i ng??i d�ng 'john_doe'
EXEC CHECK_PASSWORD_EXPIRATION('213');

