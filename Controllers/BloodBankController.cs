using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BloodBankAPI.Models;

namespace BloodBankAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BloodBankController : ControllerBase
    {
        static List<BloodBankEntry> BloodDonors = new List<BloodBankEntry>
        {
            new BloodBankEntry{Id=1,DonorName="Swetha",Age=21,BloodType="B-",ContactInfo="7842265930",Quantity=3,CollectionDate=DateTime.Now,ExpirationDate=Convert.ToDateTime("2024-11-22"),GetBloodStatus="Available"},
            new BloodBankEntry{Id=2,DonorName="Shirisha",Age=24,BloodType="B+",ContactInfo="Shirisha@gmail.com",Quantity=1,CollectionDate=Convert.ToDateTime("2024-11-20"),ExpirationDate=Convert.ToDateTime("2024-11-22"),GetBloodStatus="Available"}
        };

        // Utility method to determine the blood status
        private string DetermineBloodStatus(BloodBankEntry donor)
        {
            if (donor.ExpirationDate < DateTime.Now)
            {
                return "Expired";
            }

            if (donor.ExpirationDate > DateTime.Now && donor.GetBloodStatus != "Requested")
            {
                return "Available";
            }

            return "Requested"; 
        }

        // 1. Get All Donors information
        [HttpGet]
        public ActionResult<IEnumerable<BloodBankEntry>> GetAllDonors()
        {
            return BloodDonors;
        }

        // 2. Get the Donor by ID
        [HttpGet("{id}")]
        public ActionResult<BloodBankEntry> GetDonorByID(int id)
        {
            var donor = BloodDonors.Find(x => x.Id == id);
            if (donor == null)
            {
                return NotFound();
            }
            return donor;
        }

        // 3. Adding single donor details
        [HttpPost]
        public ActionResult<BloodBankEntry> AddDonor(BloodBankEntry b)
        {
            // Validate input details
            var validBloodTypes = new List<string> { "A+", "A-", "B+", "B-", "O+", "O-", "AB+", "AB-" };

            if (string.IsNullOrEmpty(b.DonorName) ||
                b.Age <= 0 ||
                b.Quantity <= 0 ||
                string.IsNullOrEmpty(b.ContactInfo) ||
                b.ExpirationDate == default ||
                b.ExpirationDate<b.CollectionDate||
                !validBloodTypes.Contains(b.BloodType, StringComparer.OrdinalIgnoreCase))
            {
                return BadRequest("Please enter all the required details and ensure all fields data is correct.");
            }
            // Set auto-generated fields
            b.Id = BloodDonors.Any() ? BloodDonors.Max(i => i.Id) + 1 : 1;
            b.CollectionDate = DateTime.Now;
            b.GetBloodStatus = DetermineBloodStatus(b); // Automatically assign blood status

            // Add the donor to the list
            BloodDonors.Add(b);

            return CreatedAtAction(nameof(GetDonorByID), new { id = b.Id }, b);
        }

        // 4. Add multiple donor details
        [HttpPost("bulk")]
        public ActionResult AddDonors(List<BloodBankEntry> donors)
        {
            var validBloodTypes = new List<string> { "A+", "A-", "B+", "B-", "O+", "O-", "AB+", "AB-" };

            if (donors == null || !donors.Any())
            {
                return BadRequest("The list is empty.");
            }

            var successfullyAdded = new List<BloodBankEntry>();
            var failedDonors = new List<string>();

            foreach (var donor in donors)
            {
                if (string.IsNullOrEmpty(donor.DonorName) ||
                    !validBloodTypes.Contains(donor.BloodType, StringComparer.OrdinalIgnoreCase) ||
                    donor.Age <= 0 ||
                    donor.Quantity <= 0 ||
                    string.IsNullOrEmpty(donor.ContactInfo) ||
                    donor.ExpirationDate == default)
                {
                    failedDonors.Add($"DonorName: {donor.DonorName}, BloodType: {donor.BloodType}");
                    continue;
                }

                donor.Id = BloodDonors.Any() ? BloodDonors.Max(i => i.Id) + 1 : 1;
                donor.CollectionDate = DateTime.Now;
                donor.GetBloodStatus = DetermineBloodStatus(donor); // Automatically assign blood status
                BloodDonors.Add(donor);
                successfullyAdded.Add(donor);
            }

            if (!successfullyAdded.Any())
            {
                return BadRequest(new
                {
                    Message = "All donors failed validation.",
                    FailedDonors = failedDonors
                });
            }

            return Ok(new
            {
                Message = "Batch processing completed.",
                SuccessfullyAdded = successfullyAdded,
                FailedDonors = failedDonors
            });
        }


        // 5. Update Blood donor details
        [HttpPut("{id}")]
        public IActionResult UpdateDonor(int id, BloodBankEntry b)
        {
            var donor = BloodDonors.Find(x => x.Id == id);
            if (donor == null)
            {
                return NotFound();
            }

            var validBloodTypes = new List<string> { "A+", "A-", "B+", "B-", "O+", "O-", "AB+", "AB-" };

            if (string.IsNullOrEmpty(b.DonorName) ||
                b.Age <= 0 ||
                b.Quantity <= 0 ||
                string.IsNullOrEmpty(b.ContactInfo) ||
                b.ExpirationDate == default ||
                !validBloodTypes.Contains(b.BloodType))
            {
                return BadRequest("Please enter all the required details.");
            }

            donor.DonorName = b.DonorName;
            donor.Age = b.Age;
            donor.Quantity = b.Quantity;
            donor.BloodType = b.BloodType;
            donor.ContactInfo = b.ContactInfo;
            donor.ExpirationDate = b.ExpirationDate;
            donor.GetBloodStatus = DetermineBloodStatus(b); // Automatically update blood status
            donor.CollectionDate = DateTime.Now;

            return CreatedAtAction(nameof(GetDonorByID), new { id = b.Id }, donor);
        }

        // 6. Delete Blood donor details
        [HttpDelete("{id}")]
        public IActionResult DeleteDonor(int id)
        {
            var donor = BloodDonors.Find(x => x.Id == id);
            if (donor == null)
            {
                return NotFound();
            }
            BloodDonors.Remove(donor);
            return NoContent();
        }

        // 7. Delete multiple donor details
        [HttpDelete("bulk")]
        public IActionResult DeleteDonors(List<int> ids)
        {
            if (ids== null || !ids.Any())
            {
                return BadRequest("The list of donor IDs is empty.");
            }

            var donors_not_found = new List<int>();
            foreach (var id in ids)
            {
                var donor = BloodDonors.Find(x => x.Id == id);
                if (donor == null)
                {
                    donors_not_found.Add(id);
                }
                BloodDonors.Remove(donor);
            }
            if (donors_not_found.Any())
            {
                return Ok(new
                {
                    Message = "Some donors were not found.",
                    NotFoundDonors = donors_not_found
                });
            }

            return Ok("All donors were successfully deleted.");

        }

        //8. Pagination 
        [HttpGet("page")]
        public ActionResult<IEnumerable<BloodBankEntry>> GetPageDate(int page = 1, int size = 10)
        {
            var res = BloodDonors.Skip((page - 1) * size).Take(size).ToList();
            return res;
        }

        //9. Search for blood bank entries based on blood type
        [HttpGet("Bloodtype")]
        public ActionResult<IEnumerable<BloodBankEntry>> GetDonorsByBloodType(string bloodtype)
        {
            var validBloodTypes = new List<string> { "A+", "A-", "B+", "B-", "O+", "O-", "AB+", "AB-" };
            if (!validBloodTypes.Contains(bloodtype, StringComparer.OrdinalIgnoreCase))
            {
                return BadRequest("Enter the blood type correctly");
            }

            List<BloodBankEntry> donors = BloodDonors.FindAll(x =>
                string.Equals(x.BloodType, bloodtype, StringComparison.OrdinalIgnoreCase));

            if (donors.Count == 0)
            {
                return NotFound($"No donors found with blood type {bloodtype}."); // Return message if no donors found
            }

            return Ok(donors);
        }

        //10. Search for blood bank entries by status
        [HttpGet("status")]
        public ActionResult<IEnumerable<BloodBankEntry>> GetDonorsByStatus(string status)
        {
            var available_status = new List<string> {"Available","Requested","Expired"};
            if (!available_status.Contains(status, StringComparer.OrdinalIgnoreCase))
            {
                return BadRequest("Enter any of the following status {Available, Expired, Requested}");
            }
            List<BloodBankEntry> donors = BloodDonors.FindAll(x => string.Equals(
                x.GetBloodStatus, status, StringComparison.OrdinalIgnoreCase));
            if (donors.Count == 0)
            {
                return NotFound($"The donors with the {status} status are not found");
            }
            return Ok(donors);
        }

        //11. Search for donors by Name
        [HttpGet("Name")]
        public ActionResult<IEnumerable<BloodBankEntry>> GetDonorsByName(string Name)
        {
            List<BloodBankEntry> donors = BloodDonors.FindAll(
                x => string.Equals(x.DonorName.Trim(), Name.Trim(), StringComparison.OrdinalIgnoreCase)
                );
            if (donors.Count == 0)
            {
                return NotFound($"There are no donors with specific name {Name}");
            }
            return Ok(donors);
        }

    }
}