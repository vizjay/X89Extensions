using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace XA9Extensions
{
    public static class XA9XComTest
    {
        public static void Main()
        {
            string s = System.IO.File.ReadAllText(@"C:\Project\WORKING_FOLDER\LARGE_GROUPS\Extensions\Working\4. Testing\Test.xml");
            XDocument x = XDocument.Parse(s);
            FacetsData f = new FacetsData(s);
            
            string clcl_id = f["CDMLALL", "IPCD_ID", 1];

            FacetsData.

            f["CDMLALL", "IPCD_ID", 1] = "ABCD";
            XDocument.Parse(f.ToXml()).Save(@"C:\Project\WORKING_FOLDER\LARGE_GROUPS\Extensions\Working\4. Testing\TestModified.xml");
            string D = string.Empty;


            
        }
        
        
    }
}
