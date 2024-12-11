


create user WBBAN_OTOO identified by 123
grant create session to WBBAN_OTOO
--tr??c khi t?o b?ng c?p quy?n
grant create table to WBBAN_OTOO
--tr??c khi nh?p d? li?u c?p quy?n
alter user WBBAN_OTOO quota 100M on users;
--tr??c khi t?o th? t?c c?p quy?n
grant create procedure to WBBAN_OTOO