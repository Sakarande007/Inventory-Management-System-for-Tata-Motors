# Paint Shop IMS

**Inventory Management System for Tata Motors — Chakan Rework Plant**

A desktop application for tracking painted body parts through inward scanning, rework processing, outward dispatch, and reporting. Built for shop-floor operators and supervisors at the Chakan rework facility.

---

## Overview

Paint Shop IMS replaces manual tracking with barcode-driven workflows. Operators scan QR codes at inward and outward stations; the system validates each part, records transactions, and maintains real-time stock visibility. Rework parts receive updated labels with defect codes and are routed back through the paint process.

| Module | Purpose |
|--------|---------|
| **Dashboard** | Live counts for today's inward/outward, current stock, rework volume, and recent activity |
| **Inward Scan** | Register parts entering the paint shop; flag rework and assign defect codes |
| **Outward Scan** | Dispatch parts that have a valid inward record |
| **Part Master** | Maintain part numbers, names, and descriptions |
| **Customer Master** | Manage customer reference data |
| **Reports** | Filter transactions by date range and export to Excel |

---

## Key Features

- **Barcode scanning** — Supports pipe-delimited (`PartNo|Date|Serial`) and compact formats
- **Rework workflow** — Defect code capture, modified QR generation, and automatic label printing
- **Duplicate prevention** — Blocks duplicate inward serials and outward without prior inward
- **Role-based access** — Admin and operator roles with secure login
- **Excel export** — Date-range reports exported as `.xlsx` for audit and analysis
- **Local database** — SQLite storage; no external server required on the shop floor

---

## Technology Stack

| Component | Technology |
|-----------|------------|
| Platform | .NET 9 (Windows) |
| UI | WPF with MVVM (CommunityToolkit.Mvvm) |
| Database | SQLite (Microsoft.Data.Sqlite + Dapper) |
| Security | BCrypt password hashing |
| Reporting | ClosedXML |
| Label QR | QRCoder |

---

## Requirements

- **OS:** Windows 10 or later (64-bit recommended)
- **Runtime:** [.NET 9 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/9.0) (for published builds) or [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) (for development)
- **Hardware:** USB barcode scanner (keyboard wedge mode)
- **Printer:** Label printer configured for rework QR labels (see [Label Printing](#label-printing))

---

## Installation

### For end users (published build)

1. Copy the published application folder to the target PC (e.g. `C:\PaintShopIMS\App`).
2. Ensure the .NET 9 Desktop Runtime is installed.
3. Run `PaintShopIMS.exe`.
4. On first launch, the application creates:
   - Local database at `%LocalAppData%\PaintShopIMS\paintshop.db`
   - Print folders at `C:\PaintShopIMS\PrintTemplates` and `C:\PaintShopIMS\PrintQueue`

### For developers

```bash
git clone <repository-url>
cd "Painting Project"
dotnet restore
dotnet build
dotnet run
```

### Publish a self-contained build

```bash
dotnet publish -c Release -r win-x64 --self-contained true
```

Output is written to `bin\Release\net9.0-windows\win-x64\publish\`.

---

## Getting Started

### Default login

| Field | Value |
|-------|-------|
| Username | `admin` |
| Password | `admin123` |

> **Important:** Change the default admin password after first login in a production environment.

### Typical workflow

1. **Inward (normal part)** — Open **Inward Scan**, scan the part QR. The record is saved automatically.
2. **Inward (rework part)** — Enable **Rework**, scan the part, select a defect code. A new QR label is generated and sent to the printer.
3. **Outward** — Open **Outward Scan**, scan the part (original or rework QR). The system verifies an inward record exists and has not already been outwarded.
4. **Reports** — Select a date range, generate the report, and export to Excel if needed.

---

## Barcode Format

The scanner must output one of the following formats:

### Pipe-delimited (primary)

```
PartNo|YYYY-MM-DD|SerialNumber
```

**Example:** `F1203103|2026-03-26|032627220`

### Compact (fallback)

```
PartNo (8 chars) + Date YYMMDD (6 chars) + Serial (remaining)
```

**Example:** `F120310326032627220`

### Rework QR

After rework inward, the label encodes the original data plus a defect suffix:

```
PartNo|YYYY-MM-DD|SerialNumber|RW:DefectCode
```

**Example:** `F1203103|2026-03-26|032627220|RW:02`

---

## Defect Codes

| Code | Description |
|------|-------------|
| 01 | Damage |
| 02 | Scratch |
| 03 | Dust |
| 04 | Rundown |
| 05 | Lint |
| 06 | Other |

---

## Label Printing

Rework labels are generated via a file-based print queue:

| Path | Purpose |
|------|---------|
| `C:\PaintShopIMS\PrintTemplates\label.txt` | TSC/label printer template (placeholders: `@QRDATA`, `@PartNo`, `@Rewcode`) |
| `C:\PaintShopIMS\PrintQueue\print.txt` | Generated print job (written at scan time) |
| `C:\PaintShopIMS\PrintQueue\print.bat` | Batch script that sends `print.txt` to the network printer |

**Setup steps:**

1. Edit `print.bat` and replace `YOUR_PRINTER_NAME` with the actual Windows printer share name.
2. Adjust `label.txt` if your label size or printer command set differs.
3. Test with a rework inward scan to confirm the label prints correctly.

---

## Database

- **Location:** `%LocalAppData%\PaintShopIMS\paintshop.db`
- **Backup:** Copy this file periodically for disaster recovery.
- **Tables:** `Users`, `PartMaster`, `CustomerMaster`, `InwardTransaction`, `OutwardTransaction`

Parts scanned for the first time are auto-registered in Part Master if the part number does not already exist.

---

## Project Structure

```
PaintShopIMS/
├── Data/              # Database config and initialization
├── Helpers/           # Barcode parsing, printing, defect codes, session
├── Models/            # Domain entities
├── Repositories/      # Data access (Dapper)
├── Services/          # Business logic
├── ViewModels/        # MVVM view models
├── Views/             # WPF screens (Dashboard, Scan, Parts, Reports, Login)
└── PaintShopIMS.csproj
```

---

## Client

**Tata Motors — Chakan Rework Plant**

This system is deployed to support paint shop inventory operations at the Chakan rework facility, including part traceability, rework tracking, and production reporting.

---

## License

Proprietary — developed for Tata Motors Chakan Rework Plant. Contact the project owner for usage and distribution terms.
