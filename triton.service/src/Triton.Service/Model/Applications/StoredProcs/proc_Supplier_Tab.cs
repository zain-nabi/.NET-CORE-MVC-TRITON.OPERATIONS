namespace Triton.Service.Model.Applications.StoredProcs
{
    public class proc_Supplier_Tab
    {
        public int CategoryLCID { get; set; }
        public string SupplierName { get; set; }
        public int SupplierID { get; set; }
        public int Totals { get; set; }
        public bool VATVender { get; set; }
        public string SumSubtotal { get; set; }
    }
}
