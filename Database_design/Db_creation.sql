

-- Use MartDB
Use MartDB
GO

-- Create a supplier table

CREATE TABLE Suppliers(
 SupplierID int PRIMARY KEY IDENTITY(1,1),
 SupplierName varchar(255) NOT NULL
);
GO

-- Create a customer table

CREATE TABLE Customers(
CustomerID int PRIMARY KEY IDENTITY(1,1),
CustomerName varchar(255) NOT NULL
);
GO

-- Create a Product table

CREATE TABLE Products(
ProductID int PRIMARY KEY IDENTITY(1,1),
ProductName varchar(255) NOT NULL,
Price DECIMAL(15,2) NOT NULL CHECK(Price >= 0),
SupplierID int NOT NULL,
FOREIGN KEY (SupplierID) references Suppliers(SupplierID)
);
GO

-- Create a Inventory Table

CREATE TABLE Inventory
(
    ProductID INT PRIMARY KEY,
    Quantity INT NOT NULL CHECK (Quantity >= 0),
    LastUpdated DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID)
);
GO

--Create a order table

CREATE TABLE Orders(
	OrderID int PRIMARY KEY IDENTITY(1,1),
	CustomerID int NOT NULL,
	ProductID int NOT NULL,
	Quantity int NOT NULL CHECK (Quantity > 0),
	OrderDate DATETIME NOT NULL DEFAULT GETDATE(),
	LastStatus varchar(255) NOT NULL,
	TotalAmount decimal(20,2) NOT NULL CHECK (TotalAmount >= 0),
	FOREIGN KEY (CustomerID) references Customers(CustomerID),
	FOREIGN KEY (ProductID) references Products(ProductID)
);
GO

-- Indexing

--Indexing on Product.SupplierID
Create INDEX idx_product_Supplier
ON Products(SupplierID);
GO

--Indexing on Inventory.ProductID
Create INDEX idx_inventory_Product
ON Inventory(ProductID);
GO

--Indexing on Orders.CustomerID
Create INDEX idx_orders_Customer
ON Orders(CustomerID);
GO

--Indexing on Orders.ProductID
Create INDEX idx_orders_Product
ON Orders(ProductID);
GO

--Product Stock View

CREATE VIEW vw_ProductStock AS
SELECT 
    p.ProductID,
    p.ProductName,
    p.Price,
    p.SupplierID,
    i.Quantity,
    i.LastUpdated
FROM Products p
JOIN Inventory i ON p.ProductID = i.ProductID;
GO

--Low Stock view

CREATE VIEW vw_LowStockProducts AS
SELECT 
    p.ProductID,
    p.ProductName,
    i.Quantity,
    i.LastUpdated
FROM Products p
JOIN Inventory i ON p.ProductID = i.ProductID
WHERE i.Quantity < 10;
GO

--Sales Summary View

CREATE VIEW vw_SalesSummary AS
SELECT 
    p.ProductID,
    p.ProductName,
    SUM(o.Quantity) AS TotalQuantitySold,
    SUM(o.TotalAmount) AS TotalSalesAmount
FROM Orders o
JOIN Products p ON o.ProductID = p.ProductID
GROUP BY p.ProductID, p.ProductName;
GO

-- Create order procedure

CREATE PROCEDURE sp_CreateOrder
    @CustomerID INT,
    @ProductID INT,
    @Quantity INT,
    @OrderDate DATETIME,
    @Status VARCHAR(255)
AS
BEGIN
    DECLARE @Price DECIMAL(15,2);
    DECLARE @TotalAmount DECIMAL(20,2);

    SELECT @Price = Price
    FROM Products
    WHERE ProductID = @ProductID;

    SET @TotalAmount = @Price * @Quantity;

    INSERT INTO Orders (CustomerID, ProductID, Quantity, OrderDate, LastStatus, TotalAmount)
    VALUES (@CustomerID, @ProductID, @Quantity, @OrderDate, @Status, @TotalAmount);
END
GO

-- Update inventory procedure

CREATE PROCEDURE sp_UpdateInventory
    @ProductID INT,
    @Quantity INT
AS
BEGIN

UPDATE Inventory
SET Quantity = Quantity + @Quantity,
    LastUpdated = GETDATE()
WHERE ProductID = @ProductID;

END
GO

--Get Sales Report

CREATE PROCEDURE sp_GetSalesReport
AS
BEGIN

SELECT 
    p.ProductID,
    p.ProductName,
    SUM(o.Quantity) AS TotalQuantitySold,
    SUM(o.TotalAmount) AS TotalSalesAmount
FROM Orders o
JOIN Products p ON o.ProductID = p.ProductID
GROUP BY p.ProductID, p.ProductName
ORDER BY TotalSalesAmount DESC;

END
GO

-- Insert sample data into supplier table

INSERT INTO Suppliers (SupplierName) VALUES
('Fresh Farm Suppliers'),
('Green Valley Foods'),
('Daily Dairy Distributors'),
('Golden Grain Traders'),
('Organic Harvest Supply'),
('Healthy Basket Wholesale'),
('Metro Grocery Hub'),
('Sunrise Food Products'),
('Nature Fresh Distributors'),
('Prime Retail Suppliers');
GO

-- Insert sample data into customer table

INSERT INTO Customers (CustomerName) VALUES
('Aryan Sharma'),
('Rahul Verma'),
('Priya Singh'),
('Neha Kapoor'),
('Rohan Mehta'),
('Amit Joshi'),
('Sneha Thakur'),
('Karan Malhotra'),
('Pooja Gupta'),
('Vikas Sharma'),
('Ananya Sood'),
('Mohit Rana'),
('Simran Kaur'),
('Nitin Chauhan'),
('Tanya Arora'),
('Ritika Sharma'),
('Sahil Gupta'),
('Deepak Verma'),
('Kavya Sharma'),
('Manish Kumar');
GO

-- Insert sample data into product table

INSERT INTO Products (ProductName, Price, SupplierID) VALUES
('Rice 5kg', 320.00, 4),
('Wheat Flour 10kg', 420.00, 4),
('Sugar 1kg', 45.00, 8),
('Salt 1kg', 20.00, 8),
('Milk 1L', 32.00, 3),
('Curd 500g', 40.00, 3),
('Paneer 200g', 85.00, 3),
('Butter 500g', 240.00, 3),
('Eggs 12 Pack', 78.00, 1),
('Bread Large', 35.00, 7),
('Banana 1 Dozen', 60.00, 1),
('Apple 1kg', 140.00, 9),
('Potato 1kg', 28.00, 1),
('Onion 1kg', 34.00, 1),
('Tomato 1kg', 36.00, 1),
('Cooking Oil 1L', 155.00, 2),
('Tea 500g', 210.00, 8),
('Coffee 200g', 180.00, 8),
('Biscuits Pack', 25.00, 7),
('Soap Bar', 38.00, 10),
('Shampoo 180ml', 145.00, 10),
('Toothpaste 150g', 95.00, 10),
('Detergent 1kg', 110.00, 10),
('Orange Juice 1L', 120.00, 6),
('Mango Juice 1L', 125.00, 6),
('Chips Pack', 20.00, 7),
('Maggi Pack', 18.00, 7),
('Dal Arhar 1kg', 135.00, 5),
('Dal Moong 1kg', 128.00, 5),
('Chana 1kg', 92.00, 5);
GO

-- Insert sample data into inventory table

INSERT INTO Inventory (ProductID, Quantity, LastUpdated) VALUES
(1, 120, GETDATE()),
(2, 85, GETDATE()),
(3, 150, GETDATE()),
(4, 140, GETDATE()),
(5, 95, GETDATE()),
(6, 60, GETDATE()),
(7, 45, GETDATE()),
(8, 35, GETDATE()),
(9, 70, GETDATE()),
(10, 55, GETDATE()),
(11, 40, GETDATE()),
(12, 30, GETDATE()),
(13, 125, GETDATE()),
(14, 110, GETDATE()),
(15, 90, GETDATE()),
(16, 65, GETDATE()),
(17, 38, GETDATE()),
(18, 28, GETDATE()),
(19, 150, GETDATE()),
(20, 100, GETDATE()),
(21, 50, GETDATE()),
(22, 75, GETDATE()),
(23, 80, GETDATE()),
(24, 42, GETDATE()),
(25, 33, GETDATE()),
(26, 95, GETDATE()),
(27, 130, GETDATE()),
(28, 48, GETDATE()),
(29, 44, GETDATE()),
(30, 58, GETDATE());
GO

-- Insert sample data into orders table

INSERT INTO Orders (CustomerID, ProductID, Quantity, OrderDate, LastStatus, TotalAmount) VALUES
(1, 1, 2, '2026-03-01 10:00:00', 'Delivered', 640.00),
(2, 5, 3, '2026-03-01 11:00:00', 'Delivered', 96.00),
(3, 10, 2, '2026-03-01 12:00:00', 'Delivered', 70.00),
(4, 12, 1, '2026-03-01 13:00:00', 'Delivered', 140.00),
(5, 16, 1, '2026-03-01 14:00:00', 'Delivered', 155.00),
(6, 3, 4, '2026-03-02 09:15:00', 'Delivered', 180.00),
(7, 13, 5, '2026-03-02 10:30:00', 'Delivered', 140.00),
(8, 14, 3, '2026-03-02 11:10:00', 'Delivered', 102.00),
(9, 15, 2, '2026-03-02 12:20:00', 'Delivered', 72.00),
(10, 19, 6, '2026-03-02 01:25:00', 'Delivered', 150.00),
(11, 20, 3, '2026-03-03 09:00:00', 'Delivered', 114.00),
(12, 21, 1, '2026-03-03 09:45:00', 'Delivered', 145.00),
(13, 22, 2, '2026-03-03 10:30:00', 'Delivered', 190.00),
(14, 23, 1, '2026-03-03 11:15:00', 'Delivered', 110.00),
(15, 24, 2, '2026-03-03 12:00:00', 'Delivered', 240.00),
(16, 25, 2, '2026-03-03 12:40:00', 'Delivered', 250.00),
(17, 26, 5, '2026-03-04 09:10:00', 'Delivered', 100.00),
(18, 27, 6, '2026-03-04 10:00:00', 'Delivered', 108.00),
(19, 28, 2, '2026-03-04 10:50:00', 'Delivered', 270.00),
(20, 29, 1, '2026-03-04 11:30:00', 'Delivered', 128.00),
(1, 30, 2, '2026-03-04 12:00:00', 'Delivered', 184.00),
(2, 2, 1, '2026-03-05 09:00:00', 'Pending', 420.00),
(3, 4, 3, '2026-03-05 09:30:00', 'Delivered', 60.00),
(4, 6, 2, '2026-03-05 10:10:00', 'Delivered', 80.00),
(5, 7, 1, '2026-03-05 10:45:00', 'Delivered', 85.00),
(6, 8, 1, '2026-03-05 11:20:00', 'Delivered', 240.00),
(7, 9, 2, '2026-03-05 12:00:00', 'Delivered', 156.00),
(8, 11, 1, '2026-03-05 12:30:00', 'Delivered', 60.00),
(9, 17, 1, '2026-03-06 09:05:00', 'Delivered', 210.00),
(10, 18, 1, '2026-03-06 09:45:00', 'Cancelled', 180.00),
(11, 1, 1, '2026-03-06 10:15:00', 'Delivered', 320.00),
(12, 3, 2, '2026-03-06 10:45:00', 'Delivered', 90.00),
(13, 5, 2, '2026-03-06 11:20:00', 'Delivered', 64.00),
(14, 10, 3, '2026-03-06 12:10:00', 'Delivered', 105.00),
(15, 12, 2, '2026-03-07 09:00:00', 'Delivered', 280.00),
(16, 13, 4, '2026-03-07 09:40:00', 'Delivered', 112.00),
(17, 14, 2, '2026-03-07 10:25:00', 'Delivered', 68.00),
(18, 15, 2, '2026-03-07 11:15:00', 'Pending', 72.00),
(19, 16, 1, '2026-03-07 12:00:00', 'Delivered', 155.00),
(20, 19, 4, '2026-03-07 12:30:00', 'Delivered', 100.00);
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

