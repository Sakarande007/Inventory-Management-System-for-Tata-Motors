using System;
using System.IO;
using System.Text;

namespace PaintShopIMS.Helpers
{
    public static class SystemSetupHelper
    {
        public static void Initialize()
        {
            string basePath = @"C:\PaintShopIMS";
            string templatePath = Path.Combine(basePath, "PrintTemplates");
            string queuePath = Path.Combine(basePath, "PrintQueue");

            Directory.CreateDirectory(templatePath);
            Directory.CreateDirectory(queuePath);

            // Create default label.txt if not exists
            string labelFile = Path.Combine(templatePath, "label.txt");
            if (!File.Exists(labelFile))
            {
                File.WriteAllText(labelFile, GetDefaultTemplate(), Encoding.ASCII);
            }

            // Create print.bat if not exists
            string batFile = Path.Combine(queuePath, "print.bat");
            if (!File.Exists(batFile))
            {
                File.WriteAllText(batFile, GetDefaultBat(), Encoding.ASCII);
            }
        }

        private static string GetDefaultTemplate()
        {
            return @"^L
W80,50,5,2,M0,8,5,10,0
@QRDATA

AD,250,49,1,1,0,0E,Part No: @PartNo
AD,247,106,1,1,0,0E,RW: @Rewcode
E";
        }

        private static string GetDefaultBat()
        {
            return @"@echo off
copy /b C:\PaintShopIMS\PrintQueue\print.txt \\localhost\YOUR_PRINTER_NAME";
        }
    }
}
