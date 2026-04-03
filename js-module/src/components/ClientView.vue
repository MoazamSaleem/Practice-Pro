<template>
  <div>
    <!-- Header Section -->
    <div class="row align-items-center mb-4">
      <div class="col-md-6">
        <h4 class="fw-bold">Companies</h4>
      </div>
      <div class="col-md-6 text-md-end text-center">
        <!-- Filters and Buttons -->
        <div class="d-flex flex-wrap justify-content-md-end justify-content-center">
          <select class="form-select me-2 mb-2" style="width: auto;">
            <option :value="User">{{ userEmail }}</option>
          </select>

          <select class="form-select me-2 mb-2" style="width: auto;" v-model="selectedStatus" @change="filterData">
            <option value="all">All</option>
            <option value="overdue">Overdue</option>
            <option value="due_tomorrow">Due Tomorrow</option>
            <option value="due_30">Due in 30 Days</option>
            <option value="due_60">Due in 60 Days</option>
          </select>

          <!--<button class="btn btn-secondary me-2 mb-2" @click="reloadPage">
    <i class="bi bi-arrow-clockwise"></i>
  </button>-->

          <button class="btn btn-secondary me-2 mb-2"
                  @click="reloadAllCompanies"
                  :disabled="isReloadingAll">
            <span v-if="isReloadingAll">
              <span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>
              Updating...
            </span>
            <span v-else>
              <i class="bi bi-arrow-clockwise"></i>
            </span>
          </button>

          <div v-if="reloadProgress" class="alert alert-info mt-2">
            {{ reloadProgress }}
          </div>

          <button class="btn btn-primary mb-2 fw-bold" data-bs-toggle="modal" data-bs-target="#addCompanyModal">
            <i class="bi bi-plus-lg fw-bold"></i> Add Company
          </button>
        </div>
      </div>
    </div>

    <!-- Data Table -->
    <div class="card shadow-sm">
      <div class="card-body">
        <div class="table-container">
          <table id="tbl" class="table table-striped table-bordered">
            <thead class="table-light">
              <tr>
                <th data-orderable="true" class="nowrap">Name</th>
                <th data-orderable="true" class="nowrap">Number</th>
                <th data-orderable="true" class="nowrap">Accounts Due</th>
                <th data-orderable="true" class="nowrap">Confirmation Statement</th>
                <th data-orderable="true" class="nowrap">Identity verification due</th>
                <th data-orderable="true" class="nowrap">Status</th>
                <th data-orderable="true" class="nowrap">Quaters</th>
                <th data-orderable="true" class="nowrap">VAT Return Due</th>
                <th data-orderable="true">Action</th> <!-- Disable sorting for action column -->
              </tr>
            </thead>
            <tbody>
              <tr v-for="(item, index) in filteredList" :key="item.id">
                <td class="nowrap">{{ item.name }}</td>
                <td>
                  <a :href="'https://find-and-update.company-information.service.gov.uk/company/' + item.number" target="_blank" class="text-primary">
                    {{ item.number }}
                  </a>
                </td>
                <td class="nowrap">{{ item.accountsD }}</td>
                <td class="nowrap">{{ item.cS01D }}</td>
                <td class="nowrap">{{ item.identityVerificationD }}</td>
                <td :class="['existing-class',
             item.status.toLowerCase() === 'active' ? 'text-success' : '',
             item.status.toLowerCase() === 'dissolved' ? 'text-danger' : '',
             item.status.toLowerCase() === 'liquidation' ? 'text-primary' : '']">
                  <span class="fw-bold">
                    {{ item.status }}
                  </span>
                </td>
                <td class="nowrap">{{ item.quaters }}</td>
                <td class="nowrap">{{ item.vatReturnDue }}</td>
                <td>
                  <div class="d-flex justify-content-center">
                    <button type="button" class="btn btn-danger btn-sm me-1" v-on:click="deleteList(item.id)">Delete</button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>

    <!-- Right-Side Fullscreen Modal -->
    <div class="modal fade" id="addCompanyModal" data-bs-backdrop="static" tabindex="-1" aria-labelledby="addCompanyModalLabel" aria-hidden="true">
      <div class="modal-dialog modal-fullscreen modal-right">
        <div class="modal-content">
          <div class="modal-header bg-primary text-white">
            <h5 class="modal-title text-white" id="addCompanyModalLabel">New Company</h5>
            <button type="button" class="btn-close btn-close-white fw-bold" data-bs-dismiss="modal" aria-label="Close"></button>
          </div>
          <div class="modal-body">
            <div class="container">
              <div class="row">
                <div class="col-md-12">

                  <!-- Card for Import from Companies House Section -->
                  <div class="card mb-3">
                    <div class="card-header">
                      <h5 class="card-title mb-0">Import from Companies House</h5>
                    </div>
                    <div class="card-body">
                      <div class="d-flex align-items-center mb-3">
                        <label for="RNumber" style="width: 200px;">Registration Number</label>
                        <input type="text" class="form-control" id="RNumber" placeholder="Enter Company Number" v-model="companyNumber">
                        <button class="btn btn-primary ms-2 fw-bold" @click="fetchCompanyProfile"><i class="bi bi-bolt"></i> Import</button>
                      </div>
                    </div>
                  </div>
                  <!-- Card for Company Information Section -->
                  <div class="card mb-3">
                    <div class="card-header">
                      <h5 class="card-title mb-0">Company Information</h5>
                    </div>
                    <div class="card-body">
                      <!-- Name Field -->
                      <div class="d-flex align-items-center mb-3">
                        <label for="companyName" class="fw-bold me-2" style="width: 200px;">Name:</label>
                        <div style="flex: 1;">
                          <input type="text" class="form-control" id="companyName" v-model="obj.Name">
                          <small v-if="errors.Name" class="text-danger d-block mt-1">{{ errors.Name }}</small>
                        </div>
                      </div>

                      <!-- Number Field -->
                      <div class="d-flex align-items-center mb-3">
                        <label for="Number" class="fw-bold me-2" style="width: 200px;">Number:</label>
                        <div style="flex: 1;">
                          <input type="text" class="form-control" id="Number" v-model="obj.Number">
                          <small v-if="errors.Number" class="text-danger d-block mt-1">{{ errors.Number }}</small>
                        </div>
                      </div>

                      <!-- Account Dues Field -->
                      <div class="d-flex align-items-center mb-3">
                        <label for="AccountDues" class="fw-bold me-2" style="width: 200px;">Account Dues:</label>
                        <div style="flex: 1;">
                          <input type="text" class="form-control" id="AccountDues" v-model="obj.AccountsD">
                          <small v-if="errors.AccountsD" class="text-danger d-block mt-1">{{ errors.AccountsD }}</small>
                        </div>
                      </div>

                      <!-- Confirmation Statement Field -->
                      <div class="d-flex align-items-center mb-3">
                        <label for="ConfirmationStatement" class="fw-bold me-2" style="width: 200px;">Confirmation Statement:</label>
                        <div style="flex: 1;">
                          <input type="text" class="form-control" id="ConfirmationStatement" v-model="obj.CS01D">
                          <small v-if="errors.CS01D" class="text-danger d-block mt-1">{{ errors.CS01D }}</small>
                        </div>
                      </div>

                      <!-- Company Type Field
                      <div class="d-flex align-items-center mb-3">
                        <label for="Type" class="fw-bold me-2" style="width: 200px;">Company Type:</label>
                        <div style="flex: 1;">
                          <input type="text" class="form-control" id="Type" v-model="obj.Type">
                          <small v-if="errors.Type" class="text-danger d-block mt-1">{{ errors.Type }}</small>
                        </div>
                      </div> -->

                      <!-- Status Field -->
                      <div class="d-flex align-items-center mb-3">
                        <label for="Status" class="fw-bold me-2" style="width: 200px;">Status:</label>
                        <div style="flex: 1;">
                          <input type="text" class="form-control" id="Status" v-model="obj.Status">
                          <small v-if="errors.Status" class="text-danger d-block mt-1">{{ errors.Status }}</small>
                        </div>
                      </div>
                    </div>
                  </div>

                  <!-- Card for VAT Section -->
                  <div class="card mb-3">
                    <div class="card-header">
                      <h5 class="card-title mb-0">VAT</h5>
                    </div>
                    <div class="card-body">
                      <!-- VAT Number Field -->
                      <div class="d-flex align-items-center mb-3">
                        <label for="VATNumber" class="fw-bold me-2" style="width: 200px;">VAT Number:</label>
                        <div style="flex: 1;">
                          <input type="text" class="form-control" id="VATNumber" v-model="obj.VatNum">
                        </div>
                      </div>

                      <!-- Quarters Field -->
                      <div class="d-flex align-items-start mb-3">
                        <label for="Quarters" class="fw-bold me-2" style="width: 200px;">Quarters:</label>
                        <div style="flex: 1;">
                          <select class="form-control" id="Quarters" v-model="obj.Quaters" @change="calculateVatReturnDue">
                            <option value="Jan-Apr-Jul-Oct">January/April/July/October</option>
                            <option value="Feb-May-Aug-Nov">February/May/August/November</option>
                            <option value="Mar-Jun-Sep-Dec">March/June/September/December</option>
                            <option value="Monthly">Monthly</option>
                            <option value="Yearly">Yearly</option>
                          </select>
                        </div>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <div class="modal-footer">
            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
            <button type="button" class="btn btn-primary" @click="create" data-bs-dismiss="modal">Save</button>
          </div>
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
        lst: [],
        filteredList: [],
        selectedStatus: "all",
        dtInstance: null,
        userEmail: "",

        companyNumber: "", // User input for the company number
        companyData: {}, // API response data
        obj: {
          Name: "",
          Number: "",
          AccountsD: "",
          CS01D: "",
          Type:"",
          Status: "",
          Quaters: "",
          VatReturnDue: "",
        },
        // ... existing properties ...
        isReloadingAll: false,
        reloadProgress: null,
        errors: {}
      };
    },
    created() {
      this.list();
    },
    watch: {
      "obj.Quaters": function (newVal) {
        if (newVal) {
          this.calculateVatReturnDue();
        }
      },
    },
    mounted() {
      this.fetchUserEmail();
      window.addEventListener("premiumdm:company-data-synced", this.handleCompanyDataSynced);
    },
    beforeDestroy() {
      window.removeEventListener("premiumdm:company-data-synced", this.handleCompanyDataSynced);
    },
    methods: {
      handleCompanyDataSynced() {
        this.list();
      },
      list() {
        axios
          .post("/api/ClientApi/Vlist")
          .then((res) => {
            if (res.data.status === true) {
              // Format dates before assigning them to the list
              this.lst = res.data.lst.map((item) => ({
                ...item,
                accountsD: this.formatDate(item.accountsD),
                cS01D: this.formatDate(item.cS01D),
                identityVerificationD: this.formatDate(item.identityVerificationD),
                vatReturnDue: this.formatDate(item.vatReturnDue),
                type: this.mapCompanyType(item.type)
              }));
              this.filteredList = this.lst;
              this.$nextTick(() => {
                this.initDataTable();
              });
            }
          })
          .catch((error) => {
            console.error("Error fetching data: ", error);
          });
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
      mapCompanyType(type) {
        // Map "LTD" to "Limited"
        if (type === "ltd") {
          return "Limited";
        }
        // Add other mappings if needed
        return type;
      },
      filterData() {
        if (this.selectedStatus === "all") {
          this.reloadPage();
          return;
        }

        const today = new Date();
        const tomorrow = new Date();
        tomorrow.setDate(today.getDate() + 1);
        const days30 = new Date();
        days30.setDate(today.getDate() + 30);
        const days60 = new Date();
        days60.setDate(today.getDate() + 60);

        if (this.selectedStatus === "overdue") {
          this.filteredList = this.lst.filter(item => new Date(item.accountsD) < today);
        } else if (this.selectedStatus === "due_tomorrow") {
          this.filteredList = this.lst.filter(item => new Date(item.accountsD).toDateString() === tomorrow.toDateString());
        } else if (this.selectedStatus === "due_30") {
          this.filteredList = this.lst.filter(item => new Date(item.accountsD) >= today && new Date(item.accountsD) <= days30);
        } else if (this.selectedStatus === "due_60") {
          this.filteredList = this.lst.filter(item => new Date(item.accountsD) >= today && new Date(item.accountsD) <= days60);
        } else {
          this.filteredList = this.lst;
        }

        // Get the current column order
        this.$nextTick(() => {
          const table = $('#tbl').DataTable();
          const columnOrder = table.colReorder.order(); // Get the column order

          // Clear the existing table
          table.clear();

          // Map the data according to the new column order
          const rowData = this.filteredList.map(item => {
            const data = [
              item.name,
              item.number,
              item.accountsD,
              item.cS01D,
              item.identityVerificationD,
              item.status,
              item.quaters,
              item.vatReturnDue,
              '' // Action column
            ];
            return columnOrder.map(index => data[index]); // Rearrange data based on column order
          });

          // Add the rearranged rows to the table
          rowData.forEach(row => table.row.add(row));

          // Redraw the table
          table.draw();
        });
      },
      initDataTable() {
        const table = $('#tbl');
        if (table.length && !$.fn.DataTable.isDataTable(table)) {
          // Add custom sorting for UK dates first
          $.fn.dataTable.ext.type.order['date-uk-pre'] = function (d) {
            if (!d) return 0;

            // Convert UK date format (DD-MMM-YYYY) to sortable format
            const parts = d.split('-');
            if (parts.length === 3) {
              const months = {
                'Jan': '01', 'Feb': '02', 'Mar': '03', 'Apr': '04',
                'May': '05', 'Jun': '06', 'Jul': '07', 'Aug': '08',
                'Sep': '09', 'Oct': '10', 'Nov': '11', 'Dec': '12'
              };
              const month = months[parts[1]];
              if (month) {
                return parseInt(parts[2] + month + parts[0]);
              }
            }
            return 0;
          };

          this.dtInstance = table.DataTable({
            colReorder: true,
            stateSave: true,
            stateDuration: -1,
            retrieve: true,
            pageLength: -1, 
            lengthMenu: [[-1, 200, 100, 50], ["All", 200, 100, 50]], // Optional: Add length menu options
            columnDefs: [
              {
                targets: [2, 3, 7], // Indexes of date columns
                type: 'date-uk' // Use our custom sorting type
              }
            ],
          });
        }
      },
      reloadPage() {
        window.location.reload();
      },
      AddNew() {
        window.location.href = "/Client/InCompany";
      },
      deleteList(id) {
        axios.post('/api/ClientApi/deleteList?id=' + id)
          .then((res) => {
            if (res.data.status == true) {
              alert("Deleted successfully!");
              this.list();
            }
          })
          .catch((error) => {
            console.error("Error deleting item: ", error);
          });
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
              this.showModal = true; // Open the modal after fetching data
            } else {
              alert(res.data.message);
              this.companyData = null;
            }
          })
          .catch((err) => {
            console.error(err);
            alert("Error fetching company data!");
          });
      },
      populateForm() {
        if (this.companyData) {
          this.obj.Name = this.companyData.company_name || "";
          this.obj.Number = this.companyData.company_number || "";
          this.obj.Status = this.companyData.company_status || "";
          this.obj.AccountsD = this.formatDate(this.companyData.accounts.next_due || "");
          this.obj.CS01D = this.formatDate(this.companyData.confirmation_statement.next_due || "");
          this.obj.Type = this.mapCompanyType(this.companyData.type || "");
        }
      },
      create() {
        this.errors = {}; // Reset errors before validation

        // Validation
        if (!this.obj.Name) this.errors.Name = "Name is required";
        if (!this.obj.Number) this.errors.Number = "Number is required";
        if (!this.obj.AccountsD) this.errors.AccountsD = "Account Due are required";
        if (!this.obj.CS01D) this.errors.CS01D = "Confirmation Statement is required";
        if (!this.obj.Type) this.errors.Type = "Company Type is required";
        if (!this.obj.Status) this.errors.Status = "Status is required";

        // If there are errors, stop the process
        if (Object.keys(this.errors).length > 0) {
          return;
        }
        // Calculate VAT Return Due before saving
        this.calculateVatReturnDue();
        axios 
          .post("/api/ClientApi/SaveDa", { obj: this.obj })
          .then((res) => {
            if (res.data.status) {
              alert("Saved Successfully");
              this.clear();
              this.reloadPage();
            } else {
              alert("Error saving data!");
              this.clear();
            }
          })
      },
      calculateVatReturnDue() {
        const today = new Date();
        let dueDate;

        switch (this.obj.Quaters) {
          case "Jan-Apr-Jul-Oct":
            dueDate = new Date(today.getFullYear(), 3, 7); // 7th April
            break;
          case "Feb-May-Aug-Nov":
            dueDate = new Date(today.getFullYear(), 3, 7); // 7th April
            break;
          case "Mar-Jun-Sep-Dec":
            dueDate = new Date(today.getFullYear(), 5, 7); // 7th June
            break;
          case "Monthly":
            dueDate = new Date(today.getFullYear(), today.getMonth() + 1, 7);
            break;
          case "Yearly":
            dueDate = new Date(today.getFullYear() + 1, 1, 7); // 7th February
            break;
          default:
            dueDate = null;
        }

        if (dueDate) {
          // Use our custom formatDate method to ensure consistent formatting
          this.obj.VatReturnDue = this.formatDate(dueDate);
        } else {
          this.obj.VatReturnDue = "";
        }
      },


      reloadAllCompanies() {
        if (!confirm("Are you sure you want to reload data for all companies from Companies House? This may take several minutes.")) {
          return;
        }

        this.isReloadingAll = true;
        this.reloadProgress = "Starting update process...";

        // Call the synchronous endpoint
        axios.post("/api/ClientApi/UpdateAllCompaniesSync")
          .then(response => {
            if (response.data.status) {
              alert(response.data.message);
              this.list(); // Refresh the table data
            } else {
              alert(`Error: ${response.data.message}`);
            }
          })
          .catch(error => {
            console.error("Error reloading all companies:", error);
            if (error.response) {
              alert(`Error ${error.response.status}: ${error.response.data.message || error.response.statusText}`);
            } else {
              alert("Failed to reload companies");
            }
          })
          .finally(() => {
            this.isReloadingAll = false;
            this.reloadProgress = null;
          });
      },

      clear() {
        this.obj = {
          Name: "",
          Number: "",
          AccountsD: "",
          CS01D: "",
          Type: "",
          Status: "",
          Quaters: "",
        };
      }
    }
  };
</script>

<style scoped>
  .table-container {
    height: 100vh;
    overflow-y: auto;
  }
  @media (min-width: 768px) {
    .modal-right {
      position: fixed;
      right: 0;
      top: 0;
      width: 40%;
      height: 100vh;
      margin: 0;
      transform: translateX(100%);
      transition: transform 0.3s ease-in-out;
    }

    .modal.show .modal-right {
      transform: translateX(0%);
    }
  }
  /* Desktop - normal table display */
  @media (min-width: 1486px) {
    .table-container {
      overflow-x: hidden;
    }

    #tbl {
      width: 100% !important;
    }
  }
  /* Fullscreen modal on small screens */
  @media (max-width: 1485px) {
    .modal-right {
      width: 100%;
      transform: translateX(0);
    }
  }
  .nowrap {
    white-space: nowrap; /* Prevents wrapping on large screens */
  }
  /* Modal adjustments */
  @media (min-width: 768px) {
    .modal-right {
      position: fixed;
      right: 0;
      top: 0;
      width: 40%;
      height: 100vh;
      margin: 0;
      transform: translateX(100%);
      transition: transform 0.3s ease-in-out;
    }

    .modal.show .modal-right {
      transform: translateX(0%);
    }
  }
</style>
