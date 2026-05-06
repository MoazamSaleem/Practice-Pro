<template>
  <div class="legacy-dash">
    <div class="legacy-dash-head">
      <h1>Dashboard</h1>
      <select class="legacy-dash-user" disabled>
        <option>{{ userEmail || "Current account" }}</option>
      </select>
    </div>

    <div class="legacy-kpi-grid">
      <article class="legacy-kpi">
        <strong class="kpi-value blue">{{ accountDue60 }}</strong>
        <p class="kpi-label blue">Account Due within 60 days</p>
      </article>
      <article class="legacy-kpi">
        <strong class="kpi-value amber">{{ accountDue30 }}</strong>
        <p class="kpi-label amber">Account Due within 30 days</p>
      </article>
      <article class="legacy-kpi">
        <strong class="kpi-value red">{{ accountOverdue }}</strong>
        <p class="kpi-label red">Account Due or Overdue</p>
      </article>
      <article class="legacy-kpi">
        <strong class="kpi-value blue">{{ confirmationDue60 }}</strong>
        <p class="kpi-label blue">Confirmation Statement Due within 60 days</p>
      </article>
      <article class="legacy-kpi">
        <strong class="kpi-value amber">{{ confirmationDue30 }}</strong>
        <p class="kpi-label amber">Confirmation Statement Due within 30 days</p>
      </article>
      <article class="legacy-kpi">
        <strong class="kpi-value red">{{ confirmationOverdue }}</strong>
        <p class="kpi-label red">Confirmation Statement Due or Overdue</p>
      </article>
    </div>

    <article class="legacy-chart-panel">
      <div class="legacy-chart-wrap">
        <BarChart :chart-data="chartData" :chart-options="chartOptions" />
      </div>
    </article>
  </div>
</template>

<script>
import axios from "axios";
import { Bar } from "vue-chartjs";

export default {
  components: {
    BarChart: {
      extends: Bar,
      props: ["chartData", "chartOptions"],
      watch: {
        chartData: {
          handler(newData) {
            if (newData) {
              this.renderChart(newData, this.chartOptions);
            }
          },
          deep: true
        }
      },
      mounted() {
        if (this.chartData) {
          this.renderChart(this.chartData, this.chartOptions);
        }
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
      ivDue60: 0,
      ivDue30: 0,
      ivOverdue: 0,
      chartScope: "all",
      userEmail: "",
      isLoading: false,
      isSendingEmails: false,
      chartOptions: {
        responsive: true,
        maintainAspectRatio: false,
        legend: {
          display: false
        },
        tooltips: {
          displayColors: false,
          backgroundColor: "#241530",
          titleFontColor: "#ffffff",
          bodyFontColor: "#ffffff"
        },
        scales: {
          yAxes: [{
            ticks: {
              beginAtZero: true,
              precision: 0,
              fontColor: "#756583",
              padding: 10
            },
            gridLines: {
              color: "rgba(115, 91, 137, 0.12)",
              drawBorder: false
            }
          }],
          xAxes: [{
            ticks: {
              fontColor: "#756583"
            },
            gridLines: {
              display: false,
              drawBorder: false
            }
          }]
        }
      }
    };
  },
  computed: {
    totalCompanies() {
      return this.lst.length;
    },
    userInitial() {
      const source = (this.userEmail || "User").trim();
      return source.charAt(0).toUpperCase() || "U";
    },
    topCards() {
      return [
        this.buildMetricCard("Accounts due in 60 days", this.accountDue60, "fas fa-clock", "positive", "12,60 46,60 80,28 114,18", [{ x: 80, y: 28 }, { x: 114, y: 18 }]),
        this.buildMetricCard("Accounts due in 30 days", this.accountDue30, "fa-solid fa-coins", "danger", "12,18 46,36 80,30 114,56", [{ x: 46, y: 36 }, { x: 80, y: 30 }]),
        this.buildMetricCard("Accounts overdue", this.accountOverdue, "fas fa-exclamation-circle text-danger", "danger", "12,50 46,40 80,28 114,28", [{ x: 80, y: 28 }, { x: 114, y: 28 }])
      ];
    },
    statementCards() {
      return [
        this.buildMetricCard("CS01 due in 60 days", this.confirmationDue60, "fa-regular fa-file-lines", "violet", "12,58 46,58 80,38 114,20", [{ x: 80, y: 38 }, { x: 114, y: 20 }]),
        this.buildMetricCard("CS01 due in 30 days", this.confirmationDue30, "fa-solid fa-file-circle-check", "danger", "12,20 46,32 80,24 114,54", [{ x: 46, y: 32 }, { x: 80, y: 24 }]),
        this.buildMetricCard("CS01 overdue", this.confirmationOverdue, "fa-solid fa-circle-exclamation", "danger", "12,50 46,42 80,26 114,22", [{ x: 80, y: 26 }, { x: 114, y: 22 }])
      ];
    },
    ivCards() {
      return [
        this.buildMetricCard("IV due in 60 days", this.ivDue60, "fa-solid fa-passport", "positive", "12,50 46,30 80,45 114,15", [{ x: 46, y: 30 }, { x: 114, y: 15 }]),
        this.buildMetricCard("IV due in 30 days", this.ivDue30, "fa-solid fa-id-card-clip", "danger", "12,60 46,50 80,20 114,50", [{ x: 80, y: 20 }, { x: 114, y: 50 }]),
        this.buildMetricCard("IV overdue", this.ivOverdue, "fa-solid fa-fingerprint", "danger", "12,40 46,40 80,10 114,10", [{ x: 80, y: 10 }, { x: 114, y: 10 }])
      ];
    },
    summaryBlocks() {
      const accountsAttention = this.accountDue30 + this.accountOverdue;
      const cs01Attention = this.confirmationDue30 + this.confirmationOverdue;
      return [
        {
          title: "Accounts attention",
          percent: this.metricPercent(accountsAttention),
          ratio: `${accountsAttention}/${this.totalCompanies || 0} companies`,
          primaryLabel: "60 day window",
          primaryValue: this.accountDue60,
          secondaryLabel: "30d + overdue",
          secondaryValue: accountsAttention,
          tone: "amber"
        },
        {
          title: "CS01 attention",
          percent: this.metricPercent(cs01Attention),
          ratio: `${cs01Attention}/${this.totalCompanies || 0} companies`,
          primaryLabel: "60 day window",
          primaryValue: this.confirmationDue60,
          secondaryLabel: "30d + overdue",
          secondaryValue: cs01Attention,
          tone: "violet"
        },
        {
          title: "IV attention",
          percent: this.metricPercent(this.ivDue30 + this.ivOverdue),
          ratio: `${this.ivDue30 + this.ivOverdue}/${this.totalCompanies || 0} companies`,
          primaryLabel: "60 day window",
          primaryValue: this.ivDue60,
          secondaryLabel: "30d + overdue",
          secondaryValue: this.ivDue30 + this.ivOverdue,
          tone: "positive"
        }
      ];
    },
    chartData() {
      return {
        labels: ["Account Due 60", "Account Due 30", "Account Overdue", "Confirmation Due 60", "Confirmation Due 30", "Confirmation Overdue"],
        datasets: [{
          label: "Due Counts",
          backgroundColor: ["#19b3c6", "#f89b00", "#fb4033", "#d6d6d6", "#d6d6d6", "#d6d6d6"],
          borderColor: ["#19b3c6", "#f89b00", "#fb4033", "#d6d6d6", "#d6d6d6", "#d6d6d6"],
          borderWidth: 1,
          data: [this.accountDue60, this.accountDue30, this.accountOverdue, this.confirmationDue60, this.confirmationDue30, this.confirmationOverdue]
        }]
      };
    },
    reviewCompanies() {
      const mapped = this.lst
        .map((item) => {
          const dates = [
            this.getDueMeta(item.accountsD || item.AccountsD, "Accounts due"),
            this.getDueMeta(item.cS01D || item.CS01D, "Confirmation statement due"),
            this.getDueMeta(item.identityVerificationD || item.IdentityVerificationD, "Identity verification due")
          ].filter(Boolean);
          if (!dates.length) {
            return null;
          }
          dates.sort((a, b) => a.days - b.days);
          const nextDue = dates[0];
          return {
            key: `${item.id || item.number || item.name}-${nextDue.type}`,
            name: item.name || item.Name || "Unnamed company",
            number: item.number || item.Number || "No number",
            dueType: nextDue.type,
            date: nextDue.display,
            days: nextDue.days,
            tone: this.getAlertTone(nextDue.days),
            label: this.getAlertLabel(nextDue.days),
            link: item.number || item.Number ? `https://find-and-update.company-information.service.gov.uk/company/${item.number || item.Number}` : "#"
          };
        })
        .filter(Boolean)
        .sort((a, b) => a.days - b.days);
      const urgent = mapped.filter((item) => item.days <= 60);
      return (urgent.length ? urgent : mapped).slice(0, 4);
    }
  },
  methods: {
    handleCompanyDataSynced() {
      this.fetchData();
    },
    buildMetricCard(title, value, icon, tone, sparkline, sparkDots) {
      const palette = {
        positive: { stroke: "#2fbd76", dotFill: "#f2fff8", trendIcon: "fa-solid fa-arrow-trend-up" },
        danger: { stroke: "#ff6557", dotFill: "#fff4f2", trendIcon: "fa-solid fa-arrow-trend-down" },
        violet: { stroke: "#8d52c8", dotFill: "#faf3ff", trendIcon: "fa-solid fa-arrow-trend-up" }
      };
      const theme = palette[tone] || palette.positive;
      return {
        title,
        value,
        icon,
        tone,
        trend: `${this.metricPercent(value)}%`,
        meta: "vs portfolio total",
        sparkline,
        sparkDots,
        stroke: theme.stroke,
        dotFill: theme.dotFill,
        trendIcon: theme.trendIcon
      };
    },
    async fetchData() {
      this.isLoading = true;
      try {
        const res = await axios.post("/api/ClientApi/Vlist");
        if (res.data.status === true) {
          this.lst = res.data.lst || [];
          this.calculateCounters();
        }
      } catch (error) {
        console.error("Error fetching data:", error);
      } finally {
        this.isLoading = false;
      }
    },
    refreshDashboard() {
      this.fetchData();
    },
    calculateCounters() {
      const today = this.getStartOfDay(new Date());
      const withinRange = (value, minExclusive, maxInclusive) => value !== null && value > minExclusive && value <= maxInclusive;
      
      this.accountDue60 = this.lst.filter((item) => withinRange(this.getDaysDifference(item.accountsD || item.AccountsD || item.Accountsd, today), 30, 60)).length;
      this.accountDue30 = this.lst.filter((item) => withinRange(this.getDaysDifference(item.accountsD || item.AccountsD || item.Accountsd, today), 0, 30)).length;
      this.accountOverdue = this.lst.filter((item) => {
        const diff = this.getDaysDifference(item.accountsD || item.AccountsD || item.Accountsd, today);
        return diff !== null && diff <= 0;
      }).length;

      this.confirmationDue60 = this.lst.filter((item) => withinRange(this.getDaysDifference(item.cS01D || item.CS01D, today), 30, 60)).length;
      this.confirmationDue30 = this.lst.filter((item) => withinRange(this.getDaysDifference(item.cS01D || item.CS01D, today), 0, 30)).length;
      this.confirmationOverdue = this.lst.filter((item) => {
        const diff = this.getDaysDifference(item.cS01D || item.CS01D, today);
        return diff !== null && diff <= 0;
      }).length;

      this.ivDue60 = this.lst.filter((item) => withinRange(this.getDaysDifference(item.identityVerificationD || item.IdentityVerificationD, today), 30, 60)).length;
      this.ivDue30 = this.lst.filter((item) => withinRange(this.getDaysDifference(item.identityVerificationD || item.IdentityVerificationD, today), 0, 30)).length;
      this.ivOverdue = this.lst.filter((item) => {
        const diff = this.getDaysDifference(item.identityVerificationD || item.IdentityVerificationD, today);
        return diff !== null && diff <= 0;
      }).length;
    },
    getStartOfDay(date) {
      const normalized = new Date(date);
      normalized.setHours(0, 0, 0, 0);
      return normalized;
    },
    getDaysDifference(date, today = this.getStartOfDay(new Date())) {
      if (!date) return null;
      let dueDate = this.getStartOfDay(date);
      
      // Fallback for formats that new Date() might struggle with (like DD-MMM-YYYY from AddCompany.vue)
      if (Number.isNaN(dueDate.getTime()) && typeof date === 'string' && date.includes('-')) {
        const parts = date.split('-');
        if (parts.length === 3) {
          const months = { jan: 0, feb: 1, mar: 2, apr: 3, may: 4, jun: 5, jul: 6, aug: 7, sep: 8, oct: 9, nov: 10, dec: 11 };
          const d = parseInt(parts[0], 10);
          const m = months[parts[1].toLowerCase()];
          const y = parseInt(parts[2], 10);
          if (!isNaN(d) && m !== undefined && !isNaN(y)) {
             dueDate = new Date(y, m, d);
          }
        }
      }

      if (Number.isNaN(dueDate.getTime())) return null;
      return Math.round((dueDate - today) / (1000 * 60 * 60 * 24));
    },
    metricPercent(value) {
      if (!this.totalCompanies) return 0;
      return Math.round((value / this.totalCompanies) * 100);
    },
    formatNumericDate(date) {
      const dueDate = new Date(date);
      if (Number.isNaN(dueDate.getTime())) return "No date";
      return dueDate.toLocaleDateString("en-GB", { day: "2-digit", month: "2-digit", year: "numeric" });
    },
    getDueMeta(date, type) {
      const days = this.getDaysDifference(date);
      if (days === null) return null;
      return { type, days, display: this.formatNumericDate(date) };
    },
    getAlertTone(days) {
      if (days <= 0) return "danger";
      if (days <= 30) return "amber";
      return "info";
    },
    getAlertLabel(days) {
      if (days < 0) return `${Math.abs(days)}d overdue`;
      if (days === 0) return "Due today";
      if (days <= 30) return `${days}d left`;
      return "Upcoming";
    },
    async sendDueEmails() {
      if (this.isSendingEmails) return;
      this.isSendingEmails = true;
      try {
        const res = await axios.post("/api/HomeApi/SendDueEmails");
        alert(res.data.message);
      } catch (error) {
        console.error("Error sending emails:", error);
        alert("Failed to send emails.");
      } finally {
        this.isSendingEmails = false;
      }
    },
    fetchUserEmail() {
      axios.get("/api/HomeApi/GetCurrentUserEmail")
        .then((res) => {
          if (res.data.email) {
            this.userEmail = res.data.email;
          }
        })
        .catch((error) => {
          console.error("Error fetching user email:", error);
        });
    }
  },
  mounted() {
    this.fetchData();
    this.fetchUserEmail();
    window.addEventListener("premiumdm:company-data-synced", this.handleCompanyDataSynced);
  },
  beforeDestroy() {
    window.removeEventListener("premiumdm:company-data-synced", this.handleCompanyDataSynced);
  }
};
</script>

<style scoped>
.legacy-dash { max-width: 1080px; margin: 0 auto; }
.legacy-dash-head { display:flex; justify-content:space-between; gap:12px; align-items:center; margin-bottom:20px; }
.legacy-dash-head h1 { margin:0; font-size:36px; }
.legacy-dash-user { min-width:280px; padding:8px 10px; border:1px solid #d4d4d4; background:#fff; border-radius:4px; }
.legacy-kpi-grid { display:grid; grid-template-columns:repeat(3,minmax(0,1fr)); gap:16px; }
.legacy-kpi { background:#fff; border:1px solid #d8d8d8; border-radius:6px; padding:18px; text-align:center; }
.kpi-value { display:block; font-size:44px; line-height:1; }
.kpi-label { margin:10px 0 0; font-size:18px; }
.blue { color:#2f6fce; }
.amber { color:#e5af3a; }
.red { color:#c15a67; }
.legacy-chart-panel { margin-top:32px; background:#fff; border:1px solid #d8d8d8; border-radius:6px; padding:24px; }
.legacy-chart-wrap { min-height:380px; }
@media (max-width: 900px) { .legacy-kpi-grid { grid-template-columns:repeat(2,minmax(0,1fr)); } }
@media (max-width: 640px) { .legacy-dash-head { flex-direction:column; align-items:flex-start; } .legacy-dash-user { min-width:100%; } .legacy-kpi-grid { grid-template-columns:1fr; } }
</style>

