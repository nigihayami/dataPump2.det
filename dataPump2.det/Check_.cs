using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FirebirdSql.Data.FirebirdClient;

namespace dataPump2.det.Check_
{
    class Check_
    {
        static string sql_func = @"SELECT *
                                      FROM Rdb$functions F
                                     WHERE F.Rdb$function_name IN ('TRIM', 'IIF')   ";
        public static bool check_func(FbConnectionStringBuilder fc)
        {
            bool res_ = false;
            using (FbConnection fb = new FbConnection(fc.ConnectionString))
            {
                try
                {
                    fb.Open();
                    using (FbTransaction ft = fb.BeginTransaction())
                    {
                        using (FbCommand fcon = new FbCommand(sql_func,fb,ft))
                        {
                            using (FbDataReader fr = fcon.ExecuteReader())
                            {
                                while (fr.Read())
                                {
                                    res_ = true;
                                }
                                fr.Dispose();
                            }
                            fcon.Dispose();
                        }
                        ft.Commit();
                        ft.Dispose();
                    }
                }
                catch { }
                finally { fb.Close(); }
                fb.Dispose();
            }
            return res_;
        }
    }
}
