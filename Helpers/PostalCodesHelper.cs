using System;
using Triton.Model.TritonExpress.Tables;
using Triton.Service.Model.TritonExpress.Custom;

namespace Triton.Operations.Helpers
{
    public class PostalCodesHelper
    {
        private static int? _postalCodeStatusID = 1;
        private static int? _rateAreaID = null;


        public static PostalCodes CreatePostalCode(PostalCodesModel postalCodesModel, int userId)
        {
            return new PostalCodes
            {
                ActionedOn = DateTime.Now,
                Active = true,
                ApprovalUserID = userId,
                BayName = postalCodesModel.PostalCodes.BayName,
                BayNo = postalCodesModel.PostalCodes.BayNo,
                BayRoute = postalCodesModel.PostalCodes.BayRoute,
                BranchCode = postalCodesModel.SelectedBranchCode,
                KnownAs = postalCodesModel.PostalCodes.KnownAs,
                Name = postalCodesModel.PostalCodes.Name,
                PostalCode = postalCodesModel.PostalCodes.PostalCode,
                PostalCodeRequestID = null,
                PostalCodeStatusID = postalCodesModel.SelectedPostalCodeStatusId == 0 ? _postalCodeStatusID.Value : postalCodesModel.SelectedPostalCodeStatusId,
                PostalCodeTransitTimeID = postalCodesModel.SelectedPostalCodeTransitTimeId,
                RateArea = postalCodesModel.PostalCodes.RateArea,
                RateAreaID = postalCodesModel.PostalCodes.RateAreaID.HasValue ? _rateAreaID : postalCodesModel.PostalCodes.RateAreaID,
                ServicedByLookUpCodeID = postalCodesModel.SelectedLookupcodesId,
                Suburb = postalCodesModel.PostalCodes.Suburb,
                PostalCodeID = postalCodesModel.PostalCodes.PostalCodeID
            };
        }
    }
}
