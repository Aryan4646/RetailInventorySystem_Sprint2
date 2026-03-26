interface Product {
    productID: number;
    productName: string;
    price: number;
}

interface Inventory {
    productID: number;
    quantity: number;
}

interface OrderItem {
    orderItemID?: number;
    orderID?: number;
    productID: number;
    quantity: number;
    unitPrice?: number;
    lineTotal?: number;
}

interface Order {
    orderID?: number;
    customerID: number;
    productID: number;
    quantity: number;
    orderDate: string;
    lastStatus?: string;
    totalAmount?: number;
    orderItems?: OrderItem[];
}

const apiBaseUrl = "http://localhost:5223";

const content = document.getElementById("content") as HTMLElement;
const messageBox = document.getElementById("messageBox") as HTMLElement;

let orderItemsCart: OrderItem[] = [];

function showMessage(message: string, type: string): void {
    messageBox.innerHTML = `<div class="alert alert-${type}">${message}</div>`;
}

function clearMessage(): void {
    messageBox.innerHTML = "";
}

async function showDashboard(): Promise<void> {
    clearMessage();

    content.innerHTML = `<div class="text-center py-4"><div class="spinner-border text-primary"></div></div>`;

    try {
        const [productsRes, inventoryRes, totalSalesRes] = await Promise.all([
            fetch(`${apiBaseUrl}/api/Product`),
            fetch(`${apiBaseUrl}/api/Inventory`),
            fetch(`${apiBaseUrl}/api/Order/totalsales`)
        ]);

        const products: Product[] = productsRes.ok ? await productsRes.json() : [];
        const inventory: Inventory[] = inventoryRes.ok ? await inventoryRes.json() : [];
        const totalSales: number = totalSalesRes.ok ? await totalSalesRes.json() : 0;

        const productMap = new Map<number, Product>();
        for (let p of products) productMap.set(p.productID, p);

        const totalProducts = products.length;
        const totalStockUnits = inventory.reduce((sum, i) => sum + i.quantity, 0);
        const totalInventoryValue = inventory.reduce((sum, i) => {
            const p = productMap.get(i.productID);
            return sum + (p ? p.price * i.quantity : 0);
        }, 0);
        const lowStockCount = inventory.filter(i => i.quantity < 10).length;

        content.innerHTML = `
            <h2 class="mb-4">Dashboard</h2>
            <div class="row mb-4">
                <div class="col-md-3">
                    <div class="card-box dashboard-blue">
                        <h5>Total Products</h5>
                        <h3>${totalProducts}</h3>
                    </div>
                </div>
                <div class="col-md-3">
                    <div class="card-box dashboard-green">
                        <h5>Total Stock Units</h5>
                        <h3>${totalStockUnits}</h3>
                    </div>
                </div>
                <div class="col-md-3">
                    <div class="card-box dashboard-orange">
                        <h5>Total Inventory Value</h5>
                        <h3>Rs. ${totalInventoryValue.toFixed(2)}</h3>
                    </div>
                </div>
                <div class="col-md-3">
                    <div class="card-box dashboard-gray">
                        <h5>Low Stock Alerts</h5>
                        <h3>${lowStockCount}</h3>
                    </div>
                </div>
            </div>
            <div class="row mb-4">
                <div class="col-md-4">
                    <div class="card-box dashboard-green">
                        <h5>Total Sales (Delivered)</h5>
                        <h3>Rs. ${totalSales.toFixed(2)}</h3>
                    </div>
                </div>
            </div>
        `;
    } catch {
        showMessage("Failed to load dashboard", "danger");
        content.innerHTML = "";
    }
}

async function showProducts(): Promise<void> {
    clearMessage();

    try {
        const response = await fetch(`${apiBaseUrl}/api/Product`);

        if (!response.ok) {
            throw new Error();
        }

        const products: Product[] = await response.json();

        let rows = "";

        for (let product of products) {
            rows += `
                <tr>
                    <td>${product.productID}</td>
                    <td>${product.productName}</td>
                    <td>${product.price}</td>
                </tr>
            `;
        }

        content.innerHTML = `
            <h2 class="mb-4">Product List</h2>
            <table class="table table-bordered">
                <thead class="table-dark">
                    <tr>
                        <th>Product ID</th>
                        <th>Product Name</th>
                        <th>Price</th>
                    </tr>
                </thead>
                <tbody>${rows}</tbody>
            </table>
        `;
    } catch {
        showMessage("Failed to load products", "danger");
    }
}

async function showInventory(): Promise<void> {
    clearMessage();

    try {
        const response = await fetch(`${apiBaseUrl}/api/Inventory`);

        if (!response.ok) {
            throw new Error();
        }

        const inventory: Inventory[] = await response.json();

        let rows = "";

        for (let item of inventory) {
            rows += `
                <tr>
                    <td>${item.productID}</td>
                    <td>${item.quantity}</td>
                </tr>
            `;
        }

        content.innerHTML = `
            <h2 class="mb-4">Manage Inventory</h2>

            <form id="inventoryForm" class="mb-4">
                <div class="mb-3">
                    <label class="form-label">Product ID</label>
                    <input type="number" class="form-control" id="inventoryProductID" required>
                </div>
                <div class="mb-3">
                    <label class="form-label">Quantity</label>
                    <input type="number" class="form-control" id="inventoryQuantity" required>
                </div>
                <button type="submit" class="btn btn-warning">Update Inventory</button>
            </form>

            <table class="table table-bordered">
                <thead class="table-dark">
                    <tr>
                        <th>Product ID</th>
                        <th>Quantity</th>
                    </tr>
                </thead>
                <tbody>${rows}</tbody>
            </table>
        `;

        const form = document.getElementById("inventoryForm") as HTMLFormElement;
        form.addEventListener("submit", updateInventory);
    } catch {
        showMessage("Failed to load inventory", "danger");
    }
}

async function updateInventory(event: Event): Promise<void> {
    event.preventDefault();
    clearMessage();

    const productID = Number((document.getElementById("inventoryProductID") as HTMLInputElement).value);
    const quantity = Number((document.getElementById("inventoryQuantity") as HTMLInputElement).value);

    try {
        const response = await fetch(`${apiBaseUrl}/api/Inventory/${productID}/${quantity}`, {
            method: "PUT"
        });

        if (response.ok) {
            showMessage("Inventory updated successfully", "success");
            showInventory();
        } else {
            const errorText = await response.text();
            showMessage(errorText || "Failed to update inventory", "danger");
        }
    } catch {
        showMessage("Server error", "danger");
    }
}

function showCreateOrder(): void {
    clearMessage();
    orderItemsCart = [];

    const today = new Date().toISOString().split("T")[0];

    content.innerHTML = `
        <h2 class="mb-4">Create Order</h2>

        <div class="form-container mb-4">
            <div class="mb-3">
                <label class="form-label">Customer ID</label>
                <input type="number" class="form-control" id="customerID" required>
            </div>
            <div class="mb-3">
                <label class="form-label">Order Date</label>
                <input type="date" class="form-control" id="orderDate" value="${today}" required>
            </div>

            <hr>
            <h5>Add Items</h5>

            <div class="row mb-3">
                <div class="col-md-5">
                    <label class="form-label">Product ID</label>
                    <input type="number" class="form-control" id="itemProductID">
                </div>
                <div class="col-md-4">
                    <label class="form-label">Quantity</label>
                    <input type="number" class="form-control" id="itemQuantity">
                </div>
                <div class="col-md-3 d-flex align-items-end">
                    <button class="btn btn-outline-primary w-100" onclick="addItemToCart()">+ Add Item</button>
                </div>
            </div>

            <div id="cartSection" style="display:none;">
                <table class="table table-sm table-bordered mb-3">
                    <thead class="table-light">
                        <tr><th>#</th><th>Product ID</th><th>Quantity</th><th></th></tr>
                    </thead>
                    <tbody id="cartBody"></tbody>
                </table>
            </div>

            <button class="btn btn-success" onclick="submitOrder()" id="submitOrderBtn" disabled>Submit Order</button>
            <button class="btn btn-secondary ms-2" onclick="showCreateOrder()">Reset</button>
        </div>

        <div id="orderResultSection"></div>
    `;
}

function addItemToCart(): void {
    const productID = Number((document.getElementById("itemProductID") as HTMLInputElement).value);
    const quantity = Number((document.getElementById("itemQuantity") as HTMLInputElement).value);

    if (!productID || productID <= 0) {
        showMessage("Please enter a valid Product ID", "danger");
        return;
    }
    if (!quantity || quantity <= 0) {
        showMessage("Quantity must be greater than 0", "danger");
        return;
    }

    const existing = orderItemsCart.find(i => i.productID === productID);
    if (existing) {
        existing.quantity += quantity;
    } else {
        orderItemsCart.push({ productID, quantity });
    }

    clearMessage();
    renderCart();

    (document.getElementById("itemProductID") as HTMLInputElement).value = "";
    (document.getElementById("itemQuantity") as HTMLInputElement).value = "";
}

function removeCartItem(index: number): void {
    orderItemsCart.splice(index, 1);
    renderCart();
}

function renderCart(): void {
    const cartSection = document.getElementById("cartSection") as HTMLElement;
    const cartBody = document.getElementById("cartBody") as HTMLElement;
    const submitBtn = document.getElementById("submitOrderBtn") as HTMLButtonElement;

    if (orderItemsCart.length === 0) {
        cartSection.style.display = "none";
        submitBtn.disabled = true;
        return;
    }

    cartSection.style.display = "block";
    submitBtn.disabled = false;

    cartBody.innerHTML = orderItemsCart.map((item, idx) => `
        <tr>
            <td>${idx + 1}</td>
            <td>${item.productID}</td>
            <td>${item.quantity}</td>
            <td><button class="btn btn-sm btn-outline-danger" onclick="removeCartItem(${idx})">Remove</button></td>
        </tr>
    `).join("");
}

async function submitOrder(): Promise<void> {
    clearMessage();

    const customerID = Number((document.getElementById("customerID") as HTMLInputElement).value);
    const orderDate = (document.getElementById("orderDate") as HTMLInputElement).value;

    if (!customerID || customerID <= 0) {
        showMessage("Please enter a valid Customer ID", "danger");
        return;
    }
    if (!orderDate) {
        showMessage("Please select an order date", "danger");
        return;
    }
    if (orderItemsCart.length === 0) {
        showMessage("Please add at least one item", "danger");
        return;
    }

    const orderData: Order = {
        customerID,
        productID: 0,
        quantity: 0,
        orderDate: new Date(orderDate).toISOString(),
        lastStatus: "Created",
        totalAmount: 0,
        orderItems: orderItemsCart.map(i => ({ productID: i.productID, quantity: i.quantity }))
    };

    try {
        const response = await fetch(`${apiBaseUrl}/api/Order/with-items`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(orderData)
        });

        if (response.ok) {
            const created: Order = await response.json();

            let itemRows = "";
            if (created.orderItems && created.orderItems.length > 0) {
                itemRows = created.orderItems.map((item, idx) => `
                    <tr>
                        <td>${idx + 1}</td>
                        <td>${item.productID}</td>
                        <td>${item.quantity}</td>
                        <td>Rs. ${(item.unitPrice ?? 0).toFixed(2)}</td>
                        <td>Rs. ${(item.lineTotal ?? 0).toFixed(2)}</td>
                    </tr>
                `).join("");
            }

            const resultSection = document.getElementById("orderResultSection") as HTMLElement;
            resultSection.innerHTML = `
                <div class="card border-success">
                    <div class="card-header bg-success text-white">
                        Order #${created.orderID} created successfully | Customer: ${created.customerID} | Total: Rs. ${(created.totalAmount ?? 0).toFixed(2)}
                    </div>
                    <div class="card-body p-0">
                        <table class="table table-bordered mb-0">
                            <thead class="table-dark">
                                <tr><th>#</th><th>Product ID</th><th>Qty</th><th>Unit Price</th><th>Line Total</th></tr>
                            </thead>
                            <tbody>${itemRows}</tbody>
                        </table>
                    </div>
                </div>
            `;

            orderItemsCart = [];
            renderCart();
        } else {
            const errorText = await response.text();
            showMessage(errorText || "Failed to create order", "danger");
        }
    } catch {
        showMessage("Server error", "danger");
    }
}

async function createOrder(event: Event): Promise<void> {
    event.preventDefault();
    await submitOrder();
}

function showCustomerOrders(): void {
    clearMessage();

    content.innerHTML = `
        <h2 class="mb-4">Customer Orders</h2>

        <div class="form-container mb-4">
            <div class="mb-3">
                <label class="form-label">Customer ID</label>
                <input type="number" class="form-control" id="lookupCustomerID">
            </div>
            <button class="btn btn-primary me-2" onclick="fetchCustomerOrders()">View Orders</button>
            <button class="btn btn-secondary" onclick="clearCustomerResults()">Clear</button>
        </div>

        <div id="customerOrderResults"></div>
    `;
}

async function fetchCustomerOrders(): Promise<void> {
    const resultsDiv = document.getElementById("customerOrderResults") as HTMLElement;
    const customerID = Number((document.getElementById("lookupCustomerID") as HTMLInputElement).value);

    if (!customerID || customerID <= 0) {
        showMessage("Please enter a valid Customer ID", "danger");
        return;
    }

    try {
        const response = await fetch(`${apiBaseUrl}/api/Order/customer/${customerID}`);

        if (response.status === 404) {
            resultsDiv.innerHTML = `<div class="alert alert-info">No orders found for Customer ID ${customerID}.</div>`;
            return;
        }

        if (!response.ok) {
            throw new Error();
        }

        const orders: Order[] = await response.json();

        if (!orders || orders.length === 0) {
            resultsDiv.innerHTML = `<div class="alert alert-info">No orders found for Customer ID ${customerID}.</div>`;
            return;
        }

        const totalSpend = orders.reduce((sum, o) => sum + (o.totalAmount ?? 0), 0);

        let orderCards = orders.map(o => {
            let itemRows = "";
            if (o.orderItems && o.orderItems.length > 0) {
                itemRows = o.orderItems.map(item => `
                    <tr>
                        <td>${item.productID}</td>
                        <td>${item.quantity}</td>
                        <td>Rs. ${(item.unitPrice ?? 0).toFixed(2)}</td>
                        <td>Rs. ${(item.lineTotal ?? 0).toFixed(2)}</td>
                    </tr>
                `).join("");
            }

            return `
            <div class="card mb-3">
                <div class="card-header d-flex justify-content-between">
                    <span><strong>Order #${o.orderID}</strong> - ${o.orderDate ? new Date(o.orderDate).toLocaleDateString() : ""}</span>
                    <span>
                        <span class="badge bg-secondary me-2">${o.lastStatus ?? ""}</span>
                        <strong>Rs. ${(o.totalAmount ?? 0).toFixed(2)}</strong>
                        <button class="btn btn-sm btn-outline-primary ms-2" onclick="showUpdateStatus(${o.orderID}, '${o.lastStatus}')">Update Status</button>
                    </span>
                </div>
                ${itemRows ? `
                <div class="card-body p-0">
                    <table class="table table-sm table-bordered mb-0">
                        <thead class="table-light"><tr><th>Product ID</th><th>Qty</th><th>Unit Price</th><th>Line Total</th></tr></thead>
                        <tbody>${itemRows}</tbody>
                    </table>
                </div>` : ""}
            </div>`;
        }).join("");

        resultsDiv.innerHTML = `
            <div class="row mb-3">
                <div class="col-md-4">
                    <div class="card-box dashboard-blue">
                        <h5>Customer ID</h5>
                        <h3>#${customerID}</h3>
                    </div>
                </div>
                <div class="col-md-4">
                    <div class="card-box dashboard-green">
                        <h5>Total Orders</h5>
                        <h3>${orders.length}</h3>
                    </div>
                </div>
                <div class="col-md-4">
                    <div class="card-box dashboard-orange">
                        <h5>Total Spend</h5>
                        <h3>Rs. ${totalSpend.toFixed(2)}</h3>
                    </div>
                </div>
            </div>
            ${orderCards}
        `;
    } catch {
        showMessage("Failed to fetch orders", "danger");
        resultsDiv.innerHTML = "";
    }
}

function showUpdateStatus(orderID: number, currentStatus: string | undefined): void {
    const statuses = ["Created", "Processing", "Shipped", "Delivered", "Cancelled"];
    const options = statuses.map(s => `<option value="${s}" ${s === currentStatus ? "selected" : ""}>${s}</option>`).join("");

    const modal = `
        <div class="card mt-2 border-primary" id="statusModal_${orderID}">
            <div class="card-body">
                <h6>Update Status for Order #${orderID}</h6>
                <div class="row">
                    <div class="col-md-4">
                        <select class="form-select" id="newStatus_${orderID}">
                            ${options}
                        </select>
                    </div>
                    <div class="col-md-3">
                        <button class="btn btn-primary" onclick="submitStatusUpdate(${orderID})">Update</button>
                        <button class="btn btn-secondary ms-1" onclick="document.getElementById('statusModal_${orderID}').remove()">Cancel</button>
                    </div>
                </div>
            </div>
        </div>
    `;

    const allCards = document.querySelectorAll("#customerOrderResults .card");

    allCards.forEach(card => {
        if (card.querySelector(`[onclick="showUpdateStatus(${orderID}, '${currentStatus}')"]`)) {
            const existing = document.getElementById(`statusModal_${orderID}`);
            if (existing) existing.remove();
            card.insertAdjacentHTML("afterend", modal);
        }
    });
}

async function submitStatusUpdate(orderID: number): Promise<void> {
    const newStatus = (document.getElementById(`newStatus_${orderID}`) as HTMLSelectElement).value;

    try {
        const response = await fetch(`${apiBaseUrl}/api/Order/${orderID}/status`, {
            method: "PUT",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(newStatus)
        });

        if (response.ok) {
            showMessage(`Order #${orderID} status updated to ${newStatus}`, "success");
            fetchCustomerOrders();
        } else {
            const errorText = await response.text();
            showMessage(errorText || "Failed to update status", "danger");
        }
    } catch {
        showMessage("Server error", "danger");
    }
}

function clearCustomerResults(): void {
    const resultsDiv = document.getElementById("customerOrderResults");
    if (resultsDiv) resultsDiv.innerHTML = "";
    const input = document.getElementById("lookupCustomerID") as HTMLInputElement;
    if (input) input.value = "";
    clearMessage();
}

function showOrders(): void {
    showCustomerOrders();
}

async function showReports(): Promise<void> {
    clearMessage();

    try {
        const [productsResponse, inventoryResponse] = await Promise.all([
            fetch(`${apiBaseUrl}/api/Product`),
            fetch(`${apiBaseUrl}/api/Inventory`)
        ]);

        if (!productsResponse.ok || !inventoryResponse.ok) {
            throw new Error();
        }

        const products: Product[] = await productsResponse.json();
        const inventory: Inventory[] = await inventoryResponse.json();

        const productMap = new Map<number, Product>();
        for (let product of products) {
            productMap.set(product.productID, product);
        }

        const lowStockItems = inventory.filter(item => item.quantity < 10);
        const overStockItems = inventory.filter(item => item.quantity > 150);

        let lowStockRows = "";
        let overStockRows = "";

        for (let item of lowStockItems) {
            const product = productMap.get(item.productID);

            lowStockRows += `
                <tr>
                    <td>${item.productID}</td>
                    <td>${product ? product.productName : "Unknown Product"}</td>
                    <td>${item.quantity}</td>
                </tr>
            `;
        }

        for (let item of overStockItems) {
            const product = productMap.get(item.productID);

            overStockRows += `
                <tr>
                    <td>${item.productID}</td>
                    <td>${product ? product.productName : "Unknown Product"}</td>
                    <td>${item.quantity}</td>
                </tr>
            `;
        }

        if (lowStockRows === "") {
            lowStockRows = `
                <tr>
                    <td colspan="3" class="text-center">No low stock products</td>
                </tr>
            `;
        }

        if (overStockRows === "") {
            overStockRows = `
                <tr>
                    <td colspan="3" class="text-center">No overstock products</td>
                </tr>
            `;
        }

        content.innerHTML = `
            <h2 class="mb-4">Reports</h2>

            <div class="card mb-4">
                <div class="card-body">
                    <h5 class="card-title mb-3">Low Stock Products (Less than 10)</h5>
                    <table class="table table-bordered">
                        <thead class="table-dark">
                            <tr>
                                <th>Product ID</th>
                                <th>Product Name</th>
                                <th>Quantity</th>
                            </tr>
                        </thead>
                        <tbody>${lowStockRows}</tbody>
                    </table>
                </div>
            </div>

            <div class="card">
                <div class="card-body">
                    <h5 class="card-title mb-3">Overstock Products (More than 150)</h5>
                    <table class="table table-bordered">
                        <thead class="table-dark">
                            <tr>
                                <th>Product ID</th>
                                <th>Product Name</th>
                                <th>Quantity</th>
                            </tr>
                        </thead>
                        <tbody>${overStockRows}</tbody>
                    </table>
                </div>
            </div>
        `;
    } catch {
        showMessage("Failed to load reports", "danger");
    }
}

(window as any).showDashboard = showDashboard;
(window as any).showProducts = showProducts;
(window as any).showInventory = showInventory;
(window as any).showCreateOrder = showCreateOrder;
(window as any).showOrders = showOrders;
(window as any).showCustomerOrders = showCustomerOrders;
(window as any).showReports = showReports;
(window as any).updateInventory = updateInventory;
(window as any).createOrder = createOrder;
(window as any).addItemToCart = addItemToCart;
(window as any).removeCartItem = removeCartItem;
(window as any).submitOrder = submitOrder;
(window as any).fetchCustomerOrders = fetchCustomerOrders;
(window as any).clearCustomerResults = clearCustomerResults;
(window as any).showUpdateStatus = showUpdateStatus;
(window as any).submitStatusUpdate = submitStatusUpdate;

showDashboard();
