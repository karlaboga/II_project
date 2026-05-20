# TC-03: Verify Inventory Management Edit Item Functionality

| Field | Value |
|---|---|
| **Test ID** | TC-03 |
| **Issue Reference** | #12 |
| **Tester** | Ada Gherasim |
| **Date** | May 20, 2026 |
| **Status** | ✅ Passed |

---

## Objective

Verify that an existing item in the inventory can be successfully edited through the application's interface.

---

## Preconditions

- Application is running (as verified in TC-01)
- At least one item exists in the inventory (from TC-02 or manually added)
- Inventory Management window is accessible

---

## Test Steps

| Step | Action | Expected Result | Actual Result | Screenshot |
|---|---|---|---|---|
| 1 | Navigate to Inventory Management section | Inventory window is displayed | ✅ Displayed | [Screenshot] |
| 2 | Select an existing item from the inventory list | Item is highlighted/selected | ✅ Selected | [Screenshot] |
| 3 | Click the 'Edit' button | Edit form opens with item details | ✅ Opens correctly | [Screenshot] |
| 4 | Modify item details: change Name to "Updated Test Item", Qty to 25 | Fields are updated | ✅ Updated | [Screenshot] |
| 5 | Click 'Save' or 'Update' | Changes are saved | ✅ Saved | [Screenshot] |
| 6 | Verify the changes are reflected in the inventory list | Updated item is displayed | ✅ Displayed correctly | [Screenshot] |

---

## Expected Result

- ✅ Item is successfully updated
- ✅ Modified details are correctly stored and displayed
- ✅ Confirmation message is shown

---

## Notes

Item was edited successfully. All changes were correctly reflected in the inventory list.
