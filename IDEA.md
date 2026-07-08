# "On Time" - Scheduling Web App

A modern, fast, and high-performance booking and scheduling platform designed for barbers, beauticians, and similar service professionals. The platform allows organizations to manage their schedules, services, and staff, while enabling clients to login and book appointments online without playing "phone guacamole".

---

## 🚀 Product Vision & Goal

* **For Owners:** Manage shop profiles and staff (invite or remove professionals from their shops).
* **For Professionals (Workers):** Manage their own catalog of services (with individual pricing/durations), configure their schedules per shop they work in, and view their own calendar.
* **For Clients:** A frictionless, mobile-first booking experience where they can pick a professional, select one of their services, choose a time slot, and book (requires logging in).

---

## 🛠 Tech Stack

* **Backend:** ASP.NET 10 MVC, Entity Framework Core (using SQLite locally).
* **Identity:** ASP.NET Core Identity with Cookie Authentication and **Google External Authentication** (`Microsoft.AspNetCore.Authentication.Google`).
* **Interactivity:** HTMX (for dynamic swaps, slot generation, page transitions without full reloads).
* **Styling:** TailwindCSS (v4) with **DaisyUI** (v5) for a beautiful modern design.
* **Client-side Scripting:** Vanilla JavaScript.
* **Language:** Hardcoded Portuguese (PT-PT) for initial phase.
* **Notifications & Queue:** 
  * Background worker (`IHostedService` / `BackgroundService`) processing a persistent scheduled message queue.
  * Trigger timing is configurable in `appsettings.json` (default: 12 hours before appointment).
  * Channels: Email and/or SMS, based on user preferences.

---

## 📋 Core Features

### 1. Multi-Tenant & Multi-Staff Hierarchy
* **Users & Authentication:** Users can register/login with an Email/Password or via **Google Login**.
  * By default, all new users (including those logging in with Google) register as a **Client**.
  * **Upgrade Path:** Any client can go to their profile settings and change/upgrade their account type to a **Professional** (assigning the professional claim/role to their user account).
  * Normal registration via Email/Password has a shortcut checkbox to set `isProfessional = true` immediately.
  * If a user accepts a shop invitation, the system will verify or prompt/grant the professional claim to complete the worker linkage.
* **Ownership:** A user with a `Professional` claim can create and own multiple shops (Organizations).
* **Staffing:** Shop owners can invite other registered professionals to join their shop using email invitations or unique invite links.
  * Accepting the invitation links the professional to the shop as a worker.
* **Professional-Owned Services:** Services are created and managed by the professionals themselves. Since services are defined at the professional level, prices and durations are custom to each professional (supporting distinct specialties like barbers vs. nail technicians under the same roof).

### 2. Shop / Organization Profile
* Custom shop URL slug (e.g., `ontime.site/shop/barbearia-classic`).
* Custom branding, description, address, contacts.

### 3. Cancellation Policy Settings
* Configured at the Shop level:
  * `AllowCancellation` (bool) - toggle if clients can cancel their own bookings.
  * `CancellationDeadlineHours` (int) - minimum lead time required to cancel (e.g., 24 hours).

### 4. Schedule & Availability Settings
* **Professional shifts:** Working hours configured *per professional per shop* (e.g., Professional A works at Shop X on Mondays 09:00 - 13:00, and at Shop Y on Mondays 14:00 - 19:00).
* **No Buffers (Phase 1):** Grid slots are calculated consecutively without adding extra cleanup/turnaround buffers.
* **Time Slot Math:** If a service takes 45 minutes, a slot at time `T` is available if the professional is working from `T` to `T + 45m` and has no overlapping appointments or breaks in that window.
* **Concurrency:** Strictly one appointment at a time per professional.

### 5. Access Control & Dashboard Permissions
* **Shop Owner:** Can manage the store details, add or remove professionals, and see the overview.
* **Professionals:** Can manage their own services (create, edit, delete, set price/duration), configure their working hours per shop, and view/manage their calendar.
* **Client:** Can browse shops, view professionals and their services, book appointments (requires logging in), view their own upcoming appointments, and cancel them (subject to shop cancellation policies).

---

## 🗄 Database Schema

### `Users` (Extends `IdentityUser`)
* Standard ASP.NET Core Identity fields.
* `FullName` (String)
* `AllowEmailNotifications` (Bool, default true)
* `AllowSmsNotifications` (Bool, default false)

### `Organizations` (Shops)
* `Id` (Guid)
* `Name` (String)
* `Slug` (String, unique URL handle)
* `Description` (String)
* `Address` (String)
* `PhoneNumber` (String)
* `OwnerId` (FK to `Users` - the shop creator/owner)
* `SlotDurationMinutes` (Int, e.g. 15, 30)
* `AllowCancellation` (Bool, default true)
* `CancellationDeadlineHours` (Int, default 24)

### `Services` (Owned by Professional)
* `Id` (Guid)
* `ProfessionalId` (FK to `Users`)
* `Name` (String)
* `Description` (String)
* `DurationMinutes` (Int)
* `Price` (Decimal)
* `IsActive` (Bool)

### `ShopProfessionals` (Staff Assignment)
* `Id` (Guid)
* `OrganizationId` (FK)
* `ProfessionalId` (FK to `Users`)
* `IsActive` (Bool)

### `ProfessionalSchedule` (Weekly Shifts per Shop)
* `Id` (Guid)
* `ShopProfessionalId` (FK to `ShopProfessionals`)
* `DayOfWeek` (Enum: Monday-Sunday)
* `StartTime` (TimeSpan)
* `EndTime` (TimeSpan)
* `BreakStartTime` (TimeSpan, nullable)
* `BreakEndTime` (TimeSpan, nullable)

### `ScheduleExceptions` (Holidays / Days Off per Professional)
* `Id` (Guid)
* `ShopProfessionalId` (FK to `ShopProfessionals`)
* `Date` (DateTime)
* `IsOpen` (Bool)
* `StartTime` (TimeSpan, nullable)
* `EndTime` (TimeSpan, nullable)

### `ShopInvites` (Staff Invitation System)
* `Id` (Guid)
* `OrganizationId` (FK)
* `InvitedEmail` (String)
* `Token` (String, unique Guid or hash)
* `Status` (Enum: Pending, Accepted, Expired)
* `CreatedAt` (DateTime)
* `ExpiresAt` (DateTime)

### `Appointments`
* `Id` (Guid)
* `OrganizationId` (FK)
* `ProfessionalId` (FK to `Users` - the assigned staff member)
* `ServiceId` (FK to `Services` - must belong to the professional)
* `ClientId` (FK to `Users` - required client login)
* `StartDateTime` (DateTime)
* `EndDateTime` (DateTime)
* `Notes` (String)
* `Status` (Enum: Pending, Confirmed, Cancelled, Completed)

### `NotificationQueue` (Persistent Queue for Scheduled Reminders)
* `Id` (Guid)
* `AppointmentId` (FK to `Appointments`)
* `ScheduledTime` (DateTime - when the notification should be sent)
* `NotificationType` (Enum: Reminder, Confirmation, Cancellation)
* `Status` (Enum: Pending, Processing, Sent, Failed)
* `Attempts` (Int)
* `ErrorMessage` (String, nullable)
* `ProcessedAt` (DateTime, nullable)

---

## 🗺 User Flows

### Client Booking Flow
1. Accesses `/shop/{slug}`.
2. Selects a professional from the list of workers in that shop.
3. Selects a service from that professional's personal catalog.
4. Selects an available date & time slot.
5. Clicks "Agendar" (Book).
   * **If authenticated:** Immediately confirms booking, writes to DB, inserts confirmation and reminder entries into `NotificationQueue`, and redirects to success page.
   * **If guest:** Redirects to `/Authentication/Login?returnUrl=...&prefillEmail=...` with the selected context preserved. Once authenticated, redirects back to complete booking.

### Invitation Flow
1. Shop Owner goes to Shop Settings ➔ Staff.
2. Clicks "Convidar Profissional", enters email.
3. System creates a `ShopInvite` entry and generates the link: `/invite/accept?token={token}`.
4. The invited professional clicks the link.
   * If not registered, they register via Google Login or Email/Password. They will start as a Client.
   * Once authenticated, they accept the invite, the system promotes them to a **Professional** (adds professional claim/role), links them to `ShopProfessionals`, and redirects them to their dashboard.
