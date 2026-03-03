# Vue.js Login + Dashboard Design

## Goal

Replace the existing Hackathon.Web Blazor project with a Vue 3 + TypeScript frontend that authenticates via Keycloak using subdomain-based tenancy and provides a Dashboard + Financial Runway analyzer page. Uses sprout-design-system (`design-system-next`) for UI components.

## Architecture

```
User visits {tenant}.app.com
  -> Vue app extracts realm from subdomain
  -> keycloak-js redirects to Keycloak login ({kcBase}/realms/{tenant})
  -> User authenticates, redirected back with token
  -> Vue stores token, attaches to all BFF requests
  -> BFF validates JWT (already configured), proxies to ApiService
```

### Auth Flow (Detailed)

1. User navigates to `acme.localhost:5173`
2. Vue app extracts `acme` from subdomain
3. `keycloak-js` initializes with realm=`acme`, url=`{kcBase}`, clientId=`{clientId}`
4. If no valid token, keycloak-js redirects to Keycloak hosted login page
5. After login, Keycloak redirects back with authorization code
6. keycloak-js exchanges code for JWT token (handled automatically)
7. Token stored in keycloak-js instance, attached to Axios via interceptor
8. Vue Router auth guard checks `keycloak.authenticated` before allowing navigation
9. Token refresh handled automatically by keycloak-js `onTokenExpired`

### Subdomain Extraction

```ts
// composables/useKeycloak.ts
const hostname = window.location.hostname; // e.g., "acme.localhost"
const tenant = hostname.split('.')[0];     // e.g., "acme"
```

For local dev: use `acme.localhost:5173` (browsers resolve `*.localhost` to 127.0.0.1).

## Tech Stack

- **Vue 3** (Composition API + `<script setup>`)
- **TypeScript**
- **Vite** (build tool)
- **keycloak-js** (Keycloak OIDC adapter)
- **design-system-next** v2.27.9 (sprout-design-system)
- **Vue Router** (SPA routing with auth guards)
- **Pinia** (state management)
- **Axios** (HTTP client with token interceptor)

## Project Structure

```
Hackathon.Web/
├── index.html
├── package.json
├── vite.config.ts
├── tsconfig.json
├── tailwind.config.js
├── postcss.config.js
├── env.d.ts
├── .env.development
├── src/
│   ├── main.ts                    -- App entry, registers plugins
│   ├── App.vue                    -- Root component with SprSidenav layout
│   ├── router/
│   │   └── index.ts               -- Routes + auth guard
│   ├── stores/
│   │   └── auth.ts                -- Pinia store for keycloak state
│   ├── composables/
│   │   └── useKeycloak.ts         -- keycloak-js init + tenant extraction
│   ├── services/
│   │   └── api.ts                 -- Axios instance + Bearer interceptor
│   ├── pages/
│   │   ├── LoginPage.vue          -- Loading/redirect screen
│   │   ├── DashboardPage.vue      -- Authenticated home
│   │   └── FinancialRunwayPage.vue -- CSV upload + results
│   ├── components/
│   │   ├── AppSidenav.vue         -- Sidenav wrapper
│   │   └── FinancialResults.vue   -- Results display cards/table
│   └── types/
│       └── index.ts               -- TypeScript interfaces
```

## Pages

### 1. LoginPage.vue

Not a traditional form -- just a loading screen. keycloak-js handles the actual login via redirect. If user is not authenticated, they see a brief "Redirecting to login..." with a spinner before keycloak-js redirects them.

**Components used:** `spr-card`, `spr-logo`, `spr-button` (if redirect fails, show a manual "Login" button)

### 2. DashboardPage.vue

Welcome page after login. Shows:
- User greeting (from `keycloak.tokenParsed.preferred_username`)
- Tenant/realm info
- Quick link card to Financial Runway analyzer
- Summary of recent analyses (if we add a GET endpoint later)

**Components used:** `spr-card`, `spr-avatar`, `spr-banner`, `spr-icon`, `spr-badge`

### 3. FinancialRunwayPage.vue

The main feature page. Form to upload CSV + fill in financial data + add life events, then view results.

**Form section:**
- `spr-file-upload` for CSV
- `spr-input` for monthly salary and total savings
- Dynamic life events list:
  - `spr-select` for event type (BuyHouse, BuyCar, HaveBaby, etc.)
  - `spr-input` for description
  - `spr-input` for month from now (number)
  - `spr-checkbox` for recurring vs one-time
  - `spr-input` for estimated cost
  - `spr-button` to add/remove events
- `spr-button` (tone="success") to submit

**Results section (after API response):**
- `spr-card` for runway summary (months, burn rate, adjusted runway)
- `spr-table` for categorized expenses
- `spr-table` for monthly projections
- `spr-card` for life event impacts
- `spr-card` for narrative text
- `spr-banner` for success/error states

## Component Patterns

### Axios Interceptor
```ts
// services/api.ts
import axios from 'axios';
import { useAuthStore } from '@/stores/auth';

const api = axios.create({
  baseURL: import.meta.env.VITE_BFF_URL
});

api.interceptors.request.use((config) => {
  const auth = useAuthStore();
  if (auth.token) {
    config.headers.Authorization = `Bearer ${auth.token}`;
  }
  return config;
});
```

### Auth Guard
```ts
// router/index.ts
router.beforeEach(async (to) => {
  const auth = useAuthStore();
  if (to.meta.requiresAuth && !auth.isAuthenticated) {
    auth.login(); // triggers keycloak redirect
    return false;
  }
});
```

### Keycloak Init
```ts
// composables/useKeycloak.ts
const hostname = window.location.hostname;
const tenant = hostname.split('.')[0];
const keycloak = new Keycloak({
  url: import.meta.env.VITE_KEYCLOAK_URL,
  realm: tenant,
  clientId: import.meta.env.VITE_KEYCLOAK_CLIENT_ID
});
```

## Environment Variables

```env
# .env.development
VITE_BFF_URL=http://localhost:5208
VITE_KEYCLOAK_URL=https://sso-test.sprout.ph
VITE_KEYCLOAK_CLIENT_ID=account
```

## Aspire Integration

The Vue app will be added to AppHost as a Node.js project using Aspire's npm support, or served as a static SPA proxied through the BFF. For the hackathon, running `npm run dev` alongside Aspire is sufficient.

## No BFF Changes Needed

The BFF already:
- Validates JWT Bearer tokens from any Keycloak realm under the configured base URL
- Extracts tenant from the token's issuer
- Has CORS configured to allow any origin with credentials
- Has the Financial Runway proxy endpoint (AllowAnonymous currently -- will need auth added back after testing)
