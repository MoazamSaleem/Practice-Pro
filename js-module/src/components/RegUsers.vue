<template>
  <div class="admin-page">
    <div class="admin-header">
      <div class="header-copy">
        <h2 class="title">Admin User Management</h2>
        <p class="subtitle">Manage registered users, blocking, and delegated admin access.</p>
      </div>
      <button class="reload-btn" @click="fetchData" :disabled="loading">
        <span v-if="loading">Refreshing...</span>
        <span v-else>Refresh</span>
      </button>
    </div>

    <div class="summary-grid">
      <div class="summary-card shadow-sm">
        <strong>Total Users</strong>
        <span class="count">{{ totalUsers }}</span>
      </div>
      <div class="summary-card shadow-sm">
        <strong>Admins</strong>
        <span class="count">{{ adminCount }}</span>
      </div>
      <div class="summary-card shadow-sm">
        <strong>Blocked</strong>
        <span class="count">{{ blockedCount }}</span>
      </div>
    </div>

    <div v-if="errorMessage" class="error-banner">
      {{ errorMessage }}
    </div>

    <div class="table-container shadow-sm">
      <table class="styled-table">
        <thead>
          <tr>
            <th>Email</th>
            <th>Username</th>
            <th>Status</th>
            <th>Role</th>
            <th>Actions</th>
          </tr>
        </thead>

        <tbody>
          <tr v-for="user in users" :key="user.id">
            <td class="email-cell">{{ user.email }}</td>
            <td>{{ user.userName || user.email }}</td>

            <td>
              <span class="status-badge" :class="isBlocked(user) ? 'blocked' : 'active'">
                {{ isBlocked(user) ? "Blocked" : "Active" }}
              </span>
            </td>

            <td>
              <span class="role-badge" :class="roleClass(user)">
                {{ roleLabel(user) }}
              </span>
            </td>

            <td>
              <div v-if="isMainAdmin(user)" class="action-group">
                <span class="super-admin-badge">Primary Admin</span>
              </div>
              <div v-else class="action-group">
                <button
                  v-if="!isBlocked(user)"
                  @click="blockUser(user.id)"
                  class="action-btn block-btn"
                  :disabled="loading"
                >
                  Block
                </button>
                <button
                  v-else
                  @click="unblockUser(user.id)"
                  class="action-btn unblock-btn"
                  :disabled="loading"
                >
                  Unblock
                </button>

                <button
                  v-if="!user.isAdmin"
                  @click="makeAdmin(user.id)"
                  class="action-btn promote-btn"
                  :disabled="loading"
                >
                  Make Admin
                </button>
                <button
                  v-else
                  @click="removeAdmin(user.id)"
                  class="action-btn demote-btn"
                  :disabled="loading"
                >
                  Remove Admin
                </button>
              </div>
            </td>
          </tr>

          <tr v-if="!loading && users.length === 0">
            <td colspan="5" class="no-data">No users found.</td>
          </tr>
        </tbody>
      </table>
    </div>

    <div v-if="loading" class="loading-overlay">
      <div class="spinner"></div>
      <p>Loading users...</p>
    </div>
  </div>
</template>

<script>
import axios from "axios";

export default {
  name: "AdminUsers",
  data() {
    return {
      users: [],
      totalUsers: 0,
      loading: false,
      errorMessage: "",
      mainAdmin: "info@premiumaccountants.co.uk"
    };
  },
  computed: {
    adminCount() {
      return this.users.filter((user) => user.isAdmin).length;
    },
    blockedCount() {
      return this.users.filter((user) => this.isBlocked(user)).length;
    }
  },
  methods: {
    normalizeEmail(email) {
      return (email || "").trim().toLowerCase();
    },
    isMainAdmin(user) {
      return user.isPrimaryAdmin || this.normalizeEmail(user.email) === this.mainAdmin;
    },
    isBlocked(user) {
      return (user.status || "").toLowerCase() === "blocked";
    },
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
    async fetchData() {
      this.loading = true;
      this.errorMessage = "";

      try {
        const [totalRes, usersRes] = await Promise.all([
          axios.get("/api/AdminApi/GetTotalUsers"),
          axios.get("/api/AdminApi/GetAllUsers")
        ]);

        this.totalUsers = totalRes.data.total || 0;
        this.users = usersRes.data || [];
      } catch (error) {
        console.error("Error fetching user data:", error);
        this.errorMessage = this.getErrorMessage(error, "Unable to load user management data.");
      } finally {
        this.loading = false;
      }
    },
    async runAction(request, successMessage) {
      this.errorMessage = "";

      try {
        const response = await request();
        alert((response.data && response.data.message) || successMessage);
        await this.fetchData();
      } catch (error) {
        console.error("Admin action failed:", error);
        alert(this.getErrorMessage(error, "The requested admin action failed."));
      }
    },
    async blockUser(id) {
      if (!confirm("Are you sure you want to block this user?")) return;
      await this.runAction(() => axios.put(`/api/AdminApi/BlockUser?id=${id}`), "User blocked successfully.");
    },
    async unblockUser(id) {
      if (!confirm("Are you sure you want to unblock this user?")) return;
      await this.runAction(() => axios.put(`/api/AdminApi/UnblockUser?id=${id}`), "User unblocked successfully.");
    },
    async makeAdmin(id) {
      if (!confirm("Make this user an admin?")) return;
      await this.runAction(() => axios.put(`/api/AdminApi/MakeAdmin?id=${id}`), "User promoted to admin.");
    },
    async removeAdmin(id) {
      if (!confirm("Remove admin privileges from this user?")) return;
      await this.runAction(() => axios.put(`/api/AdminApi/RemoveAdmin?id=${id}`), "Admin privileges removed.");
    }
  },
  mounted() {
    this.fetchData();
  }
};
</script>

<style scoped>
.admin-page {
  min-height: 100vh;
  background: linear-gradient(135deg, #f3f7ff, #ffffff);
  padding: 40px 60px;
  font-family: "Segoe UI", sans-serif;
  color: #243246;
}

.admin-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 20px;
  background: linear-gradient(90deg, #0d47a1, #1976d2);
  color: #ffffff;
  padding: 22px 32px;
  border-radius: 16px;
  box-shadow: 0 4px 15px rgba(13, 71, 161, 0.3);
}

.header-copy {
  display: grid;
  gap: 6px;
}

.title {
  margin: 0;
  font-size: 1.9rem;
  font-weight: 700;
}

.subtitle {
  margin: 0;
  font-size: 0.96rem;
  opacity: 0.9;
}

.reload-btn {
  min-width: 120px;
  border: 2px solid #ffffff;
  background: #ffffff;
  color: #0d47a1;
  font-weight: 700;
  padding: 10px 18px;
  border-radius: 10px;
  cursor: pointer;
  transition: all 0.2s ease;
}

.reload-btn:hover:not(:disabled) {
  background: #1565c0;
  border-color: #1565c0;
  color: #ffffff;
}

.reload-btn:disabled {
  opacity: 0.65;
  cursor: wait;
}

.summary-grid {
  display: grid;
  grid-template-columns: repeat(3, minmax(0, 1fr));
  gap: 18px;
  margin-top: 24px;
}

.summary-card {
  display: grid;
  gap: 10px;
  text-align: center;
  background: #e3f2fd;
  color: #0d47a1;
  padding: 18px 20px;
  border-radius: 14px;
}

.count {
  font-size: 1.9rem;
  font-weight: 800;
}

.error-banner {
  margin-top: 20px;
  padding: 14px 16px;
  border-radius: 12px;
  background: #fff2f0;
  color: #c62828;
  font-weight: 600;
}

.table-container {
  margin-top: 24px;
  background: #ffffff;
  border-radius: 16px;
  overflow: hidden;
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
  vertical-align: middle;
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

.status-badge,
.role-badge,
.super-admin-badge {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  min-height: 34px;
  padding: 0 12px;
  border-radius: 999px;
  font-size: 0.85rem;
  font-weight: 700;
}

.status-badge.active {
  background: #e8f5e9;
  color: #2e7d32;
}

.status-badge.blocked {
  background: #ffebee;
  color: #c62828;
}

.role-badge.primary-admin,
.super-admin-badge {
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

.action-group {
  display: flex;
  flex-wrap: wrap;
  justify-content: center;
  gap: 8px;
}

.action-btn {
  border: 0;
  border-radius: 8px;
  padding: 8px 14px;
  font-size: 0.9rem;
  font-weight: 700;
  cursor: pointer;
  transition: all 0.2s ease;
}

.action-btn:disabled {
  opacity: 0.65;
  cursor: wait;
}

.block-btn {
  background: #ffebee;
  color: #c62828;
}

.block-btn:hover:not(:disabled) {
  background: #c62828;
  color: #ffffff;
}

.unblock-btn {
  background: #e8f5e9;
  color: #2e7d32;
}

.unblock-btn:hover:not(:disabled) {
  background: #2e7d32;
  color: #ffffff;
}

.promote-btn {
  background: #fff8e1;
  color: #ef6c00;
}

.promote-btn:hover:not(:disabled) {
  background: #ef6c00;
  color: #ffffff;
}

.demote-btn {
  background: #fbe9e7;
  color: #d84315;
}

.demote-btn:hover:not(:disabled) {
  background: #d84315;
  color: #ffffff;
}

.no-data {
  color: #607d8b;
  font-style: italic;
}

.loading-overlay {
  position: fixed;
  inset: 0;
  background: rgba(255, 255, 255, 0.82);
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  gap: 12px;
  color: #1565c0;
  font-weight: 700;
}

.spinner {
  width: 44px;
  height: 44px;
  border: 4px solid #dbeafe;
  border-top-color: #1565c0;
  border-radius: 50%;
  animation: spin 1s linear infinite;
}

@keyframes spin {
  from {
    transform: rotate(0deg);
  }

  to {
    transform: rotate(360deg);
  }
}

@media (max-width: 960px) {
  .admin-page {
    padding: 26px 18px;
  }

  .admin-header {
    flex-direction: column;
    align-items: stretch;
  }

  .summary-grid {
    grid-template-columns: 1fr;
  }
}
</style>
