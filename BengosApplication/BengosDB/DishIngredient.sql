CREATE TABLE DishIngredient (
    DishId INT,
    ProdusId INT,
    Quantity DECIMAL(10,2) NOT NULL,
    PRIMARY KEY (DishId, ProdusId),
    FOREIGN KEY (DishId) REFERENCES Dish(Id),
    FOREIGN KEY (ProdusId) REFERENCES Produs(Id)
);
