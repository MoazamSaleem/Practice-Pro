<template>
  <div>
    <h1>Email has been confirmed</h1>
  </div>
</template>
<script>
  import axios from 'axios'
  export default {
    data() {
      return {
        userId: '',
        token : '',
      }
    },
    created() {
      const urlParams = new URLSearchParams(window.location.search);
      this.userId = urlParams.get('userId');
      this.token = urlParams.get('token');
      this.pageload();
    },
    methods: {
      pageload() {
        axios.get('/Account/ConfirmEmail', {
          params: {
            userId: this.userId,
            token: this.token
          }
        })
          .then(function (res) {
            if (res.data.status === true) {
              alert("Email is confirmed");
            } else {
              alert("Email not confirmed");
            }
          })
          .catch(function (error) {
            alert("An error occurred while confirming email");
          });
      
    },

  }

  };

</script>
<style></style>
