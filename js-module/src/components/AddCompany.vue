<template>
  <div class="container mt-4">
    <div class="card">
      <div class="card-header bg-primary text-white">
        <h5 class="card-title m-0">Import Company</h5>
      </div>
      <div class="card-body">
        <!-- Company Number Input -->
        <div class="input-group mb-3">
          <input v-model="companyNumber"
                 class="form-control"
                 placeholder="Enter Company Number" />
          <button class="btn btn-primary" @click="fetchCompanyProfile">
            Fetch Data
          </button>
        </div>

        <!-- Dynamic Fields Updated from API Response -->
        <div class="row g-3">
          <!-- Name -->
          <div class="col-md-6">
            <label for="companyName" class="fw-bold">Name:</label>
            <input type="text" class="form-control" id="companyName" v-model="obj.Name">
            <small v-if="errors.Name" class="text-danger">{{ errors.Name }}</small>
          </div>

          <!-- Number -->
          <div class="col-md-6">
            <label for="Number" class="fw-bold">Number:</label>
            <input type="text" class="form-control" id="Number" v-model="obj.Number">
            <small v-if="errors.Number" class="text-danger">{{ errors.Number }}</small>
          </div>

          <!-- Account Dues -->
          <div class="col-md-6">
            <label for="AccountDue" class="fw-bold">Account Due:</label>
            <input type="text" class="form-control" id="AccountDue" v-model="obj.AccountsD">
            <small v-if="errors.AccountsD" class="text-danger">{{ errors.AccountsD }}</small>
          </div>

          <!-- Confirmation Statement -->
          <div class="col-md-6">
            <label for="ConfirmationStatement" class="fw-bold">Confirmation Statement:</label>
            <input type="text" class="form-control" id="ConfirmationStatement" v-model="obj.CS01D">
            <small v-if="errors.CS01D" class="text-danger">{{ errors.CS01D }}</small>
          </div>

          <!-- Identity Verification -->
          <div class="col-md-6">
            <label for="IdentityVerification" class="fw-bold">Identity Verification Due:</label>
            <input type="text" class="form-control" id="IdentityVerification" v-model="obj.IdentityVerificationD">
          </div>

          <!-- Status -->
          <div class="col-md-6">
            <label for="Status" class="fw-bold">Status:</label>
            <input type="text" class="form-control" id="Status" v-model="obj.Status">
            <small v-if="errors.Status" class="text-danger">{{ errors.Status }}</small>
          </div>

          <!-- VAT Number -->
          <div class="col-md-6">
            <label for="VATNumber" class="fw-bold">VAT Number:</label>
            <input type="text" class="form-control" id="VATNumber" v-model="obj.VatNum">
          </div>

          <!-- Quarters -->
          <div class="col-md-6">
            <label for="Quarters" class="fw-bold">Quarters:</label>
            <select class="form-control" id="Quarters" v-model="obj.Quaters">
              <option value="">Select Quarter</option>
              <option value="Jan-Apr-Jul-Oct">January/April/July/October</option>
              <option value="Feb-May-Aug-Nov">February/May/August/November</option>
              <option value="Mar-Jun-Sep-Dec">March/June/September/December</option>
              <option value="Monthly">Monthly</option>
              <option value="Yearly">Yearly</option>
            </select>
          </div>
        </div>

        <!-- Save Button -->
        <div class="text-end mt-3">
          <button class="btn btn-success btn-sm px-4"
                  @click="create">
            Save
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
  import axios from "axios";

  export default {
    data() {
      return {
        companyNumber: "", // User input for the company number
        companyData: {}, // API response data
        obj: {
          Name: "",
          Number: "",
          AccountsD: "",
          CS01D: "",
          IdentityVerificationD: "",
          Status: "",
          Quaters: "",
          VatReturnDue: "",
          VatNum: ""
        },
        errors: {}
      };
    },
    watch: {
      "obj.Quaters": function (newVal) {
        if (newVal) {
          this.calculateVatReturnDue();
        }
      },
    },
    created() {
      this.formatDate();
    },
    methods: {
      fetchCompanyProfile() {
        if (!this.companyNumber) {
          alert("Please enter a company number.");
          return;
        }
        axios
          .get(`/api/ClientApi/fetchCompanyData?companyNumber=${this.companyNumber}`)
          .then((res) => {
            if (res.data.status === true) {
              this.companyData = JSON.parse(res.data.de);
              this.populateForm();
            } else {
              alert(res.data.message);
              this.companyData = null;
            }
          })
      },
      formatDate(dateString) {
        if (!dateString) return "";

        // If already in correct format, return as-is
        if (typeof dateString === 'string' && dateString.match(/^\d{2}-[a-zA-Z]{3}-\d{4}$/)) {
          return dateString;
        }

        const date = new Date(dateString);
        if (isNaN(date.getTime())) return ""; // Invalid date

        // Custom month abbreviations to match UK standard
        const monthNames = {
          'Jan': 'Jan', 'Feb': 'Feb', 'Mar': 'Mar', 'Apr': 'Apr',
          'May': 'May', 'Jun': 'Jun', 'Jul': 'Jul', 'Aug': 'Aug',
          'Sep': 'Sep', 'Oct': 'Oct', 'Nov': 'Nov', 'Dec': 'Dec'
        };

        const day = date.getDate().toString().padStart(2, '0');
        const month = monthNames[date.toLocaleString('en-US', { month: 'short' })];
        const year = date.getFullYear();

        return `${day}-${month}-${year}`;
      },
      populateForm() {
        if (this.companyData) {
          this.obj.Name = this.companyData.company_name || "";
          this.obj.Number = this.companyData.company_number || "";
          this.obj.Status = this.companyData.company_status || "";
          this.obj.AccountsD = this.formatDate(this.companyData.accounts.next_due || "");
          this.obj.CS01D = this.formatDate(this.companyData.confirmation_statement ? this.companyData.confirmation_statement.next_due : "");
          this.obj.IdentityVerificationD = this.formatDate((this.companyData.identity_verification && this.companyData.identity_verification.next_due) ? this.companyData.identity_verification.next_due : (this.companyData.confirmation_statement ? this.companyData.confirmation_statement.next_due : ""));
        }
      },
      create() {
        this.errors = {}; // Reset errors before validation

        // Validation
        if (!this.obj.Name) this.errors.Name = "Name is required";
        if (!this.obj.Number) this.errors.Number = "Number is required";
        if (!this.obj.AccountsD) this.errors.AccountsD = "Account Due are required";
        if (!this.obj.CS01D) this.errors.CS01D = "Confirmation Statement is required";
        if (!this.obj.Status) this.errors.Status = "Status is required";

        // If there are errors, stop the process
        if (Object.keys(this.errors).length > 0) {
          return;
        }
        axios
          .post("/api/ClientApi/SaveDa", { obj: this.obj })
          .then((res) => {
            if (res.data.status) {
              alert("Saved Successfully");
              this.clear();
              window.location.href = "/Client/FavCompany"
            } else {
              alert("Error saving data!");
            }
          })
          .catch((err) => {
            console.error(err);
            alert("Error while saving!");
          });
      },
      calculateVatReturnDue() {
        const today = new Date();
        let dueDate;

        switch (this.obj.Quaters) {
          case "Jan-Apr-Jul-Oct":
            dueDate = new Date(today.getFullYear(), 3, 7); 
            break;
          case "Feb-May-Aug-Nov":
            dueDate = new Date(today.getFullYear(), 3, 7); 
            break;
          case "Mar-Jun-Sep-Dec":
            dueDate = new Date(today.getFullYear(), 5, 7); 
            break;
          case "Monthly":
            dueDate = new Date(today.getFullYear(), today.getMonth() + 1, 7); 
            break;
          case "Yearly":
            dueDate = new Date(today.getFullYear() + 1, 1, 7); 
            break;
          default:
            dueDate = null; 
        }

        this.obj.vatReturnDue = dueDate ? dueDate.toISOString().split("T")[0] : "";
      },
      clear() {
        this.obj = {
          Name: "",
          Number: "",
          AccountsD: "",
          CS01D: "",
          IdentityVerificationD: "",
          Status: "",
          Quaters: "",
          VatNum: "",
          VatReturnDue: ""
        };
        this.companyData = null;
        this.companyNumber = "";
      },
    },
  };
</script>
