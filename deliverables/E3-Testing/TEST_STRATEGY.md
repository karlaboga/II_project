# TEST STRATEGY – Bengos Restaurant App

| Document Version | 1.0 |
|---|---|
| Project Name | Bengos Restaurant App |
| Prepared By | Ada Gherasim |
| Date | May 10, 2026 |

---

## 1. Introduction

This document defines the test strategy for **Bengos Restaurant App**, a WPF desktop application built with .NET 8.0 that provides restaurant management functionality including staff authentication, billing & payment processing, inventory management, staff shift scheduling, and digital menu browsing. The strategy outlines the testing approach, objectives, scope, levels, types, environment, and deliverables.

## 2. Test Objectives

- Verify that all functional requirements are correctly implemented
- Ensure proper input validation and error handling
- Validate role-based access control (Admin vs. regular staff)
- Confirm correct financial calculations (subtotals, discounts, change)
- Ensure data persistence correctness (shifts.txt, users.txt)
- Verify GUI responsiveness and correct UI state transitions

## 3. Scope of Testing

### In Scope

| Module | Key Functionality |
|---|---|
| **Login** | Authentication, input validation, error messages |
| **Billing** | Order display, quantity editing, discount application, total calculation |
| **Payment** | Cash/card selection, change calculation, insufficient funds handling |
| **Inventory** | CRUD operations (Add/Edit/Delete), input validation, field clearing |
| **Staff** | Shift scheduling, role-based permissions, file persistence |
| **Menu** | Category filtering (All/Food/Drinks/Desserts), visual pill selection |

### Out of Scope

- DigitalClientMenu web application (ASP.NET Core MVC)
- Standalone projects (Homework/Billing, Invetory, inventoryWPF, hw1)
- Performance testing
- Load / stress testing
- Security penetration testing

## 4. Test Levels

| Level | Description |
|---|---|
| **Integration Testing** | Test interaction between windows (Login → Staff, Billing → Payment) |
| **System Testing** | End-to-end workflows: full order → discount → payment → receipt |

## 5. Test Types

| Type | Focus |
|---|---|
| **Functional Testing** | All features work according to specifications |
| **GUI / UI Testing** | Window layout, button states, colors, visibility |
| **Negative Testing** | Invalid inputs, empty fields, out-of-range values |
| **Boundary Testing** | Discount 0% and 100%, quantity = 1, cash = exact total |
| **Security Testing** | Role-based access: waivers cannot modify other staff shifts |
| **Data Persistence Testing** | Shifts saved to / loaded from `shifts.txt` correctly |

## 6. Testing Environment

| Component | Specification |
|---|---|
| **OS** | Windows 10 / Windows 11 x64 |
| **Framework** | .NET 8.0 WPF (net8.0-windows) |
| **IDE** | Visual Studio 2022 |
| **Source Control** | Git (GitHub) |
| **Test Documentation** | Markdown files in deliverables/E3-Testing/ |

## 7. Roles & Responsibilities

| Role | Name | Responsibilities |
|---|---|---|
| **Tester** | Ada Gherasim | Test case design, test execution, defect reporting |
| **Developer** | (Team) | Bug fixes, code review |

## 8. Entry & Exit Criteria

### Entry Criteria

- Application builds successfully with no compilation errors
- All source code is available in the repository
- Test environment is set up and operational

### Exit Criteria

- All planned test cases have been executed
- At least 90% of test cases pass
- All critical/blocker defects are resolved
- Test summary report has been generated

## 9. Deliverables

| Deliverable | Description |
|---|---|
| **Test Strategy** | This document |
| **Test Plan** | Detailed test plan with test cases, input values, and expected results |
| **Test Execution Report** | Summary of test execution results |

---

## 10. Risks & Mitigation

| Risk | Impact | Mitigation |
|---|---|---|
| No automated testing framework | Manual testing is time-consuming | Focus on critical paths first |
| In-memory inventory (no persistence) | Data lost on window close | Test within single session |
| Multiple similar projects cause confusion | Wrong project tested | Clearly define scope |
