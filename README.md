# Sellow
Sellow is a multi-category marketplace where users can sell and buy products through a single account.

## Overview
The planned functionality includes:
- Publishing offers with available quantities.
- Searching and filtering offers across multiple categories.
- Adding products from different sellers to a cart.
- Placing a separate order with each seller.
- Paying upfront or choosing cash on delivery, depending on the seller's settings.
- Rating the purchase experience after receiving an order.

## Tech Stack
- **Backend:** .NET
- **Frontend:** Angular

Other technology choices will be documented as development progresses.

## Project Status

The project is in the initial requirements and planning stage. No working application is available yet. The features listed above are planned.

## Project Goals

Sellow is a solo portfolio project focused on building and delivering a complete full-stack application.

The project aims to demonstrate practical skills in software design, implementation, testing, and deployment, while leaving room for a future public release.

## Repository Structure

```text
Sellow/
├── AGENTS.md
├── CONTRIBUTING.md
├── README.md
├── docs/
│   └── product.md
└── src/
    ├── backend/
    │   ├── Sellow.slnx
    │   └── Sellow.Api/
    └── frontend/
        └── sellow-web/
```

## Local Development

### Prerequisites

The initial development environment uses:

- .NET SDK 10.0.401
- Node.js 24.21.0
- pnpm 10.34.5
- Git

The commands below use Windows PowerShell.
On macOS and Linux, replace `pnpm.cmd` with `pnpm`.

### Clone the Repository

```powershell
git clone https://github.com/KubaaPK/Sellow.git
cd Sellow
```

### Run the Backend

From the repository root:

```powershell
dotnet run --project src/backend/Sellow.Api
```

Open the address printed after `Now listening on:`.
The root endpoint currently returns `Hello World!`.

### Run the Frontend

In a separate terminal, from the repository root:

```powershell
cd src/frontend/sellow-web
pnpm.cmd install --frozen-lockfile
pnpm.cmd start
```

Open the address printed in the terminal, usually `http://localhost:4200`.

The frontend and backend run independently at this stage.
No API integration has been implemented yet.

Use `Ctrl+C` in each terminal to stop the applications.

## Verification

### Build the Backend

From the repository root:

```powershell
dotnet build src/backend/Sellow.slnx
```

### Build the Frontend

From `src/frontend/sellow-web`:

```powershell
pnpm.cmd build
```

### Run Frontend Tests

From `src/frontend/sellow-web`:

```powershell
pnpm.cmd exec ng test --watch=false
```

## Documentation

- [Product definition](docs/product.md)
- [Contribution guidelines](CONTRIBUTING.md)
- [AI collaboration guidelines](AGENTS.md)
