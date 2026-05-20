# TC-02: Verify Inventory Management Add Item Functionality

| Field | Value |
|---|---|
| **Test ID** | TC-02 |
| **Issue Reference** | #11 |
| **Tester** | Ada Gherasim |
| **Date** | May 20, 2026 |
| **Status** | ✅ Passed |

---

## Objective

Verify that a new item can be successfully added to the inventory through the application's interface.

---

## Preconditions

- Application is running (as verified in TC-01)
- Inventory Management window is accessible

---

## Test Steps

| Step | Action | Expected Result | Actual Result | Screenshot |
|---|---|---|---|---|
| 1 | Navigate to Inventory Management section | Inventory window is displayed | ✅ Displayed | [Screenshot] |
| 2 | Click the 'Add Item' button | Add form/fields become active | ✅ Active | [Screenshot] |
| 3 | Enter item details: Name=Test Item, Qty=10, Category=Electronics, Price=99.99 | Fields are populated | ✅ Populated | [Screenshot] |
| 4 | Click 'Save' or 'Add' | Item is saved | ✅ Saved successfully | [Screenshot] |
| 5 | Verify the new item appears in the inventory list | Item is visible in the list | ✅ Visible | [Screenshot] |

---

## Expected Result

- ✅ New item is successfully added
- ✅ Item details are correctly stored and displayed
- ✅ Confirmation message is shown

---

## Notes

Item was added successfully. All fields were correctly saved and displayed in the inventory list.
