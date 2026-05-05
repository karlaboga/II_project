USE RestaurantDB;
GO
-- Add Category column to Dish table
ALTER TABLE Dish ADD Category NVARCHAR(50) DEFAULT 'Main Course';
GO
-- Insert sample ingredients (Produs)
INSERT INTO Produs (Name, Quantity, Unit) VALUES
('Chicken Breast', 100, 'kg'),
('Beef Steak', 50, 'kg'),
('Rice', 200, 'kg'),
('Pasta', 150, 'kg'),
('Tomato Sauce', 100, 'bottles'),
('Chocolate', 30, 'kg'),
('Ice Cream', 50, 'liters'),
('Flour', 100, 'kg'),
('Coffee Beans', 20, 'kg'),
('Milk', 80, 'liters'),
('Orange Juice', 40, 'liters'),
('Cola', 100, 'cans'),
('Lemon', 10, 'kg'),
('Salt', 10, 'kg'),
('Pepper', 5, 'kg');
GO
-- Insert sample dishes
INSERT INTO Dish (Name, Price, Category) VALUES
('Grilled Chicken Rice', 45.99, 'Main Course'),
('Beef Steak with Pasta', 89.99, 'Main Course'),
('Chocolate Cake', 25.50, 'Dessert'),
('Ice Cream Sundae', 18.99, 'Dessert'),
('Espresso', 12.00, 'Drinks'),
('Fresh Orange Juice', 15.00, 'Drinks'),
('Classic Cola', 8.00, 'Drinks');
GO
-- Link dishes to ingredients (DishIngredient)
-- Grilled Chicken Rice (DishId=1)
INSERT INTO DishIngredient (DishId, ProdusId, Quantity) VALUES
(1, 1, 0.25), (1, 3, 0.20), (1, 14, 0.01), (1, 15, 0.01);
-- Beef Steak with Pasta (DishId=2)
INSERT INTO DishIngredient (DishId, ProdusId, Quantity) VALUES
(2, 2, 0.30), (2, 4, 0.25), (2, 5, 0.10), (2, 14, 0.01), (2, 15, 0.01);
-- Chocolate Cake (DishId=3)
INSERT INTO DishIngredient (DishId, ProdusId, Quantity) VALUES
(3, 6, 0.10), (3, 8, 0.15), (3, 10, 0.05);
-- Ice Cream Sundae (DishId=4)
INSERT INTO DishIngredient (DishId, ProdusId, Quantity) VALUES
(4, 7, 0.20), (4, 6, 0.05);
-- Espresso (DishId=5)
INSERT INTO DishIngredient (DishId, ProdusId, Quantity) VALUES
(5, 9, 0.02), (5, 10, 0.10);
-- Fresh Orange Juice (DishId=6)
INSERT INTO DishIngredient (DishId, ProdusId, Quantity) VALUES
(6, 11, 0.30), (6, 13, 0.02);
-- Classic Cola (DishId=7)
INSERT INTO DishIngredient (DishId, ProdusId, Quantity) VALUES
(7, 12, 0.33);
GO