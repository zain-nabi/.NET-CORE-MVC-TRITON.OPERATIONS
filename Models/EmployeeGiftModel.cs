using System.Collections.Generic;
using Triton.Model.TritonOps.Tables;
using LookUpCodes = Triton.Model.TritonGroup.Tables.LookUpCodes;

namespace Triton.Operations.Models
{
    public class EmployeeGiftModel
    {
        public List<LookUpCodes> LookUpCodes { get; set; }
        public EmployeeGifts EmployeeGifts { get; set; }
        public string ErrorMessage { get; set; }
    }
}
