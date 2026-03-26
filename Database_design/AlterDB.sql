USE MartDB;
GO

-- Create OrderItems table
CREATE TABLE OrderItems (
	OrderItemID int PRIMARY KEY IDENTITY(1,1),
	OrderID int NOT NULL,
	ProductID int NOT NULL,
	Quantity int NOT NULL CHECK (Quantity > 0),
	UnitPrice decimal(15,2) NOT NULL CHECK (UnitPrice >= 0),
	LineTotal decimal(20,2) NOT NULL CHECK (LineTotal >= 0),
	FOREIGN KEY (OrderID) REFERENCES Orders(OrderID),
	FOREIGN KEY (ProductID) REFERENCES Products(ProductID)
);
GO

CREATE INDEX idx_orderitems_order ON OrderItems(OrderID);
GO

CREATE INDEX idx_orderitems_product ON OrderItems(ProductID);
GO

-- Insert OrderItems for the 40 existing orders in Data_insert.sql
-- Each order was a single product so one OrderItem row per OrderID
-- OrderIDs 1-40 match the 40 rows inserted by Data_insert.sql
-- UnitPrice = TotalAmount / Quantity for each row

INSERT INTO OrderItems (OrderID, ProductID, Quantity, UnitPrice, LineTotal) VALUES
(1,  1,  2, 320.00, 640.00),
(2,  5,  3, 32.00,  96.00),
(3,  10, 2, 35.00,  70.00),
(4,  12, 1, 140.00, 140.00),
(5,  16, 1, 155.00, 155.00),
(6,  3,  4, 45.00,  180.00),
(7,  13, 5, 28.00,  140.00),
(8,  14, 3, 34.00,  102.00),
(9,  15, 2, 36.00,  72.00),
(10, 19, 6, 25.00,  150.00),
(11, 20, 3, 38.00,  114.00),
(12, 21, 1, 145.00, 145.00),
(13, 22, 2, 95.00,  190.00),
(14, 23, 1, 110.00, 110.00),
(15, 24, 2, 120.00, 240.00),
(16, 25, 2, 125.00, 250.00),
(17, 26, 5, 20.00,  100.00),
(18, 27, 6, 18.00,  108.00),
(19, 28, 2, 135.00, 270.00),
(20, 29, 1, 128.00, 128.00),
(21, 30, 2, 92.00,  184.00),
(22, 2,  1, 420.00, 420.00),
(23, 4,  3, 20.00,  60.00),
(24, 6,  2, 40.00,  80.00),
(25, 7,  1, 85.00,  85.00),
(26, 8,  1, 240.00, 240.00),
(27, 9,  2, 78.00,  156.00),
(28, 11, 1, 60.00,  60.00),
(29, 17, 1, 210.00, 210.00),
(30, 18, 1, 180.00, 180.00),
(31, 1,  1, 320.00, 320.00),
(32, 3,  2, 45.00,  90.00),
(33, 5,  2, 32.00,  64.00),
(34, 10, 3, 35.00,  105.00),
(35, 12, 2, 140.00, 280.00),
(36, 13, 4, 28.00,  112.00),
(37, 14, 2, 34.00,  68.00),
(38, 15, 2, 36.00,  72.00),
(39, 16, 1, 155.00, 155.00),
(40, 19, 4, 25.00,  100.00);
GO
