<template>
  <section class="auth-shell">
    <div class="auth-frame">
      <aside class="auth-side">
        <span class="auth-kicker">Set up your portal</span>
        <h1>Create your account and bring every deadline into one workflow.</h1>
        <p class="auth-side-copy">
          Start with a clear, secure space for managing Companies House obligations,
          client visibility, and day-to-day filing control.
        </p>

        <ul class="auth-points">
          <li><i class="fa-solid fa-circle-check"></i><span>Built for UK accountancy practices</span></li>
          <li><i class="fa-solid fa-circle-check"></i><span>Simple setup with a focused dashboard</span></li>
          <li><i class="fa-solid fa-circle-check"></i><span>Compliance tracking without clutter</span></li>
        </ul>

        <div class="auth-side-grid">
          <article class="auth-side-card">
            <strong>Quick start</strong>
            <span>Set up the full workflow for your firm</span>
          </article>
          <article class="auth-side-card">
            <strong>Secure</strong>
            <span>Protected access for your firm and team</span>
          </article>
        </div>
      </aside>

      <div class="auth-main">
        <div class="auth-main-inner">
          <div class="auth-branding">
            <img src="/finanza-1.0.0/img/logo.jpg" alt="Premium Deadline Management" class="auth-logo">
            <div>
              <span class="auth-badge">Create account</span>
              <h2>Sign up for PremiumDM</h2>
              <p>Enter your details below to create your portal access.</p>
            </div>
          </div>

          <form class="auth-form" @submit.prevent="signup">
            <div class="field">
              <label for="signup-email">Email address</label>
              <input id="signup-email" type="email" placeholder="name@company.co.uk" v-model="obj.email" required>
              <div class="error-text" v-if="emailError">{{ emailError }}</div>
            </div>

            <div class="field">
              <label for="signup-password">Password</label>
              <div class="field-control">
                <input
                  id="signup-password"
                  :type="showPassword ? 'text' : 'password'"
                  placeholder="Create a password"
                  v-model="obj.password"
                  required
                >
                <button type="button" class="field-toggle" @click="togglePassword" aria-label="Toggle password visibility">
                  <i :class="showPassword ? 'fa-solid fa-eye-slash' : 'fa-solid fa-eye'"></i>
                </button>
              </div>
              <div class="error-text" v-if="passwordError">{{ passwordError }}</div>
            </div>

            <div class="field">
              <label for="signup-confirm">Confirm password</label>
              <div class="field-control">
                <input
                  id="signup-confirm"
                  :type="showConfirmPassword ? 'text' : 'password'"
                  placeholder="Re-enter your password"
                  v-model="obj.confirmPassword"
                  required
                >
                <button type="button" class="field-toggle" @click="toggleConfirmPassword" aria-label="Toggle confirm password visibility">
                  <i :class="showConfirmPassword ? 'fa-solid fa-eye-slash' : 'fa-solid fa-eye'"></i>
                </button>
              </div>
              <div class="error-text" v-if="confirmPasswordError">{{ confirmPasswordError }}</div>
            </div>

            <button type="submit" class="auth-submit" :disabled="isLoading">
              <span v-if="!isLoading">Create account</span>
              <span v-else class="auth-loading">
                <span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>
                Creating account...
              </span>
            </button>
          </form>

          <div class="auth-footer">
            <span>Already have an account?</span>
            <a href="/Account/Login">Log in</a>
          </div>
        </div>
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
        email: "",
        password: "",
        confirmPassword: ""
      },
      emailError: "",
      passwordError: "",
      confirmPasswordError: "",
      isLoading: false,
      showPassword: false,
      showConfirmPassword: false
    };
  },
  methods: {
    togglePassword() {
      this.showPassword = !this.showPassword;
    },
    toggleConfirmPassword() {
      this.showConfirmPassword = !this.showConfirmPassword;
    },
    signup() {
      this.emailError = "";
      this.passwordError = "";
      this.confirmPasswordError = "";

      const emailPattern = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
      if (!this.obj.email) {
        this.emailError = "Email is required.";
      } else if (!emailPattern.test(this.obj.email)) {
        this.emailError = "Please enter a valid email address (e.g., user@example.com).";
      }

      if (!this.obj.password) {
        this.passwordError = "Password is required.";
      } else if (this.obj.password.length < 6) {
        this.passwordError = "Password must be at least 6 characters long.";
      } else if (!/[A-Z]/.test(this.obj.password)) {
        this.passwordError = "Password must contain at least one uppercase letter.";
      }

      if (!this.obj.confirmPassword) {
        this.confirmPasswordError = "Please confirm your password.";
      } else if (this.obj.password !== this.obj.confirmPassword) {
        this.confirmPasswordError = "Passwords do not match.";
      }

      if (this.emailError || this.passwordError || this.confirmPasswordError) {
        return;
      }

      this.isLoading = true;

      axios.post("/api/AccountApi/register", { obj: this.obj })
        .then((res) => {
          this.isLoading = false;
          if (res.data.status) {
            alert("Signup successful.");
            this.clear();
            window.location.href = "/Account/Login";
          } else {
            alert("User not created: " + res.data.message);
          }
        })
        .catch((err) => {
          this.isLoading = false;
          alert("Signup failed.");
          console.error("Signup error:", err);
        });
    },
    clear() {
      this.obj = {
        email: "",
        password: "",
        confirmPassword: ""
      };
    }
  }
};
</script>

<style scoped>
.auth-shell {
  padding: 32px 0 64px;
  background:
    radial-gradient(circle at top right, rgba(23, 105, 230, 0.12), transparent 34%),
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
  background: linear-gradient(160deg, #103a8f 0%, #1769e6 56%, #4b92ff 100%);
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
