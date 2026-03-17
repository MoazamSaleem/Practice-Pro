<template>
  <section class="auth-shell">
    <div class="auth-frame">
      <aside class="auth-side">
        <span class="auth-kicker">Secure accountant portal</span>
        <h1>Sign in and stay ahead of every filing deadline.</h1>
        <p class="auth-side-copy">
          Access your dashboard, monitor due dates, and keep every Companies House
          submission on track from one calm workspace.
        </p>

        <ul class="auth-points">
          <li><i class="fa-solid fa-circle-check"></i><span>Real-time deadline visibility</span></li>
          <li><i class="fa-solid fa-circle-check"></i><span>Secure cloud access for your team</span></li>
          <li><i class="fa-solid fa-circle-check"></i><span>Fast onboarding and practical support</span></li>
        </ul>

        <div class="auth-side-grid">
          <article class="auth-side-card">
            <strong>100+</strong>
            <span>Active firms and teams</span>
          </article>
          <article class="auth-side-card">
            <strong>One place</strong>
            <span>For every company deadline</span>
          </article>
        </div>
      </aside>

      <div class="auth-main">
        <div class="auth-main-inner">
          <div class="auth-branding">
            <img src="/finanza-1.0.0/img/logo.jpg" alt="Premium Deadline Management" class="auth-logo">
            <div>
              <span class="auth-badge">Welcome back</span>
              <h2>Log in to PremiumDM</h2>
              <p>Use your account details to continue.</p>
            </div>
          </div>

          <form class="auth-form" @submit.prevent="validateForm">
            <div class="field">
              <label for="login-email">Email address</label>
              <input id="login-email" type="email" placeholder="name@company.co.uk" v-model="obj.Email" required>
              <div class="error-text" v-if="emailError">{{ emailError }}</div>
            </div>

            <div class="field">
              <label for="login-password">Password</label>
              <div class="field-control">
                <input
                  id="login-password"
                  :type="showPassword ? 'text' : 'password'"
                  placeholder="Enter your password"
                  v-model="obj.Password"
                  required
                >
                <button type="button" class="field-toggle" @click="togglePassword" aria-label="Toggle password visibility">
                  <i :class="showPassword ? 'fa-solid fa-eye-slash' : 'fa-solid fa-eye'"></i>
                </button>
              </div>
              <div class="error-text" v-if="passwordError">{{ passwordError }}</div>
            </div>

            <div class="auth-row">
              <label class="auth-check">
                <input type="checkbox" v-model="obj.RememberMe">
                <span>Remember me</span>
              </label>
              <button type="button" class="auth-link" @click="forgotPassword">Forgot password?</button>
            </div>

            <button type="submit" class="auth-submit" :disabled="isLoading">
              <span v-if="!isLoading">Log in</span>
              <span v-else class="auth-loading">
                <span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>
                Logging in...
              </span>
            </button>
          </form>

          <div class="auth-footer">
            <span>New to PremiumDM?</span>
            <a href="/Account/SignUp">Create an account</a>
          </div>
        </div>
      </div>
    </div>

    <div v-if="showProgressModal" class="auth-overlay">
      <div class="auth-progress">
        <h3>Updating company data</h3>
        <div class="progress auth-progress-bar">
          <div
            class="progress-bar progress-bar-striped progress-bar-animated"
            role="progressbar"
            :style="{ width: progress + '%' }"
            :aria-valuenow="progress"
            aria-valuemin="0"
            aria-valuemax="100"
          >
            {{ progress }}%
          </div>
        </div>
        <p>{{ progressMessage }}</p>
      </div>
    </div>
  </section>
</template>

<script>
import axios from "axios";

export default {
  data() {
    return {
      obj: {
        Email: "",
        Password: "",
        RememberMe: false
      },
      emailError: "",
      passwordError: "",
      isLoading: false,
      progress: 0,
      progressMessage: "Starting update...",
      showProgressModal: false,
      progressInterval: null,
      showPassword: false
    };
  },
  methods: {
    togglePassword() {
      this.showPassword = !this.showPassword;
    },
    validateForm() {
      this.emailError = "";
      this.passwordError = "";

      const emailPattern = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
      if (!this.obj.Email) {
        this.emailError = "Email field is required.";
      } else if (!emailPattern.test(this.obj.Email)) {
        this.emailError = "Please enter a valid email address (e.g., user@example.com).";
      }

      if (!this.obj.Password) {
        this.passwordError = "Password field is required.";
      } else if (this.obj.Password.length < 6) {
        this.passwordError = "Password must be at least 6 characters long.";
      } else if (!/[A-Z]/.test(this.obj.Password)) {
        this.passwordError = "Password must contain at least one uppercase letter.";
      }

      if (this.emailError || this.passwordError) {
        return;
      }

      this.isLoading = true;
      this.signin();
    },
    signin() {
      axios.post("/api/AccountApi/login", { obj: this.obj })
        .then((res) => {
          if (res.data.status) {
            this.showProgressModal = true;
            this.reloadAllCompanies();
          } else {
            this.isLoading = false;
            alert("Login Failed");
          }
        })
        .catch((error) => {
          this.isLoading = false;
          alert("Login failed");
          console.error("Error during login:", error);
        });
    },
    forgotPassword() {
      window.location.href = "/Account/ForgotPassword";
    },
    reloadAllCompanies() {
      this.progress = 0;
      this.progressMessage = "Connecting to Companies House...";
      this.startProgressUpdates();

      axios.post("/api/ClientApi/UpdateAllCompaniesSync")
        .then((response) => {
          if (response.data.status) {
            this.updateProgress(100, "Update complete!");
            setTimeout(() => {
              this.showProgressModal = false;
              this.stopProgressUpdates();
              window.location.href = "/DashBoard";
            }, 1000);
          } else {
            this.updateProgress(0, "Update failed");
            setTimeout(() => {
              this.showProgressModal = false;
              this.stopProgressUpdates();
              alert("Error during company update");
            }, 1000);
          }
        })
        .catch((error) => {
          this.updateProgress(0, "Update failed");
          setTimeout(() => {
            this.showProgressModal = false;
            this.stopProgressUpdates();
            console.error("Error during company update:", error);
          }, 1000);
        });
    },
    startProgressUpdates() {
      this.progressInterval = setInterval(() => {
        if (this.progress < 90) {
          this.progress += 10;
          this.progressMessage = "Updating companies (" + this.progress + "%)...";
        }
      }, 1000);
    },
    stopProgressUpdates() {
      if (this.progressInterval) {
        clearInterval(this.progressInterval);
        this.progressInterval = null;
      }
    },
    updateProgress(value, message) {
      this.progress = value;
      this.progressMessage = message;
      if (value >= 100) {
        this.isLoading = false;
      }
    }
  },
  beforeDestroy() {
    this.stopProgressUpdates();
  }
};
</script>

<style scoped>
.auth-shell {
  padding: 32px 0 64px;
  background:
    radial-gradient(circle at top left, rgba(23, 105, 230, 0.12), transparent 34%),
    linear-gradient(180deg, #eef4ff 0%, #f7faff 100%);
}

.auth-frame {
  width: min(1180px, calc(100% - 36px));
  margin: 0 auto;
  display: grid;
  grid-template-columns: minmax(300px, 0.95fr) minmax(320px, 1.05fr);
  border: 1px solid #d8e4fb;
  border-radius: 28px;
  overflow: hidden;
  background: #ffffff;
  box-shadow: 0 30px 80px rgba(22, 56, 122, 0.12);
}

.auth-side {
  padding: 46px 40px;
  background: linear-gradient(160deg, #0f4fae 0%, #1769e6 58%, #3d8cff 100%);
  color: #ffffff;
}

.auth-kicker {
  display: inline-flex;
  align-items: center;
  min-height: 44px;
  padding: 0 18px;
  border-radius: 999px;
  border: 1px solid rgba(255, 255, 255, 0.22);
  background: rgba(255, 255, 255, 0.12);
  font-size: 15px;
  font-weight: 700;
}

.auth-side h1 {
  margin: 26px 0 0;
  color: #ffffff;
  font-size: clamp(30px, 3.3vw, 40px);
  line-height: 1.08;
  letter-spacing: -0.03em;
}

.auth-side-copy {
  margin: 24px 0 0;
  max-width: 540px;
  color: rgba(255, 255, 255, 0.92);
  font-size: 17px;
  line-height: 1.6;
}

.auth-points {
  list-style: none;
  margin: 32px 0 0;
  padding: 0;
  display: grid;
  gap: 14px;
}

.auth-points li {
  display: flex;
  align-items: center;
  gap: 12px;
  font-size: 16px;
  font-weight: 700;
}

.auth-points i {
  color: #a5f1c7;
}

.auth-side-grid {
  margin-top: 34px;
  display: grid;
  grid-template-columns: repeat(2, minmax(0, 1fr));
  gap: 14px;
}

.auth-side-card {
  border-radius: 22px;
  padding: 20px 18px;
  border: 1px solid rgba(255, 255, 255, 0.18);
  background: rgba(7, 30, 78, 0.16);
  backdrop-filter: blur(8px);
}

.auth-side-card strong {
  display: block;
  color: #ffffff;
  font-size: 24px;
  line-height: 1.1;
}

.auth-side-card span {
  display: block;
  margin-top: 8px;
  color: rgba(255, 255, 255, 0.84);
  font-size: 15px;
  line-height: 1.5;
}

.auth-main {
  display: flex;
  align-items: center;
  background: #fbfcff;
}

.auth-main-inner {
  width: 100%;
  padding: 42px 40px;
}

.auth-branding {
  display: grid;
  grid-template-columns: auto 1fr;
  align-items: center;
  gap: 18px;
}

.auth-logo {
  width: 92px;
  height: 92px;
  object-fit: contain;
  border-radius: 22px;
  border: 1px solid #dde7fa;
  background: #ffffff;
  padding: 12px;
}

.auth-badge {
  display: inline-flex;
  align-items: center;
  min-height: 34px;
  padding: 0 14px;
  border-radius: 999px;
  background: #e9f1ff;
  color: #1658c3;
  font-size: 13px;
  font-weight: 800;
  letter-spacing: 0.02em;
  text-transform: uppercase;
}

.auth-branding h2 {
  margin: 14px 0 0;
  color: #102041;
  font-size: clamp(28px, 2.6vw, 36px);
  line-height: 1.1;
  letter-spacing: -0.03em;
}

.auth-branding p {
  margin: 10px 0 0;
  color: #5a6784;
  font-size: 16px;
}

.auth-form {
  margin-top: 34px;
}

.field + .field {
  margin-top: 18px;
}

.field label {
  display: block;
  margin-bottom: 9px;
  color: #26395f;
  font-size: 13px;
  font-weight: 800;
  letter-spacing: 0.04em;
  text-transform: uppercase;
}

.field input {
  width: 100%;
  min-height: 58px;
  border: 1px solid #cddaf0;
  border-radius: 16px;
  background: #ffffff;
  color: #11203d;
  font-size: 16px;
  padding: 0 20px;
  outline: none;
  transition: border-color 0.2s ease, box-shadow 0.2s ease;
}

.field input:focus {
  border-color: #1769e6;
  box-shadow: 0 0 0 4px rgba(23, 105, 230, 0.12);
}

.field-control {
  position: relative;
}

.field-control input {
  padding-right: 56px;
}

.field-toggle {
  position: absolute;
  top: 50%;
  right: 16px;
  transform: translateY(-50%);
  width: 36px;
  height: 36px;
  border: 0;
  background: transparent;
  color: #5970a0;
  cursor: pointer;
}

.error-text {
  margin-top: 8px;
  color: #d93025;
  font-size: 14px;
  font-weight: 600;
}

.auth-row {
  margin-top: 18px;
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 14px;
  flex-wrap: wrap;
}

.auth-check {
  display: inline-flex;
  align-items: center;
  gap: 10px;
  color: #415578;
  font-size: 15px;
  font-weight: 600;
}

.auth-check input {
  width: 18px;
  height: 18px;
}

.auth-link {
  border: 0;
  padding: 0;
  background: transparent;
  color: #1769e6;
  font-size: 15px;
  font-weight: 700;
  cursor: pointer;
}

.auth-submit {
  width: 100%;
  min-height: 58px;
  margin-top: 26px;
  border: 0;
  border-radius: 999px;
  background: linear-gradient(90deg, #1769e6 0%, #338cff 100%);
  color: #ffffff;
  font-size: 18px;
  font-weight: 800;
  letter-spacing: -0.01em;
  cursor: pointer;
  transition: transform 0.2s ease, box-shadow 0.2s ease;
  box-shadow: 0 18px 32px rgba(23, 105, 230, 0.22);
}

.auth-submit:hover {
  transform: translateY(-1px);
}

.auth-submit:disabled {
  opacity: 0.75;
  cursor: wait;
}

.auth-loading {
  display: inline-flex;
  align-items: center;
  gap: 10px;
}

.auth-footer {
  margin-top: 22px;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 6px;
  flex-wrap: wrap;
  color: #64728f;
  font-size: 15px;
}

.auth-footer a {
  color: #1769e6;
  font-weight: 700;
  text-decoration: none;
}

.auth-overlay {
  position: fixed;
  inset: 0;
  z-index: 1050;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 20px;
  background: rgba(9, 18, 37, 0.42);
  backdrop-filter: blur(4px);
}

.auth-progress {
  width: min(460px, 100%);
  border-radius: 26px;
  padding: 28px 24px;
  background: #ffffff;
}

.auth-progress h3 {
  margin: 0;
  color: #132344;
  font-size: 24px;
}

.auth-progress-bar {
  height: 18px;
  margin-top: 18px;
  border-radius: 999px;
  overflow: hidden;
  background: #eaf1ff;
}

.auth-progress p {
  margin: 14px 0 0;
  color: #5a6784;
  font-size: 16px;
}

@media (max-width: 992px) {
  .auth-frame {
    grid-template-columns: 1fr;
  }

  .auth-side,
  .auth-main-inner {
    padding: 34px 28px;
  }
}

@media (max-width: 640px) {
  .auth-shell {
    padding: 28px 0 52px;
  }

  .auth-frame {
    width: calc(100% - 22px);
    border-radius: 24px;
  }

  .auth-side,
  .auth-main-inner {
    padding: 28px 20px;
  }

  .auth-side h1 {
    font-size: 34px;
  }

  .auth-side-copy,
  .auth-branding p {
    font-size: 16px;
  }

  .auth-side-grid {
    grid-template-columns: 1fr;
  }

  .auth-branding {
    grid-template-columns: 1fr;
  }

  .auth-logo {
    width: 84px;
    height: 84px;
  }

  .auth-branding h2 {
    font-size: 30px;
  }

  .field input {
    min-height: 54px;
    font-size: 16px;
  }

  .auth-submit {
    min-height: 54px;
    font-size: 18px;
  }
}
</style>
