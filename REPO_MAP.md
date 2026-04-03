# Repository Map

## Active Application Summary

This repository's active application is the root ASP.NET Core MVC project:

- Solution: `Practice Pro.sln`
- Project: `PremiumDM.csproj`
- App type: ASP.NET Core MVC + Razor + ASP.NET Identity + Entity Framework Core
- Frontend app layer: Vue 2 bundle injected into Razor views from `wwwroot/dist/build.js`
- Primary business domain: deadline tracking for Companies House-related client filings

The repository also contains a large amount of non-active content:

- `Practice Pro/`: a nested duplicate copy of the project tree
- `build_verify/`: build and publish output
- `bin/`, `obj/`: generated .NET build artifacts
- `wwwroot/AdminLTE/`, `wwwroot/DevFolio/`, `wwwroot/finanza-1.0.0/`, `wwwroot/lib/`: vendor/template/static assets

The root project explicitly excludes the nested duplicate under `Practice Pro/PremiumDM/Practice Pro/**` from compilation in `PremiumDM.csproj`.

## How The App Is Wired

### Bootstrap

- `Program.cs`
  - Standard ASP.NET Core host bootstrap.
  - Starts the app with `Startup`.

- `Startup.cs`
  - Registers MVC with runtime compilation.
  - Registers `DbCalls` with SQL Server.
  - Registers ASP.NET Identity with relaxed password policy.
  - Registers `EmailService`.
  - Adds cookie authentication settings.
  - Configures middleware pipeline:
    - static files
    - routing
    - `UseCors("AllowAllOrigins")`
    - authentication
    - `BlockUserMiddleware`
    - authorization
    - `UserActivityMiddleware`
    - controller endpoints

### Persistence

- `Models/DbCalls.cs`
  - Main EF Core `DbContext`.
  - Inherits `IdentityDbContext`.
  - Adds custom tables:
    - `client`
    - `UserActivities`
    - `UserRolesInfo`

### Auth Model

- ASP.NET Identity manages user records in standard Identity tables.
- Admin permissions are not primarily taken from `IdentityRole`; instead the app uses the custom `UserRolesInfo` table plus one hardcoded super-admin email.

### Frontend Delivery

- Razor layouts render public pages and dashboard shells.
- Vue components are globally registered in `js-module/src/main.js`.
- Those components mount inside Razor pages using custom tags such as:
  - `<login-page>`
  - `<dashboard>`
  - `<clientview>`

## Main Runtime Flows

### Public Website

1. Request hits `HomeController` or `ServiceController`.
2. Razor view returns either:
   - full static marketing markup, or
   - a layout that includes shared header/footer and site CSS.

### Signup

1. `Views/Account/SignUp.cshtml` mounts `SignupPage.vue`.
2. `SignupPage.vue` validates fields client-side.
3. POST goes to `AccountApi.register`.
4. API creates Identity user, generates email-confirmation token, sends email, and sends admin notification email.

### Login

1. `Views/Account/Login.cshtml` mounts `LoginPage.vue`.
2. `LoginPage.vue` validates credentials and posts to `AccountApi.login`.
3. `AccountApi.login` signs the user in, adds custom claims, and updates `UserActivity`.
4. Frontend then calls `ClientApi.UpdateAllCompaniesSync`.
5. User is redirected to `/DashBoard`.

### Dashboard

1. `/DashBoard` maps to `HomeController.Privacy()`.
2. `Views/Home/Privacy.cshtml` mounts `Dashboard.vue`.
3. `Dashboard.vue` loads company list from `ClientApi.Vlist`.
4. Dashboard calculates counts for accounts, CS01, and identity verification deadlines.

### Company Management

1. `/Client/FavCompany` mounts `ClientView.vue`.
2. User can fetch Companies House data, save companies, delete companies, and trigger bulk sync.
3. `/Client/InCompany` mounts `AddCompany.vue` for manual/additional import flow.

### Admin

1. `AdminController` protects admin pages.
2. `RegUsers.vue` calls `AdminApi` endpoints to:
   - list users
   - block/unblock
   - make/remove admin
3. `RegisteredUsersActivity.vue` calls `AdminApi.GetUserActivities`.

## File-By-File Map

### Root Files

- `Practice Pro.sln`
  - Single-project Visual Studio solution for `PremiumDM`.

- `PremiumDM.csproj`
  - Targets `net8.0`.
  - Declares package dependencies.
  - Excludes the nested duplicate project tree.

- `Program.cs`
  - Entry point.

- `Startup.cs`
  - Service registration and middleware pipeline.

- `appsettings.json`
  - Production/default configuration.
  - Currently stores the SQL Server connection string inline.

- `appsettings.Development.json`
  - Development logging override only.

- `libman.json`
  - Present but effectively unused; no active libraries are listed.

- `package-lock.json`
  - Root-level npm lockfile placeholder with no real packages.
  - Not the active frontend dependency lock.

- `REPO_MAP.md`
  - This repository guide.

### Models

- `Models/Client.cs`
  - Main business entity.
  - Stores company identity, due dates, VAT info, status, and owning `UserId`.

- `Models/DbCalls.cs`
  - EF Core database context.

- `Models/UserActivity.cs`
  - Tracks per-user last login, current page, recent activity, and login count for current month.

- `Models/UserRoleInfo.cs`
  - Custom admin/role assignment table tied to `IdentityUser`.

- `Models/Register.cs`
  - DTO used by signup API wrapper.

- `Models/Login.cs`
  - DTO used by login API wrapper.

- `Models/ForgetPassword.cs`
  - DTO for forgot-password request.

- `Models/ResetPassword.cs`
  - DTO for password reset submission.

- `Models/EmailConfirm.cs`
  - DTO for email confirmation request data.

- `Models/ErrorViewModel.cs`
  - Standard MVC error view model with request ID.

### Controllers

- `Controllers/HomeController.cs`
  - Public page controller.
  - Maps friendly routes like `/`, `/About`, `/Contact`, `/Service`, `/DashBoard`, policies, and sitemap.

- `Controllers/ServiceController.cs`
  - Public service-detail routes under `/Service/...`.

- `Controllers/ClientController.cs`
  - Authenticated MVC endpoints for company pages.
  - Returns views for saved companies and add-company screen.

- `Controllers/AdminController.cs`
  - Authenticated MVC endpoints for admin pages.
  - Uses custom admin check against hardcoded email or `UserRolesInfo`.

- `Controllers/Account.cs`
  - MVC account page controller.
  - Returns login/signup/reset views and logout action.
  - Also contains email confirmation action.

- `Controllers/HomeApi.cs`
  - Small utility API.
  - Exposes:
    - `SendDueEmails`
    - `GetCurrentUserEmail`

- `Controllers/ClientApi.cs`
  - Core authenticated business API for companies.
  - Exposes:
    - `SaveDa`: save company
    - `Vlist`: list current user's companies
    - `deleteList`: delete current user's company
    - `fetchCompanyData`: Companies House API lookup
    - `UpdateAllCompaniesSync`: bulk refresh for current user's companies

- `Controllers/AdminApi.cs`
  - Admin-only API for user and activity management.
  - Exposes:
    - `GetAllUsers`
    - `GetTotalUsers`
    - `BlockUser`
    - `UnblockUser`
    - `MakeAdmin`
    - `RemoveAdmin`
    - `GetUserActivities`

- `Controllers/AccountApi.cs`
  - API for account actions.
  - Exposes:
    - `register`
    - `login`
    - `forgot`
    - `reset`
  - Also updates user activity on successful login.

### Email

- `Email/EmailServices.cs`
  - Concrete SMTP sender using Gmail SMTP.
  - Used by signup, forgot password, and due-email flow.

- `Email/DueEmailService.cs`
  - Commented-out service intended for scheduled deadline reminder sending.
  - Not active.

- `Email/MyBackgroundService.cs`
  - Commented-out hosted background worker that would call `DueEmailService`.
  - Not active.

### Middleware

- `MiddleWare/BlockUserMiddleware.cs`
  - Checks `LockoutEnd` on authenticated users.
  - Signs blocked users out immediately and redirects to login with `blocked=true`.

- `MiddleWare/UserActivityMiddleware.cs`
  - Updates `UserActivity.CurrentPage` and `LastActivityTime` on each authenticated request.

### View Component

- `ViewComponents/AdminMenuViewComponent.cs`
  - Computes whether current user is admin.
  - Intended to render an admin-aware menu.
  - No matching view file is present in the active project, so it looks unused/incomplete.

### Migrations

- `Migrations/20251112115957_usersession.cs`
  - Adds `UserActivities` and `UserRolesInfo`.

- `Migrations/20260317193044_AddIdentityVerificationColumn.cs`
  - Adds `IdentityVerificationD` column to `client`.

- `Migrations/*Designer.cs`
  - EF-generated metadata snapshots for the migrations above.

- `Migrations/DbCallsModelSnapshot.cs`
  - Current EF schema snapshot for `DbCalls`.

### Views

#### Shared Infrastructure

- `Views/_ViewImports.cshtml`
  - Imports namespace and MVC tag helpers.

- `Views/_ViewStart.cshtml`
  - Default layout switch:
    - authenticated users -> `_Layout`
    - anonymous users -> `_Layout1`

- `Views/Shared/_Layout.cshtml`
  - Main authenticated shell and general layout.
  - Decides whether current page is dashboard-like.
  - Loads Vue bundle, AdminLTE/DataTables assets, shared header/footer.

- `Views/Shared/_Layout1.cshtml`
  - Legacy anonymous landing layout.
  - Mostly superseded by current static home page.

- `Views/Shared/_Layout2.cshtml`
  - Main public-site layout with shared header/footer and Vue bundle for auth pages.

- `Views/Shared/_Layout3.cshtml`
  - Dedicated layout for service-detail pages using `service-pages.css`.

- `Views/Shared/_UnifiedHeader.cshtml`
  - Shared marketing-site header with top bar, responsive nav, auth buttons, and service dropdown.

- `Views/Shared/_UnifiedFooter.cshtml`
  - Shared footer links/contact block.

- `Views/Shared/_CookieConsent.cshtml`
  - Cookie consent banner markup.

- `Views/Shared/_ValidationScriptsPartial.cshtml`
  - jQuery validation includes.

- `Views/Shared/Error.cshtml`
  - Standard MVC error page.

#### Home Views

- `Views/Home/Index.cshtml`
  - Current fully custom public landing page.
  - Large amount of inline CSS and marketing sections.
  - Does not use the standard layout.

- `Views/Home/About.cshtml`
  - Full custom about page with inline CSS and shared partials.

- `Views/Home/Contact.cshtml`
  - Contact page using `_Layout2`.
  - Contains informational form UI and map embed.

- `Views/Home/Service.cshtml`
  - Service overview page using `_Layout3`.

- `Views/Home/Privacy.cshtml`
  - Dashboard mount page.
  - Renders `<dashboard>`.

- `Views/Home/TermsAndConditions.cshtml`
  - Terms page using `policy-pages.css`.

- `Views/Home/PrivacyPolicy.cshtml`
  - Privacy page using `policy-pages.css`.

- `Views/Home/CookiesPolicy.cshtml`
  - Cookies page using `policy-pages.css`.

- `Views/Home/Sitemap.cshtml`
  - Sitemap page using `sitemap-page.css`.

#### Service Views

- `Views/Service/ClientDeadlinePrioritisation.cshtml`
  - Detailed marketing page for prioritisation feature.

- `Views/Service/DeadlineStatusVisualization.cshtml`
  - Detailed marketing page for visualization feature.

- `Views/Service/CompaniesHouseSync.cshtml`
  - Detailed marketing page for sync feature.

- `Views/Service/WorkflowEasier.cshtml`
  - Detailed marketing page for workflow feature.

- `Views/Service/AutoDeadlineTracking.cshtml`
  - Detailed marketing page for auto-tracking feature.

#### Account Views

- `Views/Account/SignUp.cshtml`
  - Mounts `SignupPage.vue`.

- `Views/Account/Login.cshtml`
  - Mounts `LoginPage.vue`.

- `Views/Account/ForgotPassword.cshtml`
  - Mounts `ForgetPassword.vue`.

- `Views/Account/ResetPassword.cshtml`
  - Mounts `ResetPassword.vue`.

- `Views/Account/ConfirmEmail.cshtml`
  - Mounts `ConfirmEmailPage.vue`.
  - Appears to be intended for client-side confirmation UI.

- `Views/Account/AccessDenied.cshtml`
  - Simple access-denied page.

#### Client Views

- `Views/Client/FavCompany.cshtml`
  - Mounts `ClientView.vue`.

- `Views/Client/InCompany.cshtml`
  - Mounts `AddCompany.vue`.

#### Admin Views

- `Views/Admin/RegisteredUsers.cshtml`
  - Mounts `RegUsers.vue`.

- `Views/Admin/UsersActivities.cshtml`
  - Mounts `RegisteredUsersActivity.vue`.

### Vue App (`js-module`)

#### Runtime Source

- `js-module/src/main.js`
  - Global Vue bootstrap.
  - Registers all component tags used by Razor.
  - Adds simple Google Analytics page-view tracking.

- `js-module/src/components/SignupPage.vue`
  - Signup UI and client-side validation.
  - Calls `/api/AccountApi/register`.

- `js-module/src/components/LoginPage.vue`
  - Login UI and validation.
  - Calls `/api/AccountApi/login`.
  - On success, immediately triggers company sync and redirects to dashboard.

- `js-module/src/components/ForgetPassword.vue`
  - Forgot-password request form.
  - Calls `/api/AccountApi/forgot`.

- `js-module/src/components/ResetPassword.vue`
  - Reset-password form.
  - Reads `email` and `token` from query string.
  - Calls `/api/AccountApi/reset`.

- `js-module/src/components/ConfirmEmailPage.vue`
  - Client-side email confirmation component.
  - Reads `userId` and `token` from query string and calls `/Account/ConfirmEmail`.

- `js-module/src/components/AddCompany.vue`
  - Add/import company form.
  - Fetches Companies House data.
  - Saves a company record through `ClientApi.SaveDa`.

- `js-module/src/components/ClientView.vue`
  - Main company table and CRUD screen.
  - Uses DataTables.
  - Supports filtering, import, delete, save, and bulk sync.

- `js-module/src/components/Dashboard.vue`
  - Main authenticated dashboard.
  - Loads company list and computes summary metrics, chart data, and review list.

- `js-module/src/components/DashboardHeader.vue`
  - Dashboard top navigation/header.
  - Loads current user email, provides refresh, add-company, and logout UI.

- `js-module/src/components/RegUsers.vue`
  - Admin user management page.

- `js-module/src/components/RegisteredUsersActivity.vue`
  - Admin user activity monitoring page with timezone selector.

- `js-module/src/App.vue`
  - Leftover scaffold file from Vue template.
  - References a missing `HelloWorld` component.
  - Not part of actual runtime flow because `main.js` mounts global components directly.

#### Active Frontend Build Files

- `js-module/package.json`
  - Actual frontend dependencies and npm scripts.

- `js-module/package-lock.json`
  - Actual frontend lockfile.

- `js-module/webpack.config.js`
  - Active webpack config used by package scripts.
  - Outputs bundle to `wwwroot/dist/build.js`.

- `js-module/.postcssrc.js`
  - PostCSS config.

- `js-module/README.md`
  - Generic Vue-template README.

#### Legacy/Template Build Files

- `js-module/config/*`
  - Vue webpack-template environment config.

- `js-module/build/*`
  - Vue webpack-template build scripts.
  - These look like leftover scaffolding; the package scripts use `webpack.config.js` instead.

### Static First-Party Assets

- `wwwroot/js/site.js`
  - Default ASP.NET placeholder; effectively unused.

- `wwwroot/js/cookie-consent.js`
  - LocalStorage-based cookie-banner dismissal.

- `wwwroot/css/dashboard.css`
  - Global dashboard shell layout adjustments.

- `wwwroot/css/responsive-overrides.css`
  - Shared responsive fixes across landing, service, about, footer, and shared grids.

- `wwwroot/css/cookie-consent.css`
  - Cookie banner styling.

- `wwwroot/css/policy-pages.css`
  - Styling for policy/legal pages.

- `wwwroot/css/service-pages.css`
  - Styling for service overview/detail pages.

- `wwwroot/css/sitemap-page.css`
  - Styling for sitemap page.

### Properties

- `Properties/launchSettings.json`
  - Local development launch profiles for IIS Express and direct project launch.

- `Properties/ServiceDependencies/*/profile.arm.json`
  - Azure App Service ARM deployment templates.
  - Variants appear duplicated.

- `Properties/PublishProfiles/*.pubxml`
  - Visual Studio publish profiles for Azure Web Deploy and folder publish.
  - Mostly deployment metadata, not runtime logic.

## Generated Or Vendor Content

- `wwwroot/dist/build.js`
  - Generated Vue production bundle.
  - Should be treated as build output, not source of truth.

- `wwwroot/lib/`
  - jQuery, Bootstrap, and validation libraries.

- `wwwroot/AdminLTE/`
  - AdminLTE vendor theme and plugins.

- `wwwroot/DevFolio/`
  - Public-site template assets.

- `wwwroot/finanza-1.0.0/`
  - Public-site template assets and stock media.

- `wwwroot/skywave/`
  - Custom image assets used by current landing/service pages.

- `build_verify/`
  - Build/publish verification output.

- `Practice Pro/`
  - Very large nested duplicate project snapshot; not part of active compile path.

## Important Technical Notes

- The app builds successfully with warnings.
- Build warning: `Microsoft.AspNet.WebApi.Core` is a legacy .NET Framework package restored into a `net8.0` project.
- Credentials and secrets are currently stored in source:
  - SQL connection string in `appsettings.json`
  - SMTP credentials in `Email/EmailServices.cs`
  - Companies House API key in `Controllers/ClientApi.cs`
- `Startup.cs` calls `UseCors("AllowAllOrigins")`, but no visible `AddCors` policy registration exists in the active startup file.
- Email confirmation flow is internally inconsistent:
  - server action `Account.ConfirmEmail` confirms using a freshly generated token instead of the incoming token
  - the component/view path for `ConfirmEmailPage.vue` appears disconnected from the controller's successful JSON response path
- `AdminMenuViewComponent` has no active view and appears unused.
- `App.vue` is scaffold residue and not part of the actual mounted app.
- Admin role logic uses custom `UserRolesInfo` instead of standard Identity role membership.
- `HomeApi.SendDueEmails` iterates all clients, not only the current user's clients.

## Current Build State

`dotnet build "Practice Pro.sln"` succeeds.

Warnings observed during build:

- legacy package compatibility warning for `Microsoft.AspNet.WebApi.Core`
- migration class naming warning for `usersession`
