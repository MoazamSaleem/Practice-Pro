<template>
  <div class="container mt-4">
    <div class="d-flex flex-wrap justify-content-between align-items-center mb-4">
      <h4 class="mb-2 mb-md-0">Dashboard</h4>
      <select class="form-select w-auto">
        <option :value="User">{{ userEmail }}</option>
      </select>
    </div>
    <div class="row">
      <!-- Account Due Cards -->
      <div class="col-md-4">
        <div class="card shadow-sm text-center">
          <div class="card-body">
            <h3 class="text-primary">{{ accountDue60 }}</h3>
            <p class="text-primary">Account Due within 60 days</p>
          </div>
        </div>
      </div>

      <div class="col-md-4">
        <div class="card shadow-sm text-center">
          <div class="card-body">
            <h3 class="text-warning">{{ accountDue30 }}</h3>
            <p class="text-warning">Account Due within 30 days</p>
          </div>
        </div>
      </div>

      <div class="col-md-4">
        <div class="card shadow-sm text-center">
          <div class="card-body">
            <h3 class="text-danger">{{ accountOverdue }}</h3>
            <p class="text-danger">Account Due or Overdue</p>
          </div>
        </div>
      </div>
    </div>

    <div class="row mt-3">
      <!-- Confirmation Statement Due Cards -->
      <div class="col-md-4">
        <div class="card shadow-sm text-center">
          <div class="card-body">
            <h3 class="text-primary">{{ confirmationDue60 }}</h3>
            <p class="text-primary">Confirmation Statement Due within 60 days</p>
          </div>
        </div>
      </div>

      <div class="col-md-4">
        <div class="card shadow-sm text-center">
          <div class="card-body">
            <h3 class="text-warning">{{ confirmationDue30 }}</h3>
            <p class="text-warning">Confirmation Statement Due within 30 days</p>
          </div>
        </div>
      </div>

      <div class="col-md-4">
        <div class="card shadow-sm text-center">
          <div class="card-body">
            <h3 class="text-danger">{{ confirmationOverdue }}</h3>
            <p class="text-danger">Confirmation Statement Due or Overdue</p>
          </div>
        </div>
      </div>
    </div>

    <div class="mt-5">
      <div class="card shadow-sm p-xxl-5">
        <div class="card-body">
          <BarChart :chart-data="chartData" :chart-options="chartOptions" />
        </div>
      </div>
    </div>

  </div>
</template>

<script>
  import axios from "axios";
  import { Bar } from 'vue-chartjs';

  export default {
    components: {
      BarChart: {
        extends: Bar,
        props: ['chartData', 'chartOptions'],
        watch: {
          chartData: {
            handler(newData) {
              this.renderChart(newData, this.chartOptions);
            },
            deep: true
          }
        },
        mounted() {
          this.renderChart(this.chartData, this.chartOptions);
        }
      }
    },
    data() {
      return {
        lst: [],
        accountDue60: 0,
        accountDue30: 0,
        accountOverdue: 0,
        confirmationDue60: 0,
        confirmationDue30: 0,
        confirmationOverdue: 0,
        chartData: null,
        chartOptions: {
          responsive: true,
          maintainAspectRatio: false
        },
        userEmail: "",
      };
    },
    methods: {
      async fetchData() {
        try {
          const res = await axios.post("/api/ClientApi/Vlist");
          if (res.data.status === true) {
            this.lst = res.data.lst || [];
            this.calculateCounters();
            this.updateChartData();
          }
        } catch (error) {
          console.error("Error fetching data:", error);
        }
      },
      calculateCounters() {
        const today = new Date();
        this.accountDue60 = this.lst.filter(item => this.getDaysDifference(item.accountsD, today) <= 60 && this.getDaysDifference(item.accountsD, today) > 30).length;
        this.accountDue30 = this.lst.filter(item => this.getDaysDifference(item.accountsD, today) <= 30 && this.getDaysDifference(item.accountsD, today) > 0).length;
        this.accountOverdue = this.lst.filter(item => this.getDaysDifference(item.accountsD, today) < 0).length;
        this.confirmationDue60 = this.lst.filter(item => this.getDaysDifference(item.dCreation, today) <= 60 && this.getDaysDifference(item.dCreation, today) > 30).length;
        this.confirmationDue30 = this.lst.filter(item => this.getDaysDifference(item.dCreation, today) <= 30 && this.getDaysDifference(item.dCreation, today) > 0).length;
        this.confirmationOverdue = this.lst.filter(item => this.getDaysDifference(item.dCreation, today) < 0).length;
      },
      updateChartData() {
        this.chartData = {
          labels: ['Account Due 60', 'Account Due 30', 'Account Overdue', 'Confirmation Due 60', 'Confirmation Due 30', 'Confirmation Overdue'],
          datasets: [
            {
              label: 'Due Counts',
              backgroundColor: ['#00bcd4', '#ff9800', '#f44336', '#00bcd4', '#ff9800', '#f44336'],
              data: [
                this.accountDue60,
                this.accountDue30,
                this.accountOverdue,
                this.confirmationDue60,
                this.confirmationDue30,
                this.confirmationOverdue
              ]
            }
          ]
        };
      },
      getDaysDifference(date, today) {
        if (!date) return null;
        const dueDate = new Date(date);
        if (isNaN(dueDate.getTime())) return null;
        return Math.floor((dueDate - today) / (1000 * 60 * 60 * 24));
      },
      async sendDueEmails() {
        try {
          const res = await axios.post("/api/HomeApi/SendDueEmails");
          alert(res.data.message);
        } catch (error) {
          console.error("Error sending emails:", error);
          alert("Failed to send emails.");
        }
      },
      fetchUserEmail() {
        axios.get("/api/HomeApi/GetCurrentUserEmail")
          .then((res) => {
            // Check if response contains the email
            if (res.data.email) {
              this.userEmail = res.data.email;
            } else {
              console.error("User email not found in response.");
            }
          })
          .catch((error) => {
            console.error("Error fetching user email:", error);
          });
      },
    },
    mounted() {
      this.fetchData();
      this.fetchUserEmail();
    }
  };
</script>

