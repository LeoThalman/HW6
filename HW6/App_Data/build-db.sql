RESTORE DATABASE AdventureWorks2014_Data
FROM DISK = 'C:\Users\pocke\Desktop\school-work\cs46X\cs460\AdventureWorks2014.bak'
WITH MOVE 'AdventureWorks2014_Data' TO 'C:\Users\pocke\Desktop\school-work\cs46X\cs460\HW6\HW6\App_Data\AdventureWorks2014.mdf',
MOVE 'AdventureWorks2014_Log' TO 'C:\Users\pocke\Desktop\school-work\cs46X\cs460\HW6\HW6\App_Data\AdventureWorks2014.ldf'
GO