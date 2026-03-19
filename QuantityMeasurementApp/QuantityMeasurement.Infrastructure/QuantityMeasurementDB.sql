USE master ;
GO

-- Kicking everyone out and roll back their unfinished business ( just for testing and development purpose alone)
IF EXISTS (Select name from sys.databases where name = 'QuantityMeasurementDB')
BEGIN
    Alter DATABASE QuantityMeasurementDB Set Single_User WITH ROLLBACK IMMEDIATE;
    DROP DATABASE QuantityMeasurementDB;
END
GO

-- Create the database
CREATE DATABASE QuantityMeasurementDB;
GO

-- Switch to the database
Use QuantityMeasurementDB;
Go

-- Creating The Tables
CREATE Table History (
    Id INT Identity(1,1 ) PRIMARY KEY,
    Operation NVARCHAR (100) NOT NULL check ( Operation in ('Add' , 'Subtract' , 'Multiply', 'MultiplyByScalar' , 'DivideByScalar' , 'Divide' , 'Convert') ),
    Value1 FLOAT NOT NULL,
    Unit1 NVARCHAR (100) NOT NULL CHECK ( Unit1 in ('Feet' , 'Inches' , 'Meter' , 'Centimeter' , 'Yard' , 'Celsius' , 'Fahrenheit' , 'Kelvin' , 'Gram' , 'Kilogram' , 'Pound' , 'Litre' , 'Millilitre' , 'Gallon') ),
    Value2 FLOAT NULL,
    Unit2 NVARCHAR (100) NULL Check ( Unit2 is Null or Unit2 in ('Feet' , 'Inches' , 'Meter' , 'Centimeter' , 'Yard' , 'Celsius' , 'Fahrenheit' , 'Kelvin' , 'Gram' , 'Kilogram' , 'Pound' , 'Litre' , 'Millilitre' , 'Gallon') ),
    TargetUnit NVARCHAR (100) NULL Check ( TargetUnit is Null or TargetUnit in ('Feet' , 'Inches' , 'Meter' , 'Centimeter' , 'Yard' , 'Celsius' , 'Fahrenheit' , 'Kelvin' , 'Gram' , 'Kilogram' , 'Pound' , 'Litre' , 'Millilitre' , 'Gallon') ),
    _Scalar FLOAT NULL,
    Result FLOAT NOT NULL,
    ResultUnit NVARCHAR (100) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

-- Creating the indexes for performance optimization
CREATE INDEX IX_Histories_CreatedAt ON History (CreatedAt);
GO

CREATE INDEX IX_Histories_Value1 ON History (Value1);
GO

CREATE INDEX IX_Histories_Operations_Values_Units on History (Operation, Value1, Unit1, Value2, Unit2 , TargetUnit , Result , ResultUnit);
GO

-- Creating the Audit Table
Create Table SystemAudit (
    LogId INT Identity(1,1 ) PRIMARY KEY,
    Id int Null ,
    ActionType NVARCHAR (100) NOT NULL Check ( ActionType in ('Create','Insert' , 'Update', 'Delete' , 'Remove' , 'Other') ),
    ActionDate DATETIME2 Not Null Default SYSDATETIME(),
    OldValue NVARCHAR (MAX) Null , -- Old value before the change, which can be null for Insert actions. Also Using MAX instead of 255 to store full row snapshots
    NewValue NVARCHAR (MAX) Null -- New value after the change, which can be null for Delete actions
);
GO

-- Creating trigger on the History Table
Create Trigger trg_History_Audit
on History
After UPDATE , Insert , Delete
AS
BEGIN
    Set NOCOUNT  On ;
    Declare @ActionType NVARCHAR (100);
    If Exists ( Select * from Inserted ) and exists ( Select * from Deleted )
       Set @ActionType = 'Update';
    ELSE If   Exists ( Select * from Inserted)
       Set @ActionType = 'Insert';
    Else Set @ActionType = 'Delete';
    Insert into SystemAudit ( Id , ActionType , ActionDate , OldValue , NewValue )
    Select 
          Coalesce ( i.Id , d.Id ),
          @ActionType,
          SYSDATETIME(),
          (Select d.* for Json Path , Without_Array_wrapper) ,
          (Select i.* for Json Path , Without_Array_wrapper)
        from Inserted i Full OUTER JOIN Deleted d on i.Id = d.Id ;
END
GO

Deny Update , Delete on SYstemAudit to public;
GO