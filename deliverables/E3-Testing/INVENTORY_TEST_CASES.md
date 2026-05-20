# Inventory Management - Test Cases

| Document Version | 1.0 |
|---|---|
| Project Name | Bengos Restaurant App |
| Prepared By | Ada Gherasim |
| Date | May 20, 2026 |
| Issue Reference | #5 |

---

## Scope

This document contains all test cases for the **Inventory Management** module of the Bengos Restaurant App, covering Add, Edit, Delete operations and input validation.

---

## Test Cases

### TC-INV-001: Add product – valid

| Field | Value |
|---|---|
| **Name** | Flour |
| **Category** | Spices |
| **Quantity** | 10 |
| **Unit** | kg |
| **Min Stock** | 5 |
| **Expected Result** | Product added to DataGrid with ID=1, all fields cleared |
| **Status** | ✅ Passed |

[Screenshot placeholder]

---

### TC-INV-002: Add product – empty name

| Field | Value |
|---|---|
| **Name** | (empty) |
| **Other fields** | filled |
| **Expected Result** | MessageBox "Please fill in all fields before adding!" |
| **Status** | ✅ Passed |

[Screenshot placeholder]

---

### TC-INV-003: Add product – invalid quantity text

| Field | Value |
|---|---|
| **Name** | Sugar |
| **Quantity** | abc |
| **Expected Result** | MessageBox "Please enter valid numbers for Quantity and Min Stock!" |
| **Status** | ✅ Passed |

[Screenshot placeholder]

---

### TC-INV-004: Add product – quantity zero

| Field | Value |
|---|---|
| **Name** | Salt |
| **Quantity** | 0 |
| **Expected Result** | MessageBox "Please enter valid numbers for Quantity and Min Stock!" |
| **Status** | ✅ Passed |

[Screenshot placeholder]

---

### TC-INV-005: Add product – quantity negative

| Field | Value |
|---|---|
| **Name** | Pepper |
| **Quantity** | -5 |
| **Expected Result** | MessageBox "Please enter valid numbers for Quantity and Min Stock!" |
| **Status** | ✅ Passed |

[Screenshot placeholder]

---

### TC-INV-006: Add product – no category selected

| Field | Value |
|---|---|
| **Name** | Rice |
| **Category** | (not selected) |
| **Expected Result** | MessageBox "Please fill in all fields before adding!" |
| **Status** | ✅ Passed |

[Screenshot placeholder]

---

### TC-INV-007: Edit product – change quantity

| Field | Value |
|---|---|
| **Name** | Flour |
| **New Quantity** | 25 |
| **Expected Result** | Flour quantity updated to 25, MessageBox "Product updated successfully!" |
| **Status** | ✅ Passed |

[Screenshot placeholder]

---

### TC-INV-008: Edit product – change category

| Field | Value |
|---|---|
| **Name** | Flour |
| **New Category** | Bakery |
| **Expected Result** | Flour category changed to Bakery |
| **Status** | ✅ Passed |

[Screenshot placeholder]

---

### TC-INV-009: Edit product – non-existent

| Field | Value |
|---|---|
| **Name** | NonExistent |
| **Expected Result** | MessageBox "Product not found. To edit, the Name must match exactly." |
| **Status** | ✅ Passed |

[Screenshot placeholder]

---

### TC-INV-010: Edit product – empty name

| Field | Value |
|---|---|
| **Name** | (empty) |
| **Expected Result** | MessageBox "Please enter the name of the product you wish to edit." |
| **Status** | ✅ Passed |

[Screenshot placeholder]

---

### TC-INV-011: Delete product – valid

| Field | Value |
|---|---|
| **Name** | Flour |
| **Expected Result** | Flour removed from DataGrid |
| **Status** | ✅ Passed |

[Screenshot placeholder]

---

### TC-INV-012: Delete product – non-existent

| Field | Value |
|---|---|
| **Name** | Ghost |
| **Expected Result** | MessageBox "Product not found!" |
| **Status** | ✅ Passed |

[Screenshot placeholder]

---

### TC-INV-013: Add multiple products

| Field | Value |
|---|---|
| **Action** | Add 3 products sequentially |
| **Expected Result** | IDs auto-increment (1, 2, 3), all appear in DataGrid |
| **Status** | ✅ Passed |

[Screenshot placeholder]

---

## Execution Summary

| Total Tests | Passed | Failed | Blocked |
|---|---|---|---|
| 13 | 13 | 0 | 0 |
