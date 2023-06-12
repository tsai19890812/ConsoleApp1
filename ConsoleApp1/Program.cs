using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;
using System.Linq;
using System.Xml;

class Program
{
 

   static void Main()
   {
      string sourceDirectory= "C:\\Users\\ASUS\\Desktop\\AA";
      string[] files = Directory.GetFiles(sourceDirectory);
      string targetDirectory = "D:\\AAB";
     try
      {
         Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
         System.Text.Encoding encode = System.Text.Encoding.GetEncoding(950);
         foreach (string file in files)
         {
            string fileName = Path.GetFileName(file); // 取得源檔案的名稱
            string xmlFileName = Path.ChangeExtension(fileName, ".xml"); // 將源檔案的副檔名改為 .xml

            string destinationFilePath = Path.Combine(targetDirectory, xmlFileName); // 生成目標 XML 檔案的路徑
            using (StreamReader sr = new StreamReader(file, encode))
            {
               string line;
               List<string[]> dataLines = new List<string[]>();

               while ((line = sr.ReadLine()) != null)
               {
                  string[] data = ProcessLine(line);
                  dataLines.Add(data);
               }
               ConvertTextToXml(dataLines, destinationFilePath);
            }
         }
      }
      catch (Exception ex)
      {
         Console.WriteLine("An error occurred: " + ex.Message);
      }
   }

   static string[] ProcessLine(string line)
   {
      Encoding encoding = Encoding.GetEncoding(950);
      byte[] bytes = encoding.GetBytes(line);

      string[] data = new string[13];
      data[0] = encoding.GetString(bytes, 0, 10);
      data[1] = encoding.GetString(bytes, 10, 10);
      data[2] = encoding.GetString(bytes, 20, 10);
      data[3] = encoding.GetString(bytes, 30, 12);
      data[4] = encoding.GetString(bytes, 42, 10);
      data[5] = encoding.GetString(bytes, 52, 10);
      data[6] = encoding.GetString(bytes, 62, 6);
      data[7] = encoding.GetString(bytes, 68, 6);
      data[8] = encoding.GetString(bytes, 74, 8);
      data[9] = encoding.GetString(bytes, 82, 3);
      data[10] = encoding.GetString(bytes, 85, 10);
      data[11] = encoding.GetString(bytes, 95, 50);
      data[12] = encoding.GetString(bytes, 145, 20);

      return data;
   }


   private static void ConvertTextToXml(List<string[]> dataLines , string destinationFilePath)
   {
      // 建立 XDocument 物件
      // 建立 XDocument 物件並設置 XML 宣告

      XDocument doc = new XDocument();


      // ...

      // 将 XML 声明作为字符串添加到 XML 文件开头
      string xmlDeclaration = @"<?xml version=""1.0"" encoding=""UTF-8""?>";
      doc.Declaration = new XDeclaration("1.0", "UTF-8", null);
      doc.AddFirst(new XComment(xmlDeclaration));

      XElement Batch = new XElement("Batch");
      doc.Add(Batch);// 目標xml 的 rootnode

      string yy = DateTime.Now.Year.ToString().Substring(2, 2);
      string mm = DateTime.Now.Month.ToString().Length == 2 ? DateTime.Now.Month.ToString() : "0" + DateTime.Now.Month.ToString();
      string dd = DateTime.Now.Day.ToString().Length == 2 ? DateTime.Now.Day.ToString() : "0" + DateTime.Now.Day.ToString();
      string hour = DateTime.Now.Hour.ToString().Length == 2 ? DateTime.Now.Hour.ToString() : "0" + DateTime.Now.Hour.ToString();
      string min = DateTime.Now.Minute.ToString().Length == 2 ? DateTime.Now.Minute.ToString() : "0" + DateTime.Now.Minute.ToString();
      string sec = DateTime.Now.Second.ToString().Length == 2 ? DateTime.Now.Second.ToString() : "0" + DateTime.Now.Second.ToString();

      string NowDate = yy + mm + dd;
      string NowTime = hour + min + sec;

      Batch.SetAttributeValue("time", NowTime);
      Batch.SetAttributeValue("date", NowDate);

      // 建立根節點

      // 建立子節點
      XElement fileName = new XElement("file");
      string fileName1 = NowDate + NowTime + ".xml";
      Batch.Add(fileName);
      fileName.SetAttributeValue("name", fileName1);

      var groupedPrescriptions = dataLines.GroupBy(dataLine => new { Days = dataLine[9], TTP = dataLine[8] });
     
      foreach (var group in groupedPrescriptions)
      {
         XElement prescription = null;
         XElement prescriptionItem = null;


         foreach (var dataLine in group)
         {
            if (prescription == null)
            {
               prescription = new XElement("Prescription");
               prescription.SetAttributeValue("PatientKey", "");
               prescription.SetAttributeValue("PrescriptionKey", "");
               prescription.SetAttributeValue("PatientID", dataLine[5]);
               prescription.SetAttributeValue("PatientName", dataLine[4]);
               prescription.SetAttributeValue("PatientSex", "");
               prescription.SetAttributeValue("PatientFrom", dataLine[3]);
               prescription.SetAttributeValue("PrescriptionDate", "");
               prescription.SetAttributeValue("PrescriptionTime", "");
               prescription.SetAttributeValue("WardBed", "");
               prescription.SetAttributeValue("PackNum", "");
               prescription.SetAttributeValue("TTP", dataLine[8]);
               prescription.SetAttributeValue("TTPText", "");
               prescription.SetAttributeValue("Barcode", "");

               fileName.Add(prescription);

               prescriptionItem = new XElement("PrescriptionItem");
               prescriptionItem.SetAttributeValue("DrugNo", "");
               prescriptionItem.SetAttributeValue("DrugName", dataLine[11]);
               prescriptionItem.SetAttributeValue("Dosage", dataLine[10]);
               prescriptionItem.SetAttributeValue("Days", dataLine[9]);
               prescriptionItem.SetAttributeValue("TTP", dataLine[8]);
               prescription.Add(prescriptionItem);
            }
            else
            {
               prescriptionItem = new XElement("PrescriptionItem");
               prescriptionItem.SetAttributeValue("DrugNo", "");
               prescriptionItem.SetAttributeValue("DrugName", dataLine[11]);
               prescriptionItem.SetAttributeValue("Dosage", dataLine[10]);
               prescriptionItem.SetAttributeValue("Days", dataLine[9]);
               prescriptionItem.SetAttributeValue("TTP", dataLine[8]);
               prescription.Add(prescriptionItem);
            }
         }


         

      }
     
      doc.Save(destinationFilePath);
      Console.WriteLine("XML file saved successfully.");
   }
   
}