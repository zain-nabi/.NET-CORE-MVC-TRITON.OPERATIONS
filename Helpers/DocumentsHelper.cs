using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using Triton.Model.TritonExpress.Tables;
using Triton.Operations.Models;

namespace Triton.Operations.Helpers
{
    public class DocumentsHelper
    {
        
        public static List<DownLoadPostalCodes> PostalCodesDTO(List<PostalCodes> postalCodes)
        {
            var dto = new List<DownLoadPostalCodes>();
            foreach (var postalCode in postalCodes)
            {
                var downLoad = new DownLoadPostalCodes
                {
                   PostalCodeID = postalCode.PostalCodeID,
                   PostalCode   = postalCode.PostalCode,
                   Name         = postalCode.Name,
                   Suburb       = postalCode.Suburb,
                   RateArea     = postalCode.RateArea,
                   BranchCode   = postalCode.BranchCode,
                   BayName      = postalCode.BayName,
                   BayNo        = postalCode.BayNo,
                   BayRoute     = postalCode.BayRoute,
                   KnownAs      = postalCode.KnownAs

                };

                dto.Add(downLoad);
            }

            return dto;
        }
        
        public static DataTable ConvertListToDataTable<T>(IEnumerable<T> listData)
        {
            var properties = TypeDescriptor.GetProperties(typeof(T));
            var values = new object[properties.Count];
            var table = new DataTable();

            for (var i = 0; i < properties.Count; i++)
            {
                var property = properties[i];
                table.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
            }

            foreach (var listItem in listData)
            {
                for (var i = 0; i < values.Length; i++)
                {
                    values[i] = properties[i].GetValue(listItem) ?? DBNull.Value;
                }

                table.Rows.Add(values);
            }

            return table;
        }

      

    }


}
