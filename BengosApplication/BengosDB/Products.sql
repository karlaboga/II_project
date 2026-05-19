CREATE TABLE Products (
    Id INT PRIMARY KEY,
    ProductName NVARCHAR(255) NOT NULL,
    Category NVARCHAR(100),
    Quantity INT NOT NULL,
    Unit NVARCHAR(50),
    Min_Stock INT NOT NULL
);
