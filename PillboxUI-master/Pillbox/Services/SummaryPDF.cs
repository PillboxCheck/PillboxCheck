using NAudio.Wave;
using Pillbox.controls;
using Pillbox.entries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using System.IO;
using iText.IO.Font.Constants;
using iText.Kernel.Font;


namespace Pillbox.Services
{
    static class SummaryPDF
    {
        static public void GenerateSummaryPDF(bool IsInternal=false)
        {
            Persona user = SqliteDataAccess.LoadAll<Persona>("PERSONAL_DETAILS").FirstOrDefault();
            List<MedicationEntry> meds = SqliteDataAccess.LoadAll<MedicationEntry>("MEDICATIONS");
            List<Condition> conditions = SqliteDataAccess.LoadAll<Condition>("CONDITIONS");
            PdfFont boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
            // Save to Downloads folder
            string desktopPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            );
            string filePath = IsInternal ? Path.Combine(Application.StartupPath, "RAGSocket", $"SummaryReport.pdf")
                :Path.Combine(desktopPath, $"SummaryReport{DateTime.Now.Hour}{DateTime.Now.Minute}.pdf")
                ;
            if (File.Exists(filePath)) { File.Delete(filePath); }
            using var writer = new PdfWriter(filePath);
            using var pdf = new PdfDocument(writer);
            var doc = new iText.Layout.Document(pdf);

            doc.Add(new Paragraph($"Date: {DateTime.Now:dd/MM/yy}"));
            doc.Add(new Paragraph($"Time: {DateTime.Now:HH:mm}"));
            doc.Add(new Paragraph($"{user.Name} ({user.Age} years old)").SetFont(boldFont));
            doc.Add(new Paragraph($"Patient weight: {user.Weight}, Patient height: {user.Height}, Patient Gender:{user.Gender})").SetFont(boldFont));

            // Continuous Meds
            doc.Add(new Paragraph("Continuous Medication").SetFont(boldFont));
            Table table1 = new Table(3).UseAllAvailableWidth();
            table1.AddHeaderCell("Name").AddHeaderCell("Dose").AddHeaderCell("Prescription");
            foreach (var med in meds.Where(m => m.IsOngoing))
            {
                table1.AddCell(med.MedicationName);
                table1.AddCell(med.Dose.ToString()+med.DoseUnit);
                table1.AddCell($"{med.Instruction}");
            }
            doc.Add(table1);

            // Temporal Meds
            doc.Add(new Paragraph("\nTemporal Medication").SetFont(boldFont));
            Table table2 = new Table(5).UseAllAvailableWidth();
            table2.AddHeaderCell("Name").AddHeaderCell("Dose").AddHeaderCell("Last Taken")
                  .AddHeaderCell("Ongoing").AddHeaderCell("Prescription");
            foreach (var med in meds.Where(m => !m.IsOngoing))
            {

                table2.AddCell(med.MedicationName);
                table2.AddCell(med.Dose.ToString()+med.DoseUnit);
                table2.AddCell(med.EndDate);
                table2.AddCell(med.IsOngoing ? "Yes" : "No");
                table2.AddCell($"{med.Instruction}");
            }
            doc.Add(table2);

            // Add allergies, conditions, and disclaimer
            doc.Add(new Paragraph("\nAllergies and condtions").SetFont(boldFont));
            //doc.Add(new Paragraph($"EPI Owned: {(user.HasEpiPen ? "Yes" : "No")}\nExpire: {user.EpiPenExpire:dd/MM/yyyy}"));

            //doc.Add(new Paragraph("\nAdditional information").SetFont(boldFont));
            foreach (var cond in conditions)
                doc.Add(new Paragraph($"-{cond.Type}: {cond.Name} ~ {cond.Severity} "));

            doc.Add(new Paragraph("\nDISCLAIMER: This report is automatically built using the data obtained from the user. It may not contain all details").SetFontSize(8));

            doc.Close();
            if (!IsInternal) { MessageBox.Show($"File saved in {filePath}"); }
            
        }
    }
}