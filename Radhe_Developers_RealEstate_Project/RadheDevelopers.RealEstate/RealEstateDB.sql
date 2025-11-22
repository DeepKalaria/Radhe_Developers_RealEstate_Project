-- RealEstateDB.sql
CREATE DATABASE RealEstateDB;
GO
USE RealEstateDB;
GO

CREATE TABLE Admin (
    AdminID INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50),
    Password NVARCHAR(50)
);
INSERT INTO Admin (Username, Password) VALUES ('admin','admin123');

CREATE TABLE Users (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100),
    Email NVARCHAR(100),
    Password NVARCHAR(50)
);

CREATE TABLE Properties (
    PropertyID INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(100),
    Location NVARCHAR(100),
    Price DECIMAL(18,2),
    Size NVARCHAR(50),
    Description NVARCHAR(255)
);

CREATE TABLE Bookings (
    BookingID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT FOREIGN KEY REFERENCES Users(UserID),
    PropertyID INT FOREIGN KEY REFERENCES Properties(PropertyID),
    BookingDate DATE DEFAULT GETDATE()
);
