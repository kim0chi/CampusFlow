# Payment & Billing System Guide

## New Features

### Student Billing System
- Students have account balances with multiple fee types
- Fee types: Tuition, Miscellaneous, Laboratory, Library, Athletic, Insurance, ID Card, Registration
- Real-time balance tracking per semester

### Payment Requirement
- Students must pay **at least 30%** of their balance before enrollment can proceed to Accounting approval
- Payment is required after Dean approval and before Accounting review

### Promissory Note System
- Students who cannot pay 30% can apply for a promissory note
- Promissory note requires:
  - Reason for inability to pay
  - Repayment deadline
  - Campus Director approval
- If approved, student can proceed without payment

### Updated Enrollment Workflow

**Payment Path:**
1. Student submits enrollment → **Submitted**
2. Dean reviews and approves → **AwaitingPayment**
3. Student pays ≥30% of balance → **AccountingReview**
4. Accounting verifies payment → **SAOReview**
5. Continue normal workflow (SAO → Library → Records → Enrolled)

**Promissory Note Path:**
1. Student submits enrollment → **Submitted**
2. Dean reviews and approves → **AwaitingPayment**
3. Student submits promissory note → **CampusDirectorReview**
4. Campus Director approves → **AccountingReview**
5. Accounting verifies promissory note → **SAOReview**
6. Continue normal workflow (SAO → Library → Records → Enrolled)

## New User Roles & Credentials

### Cashier
- **Email:** cashier@university.edu
- **Password:** Cashier123!
- **Name:** Maria Santos
- **Capabilities:**
  - View all student balances
  - Process student payments
  - Generate payment receipts
  - View payment history

### Campus Director
- **Email:** campusdirector@university.edu
- **Password:** Director123!
- **Name:** Dr. Roberto Cruz
- **Capabilities:**
  - Review promissory note applications
  - Approve/reject promissory notes with comments
  - View promissory note history

## Sample Student Balances

### John Doe (john.doe@student.edu)
- **Total Balance:** ₱39,500
- **Required Payment (30%):** ₱11,850
- **Fees:**
  - Tuition: ₱28,000
  - Miscellaneous: ₱5,000
  - Laboratory: ₱3,000
  - Library: ₱1,500
  - Athletic: ₱1,000
  - Insurance: ₱800
  - ID Card: ₱200

### Jane Smith (jane.smith@student.edu)
- **Total Balance:** ₱48,500
- **Required Payment (30%):** ₱14,550
- **Fees:**
  - Tuition: ₱35,000
  - Miscellaneous: ₱6,000
  - Laboratory: ₱5,000
  - Library: ₱1,500
  - Athletic: ₱1,000

### Mike Johnson (mike.johnson@student.edu)
- **Total Balance:** ₱32,500
- **Required Payment (30%):** ₱9,750
- **Fees:**
  - Tuition: ₱25,000
  - Miscellaneous: ₱4,500
  - Library: ₱1,500
  - Athletic: ₱1,000
  - Registration: ₱500

### Alex Wilson (alex.wilson@student.edu)
- **Total Balance:** ₱36,700
- **Required Payment (30%):** ₱11,010
- **Fees:**
  - Tuition: ₱26,000
  - Miscellaneous: ₱5,000
  - Laboratory: ₱3,000
  - Library: ₱1,500
  - Athletic: ₱1,000
  - ID Card: ₱200

## Key Controllers

### CashierController (`/Cashier/...`)
- `Index` - Dashboard with all student balances
- `ProcessPayment` - Form to process payments
- `PaymentReceipt` - View payment receipt
- `PaymentHistory` - View all payments with filters
- `StudentAccount` - View detailed student account

### StudentAccountController (`/StudentAccount/...`)
- `Index` - View own balance, fees, and payments
- `MakePayment` - Submit payment form
- `PaymentReceipt` - View payment receipt

### PromissoryNoteController (`/PromissoryNote/...`)
- `Submit` - Student submits promissory note
- `PendingReview` - Campus Director views pending notes
- `Review` - Campus Director reviews note
- `Approve` - Campus Director approves note
- `Reject` - Campus Director rejects note
- `Status` - Student views promissory note status

## Testing the System

### Test Scenario 1: Student Makes Payment
1. Login as student (john.doe@student.edu / Student123!)
2. Enroll in a course
3. Wait for Dean approval (login as dean@university.edu / Dean123!)
4. Once status is "AwaitingPayment", go to Account page
5. Click "Make Payment" and pay at least 30%
6. Verify enrollment moves to Accounting Review

### Test Scenario 2: Student Applies for Promissory Note
1. Login as student (jane.smith@student.edu / Student123!)
2. Enroll in a course
3. Wait for Dean approval
4. Once status is "AwaitingPayment", apply for promissory note
5. Provide reason and repayment deadline
6. Logout and login as Campus Director (campusdirector@university.edu / Director123!)
7. Review and approve the promissory note
8. Verify enrollment moves to Accounting Review

### Test Scenario 3: Cashier Processes Payment
1. Login as cashier (cashier@university.edu / Cashier123!)
2. View student balances on dashboard
3. Click "Process Payment"
4. Select student, enter amount, payment method
5. View generated receipt
6. Check payment history

### Test Scenario 4: Accounting Verifies Payment
1. Login as accounting (accounting@university.edu / Accounting123!)
2. View pending approvals
3. See payment information (amount paid, percentage, or promissory note)
4. Approve if requirement is met (≥30% or approved promissory note)

## Database Tables

### New Tables
- `StudentAccounts` - Student balance tracking per semester
- `Fees` - Individual fees for students
- `Payments` - Payment records
- `PromissoryNotes` - Promissory note applications
- `EnrollmentPayments` - Links payments/promissory notes to enrollments

## Navigation

New menu items will appear based on user role:
- **Students:** "My Account" link to view balance
- **Cashier:** "Cashier Dashboard", "Process Payment", "Payment History"
- **Campus Director:** "Promissory Notes" link
- **Accounting:** Enhanced approval page with payment info
