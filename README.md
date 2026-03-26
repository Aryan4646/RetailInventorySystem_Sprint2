# RetailInventorySystem_Sprint2

# 🛒 Retail Order & Inventory Analytics System

A full-stack retail management system designed to handle **orders, inventory, and analytics** using a scalable layered architecture.

---

## 📌 Project Overview

This system solves key retail problems:
- Stockouts
- Duplicate orders
- Delayed order fulfillment
- Lack of inventory visibility

---

## 🎯 Objectives

- Efficient order and inventory management
- Prevent stock issues using validation
- Provide real-time UI interaction
- Generate reports & dashboards
- Build scalable architecture

---

## 🏗️ Architecture

### 🔹 Frontend
- HTML, CSS, Bootstrap, TypeScript
- Uses `fetch` to call backend APIs
- Dynamic UI rendering

### 🔹 Backend
- ASP.NET Core Web API
- Layered Architecture:
  - Controllers → Entry point
  - Services → Business logic
  - Repositories → Data access

### 🔹 Database
- SQL Server / Azure SQL
- Tables:
  - Customers
  - Products
  - Orders
  - Inventory
  - Suppliers
  - OrderItems

---

## 🔄 Data Flow (Create Order)

1. User sends order request
2. Controller receives request
3. Service validates data
4. Repository checks stock
5. Order saved in DB
6. Inventory updated
7. Response returned

---

## 🔗 API Endpoints

### Order
- `POST /api/order` → Create Order
- `GET /api/order/{id}` → Get Order
- `GET /api/order/customer/{id}` → Customer Orders

### Product
- `GET /api/product`
- `POST /api/product`
- `PUT /api/product`

### Inventory
- `GET /api/inventory`
- `PUT /api/inventory/{id}/{qty}`

---

## 📊 Reports

### SSRS Reports
- Daily Sales Performance
- Inventory Status

### Power BI Dashboard
- Sales by Product
- Stock Levels
- Order Status Distribution

---

## ☁️ Deployment

- Backend → Azure App Service
- Database → Azure SQL
- Frontend → Hostinger

---

## 🧠 Key Concepts Used

- Layered Architecture
- SOLID Principles
- REST APIs
- Exception Handling
- Input Validation

---

## 👨‍💻 Author

Aryan Sharma