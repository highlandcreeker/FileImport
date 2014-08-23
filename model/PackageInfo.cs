using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FBPortal.Domain.Entities;
using FBPortal.Domain.Concrete;

namespace FileImport.model
{
    public static class ProductInfo
    {
        public static Vendor vendor { get; set; }
        public static Category category { get; set; }
        public static Product Parse(string[] row)
        {
            int lbIndex = 0, ozIndex = 0;
            string lbData, ozData;
            Product p = new Product();

            string packageData = row[4];

            lbIndex = packageData.ToLower().IndexOf("l");
            ozIndex = packageData.ToLower().IndexOf("o");

            if (lbIndex > 0)
            {
                lbData = packageData.Substring(lbIndex).ToLower();

                if (lbData.Contains("a")) p.PackageTypeCode = EPackageType.LBA;
                else if (lbData.Contains("+")) p.PackageTypeCode = EPackageType.LBPlus;
                else p.PackageTypeCode = EPackageType.LB;
            }
            else if (ozIndex > 0)
            {
                ozData = packageData.Substring(ozIndex).ToLower();

                if (ozData.Contains("a")) p.PackageTypeCode = EPackageType.OZA;
                else p.PackageTypeCode = EPackageType.OZ;
            }

            /*
              Example of quantity and weight in csv
             
              4/17 LBA
              109/2.2 OZ
              71/2.25 OZ
          */

            int unitOfMeasureIndex = (lbIndex > 0) ? lbIndex : ozIndex;

            packageData = packageData.Substring(0, (unitOfMeasureIndex - 1)).Replace("\\", "/");
            int index = packageData.IndexOf("/");
            int quantity = 1;
            decimal weight = 0;
          
            if (index > 0)
            {
                string _quantity = packageData.Substring(0, (index - 1));
                string _weight = packageData.Substring(index).Replace("/","");

                quantity = (int.TryParse(_quantity, out quantity)) ? quantity : 1;
                weight = (decimal.TryParse(_weight, out weight)) ? weight : 0;
            }
            else
            {
                weight = (decimal.TryParse(packageData, out weight)) ? weight : 0;
            }

            p.Weight = weight;
            p.Quantity = quantity;
            
            p.PackageType = Enum.GetName(typeof(EPackageType), p.PackageTypeCode);

            p.Number = row[1];
            p.Description = row[2];
            p.Brand = row[3];
            p.Vendor = vendor;
            p.Category = category;
            p.DateAdded = DateTime.UtcNow;

            decimal price = 0;
            p.Price = decimal.TryParse(row[6], out price) ? price : 0;

            return p;
        }
    }
}
