# Supermarket SaaS Ecosystem: Modular Architecture (Bounded Contexts)

This document defines the final approved modular structure for the Supermarket SaaS platform. By dividing the system into **Bounded Contexts**, we ensure that each module is responsible for a specific business domain, making the system easier to develop, maintain, and scale into microservices in the future.

---

### 1. Identity Module

**Primary Responsibility:** Authentication and Global Security.
This module acts as the "Security Guard" for the entire platform. It handles the "Who are you?" part of the system.

- **Global User Registry:** Manages the master account for every person (Shopkeepers, Merchants, Workers, and Customers).
- **Authentication:** Handles secure Login, Logout, and Multi-Factor Authentication (MFA).
- **Token Issuance:** Generates the signed JWT tokens. It "Printers" the token using security keys, but fills it with information (OrgId/WorkspaceId) provided by the SaaS Module.
- **Managed Credentials:** Allows Shopkeepers/Merchants to directly create usernames and passwords for their hired workers (Cashiers/Warehouse staff).
- **Security Recovery:** Manages password resets and account security protocols.

---

### 2. SaaS & Subscription Module

**Primary Responsibility:** Business Hierarchy and Subscription Billing.
This module is the "Business Brain." It manages the structure of the companies and whether they have paid to use the system.

- **Organization & Workspace Management:** Defines the "Organization" (The Company) and the "Workspaces" (The individual shops or warehouses).
- **Membership & Access Control:** Manages the "Link" between a User and a Shop. It determines if a user is an Owner, Manager, or Cashier in a specific location.
- **Subscription Engine:** Manages payment plans (Trial, Pro, Enterprise) for Organizations and enforces feature limits (e.g., "Maximum 3 Shops allowed").
- **Contextual Security:** Enforces "Where and When" a worker can log in, including IP whitelisting, device binding, and business hour restrictions.
- **Worker Termination:** Provides the immediate "Kill Switch" to block a worker's access the moment they are fired.

---

### 3. Catalog & Inventory Module

**Primary Responsibility:** Product Master Data and Physical Stock Control.
This module combines the "Description" of what you sell with the "Quantity" of what you have.

- **Product Definition:** Manages the master list of products, barcodes (EAN/UPC), categories, and brands for an Organization.
- **Localized Stock Tracking:** Tracks the real-time quantity of products inside each specific **Workspace (Shop).**
- **Stock Auditing:** Handles manual stock counts, adjustments for damaged items, and stock-in/stock-out history.
- **Alerts:** Manages low-stock notifications for specific shop locations.
- **Multi-Store Sync:** Ensures that while the "Product Name" is shared across the whole company, the "Stock Count" is unique to each shop.

---

### 4. Sales & POS (Point of Sale) Module

**Primary Responsibility:** The Retail Checkout Process.
This is the high-speed interface used by the Shopkeeper’s staff to sell to the end Customer.

- **Retail Checkout:** Handles scanning items, calculating totals, applying discounts, and finalizing the sale.
- **Receipt Management:** Generates digital and physical receipts and stores the retail transaction history.
- **Cashier Sessions:** Tracks "Open/Close" times for cashier drawers and manages worker shifts.
- **Retail Logic:** Once a sale is finished, it notifies the Catalog/Inventory module to reduce stock and the Finance module to record revenue.

---

### 5. Procurement & Marketplace Module

**Primary Responsibility:** B2B Marketplace and Supply Chain.
This module is the "Bridge" connecting the Merchant (Wholesaler) to the Shopkeeper (Retailer).

- **Merchant Portal:** Allows Merchants to upload their wholesale catalogs and manage their business profile.
- **B2B Ordering:** Allows Shopkeepers to browse Merchant products and place Purchase Orders to restock their shops.
- **Order Fulfillment:** Allows Merchants to receive orders from Shopkeepers, accept them, and track the shipping status.
- **Delivery Integration:** Automatically triggers a stock increase in the Shopkeeper's **Inventory** once a Merchant's delivery is confirmed.

---

### 6. Finance & Accounting Module

**Primary Responsibility:** Internal Ledger and Debt Management.
This is the "Accountant" of the system. It records the "Internal" movement of value.

- **General Ledger:** Uses Double-Entry Bookkeeping to record Assets, Liabilities, Revenue, and Expenses for each business.
- **Customer Debt (Debit):** Manages the "Client Accounts" where shopkeepers track debt for customers who buy on credit.
- **Customer Portal:** Provides a read-only view for invited Customers to see their balance, debt history, and invitation status.
- **Financial Reporting:** Generates Profit & Loss statements and Balance Sheets per Shop or for the entire Organization.

---

### 7. Payments Module

**Primary Responsibility:** External Money Movement (Technical Integration).
This module handles the "External" world. It is the technical bridge to banks and wallets.

- **Gateway Integration:** Manages the technical connection to **PayPal**, Stripe, or digital wallets.
- **Transaction Logging:** Stores the technical "Proof of Payment" (Transaction IDs, Webhook responses from PayPal).
- **Payment Orchestration:** Processes subscription payments for the SaaS Module and order payments for the Marketplace.
- **Status Reporting:** Notifies the Finance Module when an external payment is successfully "Cleared" so the accountant can record it.

---

### Summary of Module Interaction (The "Big Picture")

1.  **Identity** identifies the person.
2.  **SaaS** identifies the Shop/Organization and checks the Subscription status.
3.  **Catalog & Inventory** manages what is being sold and how many are left on the shelf.
4.  **Sales** handles the customer checkout.
5.  **Procurement** manages the Merchant-to-Shopkeeper wholesale orders.
6.  **Payments** handles the technical talk with PayPal/Wallets.
7.  **Finance** records all the final numbers in the accounting books and tracks customer debt.
