namespace Triton.Operations.Models
{
    public class DownLoadPostalCodes
    {
        public int PostalCodeID { get; set; }
        public string PostalCode { get; set; }
        public string Name { get; set; }
        public string Suburb { get; set; }
        public string RateArea { get; set; }
        public string BranchCode { get; set; }
        public string BayNo { get; set; }
        public string BayName { get; set; }
        public string BayRoute { get; set; }
        public string KnownAs { get; set; }
        public int? KMsFromBranch { get; set; }
    }
}
