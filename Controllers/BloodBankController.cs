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
            new BloodBankEntry{Id=1,DonorName="Swetha",Age=21,BloodType="B-",ContactInfo="7842265930",Quantity=3,CollectionDate=DateTime.Now,ExpirationDate=Convert.ToDateTime("2024-11-22"),Status="Available"},
            new BloodBankEntry{Id=2,DonorName="Shirisha",Age=24,BloodType="B+",ContactInfo="Shirisha@gmail.com",Quantity=1,CollectionDate=Convert.ToDateTime("2024-11-20"),ExpirationDate=Convert.ToDateTime("2024-11-22"),Status="Requested"}
        };

        //1. Get All Donors info
        [HttpGet]
        public ActionResult<IEnumerable<BloodBankEntry>> GetAllDonors()
        {
            return BloodDonors;
        }

        //2. Get the Donor by ID
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

        //3. Adding single donor details
        [HttpPost]
        public ActionResult<BloodBankEntry> AddDonor(BloodBankEntry b)
        {
            b.Id = BloodDonors.Any() ? BloodDonors.Max(i => i.Id) + 1 : 1;
            BloodDonors.Add(b);
            b.CollectionDate = DateTime.Now;
            return CreatedAtAction(nameof(GetDonorByID),new {id=b.Id},b);
        }

        //4. Post api blood bank adding details
        [HttpPost("bulk")]
        public ActionResult<IEnumerable<BloodBankEntry>> AddDonors(List<BloodBankEntry> b)
        {
            if(b==null || !b.Any())
            {
                return BadRequest("The list is empty.");
            }

            foreach(var donor in b)
            {
                donor.Id = BloodDonors.Max(i => i.Id) + 1;
                donor.CollectionDate = DateTime.Now;
                BloodDonors.Add(donor);
            }
            return CreatedAtAction(nameof(GetAllDonors), BloodDonors);
        }

        //5. Update Blood donor result
        [HttpPut("{id}")]
        public IActionResult UpdateDonor(int id,BloodBankEntry b)
        {
            var donor = BloodDonors.Find(x => x.Id == id);
            if (donor == null)
            {
                return NotFound();
            }
            donor.CollectionDate = DateTime.Now;
            donor.DonorName = b.DonorName;
            donor.Age = b.Age;
            donor.Quantity = b.Quantity;
            donor.BloodType = b.BloodType;
            donor.CollectionDate = b.CollectionDate;
            donor.ExpirationDate = b.ExpirationDate;
            donor.ContactInfo = b.ContactInfo;
            donor.Status = b.Status;
            return NoContent();
        }

        //6. Delete Blood donor details
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

        //7. delete bulk blood donors
        [HttpDelete("bulk")]
        public IActionResult DeleteDonors(List<int> id)
        {
            foreach(var donor in id)
            {
                var s = BloodDonors.Find(x => x.Id ==donor);
                if (s == null)
                {
                    return NotFound();
                }
                BloodDonors.Remove(s);
            }
            return NoContent();
        }

    }
}
