<template>
  <div class="dashboard-page">
    <div class="dashboard-header">
      <div class="header-left">
        <h2 class="dashboard-title">User Activity Dashboard</h2>
        <p class="subtitle">Track who is active, where they are browsing, and recent login activity.</p>
      </div>

      <div class="header-right">
        <div class="timezone-selector">
          <label for="timezone" class="timezone-label">Time zone</label>
          <select id="timezone" v-model="selectedTimezone" @change="saveTimezone" class="timezone-dropdown">
            <option v-for="zone in timezones" :key="zone" :value="zone">{{ zone }}</option>
          </select>
        </div>

        <button class="reload-btn" @click="fetchUsers" :disabled="loading">
          <span v-if="loading">Refreshing...</span>
          <span v-else>Refresh</span>
        </button>
      </div>
    </div>

    <div class="summary-grid">
      <div class="summary-card">
        <strong>Total Users</strong>
        <span>{{ users.length }}</span>
      </div>
      <div class="summary-card is-positive">
        <strong>Active Now</strong>
        <span>{{ activeUsersCount }}</span>
      </div>
      <div class="summary-card">
        <strong>Admins</strong>
        <span>{{ adminUsersCount }}</span>
      </div>
      <div class="summary-card is-warning">
        <strong>Blocked</strong>
        <span>{{ blockedUsersCount }}</span>
      </div>
    </div>

    <div v-if="errorMessage" class="error-banner">
      {{ errorMessage }}
    </div>

    <div v-if="loading" class="loading-box">
      Loading user activity...
    </div>

    <div v-else class="table-wrapper">
      <table class="styled-table">
        <thead>
          <tr>
            <th>Email</th>
            <th>Role</th>
            <th>Current Page</th>
            <th>Last Login</th>
            <th>Logins This Month</th>
            <th>Last Activity</th>
            <th>Status</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="user in users" :key="user.userId">
            <td class="email-cell">{{ user.userEmail }}</td>
            <td>
              <span class="role-badge" :class="roleClass(user)">
                {{ roleLabel(user) }}
              </span>
            </td>
            <td>{{ user.currentPage || "N/A" }}</td>
            <td>{{ formatDate(user.lastLogin) }}</td>
            <td>{{ user.loginCountThisMonth }}</td>
            <td>{{ formatDate(user.lastActivityTime) }}</td>
            <td>
              <div class="status-indicator">
                <span :class="['dot', statusClass(user)]"></span>
                <span :class="['status-text', statusTextClass(user)]">
                  {{ statusLabel(user) }}
                </span>
              </div>
            </td>
          </tr>

          <tr v-if="users.length === 0">
            <td colspan="7" class="no-data">No user activity has been recorded yet.</td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script>
import axios from "axios";

export default {
  name: "RegisteredUsersActivity",
  data() {
    return {
      users: [],
      loading: true,
      errorMessage: "",
      refreshTimer: null,
      selectedTimezone: "Europe/London",
      timezones: [
        "Europe/London",
        "UTC",
        "Asia/Karachi",
        "Asia/Dubai",
        "Asia/Kolkata",
        "Europe/Paris",
        "America/New_York",
        "America/Los_Angeles",
        "Australia/Sydney"
      ]
    };
  },
  computed: {
    activeUsersCount() {
      return this.users.filter((user) => user.isActive).length;
    },
    adminUsersCount() {
      return this.users.filter((user) => user.isAdmin).length;
    },
    blockedUsersCount() {
      return this.users.filter((user) => user.isBlocked).length;
    }
  },
  methods: {
    roleLabel(user) {
      if (user.isPrimaryAdmin) return "Primary Admin";
      if (user.isAdmin) return "Admin";
      return "User";
    },
    roleClass(user) {
      if (user.isPrimaryAdmin) return "primary-admin";
      if (user.isAdmin) return "admin";
      return "user";
    },
    statusLabel(user) {
      if (user.isBlocked) return "Blocked";
      if (user.isActive) return "Active";
      return "Inactive";
    },
    statusClass(user) {
      if (user.isBlocked) return "blocked-dot";
      if (user.isActive) return "active-dot";
      return "inactive-dot";
    },
    statusTextClass(user) {
      if (user.isBlocked) return "text-blocked";
      if (user.isActive) return "text-active";
      return "text-inactive";
    },
    getErrorMessage(error, fallback) {
      if (error.response && error.response.data) {
        if (typeof error.response.data === "string") {
          return error.response.data;
        }
        if (error.response.data.message) {
          return error.response.data.message;
        }
      }

      return fallback;
    },
    async fetchUsers() {
      this.loading = true;
      this.errorMessage = "";

      try {
        const response = await axios.get("/api/AdminApi/GetUserActivities");
        this.users = response.data || [];
      } catch (error) {
        console.error("Error fetching user activity:", error);
        this.errorMessage = this.getErrorMessage(error, "Unable to load user activity.");
      } finally {
        this.loading = false;
      }
    },
    formatDate(date) {
      if (!date) return "N/A";

      const parsedDate = new Date(date);
      if (Number.isNaN(parsedDate.getTime())) return "N/A";

      return parsedDate.toLocaleString("en-GB", {
        timeZone: this.selectedTimezone,
        year: "numeric",
        month: "short",
        day: "numeric",
        hour: "2-digit",
        minute: "2-digit",
        second: "2-digit",
        hour12: true
      });
    },
    saveTimezone() {
      localStorage.setItem("selectedTimezone", this.selectedTimezone);
    },
    startAutoRefresh() {
      this.stopAutoRefresh();
      this.refreshTimer = window.setInterval(() => {
        this.fetchUsers();
      }, 30000);
    },
    stopAutoRefresh() {
      if (this.refreshTimer) {
        window.clearInterval(this.refreshTimer);
        this.refreshTimer = null;
      }
    }
  },
  mounted() {
    const savedZone = localStorage.getItem("selectedTimezone");
    if (savedZone) {
      this.selectedTimezone = savedZone;
    }

    this.fetchUsers();
    this.startAutoRefresh();
  },
  beforeDestroy() {
    this.stopAutoRefresh();
  }
};
</script>

<style scoped>
.dashboard-page {
  min-height: 100vh;
  background: linear-gradient(135deg, #f0f4ff, #ffffff);
  padding: 40px 60px;
  font-family: "Segoe UI", sans-serif;
  color: #243246;
}

.dashboard-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 20px;
  background: rgba(25, 118, 210, 0.95);
  color: #ffffff;
  padding: 24px 32px;
  border-radius: 16px;
  box-shadow: 0 6px 24px rgba(25, 118, 210, 0.24);
}

.header-left {
  display: grid;
  gap: 6px;
}

.header-right {
  display: flex;
  align-items: center;
  gap: 16px;
  flex-wrap: wrap;
}

.dashboard-title {
  margin: 0;
  font-size: 1.9rem;
  font-weight: 700;
}

.subtitle {
  margin: 0;
  font-size: 0.96rem;
  opacity: 0.92;
}

.timezone-selector {
  display: flex;
  align-items: center;
  gap: 10px;
}

.timezone-label {
  font-size: 0.9rem;
  font-weight: 700;
}

.timezone-dropdown {
  border: 0;
  border-radius: 10px;
  padding: 10px 12px;
  background: #ffffff;
  color: #1565c0;
  font-weight: 700;
}

.reload-btn {
  min-width: 120px;
  border: 0;
  border-radius: 10px;
  padding: 10px 18px;
  background: #ffffff;
  color: #1565c0;
  font-weight: 700;
  cursor: pointer;
  transition: all 0.2s ease;
}

.reload-btn:hover:not(:disabled) {
  background: #0d47a1;
  color: #ffffff;
}

.reload-btn:disabled {
  opacity: 0.7;
  cursor: wait;
}

.summary-grid {
  display: grid;
  grid-template-columns: repeat(4, minmax(0, 1fr));
  gap: 18px;
  margin-top: 24px;
}

.summary-card {
  display: grid;
  gap: 10px;
  background: #ffffff;
  border-radius: 14px;
  padding: 18px 20px;
  box-shadow: 0 8px 20px rgba(15, 23, 42, 0.05);
}

.summary-card strong {
  color: #607d8b;
  font-size: 0.9rem;
  text-transform: uppercase;
  letter-spacing: 0.04em;
}

.summary-card span {
  color: #0d47a1;
  font-size: 1.8rem;
  font-weight: 800;
}

.summary-card.is-positive span {
  color: #2e7d32;
}

.summary-card.is-warning span {
  color: #c62828;
}

.error-banner {
  margin-top: 20px;
  padding: 14px 16px;
  border-radius: 12px;
  background: #fff2f0;
  color: #c62828;
  font-weight: 600;
}

.loading-box {
  margin-top: 24px;
  border-radius: 14px;
  padding: 18px 20px;
  background: rgba(25, 118, 210, 0.1);
  color: #1565c0;
  font-weight: 700;
}

.table-wrapper {
  margin-top: 24px;
  background: #ffffff;
  border-radius: 16px;
  box-shadow: 0 10px 25px rgba(0, 0, 0, 0.05);
  padding: 20px 24px;
  overflow-x: auto;
}

.styled-table {
  width: 100%;
  border-collapse: separate;
  border-spacing: 0;
}

.styled-table thead tr {
  background: linear-gradient(90deg, #1565c0, #1976d2);
  color: #ffffff;
}

.styled-table th,
.styled-table td {
  padding: 14px 16px;
  text-align: center;
}

.styled-table tbody tr:nth-child(even) {
  background: #f9fbff;
}

.styled-table tbody tr:hover {
  background: #e8f1ff;
}

.email-cell {
  word-break: break-word;
}

.role-badge {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  min-height: 34px;
  padding: 0 12px;
  border-radius: 999px;
  font-size: 0.85rem;
  font-weight: 700;
}

.role-badge.primary-admin {
  background: #dbeafe;
  color: #0d47a1;
}

.role-badge.admin {
  background: #fff8e1;
  color: #ef6c00;
}

.role-badge.user {
  background: #edf2f7;
  color: #546e7a;
}

.status-indicator {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
}

.dot {
  width: 10px;
  height: 10px;
  border-radius: 50%;
}

.active-dot {
  background: #43a047;
  box-shadow: 0 0 0 4px rgba(67, 160, 71, 0.15);
}

.inactive-dot {
  background: #90a4ae;
}

.blocked-dot {
  background: #e53935;
  box-shadow: 0 0 0 4px rgba(229, 57, 53, 0.12);
}

.text-active {
  color: #2e7d32;
  font-weight: 700;
}

.text-inactive {
  color: #607d8b;
  font-weight: 700;
}

.text-blocked {
  color: #c62828;
  font-weight: 700;
}

.no-data {
  color: #607d8b;
  font-style: italic;
}

@media (max-width: 1100px) {
  .summary-grid {
    grid-template-columns: repeat(2, minmax(0, 1fr));
  }
}

@media (max-width: 960px) {
  .dashboard-page {
    padding: 26px 18px;
  }

  .dashboard-header {
    flex-direction: column;
    align-items: stretch;
  }

  .header-right {
    justify-content: space-between;
  }
}

@media (max-width: 640px) {
  .summary-grid {
    grid-template-columns: 1fr;
  }

  .header-right,
  .timezone-selector {
    flex-direction: column;
    align-items: stretch;
  }
}
</style>
