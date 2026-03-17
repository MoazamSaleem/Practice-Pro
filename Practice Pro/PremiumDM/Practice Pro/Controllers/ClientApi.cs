using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Practice_App.Models;
using Practice_Pro.Models;
using RestSharp;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Practice_Pro.Controllers
{
    [Route("api/[controller]/{action?}")]
    [ApiController]
    public class ClientApi : ControllerBase
    {
        private readonly DbCalls db;
        private readonly IWebHostEnvironment env;
        public ClientApi(DbCalls db, IWebHostEnvironment env)
        {
            this.db = db;
            this.env = env;
        }

        public class _add
        {
            public Client obj { get; set; }
        }
        public JsonResult SaveDa(_add data)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get user ID from the claim
            if (string.IsNullOrEmpty(userId))
            {
                return new JsonResult(new { status = false, message = "Unauthorized access" });
            }

            data.obj.UserId = userId; // Assign client to logged-in user
            db.client.Add(data.obj);
            db.SaveChanges();
            return new JsonResult(new { status = true });
        }

        public JsonResult Vlist()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get user ID from the claim
            if (string.IsNullOrEmpty(userId))
            {
                return new JsonResult(new { status = false, message = "Unauthorized access" });
            }

            var lst = db.client.Where(c => c.UserId == userId).ToList();
            return new JsonResult(new { status = true, lst });
        }

        public JsonResult deleteList(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get user ID from the claim
            if (string.IsNullOrEmpty(userId))
            {
                return new JsonResult(new { status = false, message = "Unauthorized access" });
            }

            var de = db.client.FirstOrDefault(x => x.Id == id && x.UserId == userId);
            if (de == null)
            {
                return new JsonResult(new { status = false, message = "Client not found or unauthorized" });
            }

            db.Remove(de);
            db.SaveChanges();
            return new JsonResult(new { status = true, de });
        }
        public JsonResult fetchCompanyData(string companyNumber)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get user ID from the claim
            if (string.IsNullOrEmpty(userId))
            {
                return new JsonResult(new { status = false, message = "Unauthorized access" });
            }

            // Check if the company already exists in your database for the current user
            var existingCompany = db.client
                .FirstOrDefault(c => c.Number == companyNumber && c.UserId == userId);

            if (existingCompany != null)
            {
                return new JsonResult(new
                {
                    status = false,
                    message = "This company already exists in your saved list."
                });
            }

            var baseURL = "https://api.company-information.service.gov.uk";
            var client = new RestClient(baseURL);
            var request = new RestRequest("/company/{company_number}", Method.Get);

            request.AddHeader("Authorization", "04f0a934-7b60-42e2-98ad-26462c314b97");
            request.AddUrlSegment("company_number", companyNumber);

            var response = client.Execute(request);

            if (response.IsSuccessful)
            {
                return new JsonResult(new { status = true, de = response.Content });
            }
            else
            {
                return new JsonResult(new { status = false, message = "Company data not found!" });
            }
        }

        public JsonResult UpdateAllCompaniesSync()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return new JsonResult(new { status = false, message = "Unauthorized access" });
                }

                // Get all companies for the current user from database
                var allCompanies = db.client
                    .Where(c => c.UserId == userId)
                    .ToList();

                int updatedCount = 0;
                var baseURL = "https://api.company-information.service.gov.uk";
                var client = new RestClient(baseURL);

                foreach (var company in allCompanies)
                {
                    try
                    {
                        if (string.IsNullOrEmpty(company.Number))
                            continue;

                        // Rate limiting - synchronous delay
                        System.Threading.Thread.Sleep(200);

                        var request = new RestRequest("/company/{company_number}", Method.Get);
                        request.AddHeader("Authorization", "04f0a934-7b60-42e2-98ad-26462c314b97");
                        request.AddUrlSegment("company_number", company.Number);

                        var response = client.Execute(request);

                        if (!response.IsSuccessful)
                            continue;

                        var companyHouseData = JsonConvert.DeserializeObject<dynamic>(response.Content);
                        bool hasChanges = false;

                        // Compare name
                        string newName = companyHouseData.company_name;
                        if (company.Name != newName)
                        {
                            company.Name = newName;
                            hasChanges = true;
                        }

                        // Compare status
                        string newStatus = companyHouseData.company_status;
                        if (company.Status != newStatus)
                        {
                            company.Status = newStatus;
                            hasChanges = true;
                        }

                        // Handle accounts due date comparison
                        string newAccountsDueStr = companyHouseData.accounts?.next_due;
                        if (!string.IsNullOrEmpty(newAccountsDueStr))
                        {
                            DateTime newAccountsDueDate = DateTime.Parse(newAccountsDueStr);
                            string formattedNewDate = newAccountsDueDate.ToString("yyyy-MM-dd");

                            if (company.AccountsD != formattedNewDate)
                            {
                                company.AccountsD = formattedNewDate;
                                hasChanges = true;
                            }
                        }

                        // Handle confirmation statement due date comparison
                        string newCS01DueStr = companyHouseData.confirmation_statement?.next_due;
                        if (!string.IsNullOrEmpty(newCS01DueStr))
                        {
                            DateTime newCS01DueDate = DateTime.Parse(newCS01DueStr);
                            string formattedNewDate = newCS01DueDate.ToString("yyyy-MM-dd");

                            if (company.CS01D != formattedNewDate)
                            {
                                company.CS01D = formattedNewDate;
                                hasChanges = true;
                            }
                        }

                        if (hasChanges)
                        {
                            updatedCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error updating company {company.Number}: {ex.Message}");
                        continue;
                    }
                }

                if (updatedCount > 0)
                {
                    db.SaveChanges();
                }

                return new JsonResult(new
                {
                    status = true,
                    message = $"Updated {updatedCount} of {allCompanies.Count} companies",
                    updatedCount = updatedCount,
                    totalCount = allCompanies.Count
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    status = false,
                    message = "Error updating companies",
                    error = ex.Message
                });
            }
        }
    }
}
