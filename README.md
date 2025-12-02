# Digital Signature POC

A proof-of-concept digital signature system for trade recommendations, demonstrating secure client-side ECDSA signing with encrypted key storage.

## Overview

This POC implements a complete workflow for digitally signing trade recommendations using modern cryptographic standards. The system follows a zero-knowledge security model where private keys are generated, stored, and used entirely on the client side - **private keys never leave the client in unencrypted form**.

### Key Features

- **Client-Side Key Generation**: ECDSA P-256 key pairs generated in the browser using Web Crypto API
- **Encrypted Key Storage**: Private keys encrypted with AES-GCM before being stored server-side
- **Strong Key Derivation**: PBKDF2 with 600,000 iterations (OWASP 2024 recommendation)
- **Key Rotation**: Support for rotating signing keys while maintaining historical signatures
- **Cryptographic Verification**: Server-side signature verification using stored public keys
- **Canonical String Format**: Standardized signing format for consistency

## Architecture

This is a monorepo containing both frontend and backend applications:

```text
digital-signature-poc/
├── app/              # Nuxt 4 frontend (Vue 3, TypeScript, Tailwind CSS v4)
└── api/              # ASP.NET Core 10 backend (.NET 10, EF Core, SQLite)
    ├── Ds.Api/       # Web API layer
    └── Ds.Core/      # Domain entities and persistence
```

### Technology Stack

**Frontend:**

- Nuxt 4 (Vue 3, TypeScript)
- Tailwind CSS v4
- Web Crypto API
- Composables-based architecture

**Backend:**

- .NET 10 / ASP.NET Core
- Entity Framework Core
- SQLite database

## Security Model

### Cryptographic Operations

All cryptographic operations use the Web Crypto API for security and performance:

- **Signing**: ECDSA with P-256 curve (secp256r1)
- **Encryption**: AES-GCM with 256-bit keys
- **Key Derivation**: PBKDF2-SHA256 with 600,000 iterations
- **Hashing**: SHA-256

### Key Onboarding Flow

1. User creates a passphrase (10+ characters, mixed case, digit required)
2. System generates ECDSA P-256 key pair in the browser
3. Passphrase is used to derive an AES-GCM encryption key via PBKDF2
4. Private key is encrypted with derived key
5. Encrypted private key + salt + IV + public key stored on server
6. Original passphrase and unencrypted private key discarded from memory

### Signing Flow

1. User requests to sign a trade recommendation
2. System fetches encrypted key material from server
3. User enters passphrase
4. System derives decryption key from passphrase + stored salt
5. Private key is decrypted in memory
6. Trade metadata is hashed (SHA-256)
7. Canonical string constructed: `TRADEv1|{trade_id}|{action}|{signed_at}|{metadata_hash}`
8. Canonical string signed with ECDSA-SHA256
9. Signature + metadata submitted to server
10. Private key cleared from memory

### Server Verification

1. Server reconstructs canonical string from request
2. Signature verified using stored public key
3. Signing key validated as active
4. Signature and metadata persisted

## Getting Started

### Prerequisites

- Node.js 18+ and npm
- .NET 10 SDK
- Git

### Quick Start

1. **Clone the repository**

   ```bash
   git clone <repository-url>
   cd digital-signature-poc
   ```

2. **Start the backend**

   ```bash
   cd api
   dotnet restore
   dotnet run --project Ds.Api/Ds.Api.csproj
   ```

   The API will start at `http://localhost:5071`

3. **Start the frontend** (in a new terminal)

   ```bash
   cd app
   npm install
   npm run dev
   ```

   The app will start at `http://localhost:3000`

4. **Try it out**
   - Navigate to `http://localhost:3000`
   - Click "Manage Keys" to onboard your first signing key
   - Create a passphrase (remember it!)
   - Sign trade recommendations from the list

## Development

### Frontend Development

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
```

### Backend Development

```bash
cd api

# Restore dependencies
dotnet restore

# Build solution
dotnet build

# Run API (http://localhost:5071)
dotnet run --project Ds.Api/Ds.Api.csproj

# Watch mode (auto-restart on changes)
dotnet watch --project Ds.Api/Ds.Api.csproj
```

### Database Migrations

```bash
cd api/Ds.Core

# Create new migration
dotnet ef migrations add <MigrationName> --startup-project ../Ds.Api/Ds.Api.csproj

# Apply migrations
dotnet ef database update --startup-project ../Ds.Api/Ds.Api.csproj

# In development, delete app.db to start fresh
rm ../Ds.Api/app.db
```

## Project Structure

### Frontend (`app/`)

```text
app/
├── app/
│   ├── composables/              # Business logic (auto-imported)
│   │   ├── useApi.ts            # API client wrapper
│   │   ├── useCrypto.ts         # Cryptographic operations
│   │   ├── useKeyManagement.ts  # Key lifecycle management
│   │   ├── useSigning.ts        # Trade signing workflow
│   │   └── useTradeProposals.ts # Trade data management
│   ├── components/              # Vue components
│   ├── pages/                   # File-based routing
│   └── types/                   # TypeScript types
└── nuxt.config.ts              # Nuxt configuration
```

### Backend (`api/`)

```text
api/
├── Ds.Api/                      # Web API layer
│   ├── Controllers/             # API endpoints
│   ├── Dto/                     # Data transfer objects
│   ├── Extensions/              # C# 12 extension methods
│   └── Services/                # Business logic
└── Ds.Core/                     # Domain layer
    ├── Entities/                # Domain entities
    ├── Enumerations/            # Enums
    ├── Migrations/              # EF Core migrations
    └── Persistence/             # DbContext
```

## Key Concepts

### Composables Architecture

The frontend uses Vue composables for all business logic:

- **`useCrypto`**: Low-level cryptographic primitives (signing, encryption, hashing)
- **`useKeyManagement`**: High-level key operations (onboarding, rotation)
- **`useSigning`**: Complete signing workflow orchestration
- **`useTradeProposals`**: Trade data fetching and management
- **`useApi`**: HTTP client with error handling

### Canonical String Format

All signatures are generated from a canonical string format:

```text
TRADEv1|{trade_id}|{action}|{signed_at}|{metadata_hash}
```

Where:

- `trade_id`: Integer ID of the trade proposal (e.g., `123`)
- `action`: Lowercase action string - either `accepted` or `rejected`
- `signed_at`: Unix timestamp in milliseconds (e.g., `1733140200000`)
- `metadata_hash`: SHA-256 hash of the raw metadata JSON string (hex format)

Example:

```text
TRADEv1|123|accepted|1733140200000|a7f3b2c1d4e5f6a8b9c0d1e2f3a4b5c6d7e8f9a0b1c2d3e4f5a6b7c8d9e0f1a2
```

This ensures consistent signature verification between client and server.

### Metadata Hash Calculation

**CRITICAL**: The metadata hash MUST be computed from the raw JSON string (`metadataRaw`), not the parsed object. This ensures hash consistency between client and server.

### Key Rotation

Users can rotate their signing keys while maintaining historical signatures:

1. Old key's `SupersededAt` timestamp is set
2. Old key remains in database for signature verification
3. New key becomes the active key
4. Customer's `ActiveKeyId` updated to new key

## Configuration

### Frontend Configuration

Edit `app/nuxt.config.ts`:

```typescript
export default defineNuxtConfig({
  runtimeConfig: {
    public: {
      apiBaseUrl: "http://localhost:5071/api/v1",
    },
  },
});
```

### Backend Configuration

Edit `api/Ds.Api/appsettings.json` or `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=app.db"
  }
}
```

### Key Endpoints

**User Key Management:**

- `GET /api/v1/users/me/keys/active` - Get user's active key material
- `POST /api/v1/users/me/keys/onboarding` - Onboard new signing key
- `POST /api/v1/users/me/keys/rotate` - Rotate to new signing key

**Trade Recommendations:**

- `GET /api/v1/trades` - List trade recommendations
- `POST /api/v1/trades/proposal` - Create new random trade recommendation (for testing)
- `POST /api/v1/trades/{id}/sign` - Sign a trade recommendation

## Security Considerations

- **Passphrase Strength**: Enforced minimum requirements (10 chars, mixed case, digit)
- **Key Derivation**: 600,000 PBKDF2 iterations (OWASP 2024 recommendation)
- **Memory Management**: Private keys cleared from memory after use
- **Transport Security**: Use HTTPS in production
- **Key Storage**: Encrypted private keys only - server never sees unencrypted keys
- **Signature Verification**: All signatures verified server-side before acceptance

## License

This project is licensed under the MIT License. See the [LICENSE](./LICENSE) file for details.

## Contributing

This is a POC project. For development guidance, see [CLAUDE.md](./CLAUDE.md).
