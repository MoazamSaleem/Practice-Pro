import Vue from 'vue'

// Disable production tip
Vue.config.productionTip = false

// Global Components
Vue.component('signup-page', require('./components/SignupPage.vue').default);
Vue.component('login-page', require('./components/LoginPage.vue').default);
Vue.component('add-company', require('./components/AddCompany.vue').default);
Vue.component('clientview', require('./components/ClientView.vue').default);
Vue.component('dashboard-header', require('./components/DashboardHeader.vue').default);
Vue.component('dashboard', require('./components/Dashboard.vue').default);
Vue.component('forget-password', require('./components/ForgetPassword.vue').default);
Vue.component('reset-password', require('./components/ResetPassword.vue').default);
Vue.component('confirm-email-page', require('./components/ConfirmEmailPage.vue').default);
Vue.component('total-users', require('./components/RegUsers.vue').default);
Vue.component('user-activity', require('./components/RegisteredUsersActivity.vue').default);

// ✅ Google Analytics - manual page view tracking for SPA
if (window.gtag) {
  // Track first page load
  window.gtag('config', 'G-C83CGLH9NR', { page_path: window.location.pathname });
}

// Watch for hash or URL changes if you use hash routing
window.addEventListener('hashchange', () => {
  if (window.gtag) {
    window.gtag('config', 'G-C83CGLH9NR', { page_path: window.location.hash });
  }
});

// Create Vue app
new Vue({
  el: '#app',
})
