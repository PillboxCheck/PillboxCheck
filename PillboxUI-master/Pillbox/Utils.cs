using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Tesseract;

namespace Pillbox
{
    public static class Utils
    {

        public static bool IsAppRunning(string processName)
        {
            // Retrieve an array of all processes running with the specified name
            Process[] processes = Process.GetProcessesByName(processName);
            return processes.Length > 0;
        }

        public static int getHeightText(Label lbl)
        {
            using (Graphics g = lbl.CreateGraphics())
            {
                SizeF size = g.MeasureString(lbl.Text, lbl.Font,lbl.Width);
                return (int)Math.Ceiling(size.Height);
            }
        }

        public static byte[] ReceiveAll(NetworkStream networkStream)
        {
            var buffer = new List<byte>();
            var tempBuffer = new byte[1];

            while (networkStream.DataAvailable)
            {
                int bytesRead = networkStream.Read(tempBuffer, 0, tempBuffer.Length);

                if (bytesRead > 0)
                {
                    buffer.Add(tempBuffer[0]);
                }
            }

            return buffer.ToArray();
        }

        public class GenericClass<T>
        {
            private T data;

            public void SetData(T value)
            {
                data = value;
            }

            public T GetData()
            {
                return data;
            }
        }

        public static string TransformFile(string filePath)
        {

            string tessdataPath = Application.StartupPath;
            if (tessdataPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                tessdataPath = tessdataPath.TrimEnd(Path.DirectorySeparatorChar);
            }
            Debug.WriteLine("Tessdata Path: " + tessdataPath);
            // Specify the languages (adjust as needed)
            //string languages = "ara+chi_sim+deu+eng+fra+ita+jpn+kor+nld+por+spa";
            string languages = "eng";

            // Generate a PDF file path by replacing the image extension with ".pdf"
            string outputPdfPath = Path.ChangeExtension(filePath, null);

            // Create the PDF renderer which creates a searchable PDF at outputPdfPath.
            using (IResultRenderer renderer = Tesseract.PdfResultRenderer.CreatePdfRenderer(outputPdfPath, tessdataPath, false))
            {
                Debug.WriteLine("Renderer Path: " + outputPdfPath);
                // Begin the PDF document with a title.
                using (renderer.BeginDocument($"File Added on the: {DateTime.Now.ToString()}"))
                {
                    Debug.WriteLine("Renderer Begin Document: " + outputPdfPath);
                    // Create the Tesseract engine using the provided configuration and language settings.
                    using (TesseractEngine engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.TesseractAndLstm))
                    {
                        Debug.WriteLine("Tesseract Engine Path: " + tessdataPath);
                        // Load the image file.
                        using (var img = Pix.LoadFromFile(filePath))
                        {
                            Debug.WriteLine("Image Path: " + filePath);
                            // Process the image with Tesseract.
                            // The second parameter here (languages) can also be used as a page segmentation override if needed.
                            using (var page = engine.Process(img, languages))
                            {
                                Debug.WriteLine("Page Processed: " + filePath);
                                // Add the page (with its OCR data) to the PDF.
                                renderer.AddPage(page);
                            }
                        }
                    }
                }
                renderer.Dispose();
            }

            // Return the full path to the generated PDF.
            return outputPdfPath + ".pdf";
        }


    }

}
