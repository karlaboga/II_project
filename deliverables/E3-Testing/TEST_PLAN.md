# TEST PLAN – Bengos Restaurant App

| Document Version | 1.0 |
|---|---|
| Project Name | Bengos Restaurant App |
| Prepared By | Ada Gherasim |
| Date | May 10, 2026 |
| Test Strategy Ref | TEST_STRATEGY.md |

---

## 1. Introduction

This document defines the detailed test plan for **Bengos Restaurant App**. It contains the complete set of test cases with defined input values and expected results for each functional module: Login, Billing, Payment, Inventory, Staff Management, and Menu Viewer.

---

## 2. Features to be Tested

| Module | Features |
|---|---|
| **Login** | User authentication, input validation, error handling |
| **Billing** | Order display, quantity editing, discount calculation, totals |
| **Payment** | Cash payment, card payment, change calculation |
| **Inventory** | Add/Edit/Delete products, input validation |
| **Staff** | Shift scheduling, role-based permissions, file persistence |
| **Menu** | Category filtering, pill UI interaction |

---

## 3. Test Cases

### 3.1 Login Module

| Test ID | Test Case | Input Values | Expected Result |
|---|---|---|---|
| TC-LOGIN-001 | Valid login – waiter | username = `waiter1`, password = `pass1` | LoginWindow closes, DialogResult = true, Username = `waiter1`, Role = `waiter` |
| TC-LOGIN-002 | Valid login – admin | username = `admin`, password = `admin123` | LoginWindow closes, DialogResult = true, Username = `admin`, Role = `Admin` |
| TC-LOGIN-003 | Valid login – chef | username = `chef1`, password = `pass3` | LoginWindow closes, DialogResult = true, Username = `chef1`, Role = `chef` |
| TC-LOGIN-004 | Invalid password | username = `waiter1`, password = `wrongpass` | LblError displays "Invalid username or password!", password field cleared |
| TC-LOGIN-005 | Non-existent user | username = `ghost`, password = `anything` | LblError displays "Invalid username or password!", password field cleared |
| TC-LOGIN-006 | Empty username and password | username = `""`, password = `""` | LblError displays "Please enter username and password!" |
| TC-LOGIN-007 | Empty password only | username = `waiter1`, password = `""` | LblError displays "Please enter username and password!" |
| TC-LOGIN-008 | Users file missing | Delete or rename users.txt | MessageBox shows "Error loading users: ...", no crash |

### 3.2 Billing Module

| Test ID | Test Case | Input Values | Expected Result |
|---|---|---|---|
| TC-BILL-001 | View default order | Open BillingWindow | DataGrid shows 3 items: Cappucino (qty=3, $12.50), Tiramisu (qty=1, $7.00), IceCream (qty=3, $8.50). Subtotal = $62.00, Total = $62.00 |
| TC-BILL-002 | Edit quantity – valid | Click "Edit Order", change Cappucino qty to 2, click "Done Editing" | Cappucino Total = $25.00, Subtotal recalculated, Total updated |
| TC-BILL-003 | Edit quantity – zero | Change any item qty to 0 | MessageBox shows nothing, qty reverts (validation fails) |
| TC-BILL-004 | Edit quantity – negative | Change any item qty to -1 | MessageBox shows nothing, qty reverts |
| TC-BILL-005 | Apply discount – valid 10% | Open discount popup, enter `10`, click Apply | MessageBox "Discount of 10% applied!", Total = Subtotal - 10%, TxtDiscount shows "-$6.20 (10%)" |
| TC-BILL-006 | Apply discount – 0% | Open discount popup, enter `0`, click Apply | Discount applied, Total = Subtotal, TxtDiscount = "0.00" |
| TC-BILL-007 | Apply discount – 100% | Open discount popup, enter `100`, click Apply | Total = $0.00, full discount applied |
| TC-BILL-008 | Apply discount – invalid over 100 | Open discount popup, enter `150`, click Apply | MessageBox "Please enter a value between 0 and 100." |
| TC-BILL-009 | Apply discount – negative | Open discount popup, enter `-5`, click Apply | MessageBox "Please enter a value between 0 and 100." |
| TC-BILL-010 | Apply discount – non-numeric text | Open discount popup, enter `abc`, click Apply | MessageBox "Please enter a value between 0 and 100." |
| TC-BILL-011 | Discount live preview | Open discount popup, type `10` in textbox | Calculated discount label updates to show `$6.20` in real-time |
| TC-BILL-012 | Navigate to Payment | Click "Pay" button | PaymentWindow opens with correct total displayed |

### 3.3 Payment Module

| Test ID | Test Case | Input Values | Expected Result |
|---|---|---|---|
| TC-PAY-001 | Display total | Open PaymentWindow with total = $55.80 | TxtAmount shows `$55.80` |
| TC-PAY-002 | Card payment selected | Select "Card" radio button | CashPanel hidden (Collapsed) |
| TC-PAY-003 | Card payment confirm | Select Card, click Confirm | MessageBox "Card payment of $55.80 confirmed! Thank you!" |
| TC-PAY-004 | Cash payment – exact amount | Select Cash, enter `55.80` | TxtChange = `$0.00` (green), click Confirm → receipt shows Total=$55.80, Cash=$55.80, Change=$0.00 |
| TC-PAY-005 | Cash payment – with change | Select Cash, enter `100.00` | TxtChange = `$44.20` (green), click Confirm → receipt shows Total=$55.80, Cash=$100.00, Change=$44.20 |
| TC-PAY-006 | Cash payment – insufficient funds | Select Cash, enter `30.00` | TxtChange = "Not enough cash" (red), click Confirm → MessageBox "Please enter a valid cash amount that covers the total." |
| TC-PAY-007 | Cash payment – invalid input | Select Cash, enter `abc` | TxtChange = "-", click Confirm → MessageBox "Please enter a valid cash amount that covers the total." |
| TC-PAY-008 | Cash payment – empty field | Select Cash, leave field blank | TxtChange = "-", click Confirm → MessageBox "Please enter a valid cash amount that covers the total." |
| TC-PAY-009 | Back button | Click "Back" button | PaymentWindow closes, returns to BillingWindow |

### 3.4 Inventory Module

| Test ID | Test Case | Input Values | Expected Result |
|---|---|---|---|
| TC-INV-001 | Add product – valid | Name=`Flour`, Category=`Spices`, Qty=`10`, Unit=`kg`, MinStock=`5` | Product added to DataGrid with ID=1, all fields cleared |
| TC-INV-002 | Add product – empty name | Name=`""`, fill rest | MessageBox "Please fill in all fields before adding!" |
| TC-INV-003 | Add product – invalid quantity text | Name=`Sugar`, Qty=`abc` | MessageBox "Please enter valid numbers for Quantity and Min Stock!" |
| TC-INV-004 | Add product – quantity zero | Name=`Salt`, Qty=`0` | MessageBox "Please enter valid numbers for Quantity and Min Stock!" |
| TC-INV-005 | Add product – quantity negative | Name=`Pepper`, Qty=`-5` | MessageBox "Please enter valid numbers for Quantity and Min Stock!" |
| TC-INV-006 | Add product – no category selected | Name=`Rice`, Category not selected | MessageBox "Please fill in all fields before adding!" |
| TC-INV-007 | Edit product – change quantity | Name=`Flour`, enter Qty=`25`, click Edit | Flour quantity updated to 25, MessageBox "Product updated successfully!" |
| TC-INV-008 | Edit product – change category | Name=`Flour`, select Category=`Bakery`, click Edit | Flour category changed to Bakery |
| TC-INV-009 | Edit product – non-existent | Name=`NonExistent`, click Edit | MessageBox "Product not found. To edit, the Name must match exactly." |
| TC-INV-010 | Edit product – empty name | Name=`""`, click Edit | MessageBox "Please enter the name of the product you wish to edit." |
| TC-INV-011 | Delete product – valid | Name=`Flour`, click Delete | Flour removed from DataGrid |
| TC-INV-012 | Delete product – non-existent | Name=`Ghost`, click Delete | MessageBox "Product not found!" |
| TC-INV-013 | Add multiple products | Add 3 products sequentially | IDs auto-increment (1, 2, 3), all appear in DataGrid |

### 3.5 Staff Management Module

| Test ID | Test Case | Input Values | Expected Result |
|---|---|---|---|
| TC-STAFF-001 | Login as Admin, view Staff window | username=`admin`, password=`admin123`, open Staff | CmbStaff, BtnAdd, BtnDelete, BtnClear all enabled |
| TC-STAFF-002 | Login as waiter, view Staff window | username=`waiter1`, password=`pass1`, open Staff | CmbStaff disabled and locked to `waiter1`, BtnAdd enabled, BtnDelete and BtnClear disabled |
| TC-STAFF-003 | Admin adds shift | Staff=`waiter1`, Day=`Monday`, Shift=`Morning`, Overtime=unchecked, click Add | Shift appears in DataGrid, saved to shifts.txt, MessageBox "Shift added successfully!" |
| TC-STAFF-004 | Admin adds shift with overtime | Staff=`chef1`, Day=`Friday`, Shift=`Evening`, Overtime=checked | Shift added with Overtime=Yes, saved to shifts.txt |
| TC-STAFF-005 | Waiter adds own shift | Staff=`waiter1` (locked), Day=`Tuesday`, Shift=`Afternoon`, click Add | Shift added successfully |
| TC-STAFF-006 | Waiter tries to add for another staff | Not possible – CmbStaff is disabled for waiter (auto-locked) | N/A – UI constraint prevents this |
| TC-STAFF-007 | Admin deletes shift | Select a shift row, click Delete | Shift removed from DataGrid, shifts.txt updated |
| TC-STAFF-008 | Non-admin tries to delete | Login as waiter, select shift, BtnDelete is disabled | UI prevents deletion |
| TC-STAFF-009 | Admin clears all shifts | Click Clear, confirm Yes in dialog | All shifts removed from DataGrid, shifts.txt cleared |
| TC-STAFF-010 | Add shift without selecting day | Leave day unselected, click Add | MessageBox "Please select day and shift type!" |
| TC-STAFF-011 | Duplicate shift – same staff + day | Add shift waiter1/Monday/Morning, then add waiter1/Monday/Afternoon | Second shift replaces first (only one shift per staff per day) |
| TC-STAFF-012 | Load shifts from file | Open Staff window with existing shifts.txt | All saved shifts loaded into DataGrid |

### 3.6 Menu Module

| Test ID | Test Case | Input Values | Expected Result |
|---|---|---|---|
| TC-MENU-001 | Open Menu – default view | Open MenuWindow | All 9 items displayed in ListView, "All" pill is dark (active) |
| TC-MENU-002 | Filter by Food | Click "Food" pill | Only 3 items shown: Margherita Pizza, Caesar Salad, Grilled Salmon. Food pill becomes active (dark) |
| TC-MENU-003 | Filter by Drinks | Click "Drinks" pill | Only 3 items shown: Cappuccino, Fresh Orange Juice, Iced Tea |
| TC-MENU-004 | Filter by Desserts | Click "Desserts" pill | Only 3 items shown: Tiramisu, Chocolate Cake, Ice Cream |
| TC-MENU-005 | Filter back to All | Click "All" pill | All 9 items displayed again |
| TC-MENU-006 | Pill color changes on selection | Click "Food", then "Drinks" | Food pill returns to white, Drinks pill turns dark |
| TC-MENU-007 | Verify item details | View any item | Name, Category, Price, Description, and Ingredients all displayed correctly |

---

## 4. Test Environment

| Component | Specification |
|---|---|
| **OS** | Windows 10 / Windows 11 x64 |
| **Framework** | .NET 8.0 WPF (net8.0-windows) |
| **IDE** | Visual Studio 2022 |
| **Build** | Debug mode, Any CPU |

---

## 5. Test Execution Schedule

| Phase | Duration | Activities |
|---|---|---|
| Test Case Preparation | 1 day | Document all test cases |
| Test Execution – Round 1 | 1 day | Execute all test cases, log defects |
| Bug Fix & Retest | 1 day | Fix defects, re-execute failed tests |
| Test Summary Report | 0.5 day | Generate final report |

---

## 6. Risks & Mitigation

| Risk | Mitigation |
|---|---|
| Inventory data is in-memory (lost on close) | Test all inventory operations in a single session |
| shifts.txt file may not exist initially | Verify app handles missing file gracefully |
| No automated testing | Manual execution with thorough checklist |
| UI-dependent tests may vary by screen resolution | Test on standard 1920x1080 resolution |

---

## 7. Test Execution Summary

| Module | Total Tests | Pass | Fail | Blocked |
|---|---|---|---|---|
| Login | 8 | – | – | – |
| Billing | 12 | – | – | – |
| Payment | 9 | – | – | – |
| Inventory | 13 | – | – | – |
| Staff | 12 | – | – | – |
| Menu | 7 | – | – | – |
| **Total** | **61** | **–** | **–** | **–** |
