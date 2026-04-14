# Social Networking API

.NET Web API for a social-style app: **JWT auth**, **profiles**, **paged feed**, **follows**, **mutual connections**, **bookmarks**, **direct messages**, **photos**, **hobby interests**, and **subscription tiers**.

See **`PROJECT.md`** for a feature roadmap (posts, notifications, payments, etc.).

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Optional: [EF Core CLI](https://learn.microsoft.com/en-us/ef/core/cli/dotnet) (`dotnet tool install --global dotnet-ef`)

## Run

```bash
dotnet restore
dotnet run
```

- API (Kestrel defaults): `http://localhost:5000` (see console for the exact URL)
- Swagger UI: `/swagger`

On first run, **`MigrateAsync()`** applies EF migrations (SQLite file from `ConnectionStrings:DefaultConnection`, default **`socialapp.db`**). In **Development**, optional demo data is seeded.

## Features (summary)

| Area | Notes |
|------|--------|
| **Account** | Register / login (JWT). New users get **`emailConfirmed: false`** until they confirm (see below). |
| **Email confirmation** | Signed token (48h). **`POST /api/account/confirm-email`**, **`POST /api/account/resend-confirmation`** (auth). |
| **Profile** | `knownAs`, bio, headline, profile links, city, country, job title, hobbies. **`emailConfirmed`** is returned on user payloads. |
| **Feed** | **`GET /api/users`** or **`GET /api/users/feed`** — paged; optional **`hobbyIds`** (comma-separated); excludes users you already follow. |
| **Social graph** | Follow / unfollow, connections (mutual follows), following & followers lists (followers list gated by plan). |
| **Bookmarks** | Save / remove bookmarked profiles. |
| **Messaging & photos** | 1:1 messages; upload photos and set main. |
| **Subscriptions** | Free / Plus / Premium (follow caps, followers list visibility, feed boost). |

## Email confirmation

### How it works

1. After **register**, the API sends a confirmation message (if sending fails, registration still succeeds and the failure is logged).
2. Without SMTP, the **`LoggingEmailSender`** writes the full message to **application logs** (Information).
3. The message includes a line **`CONFIRMATION_TOKEN:`** followed by the token. Send:

   `POST /api/account/confirm-email` with JSON `{ "token": "<paste>" }`.

4. **`App:PublicApiBaseUrl`** (optional) is embedded in the email text so clients know which host to call.

### Test it manually (no inbox)

1. Ensure **`Smtp:Host`** is empty so mail is **logged**, not sent via SMTP.
2. Run the API and **register** a user (Swagger or any HTTP client).
3. Confirm the JSON response includes **`"emailConfirmed": false`**.
4. In the **console**, find the log entry for the email body and copy the token after **`CONFIRMATION_TOKEN:`**.
5. Call **`POST /api/account/confirm-email`** with that token → **204**.
6. **Login** or fetch the profile again → **`emailConfirmed": true`**.

### Test it with automated tests

```bash
dotnet test API.Tests/API.Tests.csproj --filter "FullyQualifiedName~EmailVerificationIntegrationTests"
```

### Test it with a real inbox

Set **`Smtp:Host`** (and port, credentials, SSL) plus **`Email:FromAddress`** in `appsettings` or user secrets. Restart the API, register, and use the token from the email the same way as above.

## Subscriptions

Built-in plans:

- **Free**: up to 20 new follows per UTC day; followers list locked.
- **Plus**: unlimited follows; can see followers list.
- **Premium**: same as Plus with stronger feed placement.

Endpoints:

- `GET /api/subscriptions/plans`
- `GET /api/subscriptions/me` (auth)
- `POST /api/subscriptions/subscribe` (auth) — e.g. `{ "planId": 2, "durationDays": 30, "autoRenew": true, "renewalDays": 30 }`
- `POST /api/subscriptions/auto-renew` (auth) — `{ "enabled": true }`
- `POST /api/subscriptions/cancel` (auth)

**Note:** `GET /api/users/following?list=followers` requires a plan that includes the followers list entitlement.

## Useful HTTP routes

**Public**

- `POST /api/account/register`
- `POST /api/account/login`
- `POST /api/account/confirm-email` — body `{ "token": "..." }`
- `GET /api/subscriptions/plans`

**Auth (Bearer JWT)**

- `POST /api/account/resend-confirmation`
- `GET /api/users`, `GET /api/users/feed` — query: `pageNumber`, `pageSize`, `hobbyIds`, `orderBy`
- `GET /api/users/{username}`, `PUT /api/users`
- `GET /api/users/hobbies`, `GET /api/users/interests`
- `GET /api/users/connections`
- `GET /api/users/following?list=following` or `list=followers`
- `POST` / `DELETE /api/follow/{userId}`
- `POST` / `DELETE /api/bookmarks/{userId}`
- `POST /api/messages`, `GET /api/messages`, `GET /api/messages/thread/{recipientId}`, etc.
- `POST /api/photos`, `PUT /api/photos/{id}/set-main`, `DELETE /api/photos/{id}`
- `DELETE /api/account`

**Development only**

- `GET /api/users/all` — any authenticated user in Development; requires **Admin** role in other environments.

## Configuration

Use **`appsettings.json`** / **`appsettings.Development.json`** (or environment variables / user secrets):

| Key | Purpose |
|-----|--------|
| `ConnectionStrings:DefaultConnection` | SQLite (default file `socialapp.db`) |
| `TokenKey` | JWT signing key (64+ chars). Also used to sign email tokens unless `EmailConfirmation:SigningKey` is set. |
| `EmailConfirmation:SigningKey` | Optional separate key for email confirmation HMAC. |
| `App:PublicApiBaseUrl` | Optional base URL shown in confirmation emails (e.g. `https://localhost:7158`). |
| `Email:FromAddress`, `Email:FromName` | SMTP sender display. |
| `Smtp:Host`, `Smtp:Port`, `Smtp:User`, `Smtp:Password`, `Smtp:UseSsl` | If **`Host`** is set, MailKit sends mail; if empty, emails are **log-only**. |
| `AdminUserNames` | Optional comma-separated usernames that receive the Admin role in JWTs. |

## Tests

Integration tests use **xUnit** + **`WebApplicationFactory<Program>`** with a **temporary SQLite** database (`Testing` environment, no dev seeder).

```bash
dotnet test API.Tests/API.Tests.csproj
```

Run only email verification tests:

```bash
dotnet test API.Tests/API.Tests.csproj --filter "FullyQualifiedName~EmailVerificationIntegrationTests"
```

On Windows, **stop a running `API.exe`** if the build fails because **`API.dll`** is locked.

## EF Core migrations

```bash
dotnet ef migrations add MigrationName
dotnet ef database update
```

The host also runs **`Database.MigrateAsync()`** on startup.

## Troubleshooting

- **Build / test fails on file lock** — stop the API process using the project output folder.
- **New DB file** — default SQLite path is `socialapp.db` in the working directory; change `DefaultConnection` to use another path or share a DB across runs.

---

**Suggested commit message:** `docs: expand README with email confirmation testing and config tables`
