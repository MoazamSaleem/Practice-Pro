<template>
  <div class="d-flex justify-content-center align-items-center vh-100">
    <div class="card shadow-lg p-4" style="max-width: 360px; width: 100%; border-radius: 15px;">
      <div class="text-center">
        <img src="/DevFolio/assets/img/logo.webp" alt="PremiumDM" style="width: 230px; height: 60px; margin-right: 10px;">
      </div>
      <div>
        <p class="text-center text-muted mb-4">Sign up to start your session</p>

        <!-- Email Input -->
        <div class="mb-3">
          <input type="email" class="form-control" placeholder="Email" v-model="obj.email" required />
          <div class="text-danger mt-1" v-if="emailError">{{ emailError }}</div>
        </div>

        <!-- Password Input -->
        <div class="mb-3">
          <input type="password" class="form-control" placeholder="Password" v-model="obj.password" required />
          <div class="text-danger mt-1" v-if="passwordError">{{ passwordError }}</div>
        </div>

        <!-- Confirm Password Input -->
        <div class="mb-3">
          <input type="password" class="form-control" placeholder="Confirm Password" v-model="obj.confirmPassword" required />
          <div class="text-danger mt-1" v-if="confirmPasswordError">{{ confirmPasswordError }}</div>
        </div>

        <!-- Sign Up Button -->
        <div class="d-grid">
          <button type="submit" class="btn btn-primary btn-block rounded-pill py-2" v-on:click="signup">
            Sign up
          </button>
        </div>

        <!-- Alternative Sign In Link -->
        <div class="text-center mt-3">
          <p class="text-muted">Already have an account? <a href="/Account/Login" class="text-primary">Log In</a></p>
        </div>
      </div>
    </div>
  </div>
</template>
<script>
  import axios from 'axios'

  export default {
    data() {
      return {
        obj: {
          email: "",
          password: "",
          confirmPassword: "",
        },
        emailError: "",
        passwordError: "",
        confirmPasswordError: "",
      }
    },
    methods: {
      signup(event) {
        // Clear previous error messages
        this.emailError = "";
        this.passwordError = "";
        this.confirmPasswordError = "";

        // Email Validation (basic format for all email types)
        const emailPattern = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
        if (!this.obj.email) {
          this.emailError = "Email is required.";
        } else if (!emailPattern.test(this.obj.email)) {
          this.emailError = "Please enter a valid email address (e.g., user@example.com).";
        } else {
          this.emailError = ""; // Clear error if email is valid
        }

        // Password Validation
        if (!this.obj.password) {
          this.passwordError = "Password is required.";
        } else if (this.obj.password.length < 6) {
          this.passwordError = "Password must be at least 6 characters long.";
        } else if (!/[A-Z]/.test(this.obj.password)) {
          this.passwordError = "Password must contain at least one uppercase letter.";
        }

        // Confirm Password Validation
        if (!this.obj.confirmPassword) {
          this.confirmPasswordError = "Please confirm your password.";
        } else if (this.obj.password !== this.obj.confirmPassword) {
          this.confirmPasswordError = "Passwords do not match.";
        }

        // Prevent submission if there are validation errors
        if (this.emailError || this.passwordError || this.confirmPasswordError) {
          event.preventDefault();
          return;
        }

        // Proceed with API call if validation passes
        axios.post('/api/AccountApi/register', { obj: this.obj })
          .then(res => {
            if (res.data.status) {
              alert("Signup successful.");
              this.clear();
              window.location.href = "/Account/Login";
            } else {
              alert("User not created: " + res.data.message);
            }
          })
          .catch(err => {
            console.error("Signup error:", err);
          });
      },
      clear() {
        this.obj = {
          email: "",
          password: "",
          confirmPassword: "",
        };
      }
    }
  }
</script>
