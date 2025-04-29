using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tesseract;
using static Pillbox.Utils;
using Pillbox.Managers;

namespace Pillbox.controls.Forms
{

    public partial class FileBrowser : Form
    {
        // Set the starting folder to "RAGSOCKET\LocalRepository" under your app's startup path.
        private string initialDirectory;
        // Use filePath to hold the current absolute path (initially starting at initialDirectory).
        private string filePath;
        private bool isFile = false;
        private string currentlySelectedItemName = "";
        private bool repositoryModified = false;
        private bool isPersonal;
        private CommunicationManager communicationManager;
        public FileBrowser(string relativePathToApp, CommunicationManager commsManager,  bool isPersonal=false)
        {
            InitializeComponent();
            // Set the current absolute path to the initial directory.
            initialDirectory = Path.Combine(Application.StartupPath, relativePathToApp);
            filePath = initialDirectory;
            this.FormClosing += File_FormClosing;
            this.isPersonal = isPersonal;
            communicationManager = commsManager;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Display a relative path (will be "." initially) using the initialDirectory as the base.
            filePathTextBox.Text = Path.GetRelativePath(initialDirectory, filePath);
            loadFilesAndDirectories();
        }

        public void loadFilesAndDirectories()
        {
            DirectoryInfo fileList;
            try
            {
                FileAttributes fileAttr = File.GetAttributes(filePath);

                if (isFile)
                {
                    // If a file was selected, build its absolute path and show details.
                    string tempFilePath = Path.Combine(filePath, currentlySelectedItemName);
                    FileInfo fileDetails = new FileInfo(tempFilePath);
                    fileNameLabel.Text = fileDetails.Name;
                    fileTypeLabel.Text = fileDetails.Extension;
                    fileAttr = File.GetAttributes(tempFilePath);
                    Process.Start(tempFilePath);
                }

                if ((fileAttr & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    fileList = new DirectoryInfo(filePath);
                    FileInfo[] files = fileList.GetFiles(); // Get all files.
                    DirectoryInfo[] dirs = fileList.GetDirectories(); // Get all directories.
                    listView1.Items.Clear();

                    // Add files to the list.
                    foreach (FileInfo file in files)
                    {
                        string fileExtension = file.Extension.ToUpper();
                        switch (fileExtension)
                        {
                            case ".MP3":
                            case ".MP2":
                                listView1.Items.Add(file.Name, 5);
                                break;
                            case ".EXE":
                            case ".COM":
                                listView1.Items.Add(file.Name, 7);
                                break;
                            case ".MP4":
                            case ".AVI":
                            case ".MKV":
                                listView1.Items.Add(file.Name, 6);
                                break;
                            case ".PDF":
                                listView1.Items.Add(file.Name, 4);
                                break;
                            case ".DOC":
                            case ".DOCX":
                                listView1.Items.Add(file.Name, 3);
                                break;
                            case ".PNG":
                            case ".JPG":
                            case ".JPEG":
                                listView1.Items.Add(file.Name, 9);
                                break;
                            default:
                                listView1.Items.Add(file.Name, 8);
                                break;
                        }
                    }

                    // Add directories to the list.
                    foreach (DirectoryInfo dir in dirs)
                    {
                        listView1.Items.Add(dir.Name, 10);
                    }
                }
                else
                {
                    fileNameLabel.Text = this.currentlySelectedItemName;
                }
            }
            catch (Exception ex)
            {
                // Add error handling as needed.
            }
        }

        public void loadButtonAction()
        {
            // Convert the relative path shown in the TextBox back to an absolute path.
            string relativePath = filePathTextBox.Text.Trim();
            if (string.IsNullOrEmpty(relativePath) || relativePath == ".")
            {
                filePath = initialDirectory;
            }
            else
            {
                filePath = Path.Combine(initialDirectory, relativePath);
            }
            // Normalize and update the TextBox so it always displays a relative path.
            filePathTextBox.Text = Path.GetRelativePath(initialDirectory, filePath);
            loadFilesAndDirectories();
            isFile = false;
        }

        public void goBack()
        {
            try
            {
                // Use Directory.GetParent to compute the parent folder.
                DirectoryInfo parentDir = Directory.GetParent(filePath);
                // Only allow navigation within the initialDirectory tree.
                if (parentDir != null && parentDir.FullName.StartsWith(initialDirectory))
                {
                    filePath = parentDir.FullName;
                }
                // Update the text box to show the relative path.
                filePathTextBox.Text = Path.GetRelativePath(initialDirectory, filePath);
            }
            catch (Exception ex)
            {
                // Add error handling as needed.
            }
        }

        private void goButton_Click(object sender, EventArgs e)
        {
            loadButtonAction();
        }

        private void listView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            currentlySelectedItemName = e.Item.Text;
            string newAbsolutePath = Path.Combine(filePath, currentlySelectedItemName);

            // Check if the file or directory still exists
            if (!File.Exists(newAbsolutePath) && !Directory.Exists(newAbsolutePath))
            {
                //MessageBox.Show("The selected item no longer exists. Refreshing the list.",
                //                "File Not Found",
                //                MessageBoxButtons.OK,
                //                MessageBoxIcon.Warning);
                loadFilesAndDirectories();
                return;
            }

            // Safe to get attributes now that we've verified existence
            FileAttributes fileAttr = File.GetAttributes(newAbsolutePath);
            if ((fileAttr & FileAttributes.Directory) == FileAttributes.Directory)
            {
                isFile = false;
                filePath = newAbsolutePath;
                filePathTextBox.Text = Path.GetRelativePath(initialDirectory, filePath);
            }
            else
            {
                isFile = true;
            }
        }


        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            loadButtonAction();
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            goBack();
            loadButtonAction();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Ensure that an item is selected.
            if (string.IsNullOrWhiteSpace(currentlySelectedItemName))
            {
                MessageBox.Show("No file is selected.", "Delete File", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Check if the currently selected item is a file.
            if (isFile)
            {
                // Build the full path for the file to be deleted.
                string fileToDelete = Path.Combine(filePath, currentlySelectedItemName);

                // Prompt the user for confirmation.
                DialogResult result = MessageBox.Show(
                    $"Are you sure you want to delete the file \"{currentlySelectedItemName}\"?",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        // Delete the file.
                        File.Delete(fileToDelete);

                        repositoryModified = true; // Mark the repository as modified.

                        // Inform the user.
                        MessageBox.Show("File deleted successfully.", "Delete File", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Refresh the list view to reflect changes.
                        loadFilesAndDirectories();

                        // Reset the selected file flag and name.
                        isFile = false;
                        currentlySelectedItemName = string.Empty;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred while deleting the file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                // Inform the user that the deletion action is only applicable to files.
                MessageBox.Show("The selected item is a directory and cannot be deleted with this action.", "Delete File", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void AddFileButton_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog())
            {
                dlg.Title = "Select a file to add";
                dlg.Filter = "JSON (*.json)|*.json|PDF (*.pdf)|*.pdf|Text (*.txt)|*.txt|MD (*.md)|*.md|Image (*.png;*.jpg)|*.png;*.jpg";

                if (dlg.ShowDialog() != DialogResult.OK) return;

                // 1) get the original
                string sourceFile = dlg.FileName;
                string tempSourceFile = sourceFile;
                string destinationFile = Path.Combine(filePath, Path.GetFileName(sourceFile));

                // 2) if it's an image, convert it to PDF
                var ext = Path.GetExtension(sourceFile).ToUpperInvariant();
                if (ext == ".JPG" || ext == ".PNG")
                {
                    tempSourceFile = Path.Combine(TransformFile(sourceFile));
                    destinationFile = Path.Combine(filePath, Path.GetFileName(tempSourceFile));
                    SpinWait.SpinUntil(() => File.Exists(tempSourceFile), TimeSpan.FromSeconds(10));
                }

                try
                {
                    // 3) ensure destination folder exists
                    if (!Directory.Exists(filePath))
                        Directory.CreateDirectory(filePath);

                    // 4) check overwrite
                    if (File.Exists(destinationFile))
                    {
                        var msg = $"A file named \"{Path.GetFileName(destinationFile)}\" already exists. Overwrite?";
                        if (MessageBox.Show(msg, "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                            != DialogResult.Yes)
                            return;
                    }
                    Debug.WriteLine("Source File: " + sourceFile);
                    Debug.WriteLine("Temp Source File: " + tempSourceFile); 
                    Debug.WriteLine("TempfileExisits: " + File.Exists(tempSourceFile));
                    // 5) copy the correct file
                    File.Copy(tempSourceFile, destinationFile, overwrite: true);
                    MessageBox.Show("File copied successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // 6) if we did create a temp PDF, clean it up
                    if (tempSourceFile != sourceFile && File.Exists(tempSourceFile))
                        File.Delete(tempSourceFile);
                    if (File.Exists(destinationFile) && destinationFile.EndsWith(".pdf")){
                        communicationManager.SendMessage(destinationFile, ChannelOwner.Event);
                    }
                    repositoryModified = true;
                    loadFilesAndDirectories();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error copying file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void File_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.DialogResult = repositoryModified ? DialogResult.OK : DialogResult.Cancel;
        }

        //private string TransformFile(string filePath)
        //{
            
        //    string tessdataPath = Application.StartupPath;
        //    if (tessdataPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
        //    {
        //        tessdataPath = tessdataPath.TrimEnd(Path.DirectorySeparatorChar);
        //    }
        //    Debug.WriteLine("Tessdata Path: " + tessdataPath);
        //    // Specify the languages (adjust as needed)
        //    //string languages = "ara+chi_sim+deu+eng+fra+ita+jpn+kor+nld+por+spa";
        //    string languages = "eng";
            
        //    // Generate a PDF file path by replacing the image extension with ".pdf"
        //    string outputPdfPath = Path.ChangeExtension(filePath,null);

        //    // Create the PDF renderer which creates a searchable PDF at outputPdfPath.
        //    using (IResultRenderer renderer = Tesseract.PdfResultRenderer.CreatePdfRenderer(outputPdfPath, tessdataPath, false))
        //    {
        //        Debug.WriteLine("Renderer Path: " + outputPdfPath);
        //        // Begin the PDF document with a title.
        //        using (renderer.BeginDocument($"File Added on the: {DateTime.Now.ToString()}"))
        //        {
        //            Debug.WriteLine("Renderer Begin Document: " + outputPdfPath);
        //            // Create the Tesseract engine using the provided configuration and language settings.
        //            using (TesseractEngine engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default))
        //            {
        //                Debug.WriteLine("Tesseract Engine Path: " + tessdataPath);
        //                // Load the image file.
        //                using (var img = Pix.LoadFromFile(filePath))
        //                {
        //                    Debug.WriteLine("Image Path: " + filePath);
        //                    // Process the image with Tesseract.
        //                    // The second parameter here (languages) can also be used as a page segmentation override if needed.
        //                    using (var page = engine.Process(img, languages))
        //                    {
        //                        Debug.WriteLine("Page Processed: " + filePath);
        //                        // Add the page (with its OCR data) to the PDF.
        //                        renderer.AddPage(page);
        //                    }
        //                }
        //            }
        //        }
        //        renderer.Dispose();
        //    }
           
        //    // Return the full path to the generated PDF.
        //    return outputPdfPath+".pdf";
        //}
    }
}