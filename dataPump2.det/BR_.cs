using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FirebirdSql.Data.FirebirdClient;
using FirebirdSql.Data.Services;
using System.IO;
using System.Diagnostics;

namespace dataPump2.det.BR_
{
    class BR_
    {
        static string error = "";
        static string result = "";
        static bool is_close = false;
        public static string error_
        {
            get
            {
                return error;
            }
        }
        public static string result_
        {
            get
            {
                return result;
            }
        }
        public static bool is_close_
        {
            get
            {
                return is_close;
            }
        }
        public static void b_(FbConnectionStringBuilder fc,string fbk_)
        {
            is_close = false;
            try
            {
                FbBackupFile fbk = new FbBackupFile(fbk_);
                FbBackup fr = new FbBackup();
                fr.Verbose = true;
                fr.ConnectionString = fc.ConnectionString;
                fr.BackupFiles.Add(fbk);
                fr.Options = FbBackupFlags.NoGarbageCollect;
                fr.ServiceOutput += ServiceOutput;
                fr.Execute();
            }
            catch (FbException ex)
            {
                error = ex.Message;
            }
            finally
            {
                is_close = true;
            }
        }
        public static void r_(FbConnectionStringBuilder fc, string fbk_)
        {
            is_close = false;
            try
            {
                if (!File.Exists(fc.Database))
                {
                    FbConnection.CreateDatabase(fc.ConnectionString,true);
                }
                Process p = new Process();
                p.StartInfo.FileName = @"C:\temp\det\F25\gbak.exe";
                p.StartInfo.CreateNoWindow = false;
                p.StartInfo.Arguments = @"-r -fix_fss_metadata win1251 -fix_fss_data win1251 -user dbadmin -pas cnhfiysq -v " + fc.Database + " " + fbk_;
                p.Start();

                p.WaitForExit();
                /*
                FbBackupFile fbk = new FbBackupFile(fbk_);
                FbRestore fr = new FbRestore();
                fr.Verbose = true;
                fr.ConnectionString = fc.ConnectionString;
                fr.BackupFiles.Add(fbk);
                fr.Options = FbRestoreFlags.NoShadow | FbRestoreFlags.Create | FbRestoreFlags.Replace;
                fr.ServiceOutput += new ServiceOutputEventHandler(ServiceOutput);
                fr.Execute();
                 * */
            }
            catch (FbException ex)
            {
                error = ex.Message;
            }
            finally
            {
                is_close = true;
            }
        }

        static void ServiceOutput(object sender, ServiceOutputEventArgs e)
        {
            result = e.Message;
        }
    }
}
