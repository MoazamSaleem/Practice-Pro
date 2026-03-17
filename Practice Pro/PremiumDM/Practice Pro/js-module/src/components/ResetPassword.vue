<template>
  <div class="d-flex justify-content-center align-items-center vh-100">
    <div class="col-md-6">
      <div class="card w-100">
        <div class="card-body">
          <h5 class="card-title text-center mb-3 fw-bold text-primary">Reset Password</h5>
          <p class="text-muted text-center mb-4">
            Enter your New password or Confirm Password.
          </p>
          <input type="hidden" v-model="obj.email" />
          <input type="hidden" v-model="obj.Token" />
          <input type="password" class="form-control mb-2" placeholder="New Password" v-model="obj.password">
          <div class="text-danger mt-1" v-if="passwordError">{{ passwordError }}</div>
          <input type="password" class="form-control mb-2" placeholder="Confirm Password" v-model="obj.confirmPassword">
          <div class="text-danger mt-1" v-if="confirmPasswordError">{{ confirmPasswordError }}</div>
          <button type="submit" class="btn btn-primary btn-block" v-on:click="validateForm">Reset</button>
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
          email: '',
          Token: '',
          password: '',
          confirmPassword: '',
        },
        passwordError: "",
        confirmPasswordError: "",
      }
    },
    created() {
      const urlParams = new URLSearchParams(window.location.search);
      this.obj.email = urlParams.get('email');
      this.obj.Token = urlParams.get('token');
    },
    methods: {
      validateForm(event) {
        this.passwordError = '';

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

        // Prevent form submission if there are errors
        if (this.emailError || this.passwordError) {
          event.preventDefault();
          return;
        }

        this.isLoading = true;
        this.resetPassword();
      },
      resetPassword() {
        axios.post('/api/AccountApi/reset', {
          obj: this.obj
        })
          .then(response => {
            if (response.data.status === true) {
              alert("Password reset successfully");
              window.location.href = "/Account/Login";
            } else {
              alert("Password reset failed");
            }
          })
          .catch(error => {
            console.error('Error occurred during password reset:', error);
            alert("An error occurred during password reset");
          });
      }



    }

  };</script>
<style>
</style>
