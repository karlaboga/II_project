ALTER TABLE Orders ADD OrderNumber INT NOT NULL DEFAULT 0;

UPDATE o SET OrderNumber = sq.rn
FROM Orders o
JOIN (
    SELECT Id, ROW_NUMBER() OVER (
        PARTITION BY CAST(ISNULL(OrderDate, GETDATE()) AS DATE) ORDER BY Id
    ) AS rn
    FROM Orders
) sq ON o.Id = sq.Id;