# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a proof-of-concept digital signature system for trade recommendations. It demonstrates client-side ECDSA signing with encrypted key storage, following a security model where private keys never leave the client in unencrypted form.

## Architecture

**Monorepo Structure:**
- `app/` - Nuxt 3 frontend application (Vue 3, TypeScript, Tailwind CSS)
- `api/` - ASP.NET Core 10 backend (.NET 10, Entity Framework Core, SQLite)

### Backend Architecture (C# / .NET)

**Project Structure:**
- `Ds.Api` - Web API layer with controllers and services
- `Ds.Core` - Domain entities, persistence (EF Core), and migrations

**Key Components:**
1. **Controllers:**
   - `TradesController` - Trade proposal CRUD and signing endpoint
   - `UsersController` - Key management (onboarding, rotation, active key retrieval)

2. **Services:**
   - `TradeRecommendationService` - Trade lifecycle, signature verification
   - `CustomerKeyService` - Customer key management and rotation

3. **Entities:**
   - `TradeRecommendation` - Trade proposals with metadata stored as JSON
   - `CustomerKey` - User signing keys (public key + encrypted private key + salt + IV)
   - `Customer` - User entity (seeded with faker data)

4. **Database:**
   - SQLite with Entity Framework Core
   - Database file: `api/Ds.Api/app.db`
   - Seeded with one customer on startup

**Extension Methods (C# 12):**
The API uses modern C# 12 extension syntax extensively. Extension methods are defined using the `extension(Type instance)` syntax:
```csharp
extension(TradeRecommendation recommendation)
{
    public void Sign(TradeSignRequest request) { ... }
}
```

### Frontend Architecture (Nuxt 3 / Vue 3)

**Composables Pattern:**
All business logic is organized into Vue composables in `app/composables/`:

1. **`useCrypto.ts`** - Core cryptographic operations
   - ECDSA P-256 key generation and signing
   - AES-GCM encryption/decryption
   - PBKDF2 key derivation (600,000 iterations)
   - SHA-256 hashing
   - Base64 encoding/decoding utilities

2. **`useKeyManagement.ts`** - Key lifecycle management
   - Key onboarding (first-time setup)
   - Key rotation (superseding old keys)
   - Active key status checking
   - Passphrase validation (10+ chars, uppercase, lowercase, digit)

3. **`useSigning.ts`** - Trade signing workflow
   - Canonical string construction: `TRADEv1|{trade_id}|{action}|{signed_at}|{metadata_hash}`
   - Complete signing flow: fetch key → decrypt → hash metadata → sign → submit

4. **`useTradeProposals.ts`** - Trade proposal data management
   - Fetching proposals
   - Creating proposals
   - State management

5. **`useApi.ts`** - API client wrapper
   - Centralized fetch configuration
   - Error handling

**Component Structure:**
- `app.vue` - Root application component
- `pages/index.vue` - Main trade proposals list page
- `components/PassphraseModal.vue` - Modal for passphrase entry
- `components/TradeProposalList.vue` - Trade list display
- `components/TradeProposalItem.vue` - Individual trade item
- `components/TradeProposalDetailCard.vue` - Expanded trade details

### Cryptographic Flow

**Key Onboarding:**
1. User enters passphrase (validated for complexity)
2. Generate ECDSA P-256 key pair
3. Derive AES-GCM key from passphrase using PBKDF2 (600k iterations, SHA-256)
4. Encrypt private key with AES-GCM
5. Store encrypted private key + salt + IV + public key on server

**Signing Flow:**
1. Fetch encrypted key material from server
2. User enters passphrase
3. Derive decryption key from passphrase + stored salt
4. Decrypt private key
5. Hash trade metadata (raw JSON string) with SHA-256
6. Build canonical string: `TRADEv1|{trade_id}|{action}|{signed_at}|{metadata_hash}`
7. Sign canonical string with ECDSA-SHA256
8. Submit signature + metadata to server

**Server Verification:**
1. Reconstruct canonical string from request
2. Verify ECDSA signature using stored public key
3. Validate signing key is active
4. Store signature and mark trade as signed

## Development Commands

### Frontend (Nuxt 3)

```bash
cd app

# Install dependencies
npm install

# Development server (http://localhost:3000)
npm run dev

# Production build
npm run build

# Preview production build
npm run preview

# Generate static site
npm run generate

# Prepare Nuxt (auto-runs after npm install)
npm run postinstall
```

### Backend (.NET 10)

```bash
cd api

# Restore dependencies
dotnet restore

# Build solution
dotnet build

# Run API (http://localhost:5071)
dotnet run --project Ds.Api/Ds.Api.csproj

# Run from specific project directory
cd Ds.Api
dotnet run

# Watch mode (auto-restart on changes)
dotnet watch --project Ds.Api/Ds.Api.csproj

# Create new migration
cd Ds.Core
dotnet ef migrations add <MigrationName> --startup-project ../Ds.Api/Ds.Api.csproj

# Apply migrations (happens automatically on startup via EnsureCreated)
dotnet ef database update --startup-project ../Ds.Api/Ds.Api.csproj
```

## Configuration

### Frontend Configuration

**`nuxt.config.ts`:**
- API base URL: `http://localhost:5071/api/v1` (configurable via `runtimeConfig.public.apiBaseUrl`)
- Tailwind CSS via `@tailwindcss/vite` plugin
- Dev tools enabled in development

### Backend Configuration

**`Program.cs`:**
- SQLite connection string: `Data Source=app.db`
- CORS enabled in development (allow any origin/method/header)
- OpenAPI/Swagger enabled in development
- Serilog logging to console

## Important Implementation Details

### Web Crypto API Usage

All cryptographic operations use the Web Crypto API (`window.crypto.subtle`) for security:
- ECDSA with P-256 curve for signing
- AES-GCM with 256-bit keys for encryption
- PBKDF2 with 600,000 iterations (OWASP 2024 recommendation)
- SHA-256 for hashing

### Metadata Hash Calculation

CRITICAL: The metadata hash MUST be computed from the **raw JSON string** (`metadataRaw`), NOT the parsed object. This ensures hash consistency between client and server. The raw string is stored in the database and returned by the API.

### Key Rotation

When rotating keys, the old key's `SupersededAt` timestamp is set, but the key remains in the database for historical signature verification. The customer's `ActiveKeyId` is updated to the new key.

### Extension Methods (C# 12)

The codebase uses C# 12's new extension syntax (`extension(Type instance) { }`). When modifying extensions:
- Keep extension methods in corresponding `Extensions/` files
- Use descriptive names that indicate the source type
- Follow existing patterns for consistency

### Signing Verification

The server verifies signatures by:
1. Reconstructing the canonical string using `MakeCanonicalStringV1`
2. Using the customer's active public key to verify
3. Rejecting signatures if the signing key doesn't match the active key

## Common Tasks

### Adding a New Composable

1. Create file in `app/app/composables/use<Name>.ts`
2. Export a function that returns reactive state and methods
3. Use other composables via function calls (e.g., `const crypto = useCrypto()`)
4. Auto-imported by Nuxt - no explicit import needed

### Adding a New API Endpoint

1. Add action method to appropriate controller in `Ds.Api/Controllers/`
2. Add DTO classes in `Ds.Api/Dto/` if needed
3. Implement service logic in `Ds.Api/Services/`
4. Add extension methods in `Ds.Api/Extensions/` for entity mapping
5. Update frontend `useApi` composable to call new endpoint

### Modifying Cryptographic Operations

CRITICAL: Any changes to cryptographic operations must maintain compatibility:
- Key derivation parameters (iterations, hash, salt size) must match between client/server
- Canonical string format must remain consistent for signature verification
- Test thoroughly with existing signed trades

### Database Schema Changes

1. Modify entity classes in `Ds.Core/Entities/`
2. Create migration: `dotnet ef migrations add <Name> --startup-project ../Ds.Api/Ds.Api.csproj`
3. Delete `app.db` in development to start fresh (auto-created on startup)
4. Run application - EF Core will apply migrations via `EnsureCreated()`
