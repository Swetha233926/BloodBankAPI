namespace BloodBankAPI.Models
{
    public class BloodBankEntry
    {


        public int Id { get; set; }
        public string DonorName { get; set; }
        public int Age { get; set; }
        public string BloodType { get; set; }
        public string ContactInfo { get; set; }
        public decimal Quantity { get; set; }
        public DateTime CollectionDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string GetBloodStatus { get; set; }


    }
}