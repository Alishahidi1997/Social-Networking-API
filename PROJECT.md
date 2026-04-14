# Project roadmap

This document turns the social API from a **people graph + DMs + media** foundation into a fuller product. Each section lists **what to build**, **why it matters**, **rough scope** (entities, endpoints, services), and **dependencies**. Order phases roughly by leverage: ship small vertical slices before large platforms (real-time, payments).

**Current baseline (already shipped):** JWT auth, profiles (bio, headline, links, location, job, hobbies), paged feed of non-followed users, follow/unfollow, mutual connections, bookmarks, 1:1 messages, photos, subscription tiers (follow caps, followers list, feed boost).

---

## Phase 0 — Hardening (before new surface area)

| Feature | Goal | Scope | Notes |
|--------|------|-------|--------|
| **Email verification** | Reduce fake accounts | **Done:** `EmailConfirmed` on user, HMAC-signed token (48h), `POST /api/account/confirm-email`, `POST /api/account/resend-confirmation` (auth). Logging sender when `Smtp:Host` empty; MailKit when configured. | Optional: `EmailConfirmation:SigningKey`, `App:PublicApiBaseUrl` |
| **Password reset** | Standard account recovery | `ForgotPassword` + `ResetPassword` endpoints, time-limited tokens | Same mail infra as above |
| **Rate limiting** | Abuse resistance | ASP.NET rate limiter on register, login, follow, message | Middleware / policies |
| **Block & mute** | Safety + feed quality | `UserBlock` (hard), optional `UserMute` (soft); filter blocked users from feed, DMs, follower lists | Small migrations; touch `UserRepository`, message send |

**Exit criteria:** New users can verify email; resets work; obvious spam paths throttled; blocked users cannot interact.

---

## Phase 1 — Discovery & search

| Feature | Goal | Scope | Notes |
|--------|------|-------|--------|
| **User search** | Find people | `GET /api/users/search?q=&hobbyIds=&page=` | Full-text optional later; start with `LIKE` / EF `Contains` on username, knownAs, headline |
| **Suggested accounts** | Onboarding + growth | `GET /api/users/suggestions` — score by shared hobbies, mutual connections, same city | Read-only; cache scores if slow |
| **Hashtags (optional)** | Topic discovery | `Tag` + `PostTag` if you add posts first; or tag hobbies only | Can defer until Phase 2 |

**Exit criteria:** Client can search and show a “who to follow” list without scanning the whole directory.

---

## Phase 2 — Posts & timeline (core “social” loop)

| Feature | Goal | Scope | Notes |
|--------|------|-------|--------|
| **Post entity** | Shareable content | `Post`: `AuthorId`, `Body` (markdown/plain), `CreatedUtc`, `Visibility` (public / followers), soft delete | Migration + `PostsController` |
| **Home timeline** | Feed from follows | `GET /api/feed/home?page=` — posts from followed users, reverse chronological | Join `UserFollows`; pagination |
| **User timeline** | Profile tab “Posts” | `GET /api/users/{username}/posts` | Public or followers-only per visibility |
| **Delete / edit post** | Basic moderation | `PUT` / `DELETE` own posts | Soft delete for audit |
| **Media on posts** | Rich posts | Reuse `Photo` or new `PostMedia` linking to blob/storage | Larger scope if moving off local disk |

**Exit criteria:** A user sees a chronological stream from people they follow, not only the “people discovery” feed.

---

## Phase 3 — Engagement on posts

| Feature | Goal | Scope | Notes |
|--------|------|-------|--------|
| **Reactions** | Lightweight feedback | `PostReaction`: `PostId`, `UserId`, `Kind` (enum), unique per user/post | `POST/DELETE /api/posts/{id}/reactions` |
| **Comments** | Conversation | `Comment`: `PostId`, `AuthorId`, `Body`, `ParentCommentId` nullable | Threaded replies optional in v2 |
| **Mentions** | Notify people | Parse `@username` in body; `Mention` table or extract on read | Ties to notifications (Phase 6) |
| **Repost / quote** | Reshare | `RepostOfPostId` nullable on `Post`, or separate `Repost` table | Clarify UX vs quote-post |

**Exit criteria:** Posts feel alive without opening DMs.

---

## Phase 4 — Groups & communities (optional branch)

| Feature | Goal | Scope | Notes |
|--------|------|-------|--------|
| **Groups** | Shared spaces | `Group`, `GroupMember` (role: admin/member), `GroupPost` or group-scoped posts | Permissions model |
| **Group discovery** | Growth | `GET /api/groups`, join requests | Moderation tools needed |

**Exit criteria:** You only start this if product direction is “communities,” not “Twitter-style public graph.”

---

## Phase 5 — Real-time & presence

| Feature | Goal | Scope | Notes |
|--------|------|-------|--------|
| **SignalR hub** | Live UI | Hubs for DMs typing, new message, optional live reaction counts | Scale story: Redis backplane later |
| **Online presence** | “Active now” | Last-seen already exists; optional heartbeat | Privacy setting |

**Exit criteria:** Mobile/web can show typing and instant message delivery without polling.

---

## Phase 6 — Notifications

| Feature | Goal | Scope | Notes |
|--------|------|-------|--------|
| **In-app notifications** | Engagement | `Notification`: `UserId`, `Type`, `PayloadJson`, `ReadUtc` | `GET /api/notifications`, mark read |
| **Push / email** | Off-app engagement | Queue (Hangfire, Azure Queue, or outbox table) + FCM/APNs or email | Depends on Phase 0 mail |

**Exit criteria:** Follow, mention, comment, and DM generate a durable in-app row; push is optional second step.

---

## Phase 7 — Trust, safety, admin

| Feature | Goal | Scope | Notes |
|--------|------|-------|--------|
| **Report user / content** | Safety | `Report` with target type + id, reason, status | Admin `GET/PATCH` queue |
| **Admin moderation** | Remove harmful posts | Reuse soft delete; `Admin` role endpoints | Audit log table recommended |
| **Appeals (later)** | Fairness | Status workflow on reports | Low priority |

**Exit criteria:** There is a path from “user reports” to “admin takes action” without DB surgery.

---

## Phase 8 — Monetization (real payments)

| Feature | Goal | Scope | Notes |
|--------|------|-------|--------|
| **Stripe (or similar)** | Paid plans | Checkout session, webhooks → extend `SubscriptionEndsUtc` / plan id | Replace or gate manual `subscribe` dev endpoint |
| **Invoices / receipts** | Compliance | Webhook-stored records | After first payment integration |

**Exit criteria:** Production can sell Plus/Premium without hand-editing the database.

---

## Cross-cutting concerns (every phase)

- **Migrations:** One EF migration per vertical slice; avoid mixing unrelated schema changes.
- **Tests:** For each feature add at least one integration test (factory + SQLite) covering happy path + one failure (auth, validation, or permission).
- **API versioning (optional):** If mobile clients ship slowly, consider `/api/v2` before breaking DTOs.
- **Observability:** Structured logging + request ids before scaling traffic.

---

## Suggested sequencing (summary)

1. **Phase 0** — verification, reset, rate limits, block/mute.  
2. **Phase 1** — search + suggestions.  
3. **Phase 2** — posts + home timeline (biggest product jump).  
4. **Phase 3** — reactions + comments (+ mentions when notifications exist).  
5. **Phase 6** — in-app notifications (can overlap Phase 3 for mentions).  
6. **Phase 5** — SignalR when DM/feed UX demands it.  
7. **Phase 7** — reports/admin as traffic grows.  
8. **Phase 4** — groups only if roadmap commits to communities.  
9. **Phase 8** — payments when you are ready for ops + legal.

---

## How to use this file

- Turn each **row** into a GitHub issue or project board card.  
- Keep **one primary PR per feature** (schema + API + tests together).  
- Update this roadmap when scope changes; link issues at the end of each phase section if you use a tracker.
