# 📚 Sarasavi Library Management System

A desktop application for managing the Sarasavi Library — built with **C# .NET 8** and **Windows Forms**.

---

## 📋 Table of Contents

- [About the Project](#about-the-project)
- [Features](#features)
- [Prerequisites](#prerequisites)
- [How to Run](#how-to-run)
- [Project Structure](#project-structure)
- [How Each Process Works](#how-each-process-works)
- [Book & Copy Numbering](#book--copy-numbering)
- [Business Rules](#business-rules)
- [Database](#database)
- [Technologies Used](#technologies-used)

---

## About the Project

Sarasavi is a fully stocked library with a collection of nearly 500 books. This application helps librarians manage:

- **Registered Members** — can borrow and reference books
- **Registered Visitors** — can only reference (view) books, cannot borrow

The system handles 6 core library processes: Book Registration, User Registration, Loan, Return, Reservation, and Inquiry.

---

## Features

| Process | What It Does |
|---------|-------------|
| 📖 **Book Registration** | Add new book titles and their physical copies to the catalogue |
| 👤 **User Registration** | Register new members (borrowers) and visitors |
| 📤 **Loan Process** | Issue books to members with full validation |
| 📥 **Return Process** | Accept returned books, check for reservations |
| 🔖 **Reservation** | Allow members to reserve book titles |
| 🔍 **Inquiry** | Search book availability by number, title, or author |

---

## Prerequisites

You need **one** of the following installed on your computer:

### Option A: Visual Studio (Recommended)
- **Visual Studio 2022** (Community Edition is free)
- During installation, select the **".NET desktop development"** workload

### Option B: .NET SDK Only
- Download and install the **.NET 8 SDK** from: https://dotnet.microsoft.com/download/dotnet/8.0

> **Note:** No database installation is needed. The application uses SQLite which is included automatically via NuGet packages.

---

## How to Run

### Using Visual Studio

1. Open the file `SarasaviLibrary.sln` in Visual Studio
2. Visual Studio will automatically restore NuGet packages
3. Press **F5** (or click the green ▶ Start button)
4. The application will launch with the main dashboard

### Using Command Line

Open a terminal (Command Prompt or PowerShell) and run:

```
cd c:\Users\PASINDU\Desktop\LMSProject
dotnet run --project SarasaviLibrary
```

### Clone and Run on Another Computer

1. Copy the entire `LMSProject` folder to the other computer
2. Make sure .NET 8 SDK or Visual Studio 2022 is installed
3. Open `SarasaviLibrary.sln` in Visual Studio and press **F5**
4. That's it — no extra setup needed!

---

## Project Structure

```
LMSProject/
│
├── SarasaviLibrary.sln              ← Open this in Visual Studio
│
└── SarasaviLibrary/                 ← Main project folder
    │
    ├── Program.cs                   ← Application entry point
    │
    ├── Models/                      ← Data classes (plain C# objects)
    │   ├── Book.cs                  ← Represents a book title
    │   ├── Copy.cs                  ← Represents a physical book copy
    │   ├── User.cs                  ← Represents a member or visitor
    │   ├── Loan.cs                  ← Represents a loan record
    │   └── Reservation.cs           ← Represents a reservation record
    │
    ├── DataAccess/                  ← Database operations
    │   ├── DatabaseHelper.cs        ← Creates/connects to SQLite database
    │   ├── BookRepository.cs        ← Add/search/update books and copies
    │   ├── UserRepository.cs        ← Add/search users
    │   ├── LoanRepository.cs        ← Create loans, process returns
    │   └── ReservationRepository.cs ← Manage reservations
    │
    └── Forms/                       ← User interface screens
        ├── MainForm.cs              ← Dashboard with 6 menu buttons
        ├── BookRegistrationForm.cs  ← Book & copy registration screen
        ├── UserRegistrationForm.cs  ← User registration screen
        ├── LoanForm.cs              ← Book loan screen
        ├── ReturnForm.cs            ← Book return screen
        ├── ReservationForm.cs       ← Reservation screen
        └── InquiryForm.cs           ← Book search/availability screen
```

---

## How Each Process Works

### 1. Book Registration (Process 5)

1. Select a **Classification** letter (A–Z)
2. The system auto-generates the **Book Number** (e.g., `A0001`)
3. Enter the book's **Title**, **Author**, **ISBN**, and **Publisher**
4. Choose how many **copies** to add (1–10)
5. Check **"Reference Only"** if the copies should not be borrowable
6. Click **Register Book** — copies are auto-numbered (e.g., `A00011`, `A00012`)

### 2. User Registration (Process 6)

1. The system auto-generates a **User Number** (e.g., `U0001`)
2. Enter **Name**, **Sex**, **NIC** (National Identity Card), and **Address**
3. Select **User Type**: "Member" (can borrow) or "Visitor" (reference only)
4. Click **Register User**

### 3. Loan Process (Process 1)

1. **Step 1:** Enter the Member's User Number and click **Search**
   - The system checks if the user is a Member (Visitors cannot borrow)
   - Shows the member's current loan count (maximum 5)
   - Checks for any overdue books
2. **Step 2:** Enter the Copy Number and click **Check**
   - Verifies the copy exists and is available
   - Rejects reference-only copies
3. Click **Confirm Loan** — the due date is set to 14 days from today

### 4. Return Process (Process 2)

1. Enter the **Copy Number** being returned
2. Click **Process Return**
3. The system:
   - Marks the loan as returned
   - Checks if the book title has any outstanding **reservations**
   - If reserved: shows a notification with the reserver's details, puts the copy aside, and deletes the fulfilled reservation

### 5. Reservation Process (Process 3)

1. Search and select a **Member**
2. Search and select a **Book Title**
3. Click **Reserve Book**
4. When a copy of the reserved title is returned, the librarian is notified automatically (in the Return Process)

### 6. Inquiry Process (Process 4)

1. Choose search type: **Book Number**, **Title**, or **Author**
2. Enter a search term (partial matches work for title and author)
3. Click **Search** — results show:
   - Book details
   - Total copies, Available, Loaned, Reserved, and Reference counts
4. Click on a result to see individual copy details

---

## Book & Copy Numbering

### Book Number Format: `X9999`

| Part | Description | Example |
|------|-------------|---------|
| `X` | Classification letter (A–Z) | `A` |
| `9999` | 4-digit sequence starting from 0001 | `0001` |

**Example:** `A0001` = First book in classification A

### Copy Number Format: `X99991`

The copy number is the book number + one extra digit:

| Part | Description | Example |
|------|-------------|---------|
| `X9999` | Parent book number | `A0001` |
| `1` | Copy digit (1–9, 0 for 10th) | `1` |

**Example:** `A00011` = First copy of book A0001, `A00012` = Second copy

---

## Business Rules

| Rule | Detail |
|------|--------|
| Maximum loans per member | **5 books** at a time |
| Loan period | **14 days** (2 weeks) |
| Overdue books | Member cannot borrow new books until overdue books are returned |
| Maximum copies per book | **10 copies** per book number |
| Reference copies | Cannot be loaned out — for in-library use only |
| Visitors | Can search and view books but **cannot borrow** |
| Reservations | Made per book **title** (not per specific copy) |
| Return + Reservation | When a copy is returned, the **oldest reservation** for that title is fulfilled first (FIFO) |

---

## Database

The application uses **SQLite** — a lightweight, file-based database.

- **No installation required** — SQLite is bundled with the application via NuGet
- The database file (`SarasaviLibrary.db`) is created automatically on first run
- It is stored in the same folder as the application executable
- All tables are created automatically if they don't exist

### Database Tables

| Table | Purpose |
|-------|---------|
| `Books` | Stores book titles (BookNumber, Classification, Title, Author, ISBN, Publisher) |
| `Copies` | Stores physical copies (CopyNumber, BookNumber, Status, IsBorrowable) |
| `Users` | Stores registered users (UserNumber, Name, Sex, NIC, Address, UserType) |
| `Loans` | Tracks book loans (CopyNumber, UserNumber, LoanDate, DueDate, ReturnDate) |
| `Reservations` | Tracks reservations (BookNumber, UserNumber, ReservedDate) |

---

## Technologies Used

| Technology | Version | Purpose |
|-----------|---------|---------|
| C# | 12 | Programming language |
| .NET | 8.0 | Runtime framework |
| Windows Forms | .NET 8 | Desktop UI framework |
| SQLite | via Microsoft.Data.Sqlite | Embedded database |
| Visual Studio | 2022+ | Development IDE |

---

## Troubleshooting

| Problem | Solution |
|---------|----------|
| "SDK not found" error | Install .NET 8 SDK from https://dotnet.microsoft.com/download/dotnet/8.0 |
| NuGet packages not restoring | Right-click the solution in Visual Studio → **Restore NuGet Packages** |
| Application doesn't start | Make sure you have the **".NET desktop development"** workload installed in Visual Studio |
| Database errors | Delete the `SarasaviLibrary.db` file and restart the app — it will be recreated |

---

*Developed for DIT2145 — Software Development Project, Institute of Management & Business Studies*
