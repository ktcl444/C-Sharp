using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using PortableOpenOffice;

namespace DocToPDF
{
    class DocToPDF2
    {
        static string sLastError = "";

        public static bool ConvertWordToPdf(string inputFilename, string outputFilename, bool bShow)
        {
            object oFile = inputFilename;
            object oFalse = false;
            object oTrue = true;

            object oMissing = System.Type.Missing;
            object pageBreak = Microsoft.Office.Interop.Word.WdBreakType.wdPageBreak;
            object outputFile = outputFilename;
            string tempFile = System.IO.Path.ChangeExtension(outputFilename, ".ps");
            object oTempFile = tempFile;
            if (File.Exists(tempFile) == true)
            {
                try
                {
                    File.Delete(tempFile);
                }
                catch (Exception ex)
                {
                    sLastError = ex.Message;
                    return false;
                }
            }
            if (File.Exists(outputFilename) == true)
            {
                try
                {
                    File.Delete(outputFilename);
                }
                catch (Exception ex)
                {
                    sLastError = ex.Message;
                    return false;
                }
            }
            // Create a new Word application
            Microsoft.Office.Interop.Word._Application wordApplication = new Microsoft.Office.Interop.Word.Application();

            try
            {
                // Create a new file based on our template
                Microsoft.Office.Interop.Word._Document wordDocument = wordApplication.Documents.OpenOld(ref oFile, ref oFalse, ref oTrue, ref oFalse, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing);
                // Make a Word selection object.
                Microsoft.Office.Interop.Word.Selection selection = wordApplication.Selection;
                object oRange = Microsoft.Office.Interop.Word.WdPrintOutRange.wdPrintAllDocument;
                object oItem = Microsoft.Office.Interop.Word.WdPrintOutItem.wdPrintDocumentContent;
                object oPageType = Microsoft.Office.Interop.Word.WdPrintOutPages.wdPrintAllPages;

                object oCopy = 1;
                wordDocument.PrintOut(ref oTrue, ref oFalse, ref oRange, ref oTempFile,
                ref oMissing, ref oMissing, ref oItem, ref oCopy, ref oMissing, ref oPageType, ref oTrue, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing);
                wordDocument.Close(ref oFalse, ref oMissing, ref oMissing);
                wordDocument = null;
                if (File.Exists(tempFile) == false)
                {
                    sLastError = "PostScript fiel error!";
                    return false;
                }
                ACRODISTXLib.PdfDistillerClass thDist = new ACRODISTXLib.PdfDistillerClass();
                thDist.FileToPDF(tempFile, outputFilename, "");

                return true;

            }
            catch (Exception ex)
            {
                sLastError = ex.Message;
                return false;
            }
            finally
            {
                // Finally, Close our Word application
                wordApplication.Quit(ref oMissing, ref oMissing, ref oMissing);
            }
        }

    }
}
