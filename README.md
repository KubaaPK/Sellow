# Sellow

A portfolio marketplace built with ASP.NET Core and Angular.

## Project status

Initial setup is complete: backend and frontend scaffolds, Tailwind CSS,
Spartan UI, formatting rules and CI checks.

Marketplace features are not implemented yet. The frontend currently displays
the Angular starter page with UI integration examples and is not connected
to the backend.

## Technology stack

- ASP.NET Core / .NET 10
- Angular 22 / TypeScript / RxJS
- Tailwind CSS 4 / Spartan UI
- pnpm
- GitHub Actions

Planned: EF Core and PostgreSQL.

## Prerequisites

- .NET SDK specified in `global.json`
- Node.js specified in `src/frontend/.node-version`
- pnpm specified in `src/frontend/package.json`

Docker is not required. No database is needed at this stage.

## Run locally

Clone the repository:

```shell
git clone https://github.com/KubaaPK/Sellow.git
cd Sellow
```

### Backend

Trust the local HTTPS development certificate once:

```shell
dotnet dev-certs https --trust
```

Start the API from the repository root:

```shell
dotnet restore Sellow.slnx
dotnet run --project src/backend/Sellow.Api --launch-profile https
```

Use the HTTPS address printed in the terminal. Local launch settings are
defined in `src/backend/Sellow.Api/Properties/launchSettings.json`.

The starter API exposes `/weatherforecast` and, in Development,
`/openapi/v1.json`.

### Frontend

In a separate terminal:

```shell
cd src/frontend
pnpm install --frozen-lockfile
pnpm start
```

Open the address printed in the terminal, normally http://localhost:4200.

## Local checks

Backend — run from the repository root:

```shell
dotnet restore Sellow.slnx
dotnet format Sellow.slnx --verify-no-changes --no-restore
dotnet build Sellow.slnx --configuration Release --no-restore
```

Frontend — run from `src/frontend`:

```shell
pnpm format:check
pnpm build
pnpm run test --watch=false
```

To apply formatting, use `dotnet format Sellow.slnx` for the backend
or `pnpm format` for the frontend.

CI runs backend and frontend checks on pull requests targeting `main`
and on pushes to `main`.

## Repository structure

```text
.github/workflows/    CI configuration
src/backend/         ASP.NET Core projects
src/frontend/        Angular application
Sellow.slnx           .NET solution
```

Open `Sellow.slnx` in Rider and `src/frontend` in VS Code.

## Development workflow

1. Create a short-lived branch from an up-to-date `main`.
2. Implement a focused change and run the relevant local checks.
3. Open a pull request and review the diff.
4. Wait for both Backend and Frontend CI checks to pass.
5. Squash merge and update local `main`.

Keep credentials out of tracked files. Use .NET User Secrets or environment
variables for backend secrets. Frontend code must not contain secrets.
