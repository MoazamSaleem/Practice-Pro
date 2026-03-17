<template>
  <div class="d-flex justify-content-center align-items-center vh-100">
    <div class="col-md-6">
      <div class="card w-100">
        <div class="card-body">
          <h5 class="card-title text-center mb-3 fw-bold text-primary">Forgot Password</h5>
          <p class="text-muted text-center mb-4">
            Enter your registered email address below and we'll send you instructions to reset your password.
          </p>
          <input type="text" class="form-control mb-2" placeholder="Email" v-model="obj.Email">
          <div class="text-danger mt-1" v-if="emailError">{{ emailError }}</div>
          <button type="button" class="btn btn-primary btn-block" v-on:click="validateForm">Submit</button>
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
        },
        token: '',
        emailError: '',
      }
    },
    created() {
      const urlParams = new URLSearchParams(window.location.search);
      this.obj.email = urlParams.get('email');
      this.token = urlParams.get('token');
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

        // Prevent form submission if there are errors
        if (this.emailError || this.passwordError) {
          event.preventDefault();
          return;
        }

        this.isLoading = true;
        this.forgotPassword();
      },
      forgotPassword() {
        axios.post('/api/AccountApi/forgot', {
          obj: this.obj
        })
          .then(response => {
            if (response.data.status) {
              //alert("Redirecting to reset password page...");
              alert("A password reset link has been sent. Please check your email.");
              window.location.href = "/Account/Login";
            } else {
              console.log('Password reset failed.');
            }
          })
          .catch(error => {
            console.error('Error occurred:', error);
          });
      }
    }
  };
</script>
<style>
</style>

