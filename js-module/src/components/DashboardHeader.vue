<template>
  <header class="dash-portal-header">
    <!-- Brand -->
    <a class="dash-brand" href="/">
      <img src="/finanza-1.0.0/img/logo.jpg" alt="PremiumDM Logo" class="dash-logo-img" />
    </a>

    <!-- Desktop nav -->
    <nav class="dash-portal-nav" aria-label="Dashboard navigation">
      <a class="dash-nav-link" :class="{ 'is-active': currentPath === '/DashBoard' }" href="/DashBoard">Dashboard</a>
      <a class="dash-nav-link" :class="{ 'is-active': currentPath === '/Client/FavCompany' }" href="/Client/FavCompany">Companies</a>
      <a v-if="showAdminFeatures" class="dash-nav-link" :class="{ 'is-active': currentPath === '/Admin/RegisteredUsers' }" href="/Admin/RegisteredUsers">Admin Users</a>
      <a v-if="showAdminFeatures" class="dash-nav-link" :class="{ 'is-active': currentPath === '/Admin/UsersActivities' }" href="/Admin/UsersActivities">User Activity</a>
      <button type="button" class="dash-nav-link dash-nav-button" @click="refreshDashboard" :disabled="isBusy">Refresh</button>
    </nav>

    <!-- Desktop actions -->
    <div class="dash-top-actions">
      <a class="dash-primary-action" href="/Client/InCompany">Add Company</a>
      <button type="button" class="dash-icon-btn" @click="refreshDashboard" :disabled="isBusy" title="Refresh dashboard">
        <span v-if="!isBusy"><i class="fa-solid fa-rotate-right"></i></span>
        <span v-else><span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span></span>
      </button>
      <div class="dash-profile-group">
        <div class="dash-profile" @click="toggleProfileMenu" :title="userEmail || 'Current account'">
          <span class="dash-avatar">{{ userInitial }}</span>
        </div>
        <div v-if="showProfileMenu" class="dash-profile-menu">
          <div class="dash-profile-info">
            <strong>Account</strong>
            <span>{{ userEmail }}</span>
          </div>
          <div class="dash-menu-divider"></div>
          <form id="logoutForm" action="/Account/Logout" method="get" class="dash-logout-form">
            <button type="submit" class="dash-menu-item is-logout">
              <i class="fa-solid fa-right-from-bracket"></i>
              <span>Log out</span>
            </button>
          </form>
        </div>
      </div>
    </div>

    <!-- Mobile-only: avatar + hamburger -->
    <div class="dash-mobile-bar">
      <div class="dash-profile-group">
        <div class="dash-profile" @click="toggleProfileMenu" :title="userEmail || 'Current account'">
          <span class="dash-avatar">{{ userInitial }}</span>
        </div>
        <div v-if="showProfileMenu" class="dash-profile-menu">
          <div class="dash-profile-info">
            <strong>Account</strong>
            <span>{{ userEmail }}</span>
          </div>
          <div class="dash-menu-divider"></div>
          <form action="/Account/Logout" method="get" class="dash-logout-form">
            <button type="submit" class="dash-menu-item is-logout">
              <i class="fa-solid fa-right-from-bracket"></i>
              <span>Log out</span>
            </button>
          </form>
        </div>
      </div>
      <button
        type="button"
        class="dash-hamburger"
        @click="mobileOpen = !mobileOpen"
        :aria-expanded="String(mobileOpen)"
        aria-label="Toggle navigation"
      >
        <i :class="mobileOpen ? 'fa-solid fa-xmark' : 'fa-solid fa-bars'"></i>
      </button>
    </div>

    <!-- Mobile nav drawer -->
    <div class="dash-mobile-drawer" :class="{ 'is-open': mobileOpen }" role="navigation" aria-label="Mobile navigation">
      <a class="dash-drawer-link" :class="{ 'is-active': currentPath === '/DashBoard' }" href="/DashBoard">Dashboard</a>
      <a class="dash-drawer-link" :class="{ 'is-active': currentPath === '/Client/FavCompany' }" href="/Client/FavCompany">Companies</a>
      <a v-if="showAdminFeatures" class="dash-drawer-link" :class="{ 'is-active': currentPath === '/Admin/RegisteredUsers' }" href="/Admin/RegisteredUsers">Admin Users</a>
      <a v-if="showAdminFeatures" class="dash-drawer-link" :class="{ 'is-active': currentPath === '/Admin/UsersActivities' }" href="/Admin/UsersActivities">User Activity</a>
      <button type="button" class="dash-drawer-link dash-nav-button" @click="refreshDashboard; mobileOpen = false" :disabled="isBusy">Refresh</button>
      <a class="dash-drawer-add" href="/Client/InCompany">+ Add Company</a>
    </div>
  </header>
</template>

<script>
import axios from "axios";

export default {
  props: {
    isLoading: {
      type: Boolean,
      default: false
    }
  },
  data() {
    return {
      userEmail: "",
      isSendingEmails: false,
      isRefreshing: false,
      currentPath: window.location.pathname,
      showProfileMenu: false,
      mobileOpen: false
    };
  },
  computed: {
    isBusy() {
      return this.isLoading || this.isRefreshing;
    },
    showAdminFeatures() {
      return (this.userEmail || "").trim().toLowerCase() === "info@premiumaccountants.co.uk";
    },
    userInitial() {
      const source = (this.userEmail || "User").trim();
      return source.charAt(0).toUpperCase() || "U";
    }
  },
  methods: {
    async refreshDashboard() {
      if (this.isBusy) return;

      this.isRefreshing = true;
      try {
        const res = await axios.post("/api/ClientApi/UpdateAllCompaniesSync");
        if (!res.data.status) {
          throw new Error(res.data.message || "Failed to refresh company data.");
        }

        window.dispatchEvent(new CustomEvent("premiumdm:company-data-synced", {
          detail: res.data
        }));
      } catch (error) {
        console.error("Error refreshing dashboard data:", error);
        alert("Failed to refresh company data from Companies House.");
      } finally {
        this.isRefreshing = false;
      }
    },
    toggleProfileMenu() {
      this.showProfileMenu = !this.showProfileMenu;
    },
    closeProfileMenu(e) {
      if (!this.$el.contains(e.target)) {
        this.showProfileMenu = false;
        this.mobileOpen = false;
      }
    },
    fetchUserEmail() {
      axios.get("/api/HomeApi/GetCurrentUserEmail")
        .then((res) => {
          if (res.data.email) {
            this.userEmail = res.data.email;
          }
        })
        .catch((error) => {
          console.error("Error fetching user email:", error);
        });
    }
  },
  mounted() {
    this.fetchUserEmail();
    document.addEventListener("click", this.closeProfileMenu);
  },
  beforeDestroy() {
    document.removeEventListener("click", this.closeProfileMenu);
  }
};
</script>

<style>
/* ──────────────────────────────────────────────────────────
   Dashboard Header – Mobile-first
────────────────────────────────────────────────────────── */

/* Base (mobile): brand + mobile-bar in a row */
.dash-portal-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 12px;
  padding: 10px 16px;
  background: #ffffff;
  border-bottom: 1px solid #efe7f4;
  position: sticky;
  top: 0;
  z-index: 1000;
  flex-wrap: wrap;
}

/* Desktop nav & actions hidden by default, shown at ≥760 px */
.dash-portal-nav,
.dash-top-actions {
  display: none;
}

/* Mobile-only cluster (avatar + hamburger) */
.dash-mobile-bar {
  display: flex;
  align-items: center;
  gap: 10px;
  margin-left: auto;
}

/* Mobile drawer */
.dash-mobile-drawer {
  display: none;
  width: 100%;
  flex-direction: column;
  gap: 4px;
  padding: 8px 0 12px;
  border-top: 1px solid #f0eaf6;
}

.dash-mobile-drawer.is-open {
  display: flex;
}

.dash-drawer-link {
  border: 0;
  background: transparent;
  font: inherit;
  font-size: 15px;
  font-weight: 600;
  color: #81728e;
  padding: 10px 8px;
  border-radius: 10px;
  text-align: left;
  cursor: pointer;
  display: block;
}

.dash-drawer-link.is-active {
  color: #126ADB;
  background: #edf5ff;
}

.dash-drawer-add {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  margin-top: 6px;
  min-height: 40px;
  padding: 0 18px;
  border-radius: 9px;
  background: #126ADB;
  color: #ffffff;
  font-size: 14px;
  font-weight: 700;
}

/* Hamburger button */
.dash-hamburger {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 38px;
  height: 38px;
  border: 1px solid #e8dff0;
  border-radius: 9px;
  background: #ffffff;
  color: #126ADB;
  font-size: 16px;
  cursor: pointer;
}

/* ≥ 760 px: switch to desktop layout */
@media (min-width: 760px) {
  .dash-portal-header {
    display: grid;
    grid-template-columns: auto 1fr auto;
    padding: 10px 24px;
    flex-wrap: nowrap;
  }

  .dash-portal-nav {
    display: flex;
    align-items: center;
    justify-content: center;
    gap: 16px;
    flex-wrap: wrap;
  }

  .dash-top-actions {
    display: flex;
    align-items: center;
    justify-content: flex-end;
    gap: 14px;
  }

  /* Hide mobile-only elements */
  .dash-mobile-bar,
  .dash-mobile-drawer,
  .dash-mobile-drawer.is-open,
  .dash-hamburger {
    display: none !important;
  }
}

/* ── Shared sub-components ── */
.dash-logo-img {
  height: 42px;
  border-radius: 6px;
}

.dash-brand {
  display: inline-flex;
  align-items: center;
  gap: 14px;
  color: #251830;
  text-decoration: none;
}

.dash-brand:hover,
.dash-nav-link:hover,
.dash-primary-action:hover {
  text-decoration: none;
}

.dash-nav-link {
  border: 0;
  background: transparent;
  color: #81728e;
  font: inherit;
  font-size: 15px;
  font-weight: 600;
  cursor: pointer;
  transition: color 0.2s ease;
}

.dash-nav-link.is-active {
  color: #126ADB;
}

.dash-nav-button:disabled,
.dash-icon-btn:disabled {
  opacity: 0.55;
  cursor: wait;
}

.dash-primary-action {
  min-height: 40px;
  padding: 0 18px;
  border-radius: 9px;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  background: #126ADB;
  color: #ffffff;
  font-size: 14px;
  font-weight: 700;
  transition: background 0.2s;
}

.dash-primary-action:hover {
  color: #ffffff;
  background: #0d59b8;
}

.dash-icon-btn {
  width: 40px;
  height: 40px;
  border: 1px solid #e8dff0;
  border-radius: 9px;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  background: #ffffff;
  color: #126ADB;
  cursor: pointer;
  font-size: 14px;
}

.dash-avatar {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  border: 1.5px solid #126ADB;
  background: #f0f7ff;
  color: #126ADB;
  font-size: 14px;
  font-weight: 800;
  cursor: pointer;
  transition: transform 0.2s;
}

.dash-avatar:hover {
  transform: scale(1.05);
}

.dash-profile-group {
  position: relative;
}

.dash-profile-menu {
  position: absolute;
  top: calc(100% + 12px);
  right: 0;
  min-width: 200px;
  background: #ffffff;
  border: 1px solid #e8dff0;
  border-radius: 12px;
  box-shadow: 0 10px 25px rgba(18, 106, 219, 0.12);
  padding: 8px;
  z-index: 1100;
}

.dash-profile-info {
  padding: 10px 12px;
}

.dash-profile-info strong {
  display: block;
  color: #251830;
  font-size: 12px;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.dash-profile-info span {
  display: block;
  color: #81728e;
  font-size: 13px;
  margin-top: 2px;
  word-break: break-all;
}

.dash-menu-divider {
  height: 1px;
  background: #f0f0f5;
  margin: 4px 8px;
}

.dash-menu-item {
  width: 100%;
  border: 0;
  background: transparent;
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 10px 12px;
  border-radius: 8px;
  color: #251830;
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
  text-align: left;
  transition: background 0.2s;
}

.dash-menu-item:hover {
  background: #f3f7ff;
}

.dash-menu-item.is-logout {
  color: #e03131;
}

.dash-menu-item.is-logout:hover {
  background: #fff5f5;
}

.dash-logout-form {
  margin: 0;
}
</style>
