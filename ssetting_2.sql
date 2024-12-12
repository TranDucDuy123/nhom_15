SELECT GETNEXTEMPLOYEEID() FROM dual;

SELECT GETNEXTCUSTOMERID() FROM dual;

SELECT
    *
FROM khachhang

-- setting show test
SET SERVEROUTPUT ON;
--s?a l?i c?t sdt c?a user
ALTER TABLE USERS MODIFY SODT VARCHAR2(24);

--thay ??i, thêm c?t check thay ??i m?t kh?u ??nh kì
ALTER TABLE Users
ADD LastPasswordChanged DATE;

SELECT *
FROM Users
WHERE Username = :username
  AND LastPasswordChanged < SYSDATE - 30;
  


