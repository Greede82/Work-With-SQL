IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'Dispanserizatsia')
BEGIN
    CREATE DATABASE Dispanserizatsia
END
go

Use Dispanserizatsia
go

IF NOT EXISTS (SELECT * FROM sys.types WHERE name = 'AllNames')
BEGIN
    Create Type AllNames from Varchar(40)
END
go

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'DocDiagnoz')
BEGIN
    Create table DocDiagnoz
    (
        Cod_Diagnoz Int NOT NULL PRIMARY KEY,
        Diagnoz Varchar(100)
    )

    Insert into DocDiagnoz(Cod_Diagnoz, Diagnoz) VALUES (1001, 'Nasmork')
    Insert into DocDiagnoz(Cod_Diagnoz, Diagnoz) VALUES (1002, 'Insult')
    Insert into DocDiagnoz(Cod_Diagnoz, Diagnoz) VALUES (1003, 'Gripp')
    Insert into DocDiagnoz(Cod_Diagnoz, Diagnoz) VALUES (1004, 'Gonorea')
    Insert into DocDiagnoz(Cod_Diagnoz, Diagnoz) VALUES (1005, 'Gerpes')
    Insert into DocDiagnoz(Cod_Diagnoz, Diagnoz) VALUES (1006, 'Kosoglazie')
    Insert into DocDiagnoz(Cod_Diagnoz, Diagnoz) VALUES (1007, 'Bronhit')
    Insert into DocDiagnoz(Cod_Diagnoz, Diagnoz) VALUES (1008, 'Karies')
    Insert into DocDiagnoz(Cod_Diagnoz, Diagnoz) VALUES (1009, 'RakPochek')
    Insert into DocDiagnoz(Cod_Diagnoz, Diagnoz) VALUES (1010, 'Holera')
END
go

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Pacient')
BEGIN
    Create table Pacient
    (
        Cod_Pacient Int NOT NULL PRIMARY KEY,
        FIO_Pacient AllNames,
        Adress Varchar(65),
        IDNP Char(13),
        Strahovka Char(15),
        Nr_Uchastka SmallInt
    )

    Insert into Pacient(Cod_Pacient, FIO_Pacient, Adress, IDNP, Strahovka, Nr_Uchastka) 
    VALUES (1, 'Repin Gavriil Adrianovich', 'g. Volovo, ul. Darvina, dom 88, kvartira 229', '1000000000001', 'Zastrahovan1', 1)
    
    Insert into Pacient(Cod_Pacient, FIO_Pacient, Adress, IDNP, Strahovka, Nr_Uchastka) 
    VALUES (2, 'Jambusheva Ksenija Svjatoslavovna', 'g. Starodub, ul. Gollandskaja, dom 40, kvartira 169', '1000000000002', 'Zastrahovan2', 1)
    
    Insert into Pacient(Cod_Pacient, FIO_Pacient, Adress, IDNP, Strahovka, Nr_Uchastka) 
    VALUES (3, 'Krupnov Timur Anatolievich', 'g. Kshenskij, ul. Dekabristov, dom 6, kvartira 236', '1000000000003', 'Zastrahovan3', 2)
    
    Insert into Pacient(Cod_Pacient, FIO_Pacient, Adress, IDNP, Strahovka, Nr_Uchastka) 
    VALUES (4, 'Fammusa Vladlena Olegovna', 'g. Ikrjanoe, ul. 15 let oktjabrja, dom 71, kvartira 142', '1000000000004', 'Zastrahovan4', 2)
    
    Insert into Pacient(Cod_Pacient, FIO_Pacient, Adress, IDNP, Strahovka, Nr_Uchastka) 
    VALUES (5, 'Kaluger Taras Kazimirovich', 'g. Kasli, ul. Avangardnaja, dom 64, kvartira 238', '1000000000005', 'Zastrahovan5', 3)
    
    Insert into Pacient(Cod_Pacient, FIO_Pacient, Adress, IDNP, Strahovka, Nr_Uchastka) 
    VALUES (6, 'Lagoshina Inessa Vasilievna', 'g. Petrovsk, ul. Gajdara, dom 35, kvartira 191', '1000000000006', 'Zastrahovan6', 3)
    
    Insert into Pacient(Cod_Pacient, FIO_Pacient, Adress, IDNP, Strahovka, Nr_Uchastka) 
    VALUES (7, 'Tattar Stepan Innokentievich', 'g. Staraja Chara, ul. Glinki, dom 79, kvartira 172', '1000000000007', 'Zastrahovan7', 4)
    
    Insert into Pacient(Cod_Pacient, FIO_Pacient, Adress, IDNP, Strahovka, Nr_Uchastka) 
    VALUES (8, 'Sjomina Efrosinija Davidovna', 'g. Tosno, ul. Bazovskaja, dom 65, kvartira 253', '1000000000008', 'Zastrahovan8', 4)
    
    Insert into Pacient(Cod_Pacient, FIO_Pacient, Adress, IDNP, Strahovka, Nr_Uchastka) 
    VALUES (9, 'Tsulukidze Raisa Efimovna', 'g. Buzuluk, ul. Sovetskaja, dom 96, kvartira 276', '1000000000009', 'Zastrahovan9', 5)
    
    Insert into Pacient(Cod_Pacient, FIO_Pacient, Adress, IDNP, Strahovka, Nr_Uchastka) 
    VALUES (10, 'Gustokashin Feofan Artemievich', 'g. Ulety, ul. Aviamotornaja ulitsa, dom 70, kvartira 213', '1000000000010', 'Zastrahovan10', 5)
END
go

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Doctor')
BEGIN
    Create table Doctor
    (
        Cod_Doctor Int NOT NULL PRIMARY KEY,
        FIO_Doctor AllNames,
        Nr_Uchastka_DOC SmallInt,
        Nr_Cabinet Int
    )

    Insert into Doctor(Cod_Doctor, FIO_Doctor, Nr_Uchastka_DOC, Nr_Cabinet) 
    VALUES (3001, 'Kolenko Cheslav Anikitevich', 1, 12)
    
    Insert into Doctor(Cod_Doctor, FIO_Doctor, Nr_Uchastka_DOC, Nr_Cabinet) 
    VALUES (3002, 'Sharonova Rozalija Feliksovna', 2, 72)
    
    Insert into Doctor(Cod_Doctor, FIO_Doctor, Nr_Uchastka_DOC, Nr_Cabinet) 
    VALUES (3003, 'Fotina Ekaterina Nikitevna', 3, 17)
    
    Insert into Doctor(Cod_Doctor, FIO_Doctor, Nr_Uchastka_DOC, Nr_Cabinet) 
    VALUES (3004, 'Kukleva Margarita Timofeevna', 4, 102)
    
    Insert into Doctor(Cod_Doctor, FIO_Doctor, Nr_Uchastka_DOC, Nr_Cabinet) 
    VALUES (3005, 'Dudnik Artem Bogdanovich', 5, 5)
END
go

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Lekarstvo')
BEGIN
    Create table Lekarstvo
    (
        Cod_Lekarstva Int NOT NULL PRIMARY KEY,
        Name_Lekarstva Varchar(100),
        Dozirovka Int,
        Type_Upakovka Varchar(30),
        Gruppa Varchar(30)
    )

    Insert into Lekarstvo(Cod_Lekarstva, Name_Lekarstva, Dozirovka, Type_Upakovka, Gruppa)
    VALUES (4001, 'Antibiotiki', 101, 'Bumazhnaja', 'Antibiotiki')
    
    Insert into Lekarstvo(Cod_Lekarstva, Name_Lekarstva, Dozirovka, Type_Upakovka, Gruppa)
    VALUES (4002, 'Antigistaminnye', 102, 'Bumazhnaja', 'Antigistaminnye')
    
    Insert into Lekarstvo(Cod_Lekarstva, Name_Lekarstva, Dozirovka, Type_Upakovka, Gruppa)
    VALUES (4003, 'Antidepressanty', 103, 'Bumazhnaja', 'Psihotropnye')
    
    Insert into Lekarstvo(Cod_Lekarstva, Name_Lekarstva, Dozirovka, Type_Upakovka, Gruppa)
    VALUES (4004, 'Tsizaprid', 104, 'Bumazhnaja', 'Prokinetiki')
    
    Insert into Lekarstvo(Cod_Lekarstva, Name_Lekarstva, Dozirovka, Type_Upakovka, Gruppa)
    VALUES (4005, 'Takrolimus', 105, 'Bumazhnaja', 'Immunodepressanty')
END
go

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Priem')
BEGIN
    Create table Priem
    (
        Cod_Priema Int NOT NULL PRIMARY KEY,
        Cod_Doctor Int FOREIGN KEY REFERENCES Doctor(Cod_Doctor),
        Cod_Pacient Int FOREIGN KEY REFERENCES Pacient(Cod_Pacient),
        Cod_Diagnoz Int FOREIGN KEY REFERENCES DocDiagnoz(Cod_Diagnoz),
        Data_Priema Date,
        Time_Priema Decimal(5,2)
    )

    Insert into Priem(Cod_Priema, Cod_Doctor, Cod_Pacient, Cod_Diagnoz, Data_Priema, Time_Priema)
    VALUES (1, 1, 1, 1002, '2018-01-12', 12.30)
    
    Insert into Priem(Cod_Priema, Cod_Doctor, Cod_Pacient, Cod_Diagnoz, Data_Priema, Time_Priema)
    VALUES (2, 2, 2, 1005, '2018-01-22', 10.30)
    
    Insert into Priem(Cod_Priema, Cod_Doctor, Cod_Pacient, Cod_Diagnoz, Data_Priema, Time_Priema)
    VALUES (3, 1, 3, 1001, '2018-01-18', 9.25)
    
    Insert into Priem(Cod_Priema, Cod_Doctor, Cod_Pacient, Cod_Diagnoz, Data_Priema, Time_Priema)
    VALUES (4, 4, 4, 1006, '2018-02-2', 12.15)
    
    Insert into Priem(Cod_Priema, Cod_Doctor, Cod_Pacient, Cod_Diagnoz, Data_Priema, Time_Priema)
    VALUES (5, 5, 5, 1009, '2018-02-8', 13.40)
    
    Insert into Priem(Cod_Priema, Cod_Doctor, Cod_Pacient, Cod_Diagnoz, Data_Priema, Time_Priema)
    VALUES (6, 5, 6, 1001, '2018-02-18', 13.20)
    
    Insert into Priem(Cod_Priema, Cod_Doctor, Cod_Pacient, Cod_Diagnoz, Data_Priema, Time_Priema)
    VALUES (7, 4, 7, 1001, '2018-01-7', 11.25)
    
    Insert into Priem(Cod_Priema, Cod_Doctor, Cod_Pacient, Cod_Diagnoz, Data_Priema, Time_Priema)
    VALUES (8, 3, 8, 1002, '2018-01-19', 10.20)
    
    Insert into Priem(Cod_Priema, Cod_Doctor, Cod_Pacient, Cod_Diagnoz, Data_Priema, Time_Priema)
    VALUES (9, 2, 9, 1002, '2018-03-24', 9.50)
    
    Insert into Priem(Cod_Priema, Cod_Doctor, Cod_Pacient, Cod_Diagnoz, Data_Priema, Time_Priema)
    VALUES (10, 1, 10, 1003, '2018-01-30', 11.45)
END
go

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Retsept')
BEGIN
    Create table Retsept
    (
        Nr_Retsepta Int NOT NULL PRIMARY KEY,
        Cod_Priema Int FOREIGN KEY REFERENCES Priem(Cod_Priema)
    )

    Insert into Retsept(Nr_Retsepta, Cod_Priema)
    VALUES (5001, 1)
    
    Insert into Retsept(Nr_Retsepta, Cod_Priema)
    VALUES (5002, 2)
    
    Insert into Retsept(Nr_Retsepta, Cod_Priema)
    VALUES (5003, 3)
    
    Insert into Retsept(Nr_Retsepta, Cod_Priema)
    VALUES (5004, 4)
    
    Insert into Retsept(Nr_Retsepta, Cod_Priema)
    VALUES (5005, 5)
END
go

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Lechenie')
BEGIN
    Create table Lechenie
    (
        Cod_Lekarstva Int FOREIGN KEY REFERENCES Lekarstvo(Cod_Lekarstva),
        Nr_Retsepta Int FOREIGN KEY REFERENCES Retsept(Nr_Retsepta),
        PRIMARY KEY (Cod_Lekarstva, Nr_Retsepta)
    )

    Insert into Lechenie(Cod_Lekarstva, Nr_Retsepta)
    VALUES (4001, 5001)
    
    Insert into Lechenie(Cod_Lekarstva, Nr_Retsepta)
    VALUES (4002, 5005)
    
    Insert into Lechenie(Cod_Lekarstva, Nr_Retsepta)
    VALUES (4003, 5004)
    
    Insert into Lechenie(Cod_Lekarstva, Nr_Retsepta)
    VALUES (4004, 5002)
    
    Insert into Lechenie(Cod_Lekarstva, Nr_Retsepta)
    VALUES (4005, 5003)
    
    Insert into Lechenie(Cod_Lekarstva, Nr_Retsepta)
    VALUES (4002, 5003)
END
go

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Users')
BEGIN
    CREATE TABLE Users (
        UserId INT IDENTITY(1,1) PRIMARY KEY,
        Username NVARCHAR(50) NOT NULL UNIQUE,
        Password NVARCHAR(100) NOT NULL,
        FullName NVARCHAR(100) NOT NULL,
        Role INT NOT NULL,
        CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
        IsActive BIT NOT NULL DEFAULT 1
    )
    

    INSERT INTO Users (Username, Password, FullName, Role, CreatedDate, IsActive)
    VALUES ('admin', '8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918', 'Администратор системы', 1, GETDATE(), 1)
END
go

IF NOT EXISTS (SELECT * FROM sys.views WHERE name = 'UchastokView')
BEGIN
    EXEC('CREATE VIEW UchastokView AS
    SELECT Nr_Uchastka, Count(*) AS Kolvo
    FROM Pacient
    GROUP BY Nr_Uchastka')
END
go

IF NOT EXISTS (SELECT * FROM sys.views WHERE name = 'KolDiagnoz')
BEGIN
    EXEC('CREATE VIEW KolDiagnoz AS
    SELECT Diagnoz, Count(*) AS Kolvo
    FROM DocDiagnoz, Pacient, Priem
    WHERE DocDiagnoz.Cod_Diagnoz = Priem.Cod_Diagnoz AND Pacient.Cod_Pacient = Priem.Cod_Pacient
    GROUP BY Diagnoz')
END
go

IF NOT EXISTS (SELECT * FROM sys.views WHERE name = 'LekMax')
BEGIN
    EXEC('CREATE VIEW LekMax AS
    SELECT Lekarstvo.Name_Lekarstva, Count(Lechenie.Cod_Lekarstva) AS Kol_vo
    FROM Lekarstvo
    INNER JOIN Lechenie ON Lekarstvo.Cod_Lekarstva = Lechenie.Cod_Lekarstva
    GROUP BY Lekarstvo.Name_Lekarstva')
END
go

IF NOT EXISTS (SELECT * FROM sys.procedures WHERE name = 'GetDoctorWorkload')
BEGIN
    EXEC('CREATE PROCEDURE GetDoctorWorkload
        @StartDate DATE = NULL,
        @EndDate DATE = NULL
    AS
    BEGIN
        SELECT 
            d.Cod_Doctor,
            d.FIO_Doctor,
            COUNT(p.Cod_Priema) AS AppointmentCount
        FROM 
            Doctor d
        LEFT JOIN 
            Priem p ON d.Cod_Doctor = p.Cod_Doctor
        WHERE
            (@StartDate IS NULL OR p.Data_Priema >= @StartDate) AND
            (@EndDate IS NULL OR p.Data_Priema <= @EndDate)
        GROUP BY 
            d.Cod_Doctor, d.FIO_Doctor
        ORDER BY 
            AppointmentCount DESC
    END')
END
go

IF NOT EXISTS (SELECT * FROM sys.procedures WHERE name = 'GetDiagnosisStatistics')
BEGIN
    EXEC('CREATE PROCEDURE GetDiagnosisStatistics
        @StartDate DATE = NULL,
        @EndDate DATE = NULL
    AS
    BEGIN
        SELECT 
            diag.Cod_Diagnoz,
            diag.Diagnoz,
            COUNT(p.Cod_Priema) AS AppointmentCount
        FROM 
            DocDiagnoz diag
        LEFT JOIN 
            Priem p ON diag.Cod_Diagnoz = p.Cod_Diagnoz
        WHERE
            (@StartDate IS NULL OR p.Data_Priema >= @StartDate) AND
            (@EndDate IS NULL OR p.Data_Priema <= @EndDate)
        GROUP BY 
            diag.Cod_Diagnoz, diag.Diagnoz
        ORDER BY 
            AppointmentCount DESC
    END')
END
go

IF NOT EXISTS (SELECT * FROM sys.procedures WHERE name = 'GetMedicineStatistics')
BEGIN
    EXEC('CREATE PROCEDURE GetMedicineStatistics
        @StartDate DATE = NULL,
        @EndDate DATE = NULL
    AS
    BEGIN
        SELECT 
            l.Cod_Lekarstva,
            l.Name_Lekarstva,
            l.Gruppa,
            COUNT(lech.Nr_Retsepta) AS PrescriptionCount
        FROM 
            Lekarstvo l
        LEFT JOIN 
            Lechenie lech ON l.Cod_Lekarstva = lech.Cod_Lekarstva
        LEFT JOIN 
            Retsept r ON lech.Nr_Retsepta = r.Nr_Retsepta
        LEFT JOIN 
            Priem p ON r.Cod_Priema = p.Cod_Priema
        WHERE
            (@StartDate IS NULL OR p.Data_Priema >= @StartDate) AND
            (@EndDate IS NULL OR p.Data_Priema <= @EndDate)
        GROUP BY 
            l.Cod_Lekarstva, l.Name_Lekarstva, l.Gruppa
        ORDER BY 
            PrescriptionCount DESC
    END')
END
go
