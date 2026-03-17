<template>
  <div class="dashboard-page">
    <!-- Header -->
    <div class="dashboard-header">
      <div class="header-left">
        <h2 class="dashboard-title">👥 User Activity Dashboard</h2>
        <p class="subtitle">Monitor user engagement and recent activity</p>
      </div>

      <div class="header-right">
        <!-- 🌍 Time Zone Selector -->
        <div class="timezone-selector">
          <select id="timezone" v-model="selectedTimezone" @change="saveTimezone" class="timezone-dropdown">
            <option v-for="zone in timezones" :key="zone" :value="zone">{{ zone }}</option>
          </select>
        </div>

        <!-- 🔄 Reload Button -->
        <button class="reload-btn" @click="fetchUsers">🔄 Refresh</button>
      </div>
    </div>

    <!-- Loading -->
    <div v-if="loading" class="alert alert-info loading-box">
      Loading user activity...
    </div>

    <!-- Table -->
    <div v-else class="table-wrapper">
      <table class="styled-table">
        <thead>
          <tr>
            <th>Email</th>
            <th>Current Page</th>
            <th>Last Login</th>
            <th>Logins This Month</th>
            <th>Last Activity</th>
            <th>Status</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="user in users" :key="user.userId">
            <td>{{ user.userEmail }}</td>
            <td>{{ user.currentPage || 'N/A' }}</td>
            <td>{{ formatDate(user.lastLogin) }}</td>
            <td>{{ user.loginCountThisMonth }}</td>
            <td>{{ formatDate(user.lastActivityTime) }}</td>
            <td>
              <div class="status-indicator">
                <span :class="['dot', user.isActive ? 'active-dot' : 'inactive-dot']"></span>
                <span :class="['status-text', user.isActive ? 'text-active' : 'text-inactive']">
                  {{ user.isActive ? 'Active' : 'Inactive' }}
                </span>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script>
  export default {
    name: "RegisteredUsersActivity",
    data() {
      return {
        users: [],
        loading: true,
        selectedTimezone: "Asia/Karachi",
        timezones: [
          "Asia/Karachi",
          "Asia/Dubai",
          "Asia/Kolkata",
          "Europe/London",
          "Europe/Paris",
          "America/New_York",
          "America/Los_Angeles",
          "Australia/Sydney",
          "UTC",
        ],
      };
    },
    methods: {
      async fetchUsers() {
        this.loading = true;
        try {
          const response = await fetch("/api/AdminApi/GetUserActivities", {
            headers: {
              Authorization: `Bearer ${localStorage.getItem("token")}`,
            },
          });

          if (!response.ok) {
            if (response.status === 403) alert("Access Denied: Admins only.");
            throw new Error("Failed to fetch data");
          }

          this.users = await response.json();
        } catch (error) {
          console.error("Error fetching user activity:", error);
        } finally {
          this.loading = false;
        }
      },
      formatDate(date) {
        if (!date) return "N/A";
        const cleanDate = date.replace(" ", "T");
        const utcDate = new Date(cleanDate + "Z");
        return utcDate.toLocaleString("en-GB", {
          timeZone: this.selectedTimezone,
          year: "numeric",
          month: "short",
          day: "numeric",
          hour: "2-digit",
          minute: "2-digit",
          second: "2-digit",
          hour12: true,
        });
      },
      saveTimezone() {
        localStorage.setItem("selectedTimezone", this.selectedTimezone);
      },
    },
    mounted() {
      const savedZone = localStorage.getItem("selectedTimezone");
      if (savedZone) this.selectedTimezone = savedZone;
      this.fetchUsers();
    },
  };
</script>

<style scoped>
  /* 🌐 Layout */
  .dashboard-page {
    min-height: 100vh;
    background: linear-gradient(135deg, #f0f4ff, #ffffff);
    padding: 40px 60px;
    font-family: "Inter", "Segoe UI", sans-serif;
    color: #333;
  }

  /* 🩵 Header */
  .dashboard-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    background: rgba(25, 118, 210, 0.9);
    color: white;
    padding: 25px 40px;
    border-radius: 16px;
    box-shadow: 0 6px 25px rgba(25, 118, 210, 0.3);
    margin-bottom: 40px;
    backdrop-filter: blur(10px);
    animation: fadeInDown 0.8s ease;
  }

  .header-left {
    display: flex;
    flex-direction: column;
  }

  .header-right {
    display: flex;
    align-items: center;
    gap: 20px;
  }

  .dashboard-title {
    font-weight: 700;
    font-size: 1.9rem;
    margin: 0;
  }

  .subtitle {
    font-size: 0.95rem;
    opacity: 0.9;
    margin-top: 4px;
  }

  /* 🌍 Time Zone Selector */
  .timezone-selector {
    display: flex;
    align-items: center;
    gap: 10px;
  }

  .timezone-label {
    font-weight: 600;
    font-size: 0.9rem;
  }

  .timezone-dropdown {
    border: none;
    border-radius: 8px;
    padding: 8px 12px;
    font-size: 0.9rem;
    color: #1565c0;
    background-color: #ffffff;
    font-weight: 600;
    cursor: pointer;
    transition: all 0.3s ease;
    box-shadow: 0 2px 6px rgba(0, 0, 0, 0.1);
  }

    .timezone-dropdown:hover {
      background-color: #e3f2fd;
    }

  /* 🔄 Reload Button */
  .reload-btn {
    background: white;
    color: #1565c0;
    border: none;
    padding: 10px 22px;
    font-weight: 600;
    border-radius: 10px;
    cursor: pointer;
    transition: all 0.3s ease;
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.15);
  }

    .reload-btn:hover {
      background-color: #0d47a1;
      color: white;
      transform: scale(1.05);
    }

  /* 📋 Table */
  .table-wrapper {
    background: white;
    border-radius: 16px;
    box-shadow: 0 10px 25px rgba(0, 0, 0, 0.05);
    padding: 20px 25px;
    animation: fadeIn 1s ease;
    overflow-x: auto;
  }

  .styled-table {
    width: 100%;
    border-collapse: separate;
    border-spacing: 0;
    font-size: 0.95rem;
    border-radius: 16px;
    overflow: hidden;
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
    }

    .styled-table tbody tr {
      background-color: #fff;
      transition: background-color 0.3s ease, transform 0.2s ease;
    }

      .styled-table tbody tr:nth-child(even) {
        background-color: #f9fbff;
      }

      .styled-table tbody tr:hover {
        background-color: #e8f0fe;
        transform: scale(1.01);
        box-shadow: 0 4px 10px rgba(21, 101, 192, 0.15);
      }

  /* 🟢 Status Indicator */
  .status-indicator {
    display: flex;
    align-items: center;
    justify-content: center;
    gap: 6px;
  }

  .dot {
    width: 10px;
    height: 10px;
    border-radius: 50%;
  }

  .active-dot {
    background-color: #4caf50;
    box-shadow: 0 0 6px rgba(76, 175, 80, 0.5);
  }

  .inactive-dot {
    background-color: #9e9e9e;
  }

  .text-active {
    color: #4caf50;
    font-weight: 600;
  }

  .text-inactive {
    color: #757575;
  }

  /* 🔄 Loading */
  .loading-box {
    font-size: 1rem;
    border-radius: 10px;
    width: 60%;
    margin: 50px auto;
    background: rgba(25, 118, 210, 0.1);
    box-shadow: 0 4px 10px rgba(25, 118, 210, 0.1);
  }

  /* ✨ Animations */
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
</style>
