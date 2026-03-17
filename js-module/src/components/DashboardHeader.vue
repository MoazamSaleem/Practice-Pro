<template>
  <header class="dash-portal-header">
    <a class="dash-brand" href="/">
      <img src="/finanza-1.0.0/img/logo.jpg" alt="PremiumDM Logo" class="dash-logo-img" />
      <!-- <span class="dash-brand-copy">
        <strong>PremiumDM</strong>
        <small>Deadline Management</small>
      </span> -->
    </a>

    <nav class="dash-portal-nav" aria-label="Dashboard navigation">
      <a class="dash-nav-link" :class="{ 'is-active': currentPath === '/DashBoard' }" href="/DashBoard">Dashboard</a>
      <a class="dash-nav-link" :class="{ 'is-active': currentPath === '/Client/FavCompany' }" href="/Client/FavCompany">Companies</a>
      <!-- <button type="button" class="dash-nav-link dash-nav-button" @click="sendDueEmails" :disabled="isSendingEmails">Reminders</button> -->
      <button type="button" class="dash-nav-link dash-nav-button" @click="refreshDashboard" :disabled="isLoading">Refresh</button>
    </nav>

    <div class="dash-top-actions">
      <a class="dash-primary-action" href="/Client/InCompany">Add Company</a>
      <!-- <button type="button" class="dash-icon-btn" @click="sendDueEmails" :disabled="isSendingEmails" title="Send reminders">
        <span v-if="!isSendingEmails"><i class="fa-regular fa-bell"></i></span>
        <span v-else><span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span></span>
      </button> -->
      <button type="button" class="dash-icon-btn" @click="refreshDashboard" :disabled="isLoading" title="Refresh dashboard">
        <span v-if="!isLoading"><i class="fa-solid fa-rotate-right"></i></span>
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
      currentPath: window.location.pathname,
      showProfileMenu: false
    };
  },
  computed: {
    userInitial() {
      const source = (this.userEmail || "User").trim();
      return source.charAt(0).toUpperCase() || "U";
    }
  },
  methods: {
    refreshDashboard() {
      if (typeof this.$parent.fetchData === 'function') {
        this.$parent.fetchData();
      } else {
        window.location.reload();
      }
    },
    toggleProfileMenu() {
      this.showProfileMenu = !this.showProfileMenu;
    },
    closeProfileMenu(e) {
      if (!this.$el.contains(e.target)) {
        this.showProfileMenu = false;
      }
    },
    async sendDueEmails() {
      if (this.isSendingEmails) return;
      this.isSendingEmails = true;
      try {
        const res = await axios.post("/api/HomeApi/SendDueEmails");
        alert(res.data.message);
      } catch (error) {
        console.error("Error sending emails:", error);
        alert("Failed to send emails.");
      } finally {
        this.isSendingEmails = false;
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
/* Global styles for the dashboard header so it works across components if needed, or put in scoped if only used here */
.dash-portal-header {
  display: grid;
  grid-template-columns: auto 1fr auto;
  align-items: center;
  gap: 16px;
  padding: 10px 24px;
  background: #ffffff;
  border-bottom: 1px solid #efe7f4;
  position: sticky;
  top: 0;
  z-index: 1000;
}

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



.dash-brand-copy strong {
  color: #222036;
  font-size: 16px;
  line-height: 1.1;
}

.dash-brand-copy small {
  color: #85778f;
  font-size: 9px;
  letter-spacing: 0.05em;
  text-transform: uppercase;
}

.dash-portal-nav {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 16px;
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

.dash-top-actions {
  display: flex;
  align-items: center;
  justify-content: flex-end;
  gap: 14px;
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
