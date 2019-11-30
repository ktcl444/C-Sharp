using Extensibility;
using EnvDTE;
using EnvDTE80;
using System.Collections.Generic;
using System.Text;

using WindowsInstaller;
using System.Runtime.InteropServices;
using System.Collections;
using System.Diagnostics;

using System;
//using System.Drawing;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;


namespace InsertFeature
{
    public class MSI_API : INotifyPropertyChanged
    {
        #region PropertyChanged
        // INotifyPropertyChanged - Ereignisbehandlung
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        #endregion

        #region Info

        // Hifsvariable für Meldungen
        OutputWindowPane owP = null;
        private string ErrorText = "{0} **** {1}";
        
        /// <summary>
        /// Gibt bei jeder Änderung mit Debug.Print den Inhalt aus
        /// </summary>
        private string _LastErrorText = null;
        public string LastErrorText
        {
            get { return _LastErrorText;}
            set 
            { 
                _LastErrorText = value;
                NotyPropertyChanged("Geändert");
                owP.OutputString(_LastErrorText + "\n");
                owP.ForceItemsToTaskList();
                Debug.Print(_LastErrorText);
            }
        }
        #endregion

        #region Property

        private int _FeatureCount;
        public int FeatureCount
        {
            get { return _FeatureCount; }
            set { _FeatureCount = value; }
        }

        #endregion

        #region Variablen

        // sonstige Variablen

        // Tabelle zum Zwischenspeichern der Feature
        DataTable FeatureTable = new DataTable("SelectTree");
        DataColumn column;

        // Installer-Klasse und MSI-Database
        Installer Installer = new WindowsInstallerClass() as Installer;
        public Database Database = null;

        #endregion

        /// <summary>
        /// MSI_API initialisieren
        /// </summary>
        /// <param name="FileName"></param>
        public MSI_API(string FileName, DTE2 dte)
        {
            try
            {

                // Open the pane to the Output window "Erstellen"
                OutputWindow ow = dte.ToolWindows.OutputWindow;
                owP = ow.OutputWindowPanes.Item(1);

                // Installer-Datenbank öffnen (MSI-Datei)
                Database = Installer.OpenDatabase(FileName, MsiOpenDatabaseMode.msiOpenDatabaseModeTransact);

                // Declare DataRow objects for FeatureTable
                // Create new DataColumn, set DataType, 
                // ColumnName and add to DataTable.    
 
                column = new DataColumn();
                column.DataType = Type.GetType("System.String");
                column.ColumnName = "Component";                    // Component
                column.Unique = false;
                // Add the Column to the DataColumnCollection.
                FeatureTable.Columns.Add(column);

                column = new DataColumn();
                column.DataType = Type.GetType("System.String");
                column.ColumnName = "Feature";                      // Feature
                column.Unique = false;
                // Add the Column to the DataColumnCollection.
                FeatureTable.Columns.Add(column);

                // Anzahl der Feature
                FeatureCount = GetFeatureCount();

            }
            catch (Exception ex)
            {
                LastErrorText = string.Format(ErrorText, "ERROR", "Beim initialieren der API");
            }
        }

        #region Test
        /// <summary>
        /// Startfunktion zum Überprüfen der Installer-Datei
        /// </summary>
        /// <returns>true, Test ok</returns>
        public bool StartTest() //**
        {
            bool TestOk = false;
            LastErrorText = string.Format(ErrorText, "TEST ", "Ob der Dialog 'SelectionTree' enthalten ist");
            if (TestSelectionTree() == true)
            {
                // Zuerst alle Componenten und Feature zwischenspeichern
                LastErrorText = string.Format(ErrorText, "INFO ", "Zuerst alle Componenten und Feature zwischenspeichern");
                FeatureTable = SaveComponentFeature(FeatureTable);

                TestOk = true;
            }
            else
            {
                LastErrorText = string.Format(ErrorText,"ERROR","Die Installerdatei enthalt keinen SelectionTree");
            }
            if (TestOk) LastErrorText = string.Format(ErrorText, "OK   ", "Die Installerdatei kann angepasst werden");
            else LastErrorText = string.Format(ErrorText, "ERROR", "Die Installerdatei kann nicht angepasst werden");
            return TestOk;
        }
        /// <summary>
        /// Gibt die Anzahl der verwendeten Feature zurück
        /// </summary>
        /// <returns></returns>
        /// **************************************************************************
        private int GetFeatureCount() //**
        {
            int ReturnValue = 0;
            // Objektvariablen für das Installerobjekt festlegen
            WindowsInstaller.View View = null;		// View
            WindowsInstaller.Record Record = null;  // Record
            string Query = null;                    // Query
            try
            {
                // Query bilden
                Query = "SELECT Value FROM `Property` WHERE 'Property.Property' = 'FeatureCount'";
                Query = string.Format("SELECT Value FROM `Property` WHERE Property.Property = '{0}'", "FeatureCount");

                // View öffnen
                View = Database.OpenView(Query);

                // View ausführen
                View.Execute(null);

                // Datensatz abrufen
                Record = View.Fetch();
                ReturnValue = Convert.ToInt32(Record.get_StringData(1));
            }
            catch
            {
                LastErrorText = string.Format(ErrorText, "ERROR", "Der 'FeatureCount' kann nicht zurückgegeben werden");
                ReturnValue = 0;
            }
            return ReturnValue;
        }

        /// <summary>
        /// Testen ob Dialog SelectionTree in der Tabelle Dialog
        /// </summary>
        /// <returns></returns>
        private bool TestSelectionTree() //**
        {
            // enhält die Installer-Datei den Dialog SelectionTree ?
            bool Dialog_ok = false;

            // Objektvariablen für das Installerobjekt festlegen
            WindowsInstaller.View View = null;		// View
            WindowsInstaller.Record Record = null;  // Record
            string Query = null;                    // Query
            try
            {
                // Query suchen 'SelectionTree' konstruieren
                Query = string.Format("SELECT * FROM `Dialog` WHERE Dialog.Dialog = 'SelectionTree'");

                // View öffnen
                View = Database.OpenView(Query);

                // View ausführen
                View.Execute(null);

                // Datensatz abrufen
                Record = View.Fetch();
                while (Record != null)
                {
                    Record = View.Fetch();
                    // Dialog SelectionTree ist in der Datenbank vorhanden
                    Dialog_ok = true;
                }
                View.Close();
                LastErrorText = string.Format(ErrorText, "OK   ", "Der Dialog 'SelectionTree' ist in der Installer-Datei enthalten");
            }
            catch
            {
                LastErrorText = string.Format(ErrorText,"ERROR","Der Dialog 'SelectionTree' ist nicht in der Installer-Datei enthalten");
                Dialog_ok = false;
            }
            return Dialog_ok;
        }
        /// <summary>
        /// Fügt die Key's und Dateinamen aus der Tabelle 'Files'
        /// in das Array FileList ein.
        /// ungerader Index = Filename
        /// gerader Indes = Binäre Auflistungs-Nr der Datei
        /// </summary>
        public DataTable SaveComponentFeature(DataTable dt) //**
        {
            // Objektvariablen für das Installerobjekt festlegen
            WindowsInstaller.View View = null;		// View
            WindowsInstaller.Record Record = null;  // Record
            string Query = null;                    // Query
            string Feature = null;
            try
            {
                // Alle Files auflisten
                Query = string.Format("SELECT * FROM `File`");

                // View öffnen
                View = Database.OpenView(Query);

                // View ausführen
                View.Execute(null);
                DataRow dr;

                // Datensatz abrufen
                Record = View.Fetch();
                while (Record != null)
                {
                    string File = null;
                    string Data2 = Record.get_StringData(2);
                    string Data3 = Record.get_StringData(3);

                    int pos = Data2.IndexOf(".");
                    if (pos > 0)
                    {
                        File = Data2.Substring(pos + 1);
                    }
                    else
                    {
                        int pos2 = Data3.IndexOf("|");
                        File = Data3.Substring(pos2 + 1);
                    }

                    // Componet-ID in die erste Spalte stellen
                    dr = dt.NewRow();
                    dr["Component"] = Data2;

                    // Feature-Name in die zweite Spalte stellen
                    Feature = FeatureID(File);
                    dr["Feature"] = Feature;

                    dt.Rows.Add(dr);

                    LastErrorText = string.Format(ErrorText, "OK   ", "Die Componente zu " + Feature + " wird zwischengespeichert");
                    Record = View.Fetch();
                }
                // FileCount = FileList.Count;
                View.Close();
            }
            catch
            {
                LastErrorText = string.Format(ErrorText, "ERROR", "Die Tabelle 'File' kann nicht gelesen werden");
            }
            return dt;
        }
        private string FeatureID(string File) //**
        {
            string ValueFiles;
            // Objektvariablen für das Installerobjekt festlegen
            WindowsInstaller.View View = null;		// View
            WindowsInstaller.Record Record = null;  // Record
            string Query = null;                    // Query
            try
            {
                for (int i = 1; i <= FeatureCount; i++)
                {
                    // Query bilden
                    Query = string.Format("SELECT Value FROM `Property` WHERE Property.Property = '{0}'", "Feature" + i.ToString() + "Files");

                    // View öffnen
                    View = Database.OpenView(Query);

                    // View ausführen
                    View.Execute(null);

                    // Datensatz abrufen
                    Record = View.Fetch();
                    ValueFiles = Record.get_StringData(1);
                    View = null;
                    if (ValueFiles == File) return "Feature" + i.ToString();
                }
            }
            catch
            {
                LastErrorText = string.Format(ErrorText, "ERROR", "Der Test auf " + "ist fehlerhaft verlaufen");
            }
            return null;
        }
        #endregion

        #region Insert
        /// <summary>
        /// Startfunktion zum Einfügen der Features
        /// </summary>
        public void StartInsert() //**
        {
            try
            {
                LastErrorText = string.Format(ErrorText, "INFO ", "Die Installerdatei wird angepasst");
                LastErrorText = string.Format(ErrorText, "INFO ", "Die Tabelle 'Feature' wird geleert");
                if (ClearFeature() == false) throw new ArgumentNullException();

                LastErrorText = string.Format(ErrorText, "INFO ", "Die Tabelle 'FeatureComponents' wird geleert");
                if (ClearFeatureComponets() == false) throw new ArgumentNullException();


                LastErrorText = string.Format(ErrorText, "INFO ", "Alle 'Feature'-Propertys werden gelöscht");
                if (ClearProperty() == false) throw new ArgumentNullException();
            
                LastErrorText = string.Format(ErrorText, "INFO ", "Die Tabellen 'Feature' und 'FeatureComponents' werden neu gefüllt");

                for (int i = FeatureCount; i > 0; i--)
                {
                    if (InsertFeature("Feature" + i.ToString()) == false) throw new ArgumentNullException();
                }
                bool ok = false;
                InsertFeatureComponents(FeatureTable, out ok);
                if (ok == true)
                {
                    LastErrorText = string.Format(ErrorText, "=====", " Die Installerdatei enthält nun alle Feature =====");
                }
            }
            catch
            {
                LastErrorText = string.Format(ErrorText, "=====", " Die Installerdatei ist fehlerhaft =====");
            }

         
        }

        /// <summary>
        /// Clear Tabelle Feature
        /// </summary>
        private bool ClearFeature() //**
        {
            bool ok = false;
            // Objektvariable für das Installerobjekt festlegen
            WindowsInstaller.View View = null;		// View
            //WindowsInstaller.Record Record = null;  // Record
            string Query = null;                    // Query
            try
            {
                Query = string.Format("DELETE FROM `{0}` WHERE `{0}`.`{0}`='{1}'", "Feature", "DefaultFeature");

                // View öffnen
                View = Database.OpenView(Query);

                // View ausführen
                View.Execute(null);
                Database.Commit();
                View.Close();
                ok = true;
            }
            catch
            {
                LastErrorText = string.Format(ErrorText, "ERROR", "Die Tabelle 'Feature' kann nicht geleert werden");
                ok = false;
            }
            return ok;
        }

        /// <summary>
        /// Clear Tabelle FeatureComponets
        /// </summary>
        public bool ClearFeatureComponets() //**
        {
            bool ok = false;
            // Objektvariable für das Installerobjekt festlegen
            WindowsInstaller.View View = null;		// View
            //WindowsInstaller.Record Record = null;  // Record
            string Query = null;                    // Query
            try
            {
                Query = string.Format("DELETE FROM `{0}` WHERE `{0}`.`{1}`='{2}'", "FeatureComponents", "Feature_", "DefaultFeature");

                // View öffnen
                View = Database.OpenView(Query);

                // View ausführen
                View.Execute(null);
                Database.Commit();
                View.Close();
                ok = true;
            }
            catch
            {
                LastErrorText = string.Format(ErrorText, "ERROR", "Die Tabelle 'FeatureComponents' kann nicht geleert werden");
                ok = false;
            }
            return ok;
        }

        /// <summary>
        /// Bereitet die zu löschenden überzähligen Propertys vor
        /// zur Löschung in DeleteProperty
        /// </summary>
        
        public bool ClearProperty() //**
        {
            bool ok = false;
            try
            {
                for (int i = FeatureCount + 1; i < 10; i++)
                {
                    DeleteProperty("Feature" + i.ToString() + "Attributes");
                    DeleteProperty("Feature" + i.ToString() + "Description");
                    DeleteProperty("Feature" + i.ToString() + "Directory");
                    DeleteProperty("Feature" + i.ToString() + "Display");
                    DeleteProperty("Feature" + i.ToString() + "Files");
                    DeleteProperty("Feature" + i.ToString() + "Level");
                    DeleteProperty("Feature" + i.ToString() + "Parent");
                    DeleteProperty("Feature" + i.ToString() + "Title");
                }
                ok = true;
            }
            catch
            {
               LastErrorText = string.Format(ErrorText, "ERROR", "Die Features können nicht gelöscht werden");
               ok = false;
            }
            return ok;
        }
        
        /// <summary>
        /// Löscht das übergebene Property
        /// </summary>
        /// <param name="Property"></param>
        
        public void DeleteProperty(string Property) //**
        {
            // Objektvariable für das Installerobjekt festlegen
            WindowsInstaller.View View = null;		// View
            string Query = null;                    // Query
            Query = string.Format("DELETE FROM `{0}` WHERE `{0}`.`{0}`='{1}'", "Property", Property);

            // View öffnen
            View = Database.OpenView(Query);

            // View ausführen
            View.Execute(null);
            Database.Commit();
            View.Close();
        }
         
                /// <summary>
        /// Fügt die Feature in die Installerdatei ein
        /// </summary>
        /// <param name="Feature"></param>
        /// <returns>true wenn ok</returns>
        public bool InsertFeature(string Feature) //**
        {
            bool InsertOK = false;

              // Objektvariablen für das Installerobjekt festlegen
            string Parent = null;
            // Liste der Dateizugehörigkeit zum Feature
            ArrayList FeatureFileList = new ArrayList();
            
            // Speicher für Die Vorgaben
            FeatureRecord featurerecord = new FeatureRecord();
            try
            {
                // Parent = null wenn "0"
                string ParentValue = GetPropertyValue(Feature + "Parent");
                if (ParentValue.Contains("0") == false) Parent = "Feature" + ParentValue;

                // Feature  in ´Record übertragen
                featurerecord.Parent = GetPropertyValue(Feature + "Parent");
                featurerecord.Title = GetPropertyValue(Feature + "Title");
                featurerecord.Description = GetPropertyValue(Feature + "Description");
                featurerecord.Display = GetPropertyValue(Feature + "Display");
                featurerecord.Level = GetPropertyValue(Feature + "Level");
                featurerecord.Directory = GetPropertyValue(Feature + "Directory");
                featurerecord.Attributes = GetPropertyValue(Feature + "Attributes");
                featurerecord.Files = GetPropertyValue(Feature + "Files");
                LastErrorText = string.Format(ErrorText, "OK   ", Feature + " wird in 'Feature' eingefügt");
                InsertOK = InsertFeatureRecord(Feature, Parent, featurerecord);
            }
            catch
            {
                LastErrorText = string.Format(ErrorText, "ERROR", "Die Features können nicht eingefügt werden");
                InsertOK = false;
            }
            return InsertOK;
        }
        /// <summary>
        /// Feature und die dadugehörige Komponente (Datei-Guid)
        /// in die Tabelle 'FeatureComponents' einfügen
        /// </summary>
        /// <param name="Feature"></param>
        /// <param name="Component"></param>
        private void InsertFeatureComponents(DataTable dt, out bool ok) //**
        {
            // Objektvariablen für das Installerobjekt festlegen
            WindowsInstaller.View View = null;		// View
            string Query = null;                    // Query
            try
            {
                DataTableReader tr = new DataTableReader(dt);
                while (tr.Read())
                {
                    if (!tr.HasRows)
                    {
                        throw new ArgumentNullException();
                    }
                    else
                    {
                        Query = string.Format("INSERT INTO FeatureComponents" +
                                 "(Feature_, Component_)" +
                                "VALUES ('" + tr[1] + "','" + tr[0] + "')");
                        // View öffnen
                        View = Database.OpenView(Query);
                        // View ausführen
                        View.Execute(null);
                        Database.Commit();
                        View.Close();
                    }
                }
            }
            catch
            {
                LastErrorText = string.Format(ErrorText, "ERROR", "Die 'FeatureComponents' kann nicht eingefügt werden");
                ok = false;
            }
            ok = true;
        }

        /// <summary>
        /// Fügt die Features in die Tabelle 'Feature' ein
        /// </summary>
        /// <param name="Feature"></param>
        /// <param name="Parent"></param>
        /// <param name="featurerecord"></param>
        private bool InsertFeatureRecord(string Feature,string Parent, FeatureRecord featurerecord) //**
        {
            // Objektvariablen für das Installerobjekt festlegen
            WindowsInstaller.View View = null;		// View
            string Query = null;                    // Query
            try
            {
                Query = string.Format("INSERT INTO Feature" +
                         "(Feature, Feature_Parent, Title, Description, " +
                         " Display, Level, Directory_, Attributes)" +
                    "VALUES ('" + Feature + "','" + Parent + "','" + featurerecord.Title + "','" +
                          featurerecord.Description + "','" + featurerecord.Display + "','" +
                        featurerecord.Level + "','" + featurerecord.Directory + "','" +
                        featurerecord.Attributes + "')");


                // View öffnen
                View = Database.OpenView(Query);
                // View ausführen
                View.Execute(null);
                Database.Commit();
                View.Close();
            }
            catch
            {
                LastErrorText = string.Format(ErrorText, "ERROR", "Es kann kein Record in die Tabelle Feature eingefügt werden");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Gibt den Wert des Property zurück
        /// </summary>
        /// <param name="Property"></param>
        /// <returns>PropertyValue</returns>
        private string GetPropertyValue(string Property) //**
        {
            // Objektvariablen für das Installerobjekt festlegen
            WindowsInstaller.View View = null;		// View
            WindowsInstaller.Record Record = null;  // Record
            string Query = null;                    // Query
            try
            {
                // Query bilden
                Query = string.Format("SELECT Value FROM `Property` WHERE Property.Property = '{0}'", Property);

                // View öffnen
                View = Database.OpenView(Query);

                // View ausführen
                View.Execute(null);

                // Datensatz abrufen
                Record = View.Fetch();
            }
            catch
            {
                LastErrorText = string.Format(ErrorText, "ERROR", "Es kann kein Property gelesen werden");
            }
            return Record.get_StringData(1);
        }
        #endregion
    }

    /// <summary>
    /// Satzstruktur der Tabelle
    /// </summary>
    public class FeatureRecord
    {
        public string Feature;
        public string Parent;
        public string Title;
        public string Description;
        public string Display;
        public string Level;
        public string Directory;
        public string Attributes;
        public string Files;
    }
    /// <summary>
    /// COM-Import der Windows Installer Komponente
    /// </summary>
    [ComImport, Guid("000c1090-0000-0000-c000-000000000046")]
    internal class WindowsInstallerClass { }

}
