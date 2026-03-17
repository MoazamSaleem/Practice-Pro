<!--<template>
  <div class="d-flex justify-content-center align-items-center vh-100">
    <div class="card shadow-lg p-4" style="max-width: 360px; width: 100%; border-radius: 15px;">
      <div class="text-center">
        <img src="/DevFolio/assets/img/logo.webp" alt="PremiumDM" style="width: 230px; height: 60px; margin-right: 10px;">
      </div>
      <div>
        <p class="text-center text-muted mb-4">Sign in to start your session</p>-->

        <!-- Email Input -->
        <!--<div class="mb-3">
          <input type="email" class="form-control" placeholder="Email" v-model="obj.Email" required />
          <div class="text-danger mt-1" v-if="emailError">{{ emailError }}</div>
        </div>-->

        <!-- Password Input -->
        <!--<div class="mb-3">
          <input type="password" class="form-control" placeholder="Password" v-model="obj.Password" required />
          <div class="text-danger mt-1" v-if="passwordError">{{ passwordError }}</div>
        </div>-->

        <!-- Remember Me -->
        <!--<div class="mb-3">
          <input type="checkbox" name="obj.RememberMe" />Remember Me
        </div>-->

        <!-- Login Button -->
        <!--<div class="d-grid">
          <button type="submit" class="btn btn-primary btn-block rounded-pill py-2" v-on:click="validateForm">
            Login
          </button>
        </div>

        <div class="text-center mt-3">
          <p class="text-muted">Don't have an account yet? <a href="/Account/SignUp" class="text-primary">Create one now</a></p>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
  import axios from 'axios';
  export default {
    data() {
      return {
        obj: {
          Email: '',
          Password: '',
          RememberMe: false
        },
        emailError: '',
        passwordError: ''
      };
    },
    methods: {
      validateForm(event) {
        this.emailError = '';
        this.passwordError = '';

        // Email validation
        const emailPattern = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
        if (!this.obj.Email) {
          this.emailError = 'Email field is required.';
        } else if (!emailPattern.test(this.obj.Email)) {
          this.emailError = 'Please enter a valid email address (e.g., user@example.com).';
        }

        // Password validation
        if (!this.obj.Password) {
          this.passwordError = 'Password field is required.';
        } else if (this.obj.Password.length < 6) {
          this.passwordError = 'Password must be at least 6 characters long.';
        } else if (!/[A-Z]/.test(this.obj.Password)) {
          this.passwordError = 'Password must contain at least one uppercase letter.';
        }

        // Prevent form submission if there are errors
        if (this.emailError || this.passwordError) {
          event.preventDefault();
          return;
        }

        // Proceed with API call if validation passes
        this.signin();
      },
      signin() {
        axios.post('/api/AccountApi/login', { obj: this.obj })
          .then((res) => {
            if (res.data.status) {
              alert("Login successful");
              window.location.href = "/DashBoard";
              this.reloadAllCompanies()
            } else {
              alert("Login Failed");
            }
          })
          .catch((error) => {
            alert("Login failed");
            console.error('Error during login:', error);
          });
      },
    }
  };
</script>-->




<template>
  <div class="d-flex justify-content-center align-items-center vh-100">
    <div class="card shadow-lg p-4" style="max-width: 360px; width: 100%; border-radius: 15px;">
      <div class="text-center">
        <img src="/DevFolio/assets/img/logo.webp" alt="PremiumDM" style="width: 230px; height: 60px; margin-right: 10px;">
      </div>
      <div>
        <p class="text-center text-muted mb-4">Sign in to start your session</p>

        <!-- Email Input -->
        <div class="mb-3">
          <input type="email" class="form-control" placeholder="Email" v-model="obj.Email" required />
          <div class="text-danger mt-1" v-if="emailError">{{ emailError }}</div>
        </div>

        <!-- Password Input -->
        <div class="mb-3 position-relative">
          <input :type="showPassword ? 'text' : 'password'"
                 class="form-control"
                 placeholder="Password"
                 v-model="obj.Password"
                 required />

          <!-- Toggle Eye Button -->
          <span class="position-absolute"
                @click="togglePassword"
                style="top: 8px; right: 12px; cursor: pointer;">
            <i :class="showPassword ? 'fas fa-eye-slash' : 'fas fa-eye'"></i>
          </span>

          <div class="text-danger mt-1" v-if="passwordError">{{ passwordError }}</div>
        </div>

        <!-- Remember Me -->
        <div class="mb-3">
          <input type="checkbox" name="obj.RememberMe" />Remember Me
        </div>

        <!-- Login Button -->
        <div class="d-grid">
          <button type="submit" class="btn btn-primary btn-block rounded-pill py-2" v-on:click="validateForm" :disabled="isLoading">
            <span v-if="!isLoading">Login</span>
            <span v-else>
              <span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>
              Logging in...
            </span>
          </button>
        </div>

        <!-- Progress Modal (Vue-controlled) -->
        <div v-if="showProgressModal" class="modal-backdrop fade show"></div>
        <div v-if="showProgressModal" class="modal fade show d-block" tabindex="-1">
          <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
              <div class="modal-header">
                <h5 class="modal-title">Updating Company Data</h5>
              </div>
              <div class="modal-body">
                <div class="progress mb-3">
                  <div class="progress-bar progress-bar-striped progress-bar-animated"
                       role="progressbar"
                       :style="{ width: progress + '%' }"
                       :aria-valuenow="progress"
                       aria-valuemin="0"
                       aria-valuemax="100">
                    {{ progress }}%
                  </div>
                </div>
                <p class="text-center">{{ progressMessage }}</p>
              </div>
            </div>
          </div>
        </div>
        <div class="text-end mt-2">
          <button class="btn btn-link text-primary p-0" @click="forgotPassword">
            Forgot Password?
          </button>
        </div>

        <div class="text-center mt-3">
          <p class="text-muted">Don't have an account yet? <a href="/Account/SignUp" class="text-primary">Create one now</a></p>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
  import axios from 'axios';

  export default {
    data() {
      return {
        obj: {
          Email: '',
          Password: '',
          RememberMe: false
        },
        emailError: '',
        passwordError: '',
        isLoading: false,
        progress: 0,
        progressMessage: 'Starting update...',
        showProgressModal: false,
        progressInterval: null,
        showPassword: false,
      };
    },
    methods: {
      togglePassword() {
        this.showPassword = !this.showPassword;
      },
      validateForm(event) {
        this.emailError = '';
        this.passwordError = '';

        // Email validation
        const emailPattern = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
        if (!this.obj.Email) {
          this.emailError = 'Email field is required.';
        } else if (!emailPattern.test(this.obj.Email)) {
          this.emailError = 'Please enter a valid email address (e.g., user@example.com).';
        }

        // Password validation
        if (!this.obj.Password) {
          this.passwordError = 'Password field is required.';
        } else if (this.obj.Password.length < 6) {
          this.passwordError = 'Password must be at least 6 characters long.';
        } else if (!/[A-Z]/.test(this.obj.Password)) {
          this.passwordError = 'Password must contain at least one uppercase letter.';
        }

        // Prevent form submission if there are errors
        if (this.emailError || this.passwordError) {
          event.preventDefault();
          return;
        }

        this.isLoading = true;
        this.signin();
      },
      signin() {
        axios.post('/api/AccountApi/login', { obj: this.obj })
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
            console.error('Error during login:', error);
          });
      },

      forgotPassword() {
        window.location.href = "/Account/ForgotPassword";
      },

      reloadAllCompanies() {
        // Reset progress
        this.progress = 0;
        this.progressMessage = "Connecting to Companies House...";

        // Start progress updates
        this.startProgressUpdates();

        axios.post("/api/ClientApi/UpdateAllCompaniesSync")
          .then(response => {
            if (response.data.status) {
              this.updateProgress(100, "Update complete!");
              setTimeout(() => {
                this.showProgressModal = false;
                this.stopProgressUpdates();
                this.list(); // Refresh the table data
                window.location.href = "/DashBoard";
              }, 1000);
            } else {
              this.updateProgress(0, "Update failed");
              setTimeout(() => {
                this.showProgressModal = false;
                this.stopProgressUpdates();
                alert(`Error during company update`);
              }, 1000);
            }
          })
          .catch(error => {
            this.updateProgress(0, "Update failed");
            setTimeout(() => {
              this.showProgressModal = false;
              this.stopProgressUpdates();
              console.error('Error during company update:', error);
            }, 1000);
          });
      },

      startProgressUpdates() {
        // This is just for demonstration - replace with actual progress updates from your API
        this.progressInterval = setInterval(() => {
          if (this.progress < 90) {
            this.progress += 10;
            this.progressMessage = `Updating companies (${this.progress}%)...`;
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
      },

      list() {
        axios
          .post("/api/ClientApi/Vlist")
          .then((res) => {
            if (res.data.status === true) {
              this.lst = res.data.lst;
            }
          });
      }
    },
    beforeUnmount() {
      // Clean up interval when component is destroyed
      this.stopProgressUpdates();
    }
  };
</script>

<style scoped>
  .modal-backdrop {
    opacity: 0.5;
    background-color: #000;
    position: fixed;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    z-index: 1040;
  }

  .modal {
    position: fixed;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    z-index: 1050;
    overflow: hidden;
    outline: 0;
    display: none;
  }

    .modal.show {
      display: block;
      overflow-x: hidden;
      overflow-y: auto;
    }
</style>
