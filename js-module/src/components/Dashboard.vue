<template>
  <div>
    <div class="dash-page-head">
      <div class="dash-page-copy-block">
        <p class="dash-breadcrumb">Home / User Portal</p>
        <h1>Deadline dashboard</h1>
        <p class="dash-page-copy">Monitor account filings, confirmation statements, and urgent company deadlines from one structured view.</p>
      </div>
      <div class="dash-account-pill">
        <i class="fa-solid fa-user"></i>
        <span>{{ userEmail || "Current account" }}</span>
      </div>
    </div>
    
    <div class="dash-cards">
      <article v-for="card in statementCards" :key="card.title" class="dash-card" :class="`is-${card.tone}`">
        <div class="dash-card-topline">
          <div class="dash-card-head">
            <span class="dash-card-icon"><i :class="card.icon"></i></span>
            <h2>{{ card.title }}</h2>
          </div>
          <button type="button" class="dash-more" tabindex="-1" aria-hidden="true"><i class="fa-solid fa-ellipsis-vertical"></i></button>
        </div>

        <div class="dash-card-bottom">
          <div>
            <strong class="dash-card-value">{{ card.value }}</strong>
            <p class="dash-card-trend" :class="`is-${card.tone}`">
              <i :class="card.trendIcon"></i>
              <span>{{ card.trend }}</span>
              <small>{{ card.meta }}</small>
            </p>
          </div>

          <svg class="dash-sparkline" viewBox="0 0 128 76" aria-hidden="true">
            <polyline :points="card.sparkline" fill="none" :stroke="card.stroke" stroke-width="3" stroke-linecap="round" stroke-linejoin="round"></polyline>
            <circle v-for="(point, index) in card.sparkDots" :key="card.title + index" :cx="point.x" :cy="point.y" r="5.5" :fill="card.dotFill" :stroke="card.stroke" stroke-width="2"></circle>
          </svg>
        </div>
      </article>
    </div>
        <div class="dash-cards">
          <article v-for="card in topCards" :key="card.title" class="dash-card" :class="`is-${card.tone}`">
            <div class="dash-card-topline">
              <div class="dash-card-head">
                <span class="dash-card-icon"><i :class="card.icon"></i></span>
                <h2>{{ card.title }}</h2>
              </div>
              <button type="button" class="dash-more" tabindex="-1" aria-hidden="true"><i class="fa-solid fa-ellipsis-vertical"></i></button>
            </div>

            <div class="dash-card-bottom">
              <div>
                <strong class="dash-card-value">{{ card.value }}</strong>
                <p class="dash-card-trend" :class="`is-${card.tone}`">
                  <i :class="card.trendIcon"></i>
                  <span>{{ card.trend }}</span>
                  <small>{{ card.meta }}</small>
                </p>
              </div>

              <svg class="dash-sparkline" viewBox="0 0 128 76" aria-hidden="true">
                <polyline :points="card.sparkline" fill="none" :stroke="card.stroke" stroke-width="3" stroke-linecap="round" stroke-linejoin="round"></polyline>
                <circle v-for="(point, index) in card.sparkDots" :key="card.title + index" :cx="point.x" :cy="point.y" r="5.5" :fill="card.dotFill" :stroke="card.stroke" stroke-width="2"></circle>
              </svg>
            </div>
          </article>
        </div>

        <div class="dash-main-grid">
          <article class="dash-panel">
            <div class="dash-panel-head">
              <h3>Deadline Summary</h3>
              <button type="button" class="dash-more" tabindex="-1" aria-hidden="true"><i class="fa-solid fa-ellipsis-vertical"></i></button>
            </div>

            <div class="dash-summary-list">
              <div class="dash-summary-block" v-for="block in summaryBlocks" :key="block.title">
                <div class="dash-summary-top">
                  <div>
                    <span>{{ block.title }}</span>
                    <strong>{{ block.percent }}%</strong>
                  </div>
                  <small>{{ block.ratio }}</small>
                </div>
                <div class="dash-progress-track">
                  <span :class="`is-${block.tone}`" :style="{ width: block.percent + '%' }"></span>
                </div>
                <div class="dash-summary-meta">
                  <span>{{ block.primaryLabel }}: {{ block.primaryValue }}</span>
                  <span>{{ block.secondaryLabel }}: {{ block.secondaryValue }}</span>
                </div>
              </div>
            </div>
          </article>

          <article class="dash-panel dash-panel-chart">
            <div class="dash-panel-head dash-panel-head-chart">
              <h3>Deadline Distribution</h3>
              <div class="dash-segment" role="tablist" aria-label="Chart scope">
                <button type="button" class="dash-segment-btn" :class="{ 'is-active': chartScope === 'all' }" @click="chartScope = 'all'">All</button>
                <button type="button" class="dash-segment-btn" :class="{ 'is-active': chartScope === 'accounts' }" @click="chartScope = 'accounts'">Accounts</button>
                <button type="button" class="dash-segment-btn" :class="{ 'is-active': chartScope === 'cs01' }" @click="chartScope = 'cs01'">CS01</button>
                <button type="button" class="dash-segment-btn" :class="{ 'is-active': chartScope === 'iv' }" @click="chartScope = 'iv'">ID Verification</button>
              </div>
            </div>

            <div class="dash-chart-wrap">
              <BarChart :chart-data="chartData" :chart-options="chartOptions" :key="chartScope" />
            </div>
          </article>

          <article class="dash-panel dash-panel-review">
            <div class="dash-panel-head">
              <h3>Review Deadlines</h3>
              <button type="button" class="dash-more" tabindex="-1" aria-hidden="true"><i class="fa-solid fa-ellipsis-vertical"></i></button>
            </div>

            <div v-if="reviewCompanies.length" class="dash-review-table">
              <a v-for="item in reviewCompanies" :key="item.key" class="dash-review-row" :href="item.link" target="_blank" rel="noreferrer">
                <div class="dash-review-dateblock">
                  <strong>{{ item.date }}</strong>
                  <span>{{ item.number }}</span>
                </div>
                <div class="dash-review-company">
                  <strong>{{ item.name }}</strong>
                  <span>{{ item.dueType }}</span>
                </div>
                <div class="dash-review-status">
                  <span class="dash-status" :class="`is-${item.tone}`">{{ item.label }}</span>
                </div>
              </a>
            </div>
            <div v-else class="dash-empty">No companies are loaded for review yet.</div>
          </article>
        </div>

        <div class="dash-cards dash-cards-secondary">
          <article v-for="card in ivCards" :key="card.title" class="dash-card" :class="`is-${card.tone}`">
            <div class="dash-card-topline">
              <div class="dash-card-head">
                <span class="dash-card-icon"><i :class="card.icon"></i></span>
                <h2>{{ card.title }}</h2>
              </div>
              <button type="button" class="dash-more" tabindex="-1" aria-hidden="true"><i class="fa-solid fa-ellipsis-vertical"></i></button>
            </div>

            <div class="dash-card-bottom">
              <div>
                <strong class="dash-card-value">{{ card.value }}</strong>
                <p class="dash-card-trend" :class="`is-${card.tone}`">
                  <i :class="card.trendIcon"></i>
                  <span>{{ card.trend }}</span>
                  <small>{{ card.meta }}</small>
                </p>
              </div>

              <svg class="dash-sparkline" viewBox="0 0 128 76" aria-hidden="true">
                <polyline :points="card.sparkline" fill="none" :stroke="card.stroke" stroke-width="3" stroke-linecap="round" stroke-linejoin="round"></polyline>
                <circle v-for="(point, index) in card.sparkDots" :key="card.title + index" :cx="point.x" :cy="point.y" r="5.5" :fill="card.dotFill" :stroke="card.stroke" stroke-width="2"></circle>
              </svg>
            </div>
          </article>
        </div>
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
      if (this.chartScope === "accounts") {
        return {
          labels: ["Due in 60 days", "Due in 30 days", "Overdue"],
          datasets: [{
            label: "Accounts",
            backgroundColor: ["#b38add", "#9a64d0", "#7d3fbc"],
            borderRadius: 12,
            borderSkipped: false,
            data: [this.accountDue60, this.accountDue30, this.accountOverdue]
          }]
        };
      }
      if (this.chartScope === "cs01") {
        return {
          labels: ["Due in 60 days", "Due in 30 days", "Overdue"],
          datasets: [{
            label: "CS01",
            backgroundColor: ["#ccb1eb", "#a86cd8", "#8642c4"],
            borderRadius: 12,
            borderSkipped: false,
            data: [this.confirmationDue60, this.confirmationDue30, this.confirmationOverdue]
          }]
        };
      }
      if (this.chartScope === "iv") {
        return {
          labels: ["Due in 60 days", "Due in 30 days", "Overdue"],
          datasets: [{
            label: "Identity Verification",
            backgroundColor: ["#98ebca", "#59d5a4", "#2fbd76"],
            borderRadius: 12,
            borderSkipped: false,
            data: [this.ivDue60, this.ivDue30, this.ivOverdue]
          }]
        };
      }
      return {
        labels: ["Acc", "CS01", "IV"],
        datasets: [
          {
            label: "Overdue",
            backgroundColor: "#ff6557",
            data: [this.accountOverdue, this.confirmationOverdue, this.ivOverdue]
          },
          {
            label: "30 Days",
            backgroundColor: "#be98e5",
            data: [this.accountDue30, this.confirmationDue30, this.ivDue30]
          },
          {
            label: "60 Days",
            backgroundColor: "#d9c5f2",
            data: [this.accountDue60, this.confirmationDue60, this.ivDue60]
          }
        ]
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
.dash-shell {
  width: min(1660px, calc(100% - 40px));
  margin: 76px auto 56px;
  font-family: "Rethink Sans", sans-serif;
}

.dash-frame {
  overflow: hidden;
  border-radius: 34px;
  border: 1px solid #e8dcf0;
  background: linear-gradient(180deg, #fbf9fd 0%, #f7f4fb 100%);
  box-shadow: 0 34px 78px rgba(63, 31, 88, 0.08);
}

.dash-portal-header {
  display: grid;
  grid-template-columns: auto 1fr auto;
  align-items: center;
  gap: 20px;
  padding: 24px 34px;
  background: #ffffff;
  border-bottom: 1px solid #efe7f4;
}

.dash-brand {
  display: inline-flex;
  align-items: center;
  gap: 14px;
  color: #251830;
  text-decoration: none;
}

.dash-brand:hover,
.dash-nav-link:hover,
.dash-primary-action:hover,
.dash-review-row:hover {
  text-decoration: none;
}

.dash-brand-mark {
  width: 62px;
  height: 62px;
  border-radius: 18px;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #7a3bb3 0%, #3fcae0 100%);
  color: #ffffff;
  font-size: 24px;
}

.dash-brand-copy {
  display: flex;
  flex-direction: column;
}

.dash-brand-copy strong {
  color: #222036;
  font-size: 26px;
  line-height: 1;
}

.dash-brand-copy small {
  margin-top: 5px;
  color: #85778f;
  font-size: 13px;
  letter-spacing: 0.08em;
  text-transform: uppercase;
}
.dash-portal-nav {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 24px;
  flex-wrap: wrap;
}

.dash-nav-link {
  border: 0;
  background: transparent;
  color: #81728e;
  font: inherit;
  font-size: 18px;
  font-weight: 600;
  cursor: pointer;
  transition: color 0.2s ease;
}

.dash-nav-link.is-active {
  color: #6d317f;
}

.dash-nav-button:disabled,
.dash-icon-btn:disabled {
  opacity: 0.55;
  cursor: wait;
}

.dash-top-actions {
  display: flex;
  align-items: center;
  justify-content: flex-end;
  gap: 14px;
}

.dash-primary-action {
  min-height: 56px;
  padding: 0 30px;
  border-radius: 16px;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  background: #6f347f;
  color: #ffffff;
  font-size: 18px;
  font-weight: 700;
}

.dash-primary-action:hover {
  color: #ffffff;
  background: #5e2c6d;
}

.dash-icon-btn {
  width: 56px;
  height: 56px;
  border: 1px solid #e8dff0;
  border-radius: 16px;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  background: #ffffff;
  color: #6f347f;
  cursor: pointer;
}

.dash-avatar {
  width: 58px;
  height: 58px;
  border-radius: 50%;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  border: 2px solid #8b4fca;
  background: linear-gradient(135deg, #f7eefc 0%, #eee9ff 100%);
  color: #5c2b6e;
  font-size: 21px;
  font-weight: 800;
}

/* Removed redundant .dash-body padding as it's handled by global dashboard.css */


.dash-page-head {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  gap: 22px;
  padding: 2px 0 20px;
}

.dash-breadcrumb {
  margin: 0;
  color: #8a7c94;
  font-size: 13px;
  font-weight: 600;
}

.dash-page-head h1 {
  margin: 10px 0 0;
  color: #1f1a2b;
  font-size: clamp(28px, 3vw, 36px);
  line-height: 1.1;
  letter-spacing: -0.02em;
}

.dash-page-copy {
  margin: 12px 0 0;
  max-width: 800px;
  color: #6d6179;
  font-size: 15px;
  line-height: 1.5;
}

.dash-account-pill {
  min-height: 56px;
  padding: 0 22px;
  border-radius: 999px;
  display: inline-flex;
  align-items: center;
  gap: 10px;
  border: 1px solid #e5dae9;
  background: #ffffff;
  color: #594868;
  font-size: 16px;
  font-weight: 700;
}

.dash-cards {
  margin-top: 18px;
  display: grid;
  grid-template-columns: repeat(3, minmax(0, 1fr));
  gap: 22px;
}

.dash-cards-secondary {
  margin-top: 22px;
}

.dash-card,
.dash-panel {
  border-radius: 22px;
  border: 1px solid #ece3f1;
  background: #ffffff;
}

.dash-card {
  padding: 26px 28px;
}

.dash-card-topline,
.dash-panel-head,
.dash-summary-top,
.dash-summary-meta,
.dash-card-bottom {
  display: flex;
  justify-content: space-between;
}

.dash-card-topline,
.dash-panel-head,
.dash-card-bottom {
  align-items: flex-start;
  gap: 14px;
}

.dash-card-head {
  display: flex;
  align-items: center;
  gap: 16px;
}

.dash-card-icon {
  width: 64px;
  height: 64px;
  border-radius: 18px;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  background: #faf5ff;
  border: 1px solid #eee3f6;
  color: #7b46ab;
  font-size: 24px;
}

.dash-card.is-positive .dash-card-icon {
  color: #27b66f;
}

.dash-card.is-danger .dash-card-icon {
  color: #f05d50;
}

.dash-card h2 {
  margin: 0;
  color: #211b2f;
  font-size: 16px;
  line-height: 1.35;
}

.dash-more {
  width: 34px;
  height: 34px;
  border: 0;
  background: transparent;
  color: #9f93ab;
  display: inline-flex;
  align-items: center;
  justify-content: center;
}

.dash-card-bottom {
  margin-top: 28px;
  align-items: flex-end;
}

.dash-card-value {
  display: block;
  color: #1b2236;
  font-size: clamp(32px, 2.5vw, 42px);
  line-height: 1;
  letter-spacing: -0.03em;
}

.dash-card-trend {
  margin: 14px 0 0;
  display: inline-flex;
  align-items: center;
  gap: 8px;
  font-size: 16px;
  font-weight: 700;
}

.dash-card-trend small {
  color: #7f718d;
  font-size: 16px;
  font-weight: 600;
}

.dash-card-trend.is-positive {
  color: #27b66f;
}

.dash-card-trend.is-danger {
  color: #f05d50;
}

.dash-card-trend.is-violet {
  color: #8d52c8;
}

.dash-sparkline {
  width: 156px;
  height: 92px;
  flex-shrink: 0;
}

.dash-main-grid {
  margin-top: 22px;
  display: grid;
  grid-template-columns: minmax(0, 0.95fr) minmax(0, 1.05fr) minmax(0, 0.95fr);
  gap: 22px;
}

.dash-panel {
  padding: 24px 26px;
}

.dash-panel-head h3 {
  margin: 0;
  color: #1f1a2d;
  font-size: 20px;
  line-height: 1.25;
}

.dash-summary-list,
.dash-review-table {
  margin-top: 20px;
  display: grid;
  gap: 24px;
}

.dash-summary-top {
  align-items: flex-end;
  gap: 16px;
}

.dash-summary-top span {
  display: block;
  color: #6f6280;
  font-size: 16px;
}
.dash-summary-top strong {
  display: block;
  margin-top: 4px;
  color: #211b2f;
  font-size: 24px;
  line-height: 1;
}

.dash-summary-top small {
  color: #847791;
  font-size: 16px;
  font-weight: 600;
}

.dash-progress-track {
  margin-top: 14px;
  height: 10px;
  border-radius: 999px;
  overflow: hidden;
  background: #ece9f1;
}

.dash-progress-track span {
  display: block;
  height: 100%;
  border-radius: inherit;
}

.dash-progress-track .is-amber {
  background: linear-gradient(90deg, #ffb52c 0%, #f3a319 100%);
}

.dash-progress-track .is-violet {
  background: linear-gradient(90deg, #a76adb 0%, #8d52c8 100%);
}

.dash-progress-track .is-positive {
  background: linear-gradient(90deg, #27b66f 0%, #2fbd76 100%);
}

.dash-summary-meta {
  margin-top: 12px;
  align-items: center;
  gap: 10px;
  color: #7b6f87;
  font-size: 14px;
  font-weight: 600;
}

.dash-panel-head-chart {
  align-items: center;
}

.dash-segment {
  display: inline-flex;
  align-items: center;
  padding: 4px;
  border-radius: 999px;
  border: 1px solid #eadff0;
  background: #fbf8fd;
}

.dash-segment-btn {
  min-height: 36px;
  padding: 0 14px;
  border: 0;
  border-radius: 999px;
  background: transparent;
  color: #7f738e;
  font: inherit;
  font-size: 14px;
  font-weight: 700;
  cursor: pointer;
}

.dash-segment-btn.is-active {
  background: #ffffff;
  color: #5f2c71;
  box-shadow: 0 1px 2px rgba(52, 25, 73, 0.06);
}

.dash-chart-wrap {
  margin-top: 20px;
  min-height: 390px;
}

.dash-review-row {
  display: grid;
  grid-template-columns: 120px minmax(0, 1fr) auto;
  align-items: center;
  gap: 16px;
  color: inherit;
}

.dash-review-dateblock strong,
.dash-review-company strong {
  display: block;
  color: #2b2239;
  font-size: 15px;
  font-weight: 700;
  line-height: 1.35;
}

.dash-review-dateblock span,
.dash-review-company span {
  display: block;
  margin-top: 4px;
  color: #8a7d97;
  font-size: 14px;
  line-height: 1.4;
}

.dash-review-status {
  display: flex;
  align-items: center;
  justify-content: flex-end;
}

.dash-status {
  min-height: 32px;
  padding: 0 12px;
  border-radius: 8px;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  font-size: 13px;
  font-weight: 700;
}

.dash-status.is-info {
  background: #ebf3ff;
  color: #1966d2;
}

.dash-status.is-amber {
  background: #fff3df;
  color: #d5850a;
}

.dash-status.is-danger {
  background: #ffe9e6;
  color: #e05548;
}

.dash-empty {
  margin-top: 18px;
  min-height: 180px;
  border-radius: 16px;
  border: 1px dashed #e1d3ea;
  display: flex;
  align-items: center;
  justify-content: center;
  color: #8a7e97;
  font-size: 15px;
  font-weight: 600;
  text-align: center;
}

@media (max-width: 1200px) {
  .dash-portal-header {
    grid-template-columns: 1fr;
    justify-items: start;
  }

  .dash-portal-nav,
  .dash-top-actions {
    justify-content: flex-start;
  }

  .dash-main-grid {
    grid-template-columns: 1fr 1fr;
  }

  .dash-panel-review {
    grid-column: 1 / -1;
  }
}

@media (max-width: 960px) {
  .dash-shell {
    width: calc(100% - 18px);
    margin: 46px auto 40px;
  }

  .dash-page-head,
  .dash-cards,
  .dash-main-grid {
    grid-template-columns: 1fr;
    flex-direction: column;
  }

  .dash-review-row {
    grid-template-columns: 1fr;
  }

  .dash-review-status {
    justify-content: flex-start;
  }
}

@media (max-width: 640px) {
  .dash-frame {
    border-radius: 24px;
  }

  .dash-portal-header,
  .dash-body {
    padding: 18px;
  }

  .dash-brand-mark {
    width: 54px;
    height: 54px;
    font-size: 20px;
  }

  .dash-brand-copy strong {
    font-size: 22px;
  }

  .dash-primary-action,
  .dash-account-pill {
    width: 100%;
    justify-content: center;
  }

  .dash-top-actions {
    width: 100%;
    flex-wrap: wrap;
  }

  .dash-card,
  .dash-panel {
    padding: 18px;
  }

  .dash-page-head h1 {
    font-size: 40px;
  }

  .dash-page-copy,
  .dash-card-trend,
  .dash-card-trend small {
    font-size: 16px;
  }

  .dash-card-bottom {
    flex-direction: column;
    align-items: flex-start;
  }

  .dash-sparkline {
    width: 100%;
    max-width: 140px;
  }

  .dash-summary-meta,
  .dash-panel-head-chart {
    flex-direction: column;
    align-items: flex-start;
  }
}
</style>
