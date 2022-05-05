using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Triton.Service.Model.HRM.Custom;
using Triton.Service.Model.LeaveManagement.Tables;
using Triton.Service.Utils;

namespace Triton.Service.Data
{
    public class OFOCodeService
    {
        public static async Task<List<OFOCodeCustomModel>> GetAllAsync()
        {
            return await RestApiHelper.GetAsync<List<OFOCodeCustomModel>>(new Uri(UrlHelper.Api.RecruitmentApi, $"{UrlHelper.Controller.OFOCode}GetAllAsync"));
        }

        public static async Task<bool> IsOFOCodeExistsAsync(string ofoCodeDescription, int year)
        {
            return await RestApiHelper.GetAsync<bool>(new Uri(UrlHelper.Api.RecruitmentApi, $"{UrlHelper.Controller.OFOCode}IsOFOCodeExistsAsync?ofoCodeDescription={ofoCodeDescription}&year={year}"));
        }

        public static async Task<OFOCode> GetByIdAsync(int ofoCodeId)
        {
            return await RestApiHelper.GetAsync<OFOCode>(new Uri(UrlHelper.Api.RecruitmentApi, $"{UrlHelper.Controller.OFOCode}GetByIdAsync?ofoCodeId={ofoCodeId}"));
        }

        public static async Task<bool> UpdateAsync(OFOCode ofoCode)
        {
            return await RestApiHelper.PutAsync(new Uri(UrlHelper.Api.RecruitmentApi, $"{UrlHelper.Controller.OFOCode}UpdateAsync"), ofoCode);
        }

        public static async Task<bool> InsertAsync(OFOCode ofoCode)
        {
            return await RestApiHelper.InsertAsync(new Uri(UrlHelper.Api.RecruitmentApi, $"{UrlHelper.Controller.OFOCode}InsertAsync"), ofoCode);
        }

        public static async Task<bool> DeleteAsync(OFOCode ofoCode)
        {
            return await RestApiHelper.PutAsync(new Uri(UrlHelper.Api.RecruitmentApi, $"{UrlHelper.Controller.OFOCode}DeleteAsync"), ofoCode);
        }

        public static async Task<List<OFOCode>> GetCoursesAsync()
        {
            return await RestApiHelper.GetAsync<List<OFOCode>>(new Uri(UrlHelper.Api.RecruitmentApi, $"{UrlHelper.Controller.OFOCode}GetOFOCodesAsync"));
        }

    }
}
