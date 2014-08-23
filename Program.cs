using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FBPortal.Domain.Entities;
using FBPortal.Domain.Concrete;

namespace FileImport
{
    class Program
    {
        static void Main(string[] args)
        {
            string filePath = args[0];
            List<Product> products = new System.Collections.Generic.List<Product>();
            List<string> exceptions = new List<string>();

            VendorRepository vr = new VendorRepository();
            Vendor usFoods = vr.Vendors.Where(v => v.Name == "US Foods").First();
            CategoryRepository cr = new CategoryRepository();
            Category beef = cr.Categories.Where(c => c.Name == "Beef").First();

           model.ProductInfo.category = new Category() { ID = beef.ID, Name = beef.Name };
            model.ProductInfo.vendor = new Vendor() { ID = usFoods.ID, Name = usFoods.Name, Description = usFoods.Description, DateAdded = usFoods.DateAdded };

            using (System.IO.StreamReader rdr = new System.IO.StreamReader(filePath))
            {
                using (Microsoft.VisualBasic.FileIO.TextFieldParser tfp = new Microsoft.VisualBasic.FileIO.TextFieldParser(rdr))
                {
                    tfp.Delimiters = new string[] { "," };
                    tfp.CommentTokens = new string[] { "#" };
                    tfp.HasFieldsEnclosedInQuotes = true;

                    while (!tfp.EndOfData)
                    {
                        if (tfp.LineNumber == 1)
                        {
                            tfp.ReadFields();
                            continue;
                        }

                        string[] row = tfp.ReadFields();

                        try
                        {
                            Product p = model.ProductInfo.Parse(row);
                            products.Add(p);
                        }
                        catch (Exception)
                        {
                            exceptions.Add(string.Join("\t",row));
                        }
                        
                    }
                }
            }

            System.IO.File.WriteAllLines(@"c:\errors.txt", exceptions.ToArray());

            FBPortal.Domain.Concrete.ProductRepository pr = new FBPortal.Domain.Concrete.ProductRepository();
            pr.AddRange(products.ToArray());

            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }
}
