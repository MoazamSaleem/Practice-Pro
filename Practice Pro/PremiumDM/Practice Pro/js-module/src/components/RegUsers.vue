<template>
  <div class="admin-page">
    <!-- 🌟 Header -->
    <div class="admin-header">
      <div class="header-left">
        <h2 class="title">👑 Admin User Management</h2>
        <p class="subtitle">Manage and monitor registered users efficiently</p>
      </div>
      <button class="reload-btn" @click="fetchData">🔄 Refresh</button>
    </div>

    <!-- 📊 Summary -->
    <div class="summary-card shadow-sm">
      <strong>Total Registered Users:</strong>
      <span class="count">{{ totalUsers }}</span>
    </div>

    <!-- 📋 Table -->
    <div class="table-container shadow-sm">
      <table class="styled-table">
        <thead>
          <tr>
            <th>Email</th>
            <th>Username</th>
            <th>Status</th>
            <th>Role</th>
            <th>Action</th>
          </tr>
        </thead>

        <tbody>
          <tr v-for="u in users" :key="u.id">
            <td>{{ u.email }}</td>
            <td>{{ u.userName }}</td>

            <td>
              <span class="status-badge" :class="u.lockoutEnd ? 'blocked' : 'active'">
                {{ u.lockoutEnd ? 'Blocked' : 'Active' }}
              </span>
            </td>

            <td>
              <span class="role-badge" :class="u.isAdmin ? 'admin' : 'user'">
                {{ u.isAdmin ? 'Admin' : 'User' }}
              </span>
            </td>

            <td>
              <!-- 🧱 Block/Unblock (cannot block super admin) -->
              <button v-if="!u.lockoutEnd && u.email !== mainAdmin"
                      @click="blockUser(u.id)"
                      class="action-btn block-btn">
                🚫 Block
              </button>
              <button v-else-if="u.lockoutEnd && u.email !== mainAdmin"
                      @click="unblockUser(u.id)"
                      class="action-btn unblock-btn">
                ✅ Unblock
              </button>

              <!-- 👑 Make or Remove Admin -->
              <button v-if="!u.isAdmin && u.email !== mainAdmin"
                      @click="makeAdmin(u.id)"
                      class="action-btn promote-btn">
                ⭐ Make Admin
              </button>

              <button v-else-if="u.isAdmin && u.email !== mainAdmin"
                      @click="removeAdmin(u.id)"
                      class="action-btn demote-btn">
                🗑 Remove Admin
              </button>

              <!-- 🛡 Super admin tag -->
              <span v-if="u.email === mainAdmin"
                    class="super-admin-badge"
                    title="Super Admin">
                👑 Admin
              </span>
            </td>
          </tr>

          <tr v-if="users.length === 0">
            <td colspan="5" class="no-data">No users found.</td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- ⏳ Loading -->
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
        mainAdmin: "awaisshahbaz480@gmail.com", // ✅ Your hardcoded main admin
      };
    },
    methods: {
      async fetchData() {
        try {
          this.loading = true;
          const [totalRes, usersRes] = await Promise.all([
            axios.get("/api/AdminApi/GetTotalUsers"),
            axios.get("/api/AdminApi/GetAllUsers"),
          ]);
          this.totalUsers = totalRes.data.total;
          this.users = usersRes.data;
        } catch (error) {
          console.error("Error fetching user data:", error);
        } finally {
          this.loading = false;
        }
      },

      async blockUser(id) {
        if (confirm("Are you sure you want to block this user?")) {
          try {
            await axios.put(`/api/AdminApi/BlockUser?id=${id}`);
            alert("User blocked successfully!");
            this.fetchData();
          } catch (error) {
            console.error("Error blocking user:", error);
            alert("Failed to block user.");
          }
        }
      },

      async unblockUser(id) {
        if (confirm("Are you sure you want to unblock this user?")) {
          try {
            await axios.put(`/api/AdminApi/UnblockUser?id=${id}`);
            alert("User unblocked successfully!");
            this.fetchData();
          } catch (error) {
            console.error("Error unblocking user:", error);
            alert("Failed to unblock user.");
          }
        }
      },

      async makeAdmin(id) {
        if (confirm("Make this user an Admin?")) {
          try {
            await axios.put(`/api/AdminApi/MakeAdmin?id=${id}`);
            alert("User promoted to Admin!");
            this.fetchData();
          } catch (error) {
            console.error("Error making admin:", error);
            alert("Failed to make admin.");
          }
        }
      },

      async removeAdmin(id) {
        if (confirm("Remove Admin privileges from this user?")) {
          try {
            await axios.put(`/api/AdminApi/RemoveAdmin?id=${id}`);
            alert("Admin privileges removed!");
            this.fetchData();
          } catch (error) {
            console.error("Error removing admin:", error);
            alert("Failed to remove admin.");
          }
        }
      },
    },
    mounted() {
      this.fetchData();
    },
  };
</script>

<style scoped>
  /* 🌐 Layout */
  .admin-page {
    min-height: 100vh;
    background: linear-gradient(135deg, #f3f7ff, #ffffff);
    padding: 40px 60px;
    font-family: "Inter", "Segoe UI", sans-serif;
    color: #333;
  }

  /* 🩵 Header */
  .admin-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    background: linear-gradient(90deg, #0d47a1, #1976d2);
    color: white;
    padding: 20px 40px;
    border-radius: 14px;
    box-shadow: 0 4px 15px rgba(13, 71, 161, 0.3);
    margin-bottom: 30px;
    animation: fadeInDown 0.8s ease;
  }

  .title {
    font-weight: 700;
    font-size: 1.8rem;
    margin: 0;
  }

  .subtitle {
    font-size: 0.9rem;
    opacity: 0.9;
  }

  /* 🔄 Reload Button */
  .reload-btn {
    background-color: white;
    color: #0d47a1;
    border: 2px solid #0d47a1;
    font-weight: 600;
    padding: 8px 18px;
    border-radius: 8px;
    cursor: pointer;
    transition: all 0.3s ease;
  }

    .reload-btn:hover {
      background-color: #1565c0;
      color: white;
      transform: scale(1.05);
    }

  /* 📊 Summary Card */
  .summary-card {
    text-align: center;
    background: #e3f2fd;
    color: #0d47a1;
    font-weight: 600;
    font-size: 1.1rem;
    padding: 15px 20px;
    border-radius: 12px;
    margin-bottom: 25px;
  }

    .summary-card .count {
      font-size: 1.4rem;
      font-weight: 700;
      color: #0d47a1;
      margin-left: 10px;
    }

  /* 📋 Table */
  .table-container {
    background: white;
    border-radius: 16px;
    overflow: hidden;
    animation: fadeIn 1s ease;
  }

  .styled-table {
    width: 100%;
    border-collapse: separate;
    border-spacing: 0;
    font-size: 0.95rem;
  }

    .styled-table thead tr {
      background: linear-gradient(90deg, #1565c0, #1976d2);
      color: white;
      text-align: center;
      font-weight: 600;
    }

    .styled-table th,
    .styled-table td {
      padding: 14px 16px;
      text-align: center;
      vertical-align: middle;
    }

    .styled-table tbody tr {
      transition: background-color 0.3s ease, transform 0.2s ease;
    }

      .styled-table tbody tr:nth-child(even) {
        background-color: #f9fbff;
      }

      .styled-table tbody tr:hover {
        background-color: #e3f2fd;
        transform: scale(1.01);
      }

  /* 🟢 Status Badges */
  .status-badge {
    display: inline-block;
    padding: 6px 14px;
    border-radius: 12px;
    font-weight: 600;
    font-size: 0.85rem;
    box-shadow: 0 2px 6px rgba(0, 0, 0, 0.1);
  }

    .status-badge.active {
      background: #4caf50;
      color: white;
    }

    .status-badge.blocked {
      background: #e53935;
      color: white;
    }

  /* 🏷 Role Badges */
  .role-badge {
    display: inline-block;
    padding: 6px 14px;
    border-radius: 12px;
    font-weight: 600;
    font-size: 0.85rem;
  }

    .role-badge.admin {
      background: #ffb300;
      color: #fff;
    }

    .role-badge.user {
      background: #90a4ae;
      color: #fff;
    }

  /* 🎯 Buttons */
  .action-btn {
    border: none;
    border-radius: 8px;
    padding: 6px 14px;
    font-size: 0.9rem;
    font-weight: 600;
    cursor: pointer;
    transition: all 0.3s ease;
    box-shadow: 0 2px 6px rgba(0, 0, 0, 0.1);
    margin: 3px;
  }

  .block-btn {
    background: #ffebee;
    color: #d32f2f;
  }

    .block-btn:hover {
      background: #d32f2f;
      color: white;
      transform: scale(1.05);
    }

  .unblock-btn {
    background: #e8f5e9;
    color: #2e7d32;
  }

    .unblock-btn:hover {
      background: #2e7d32;
      color: white;
      transform: scale(1.05);
    }

  .promote-btn {
    background: #fff8e1;
    color: #f57c00;
  }

    .promote-btn:hover {
      background: #f57c00;
      color: white;
      transform: scale(1.05);
    }

  .demote-btn {
    background: #fbe9e7;
    color: #d84315;
  }

    .demote-btn:hover {
      background: #d84315;
      color: white;
      transform: scale(1.05);
    }

  /* 👑 Super Admin Badge */
  .super-admin-badge {
    background: #1565c0;
    color: white;
    font-weight: 700;
    padding: 6px 10px;
    border-radius: 10px;
  }

  /* ⏳ Loading */
  .loading-overlay {
    position: fixed;
    inset: 0;
    background: rgba(255, 255, 255, 0.8);
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    font-weight: 600;
    font-size: 1rem;
    color: #1565c0;
  }

  .spinner {
    border: 4px solid #e3f2fd;
    border-top: 4px solid #1565c0;
    border-radius: 50%;
    width: 45px;
    height: 45px;
    animation: spin 1s linear infinite;
    margin-bottom: 10px;
  }

  /* 🕹️ Animations */
  @keyframes spin {
    0% {
      transform: rotate(0deg);
    }

    100% {
      transform: rotate(360deg);
    }
  }

  @keyframes fadeIn {
    from {
      opacity: 0;
      transform: translateY(15px);
    }

    to {
      opacity: 1;
      transform: translateY(0);
    }
  }

  @keyframes fadeInDown {
    from {
      opacity: 0;
      transform: translateY(-20px);
    }

    to {
      opacity: 1;
      transform: translateY(0);
    }
  }

  /* 🚫 No Data */
  .no-data {
    text-align: center;
    font-style: italic;
    color: #757575;
  }
</style>
