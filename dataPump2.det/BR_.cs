using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FirebirdSql.Data.FirebirdClient;
using FirebirdSql.Data.Services;

namespace dataPump2.det
{
    class BR_
    {
        static string error = "";
        static string result = "";
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
        public static bool b_(FbConnectionStringBuilder fc,string fbk_)
        {
            bool res_ = true;
            try
            {
                FbBackupFile fbk = new FbBackupFile(fbk_);
                FbBackup fr = new FbBackup();
                fr.ConnectionString = fc.ConnectionString;
                fr.BackupFiles.Add(fbk);
                fr.Options = FbBackupFlags.NoGarbageCollect;
                fr.ServiceOutput += new ServiceOutputEventHandler(ServiceOutput);
                fr.Execute();
            }
            catch (FbException ex)
            {
                result = ex.Message; 
            }
            return res_;
        }
        public static bool r_(FbConnectionStringBuilder fc, string fbk_)
        {
            bool res_ = true;
            try
            {
                FbBackupFile fbk = new FbBackupFile(fbk_);
                FbRestore fr = new FbRestore();
                fr.ConnectionString = fc.ConnectionString;
                fr.BackupFiles.Add(fbk);
                fr.Options = FbRestoreFlags.NoShadow | FbRestoreFlags.Create | FbRestoreFlags.Replace;                
                fr.ServiceOutput += new ServiceOutputEventHandler(ServiceOutput);
                fr.Execute();
            }
            catch (FbException ex)
            {
                result = ex.Message;
            }
            return res_;
        }

        static void ServiceOutput(object sender, ServiceOutputEventArgs e)
        {
            result = e.Message;
        }
    }
}
