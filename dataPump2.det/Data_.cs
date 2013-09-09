using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FirebirdSql.Data.FirebirdClient;
using System.IO;
using System.Text.RegularExpressions;

namespace dataPump2.det.Data_
{
    class Data_
    {
        #region com
        static List<string> com = new List<string> { };
        #endregion
        #region SQL
        static string sql_procedures = " select P.RDB$PROCEDURE_NAME from RDB$PROCEDURES P";
        static string sql_procedures_f = " select PP.RDB$PARAMETER_NAME, RDB$FIELD_SOURCE, PP.RDB$PARAMETER_TYPE, RF.RDB$FIELD_TYPE, RF.RDB$FIELD_LENGTH, " +
                                "          RF.RDB$FIELD_SCALE, RF.RDB$FIELD_SUB_TYPE, RF.RDB$SEGMENT_LENGTH, RF.RDB$FIELD_PRECISION " +
                                "   from RDB$PROCEDURE_PARAMETERS PP " +
                                "   left join RDB$FIELDS RF on PP.RDB$FIELD_SOURCE = RF.RDB$FIELD_NAME " +
                                "   where PP.RDB$PROCEDURE_NAME = @a " + //name_procedures
                                "   order by PP.RDB$PARAMETER_TYPE, PP.RDB$PARAMETER_NUMBER ";
        static string sql_procedures_b = "select P.RDB$PROCEDURE_SOURCE  from RDB$PROCEDURES P where P.RDB$PROCEDURE_NAME = @a ";//name procedures
        static string sql_triggers = @"SELECT T.Rdb$trigger_name, T.Rdb$relation_name, T.Rdb$trigger_sequence,
                                               T.Rdb$trigger_type, T.Rdb$trigger_source, T.Rdb$trigger_inactive
                                          FROM Rdb$triggers T
                                         WHERE (T.Rdb$system_flag IS NULL OR T.Rdb$system_flag = 0)
                                           AND NOT T.Rdb$trigger_name STARTING WITH 'REPL_'
                                           AND NOT T.Rdb$trigger_name STARTING WITH 'TREE_'
                                         ORDER BY T.Rdb$trigger_name";
        static string sql_triggers_b = "select t.RDB$TRIGGER_SOURCE from RDB$TRIGGERS T where t.rdb$TRIGGER_NAME = @a ";//trigger name

        #endregion
        #region Наборы
        //символы
        static string[] list_en = {"A","B","C","D","E","F","G","H","I","J","K","L","M"
                             ,"N","O","P","Q","R","S","T","U","V","W","X","Y","Z"
                             ,"1","2","3","4","5","6","7","8","9"
        					 ,"_"};
        static string[] list_reserv = {"ABSOLUTE","ACTION","ABORT","ACTIVE","ADD","AFTER","ALL","ALLOCATE","ALTER","ANALYZE","AND","ANY","ARE","AS","ASC","ASCENDING","ASSERTION","AT","AUTHORIZATION","AUTO","AUTO_INCREMENT","AUTOINC","AVG",
                                "BACKUP","BEFORE","BEGIN","BETWEEN","BIGINT","BINARY","BIT","BLOB","BOOLEAN","BOTH","BREAK","BROWSE","BULK","BY","BYTES",
                                "CACHE","CALL","CASCADE","CASCADED","CASE","CAST","CATALOG","CHANGE","CHAR","CHARACTER","CHARACTER_LENGTH","CHECK","CHECKPOINT","CLOSE","CLUSTER","CLUSTERED","COALESCE","COLLATE","COLUMN","COLUMNS","COMMENT","COMMIT","COMMITTED","COMPUTE","COMPUTED","CONDITIONAL","CONFIRM","CONNECT","CONNECTION","CONSTRAINT","CONSTRAINTS","CONTAINING","CONTAINS","CONTAINSTABLE","CONTINUE","CONTROLROW","CONVERT","COPY","COUNT","CREATE","CROSS","CSTRING","CUBE","CURRENT","CURRENT_DATE","CURRENTJTIME","CURRENT_TIMESTAMP","CURRENT_USER","CURSOR",
                                "DATABASE","DATABASES","DATE","DATETIME","DAY","DBCC","DEALLOCATE","DECIMAL","DEBUG","DECLARE","DEC","DEFAULT","DELETE","DENY","DESC","DESCENDING","DISK","DIV","DESCRIBE","DISTINCT","DO","DISCONNECT","DISTRIBUTED","DOMAIN","DOUBLE","DROP","DUMMY","DUMP",
                                "ELSE","ELSEIF","ENCLOSED","END","ERRLVL","ERROREXIT","ESCAPE","ESCAPED","EXCEPT","EXCEPTION","EXEC","EXECUTE","EXISTS","EXIT","EXPLAIN","EXTEND","EXTERNAL","EXTRACT",
                                "FALSE","FETCH","FIELD","FIELDS","FILE","FILLFACTOR","FILTER","FLOAT","FLOPPY","FOR","FORCE","FOREIGN","FOUND","FREETEXT","FREETEXTTABLE","FROM","FULL","FUNCTION",
                                "GENERATOR","GET","GLOBAL","GO","GOTO","GRANT","GROUP","HAVING",
                                "HOLDLOCK","HOUR",
                                "IDENTITY","IF","IN","INACTIVE","INDEX","INDICATOR","INFILE","INNER","INOUT","INPUT","INSENSITIVE","INSERT","INT","INTEGER","INTERSECT","INTERVAL","INTO","IS","ISOLATION",
                                "JOIN",
                                "KEY","KILL",
                                "LANGUAGE","LAST","LEADING","LEFT","LENGTH","LEVEL","LIKE","LIMIT","LINENO","LINES","LISTEN","LOAD","LOCAL","LOCK","LOGFILE","LONG","LOWER",
                                "MANUAL","MATCH","MAX","MERGE","MESSAGE","MIN","MINUTE","MIRROREXIT","MODULE","MONEY","MONTH","MOVE",
                                "NAMES","NATIONAL","NATURAL","NCHAR","NEXT","NEW","NO","NOCHECK","NONCLUSTERED","NONE","NOT","NULL","NULLIF","NUMERIC",
                                "OF","OFF","OFFSET","OFFSETS","ON","ONCE","ONLY","OPEN","OPTION","OR","ORDER","OUTER","OUTPUT","OVER","OVERFLOW","OVERLAPS",
                                "PAD","PAGE","PAGES","PARAMETER","PARTIAL","PASSWORD","PERCENT","PERM","PERMANENT","PIPE","PLAN","POSITION","PRECISION","PREPARE","PRIMARY","PRINT","PRIOR","PRIVILEGES","PROC","PROCEDURE","PROCESSEXIT","PROTECTED","PUBLIC","PURGE",
                                "RAISERROR","READ","READTEXT","REAL","REFERENCES","REGEXP","RELATIVE","RENAME","REPEAT","REPLACE","REPLICATION","REQUIRE","RESERV","RESERVING","RESET","RESTORE","RESTRICT","RETAIN","RETURN","RETURNS","REVOKE","RIGHT","ROLLBACK","ROLLUP","ROWCOUNT","RULE",
                                "SAVE","SAVEPOINT","SCHEMA","SECOND","SECTION","SEGMENT","SELECT","SENSITIVE","SEPARATOR","SEQUENCE","SESSION_USER","SET","SETUSER","SHADOW","SHARED","SHOW","SHUTDOWN","SINGULAR","SIZE","SMALLINT","SNAPSHOT","SOME","SORT","SPACE","SQL","SQLCODE","SQLERROR","STABILITY","STARTING","STARTS","STATISTICS","SUBSTRING","SUM","SUSPEND",
                                "TABLE","TABLES","TAPE","TEMP","TEMPORARY","THEN","TO","TRAN","TRIGGER","TRUNCATE",
                                "UNIQUE","UPDATETEXT","USE",
                                "VALUE","VARIABLE","VIEW",
                                "WAITFOR","WHILE","WRITE",
                                "YEAR","TEXT","TIME","TOP","TRANSACTION","TRIM","UNCOMMITTED","UNTIL","UPPER","USER","VALUES","VARYING","VOLUME",
                                "WHEN","WITH","WRITETEXT","ZONE","TEXTSIZE","TIMESTAMP","TRAILING","TRANSLATE","TRUE","UNION","UPDATE","USAGE","USING","VARCHAR",
                                "VERBOSE","WAIT","WHERE","WORK","XOR"};
        #endregion
        #region Вспомогательные функции
        static string get_field_type(string FTYPE, string FLEN, string FSCALE, string FSUBTYPE, string FSEGMENTSIZE, string FPRECISION, bool is_func)
        {
            //для функций есть ограничения для BLOB - их нужно просто так выводить
            string FIELD_SOURCE = "";
            if (FTYPE == "261")
            {
                FIELD_SOURCE = "BLOB";
            }
            else
            {
                try
                {
                    FIELD_SOURCE = get_field_type(FTYPE,
                                                        FLEN,
                                                        FSCALE,
                                                        FSUBTYPE,
                                                        FSEGMENTSIZE,
                                                        FPRECISION);
                }
                catch (Exception ex)
                {

                }
            }
            return FIELD_SOURCE;
        }
        static string get_field_type(string FTYPE, string FLEN, string FSCALE, string FSUBTYPE, string FSEGMENTSIZE, string FPRECISION)
        {
            string FIELD_SOURCE = "";
            if (FPRECISION == string.Empty)
            {
                FPRECISION = "0";
            }
            if (FSUBTYPE == string.Empty)
            {
                FSUBTYPE = "0";
            }
            if (FSEGMENTSIZE == string.Empty)
            {
                FSEGMENTSIZE = "0";
            }
            try
            {
                FIELD_SOURCE = get_field_type(Convert.ToInt32(FTYPE),
                                                        Convert.ToInt32(FLEN),
                                                        Convert.ToInt32(FSCALE),
                                                        Convert.ToInt32(FSUBTYPE),
                                                        Convert.ToInt32(FSEGMENTSIZE),
                                                        Convert.ToInt32(FPRECISION));
            }
            catch (Exception ex)
            {

            }

            return FIELD_SOURCE;
        }
        static string get_field_type(int FTYPE, int FLEN, int FSCALE, int FSUBTYPE, int FSEGMENTSIZE, int FPRECISION)
        {
            string FIELD_SOURCE = "";
            int FCHARLEN = 0;
            if (FCHARLEN == 0)
            {
                FCHARLEN = FLEN;
            }

            if (FTYPE == 261)
            {
                FIELD_SOURCE = "BLOB SUB_TYPE " + FSUBTYPE.ToString() + " SEGMENT SIZE " + FSEGMENTSIZE.ToString();
            }
            else
                if (FTYPE == 14)
                {
                    FIELD_SOURCE = "CHAR(" + FCHARLEN + ")";
                }
                else
                    if (FTYPE == 37)
                    {
                        FIELD_SOURCE = "VARCHAR(" + FCHARLEN + ")";
                    }
                    else
                        if (FTYPE == 12)
                        {
                            FIELD_SOURCE = "DATE";
                        }
                        else
                            if (FTYPE == 13)
                            {
                                FIELD_SOURCE = "TIME";
                            }
                            else
                                if (FTYPE == 35)
                                {
                                    FIELD_SOURCE = "TIMESTAMP";
                                }
                                else
                                    if (FTYPE == 7)
                                    {
                                        if ((FSCALE < 0) || (FSUBTYPE == 1) || (FSUBTYPE == 2))
                                        {
                                            if (FSUBTYPE == 2)
                                            {
                                                FIELD_SOURCE = "DECIMAL";
                                            }
                                            else
                                            {
                                                FIELD_SOURCE = "NUMERIC";
                                            }
                                            if (FPRECISION > 0)
                                            {
                                                FIELD_SOURCE = FIELD_SOURCE + "(" + FPRECISION + "," + (FSCALE * -1) + ")";
                                            }
                                            else
                                            {
                                                FIELD_SOURCE = FIELD_SOURCE + "(4," + (FSCALE * -1) + ")";
                                            }
                                        }
                                        else
                                            FIELD_SOURCE = "SMALLINT";
                                    }
                                    else
                                        if (FTYPE == 8)
                                        {
                                            if ((FSCALE < 0) || (FSUBTYPE == 1) || (FSUBTYPE == 2))
                                            {
                                                if (FSUBTYPE == 2)
                                                {
                                                    FIELD_SOURCE = "DECIMAL";
                                                }
                                                else
                                                {
                                                    FIELD_SOURCE = "NUMERIC";
                                                }
                                                if (FPRECISION > 0)
                                                {
                                                    FIELD_SOURCE = FIELD_SOURCE + "(" + FPRECISION + "," + (FSCALE * -1) + ")";
                                                }
                                                else
                                                    FIELD_SOURCE = FIELD_SOURCE + "(9," + (FSCALE * -1) + ")";
                                            }
                                            else
                                            {
                                                FIELD_SOURCE = "INTEGER";
                                            }
                                        }
                                        else
                                            if (FTYPE == 27)
                                            {
                                                if ((FSCALE < 0) || (FSUBTYPE == 1) || (FSUBTYPE == 2))
                                                {
                                                    if (FSUBTYPE == 2)
                                                    {
                                                        FIELD_SOURCE = "DECIMAL";
                                                    }
                                                    else
                                                    {
                                                        FIELD_SOURCE = "NUMERIC";
                                                    }
                                                    if (FPRECISION > 0)
                                                    {
                                                        FIELD_SOURCE = FIELD_SOURCE + "(" + FPRECISION + "," + (FSCALE * -1) + ")";
                                                    }
                                                    else
                                                    {
                                                        FIELD_SOURCE = FIELD_SOURCE + "(9," + (FSCALE * -1) + ")";
                                                    }
                                                }
                                                else
                                                    FIELD_SOURCE = "DOUBLE PRECISION";
                                            }
                                            else
                                                if (FTYPE == 16)
                                                {
                                                    if ((FSCALE < 0) || (FSUBTYPE == 1) || (FSUBTYPE == 2))
                                                    {
                                                        if (FSUBTYPE == 2)
                                                        {
                                                            FIELD_SOURCE = "DECIMAL";
                                                        }
                                                        else
                                                        {
                                                            FIELD_SOURCE = "NUMERIC";
                                                        }
                                                        if (FPRECISION > 0)
                                                        {
                                                            FIELD_SOURCE = FIELD_SOURCE + "(" + FPRECISION + "," + (FSCALE * -1) + ")";
                                                        }
                                                        else
                                                        {
                                                            FIELD_SOURCE = FIELD_SOURCE + "(18," + (FSCALE * -1) + ")";
                                                        }
                                                    }
                                                    else
                                                    {
                                                        FIELD_SOURCE = "BIGINT";
                                                    }
                                                }
                                                else
                                                    if (FTYPE == 10)
                                                    {
                                                        FIELD_SOURCE = "FLOAT";
                                                    }
                                                    else
                                                        if (FTYPE == 40)
                                                        {
                                                            FIELD_SOURCE = "CSTRING(" + FLEN.ToString() + ")";
                                                        }

            return FIELD_SOURCE;
        }
        static bool is_en_string(string st)
        {
            //проверяет - состоит ли строка только из English
            bool is_en = true;
            for (int i = 0; i <= st.Length - 1; i++)
            {
                bool is_find = false;
                foreach (string x in list_en)
                {
                    if (x == st[i].ToString())
                    {
                        is_find = true;
                    }
                }
                if (!is_find)
                {
                    is_en = false;
                }
            }
            return is_en;
        }
        static bool is_reserv(string st)
        {
            //зарезервированное слово?
            bool is_find = false;
            foreach (string x in list_reserv)
            {
                if (x == st.ToString().Trim())
                {
                    is_find = true;
                }
            }
            return is_find;
        }
        #endregion
        
        public static void run_replace(FbConnectionStringBuilder fc)
        {
            com.Add(@"DECLARE EXTERNAL FUNCTION TRIM_
                                            CSTRING(256)
                                        RETURNS CSTRING(256)
                                        ENTRY_POINT 'fn_trim' MODULE_NAME 'rfunc'");

            com.Add(@"DECLARE EXTERNAL FUNCTION IIF_
                            INTEGER,
                            INTEGER,
                            INTEGER
                        RETURNS INTEGER BY VALUE
                        ENTRY_POINT 'fn_iif' MODULE_NAME 'rfunc'");
            com.Add("execute procedure re_bild_triggers(2,null)");
            com.Add("EXECUTE PROCEDURE Re_bild_trees(2, NULL)");

            fc.UserID = "sysdba";
            fc.Password = "masterkey";
            
            #region PROCEDURES
            using (FbConnection fb = new FbConnection(fc.ConnectionString))
            {
                try
                {
                    fb.Open();
                    using (FbTransaction ft = fb.BeginTransaction())
                    {
                        using (FbCommand fcon = new FbCommand(sql_procedures, fb, ft))
                        {
                            using (FbDataReader fr = fcon.ExecuteReader())
                            {
                                while (fr.Read())
                                {
                                    string dll_command = "create or alter procedure ";
                                    if (is_reserv(fr[0].ToString()))
                                    {
                                        dll_command += "\"" + fr[0].ToString() + "\"" + "\n";
                                    }
                                    else
                                    {
                                        dll_command += fr[0].ToString() + "\n";
                                    }
                                    //Список полей
                                    string fields_in = "";
                                    string fields_out = "";
                                    bool is_out = false;
                                    using (FbCommand fcon_a = new FbCommand(sql_procedures_f, fb, ft))
                                    {
                                        fcon_a.Parameters.Add("@a", FbDbType.VarChar, 31);
                                        fcon_a.Parameters[0].Value = fr[0].ToString();
                                        using (FbDataReader fr_a = fcon_a.ExecuteReader())
                                        {
                                            while (fr_a.Read())
                                            {
                                                //резервное?
                                                var fields_name = fr_a[0].ToString().Trim();
                                                if (is_reserv(fields_name))
                                                {
                                                    fields_name = "\"" + fields_name + "\"";
                                                }
                                                //Извлекаем тип данных
                                                string field_type = get_field_type(fr_a[3].ToString(), fr_a[4].ToString(), fr_a[5].ToString(), fr_a[6].ToString(), fr_a[7].ToString(), fr_a[8].ToString());
                                                //теперь в зависимости от типа параметра (вх/вых) - запишем в определенный блок
                                                if (fr_a[2].ToString() == "0")
                                                {
                                                    //входной

                                                    if (fields_in == "")
                                                    {
                                                        fields_in += "( " + fields_name + " " + field_type;
                                                    }
                                                    else
                                                    {
                                                        fields_in += ",\n " + fields_name + " " + field_type;
                                                    }
                                                }
                                                else
                                                {
                                                    //значит выходной
                                                    if (!is_out)
                                                    {
                                                        //ставим флаг, что есть выходные параметры
                                                        is_out = true;
                                                    }
                                                    if (fields_out == "")
                                                    {
                                                        fields_out += "returns ( " + fields_name + " " + field_type;
                                                    }
                                                    else
                                                    {
                                                        fields_out += ",\n " + fields_name + " " + field_type;
                                                    }
                                                }
                                            }
                                            fr_a.Dispose();
                                        }
                                        fcon_a.Dispose();
                                    }
                                    //теперь соединяем 
                                    if (fields_in != "")
                                    {
                                        fields_in += "\n )";
                                    }
                                    if (fields_out != "")
                                    {
                                        fields_out += "\n )";
                                    }
                                    dll_command += fields_in + fields_out + "\n AS\n";
                                    //Добавляем содержимое
                                    try
                                    {
                                        using (FbCommand fcon_b = new FbCommand(sql_procedures_b, fb, ft))
                                        {
                                            fcon_b.Parameters.Add("@a", FbDbType.VarChar, 31);
                                            fcon_b.Parameters[0].Value = fr[0].ToString();
                                            using (FbDataReader fr_b = fcon_b.ExecuteReader())
                                            {
                                                while (fr_b.Read())
                                                {
                                                    dll_command += "\n" + Regex.Replace(fr_b.GetString(0), @"\bTrim\b", "TRIM_", RegexOptions.IgnoreCase);
                                                    dll_command = Regex.Replace(dll_command, @"\bIIF\b", "IIF_", RegexOptions.IgnoreCase);
                                                    //.Replace("TRIM", "TRIM_").Replace("Trim", "TRIM_").Replace("trim", "TRIM_").Replace("IIF", "IIF_").Replace("iif", "IIF_").Replace("Iif", "IIF_");
                                                }
                                                fr_b.Dispose();
                                            }
                                            fcon_b.Dispose();
                                        }
                                    }
                                    catch { }
                                    com.Add(dll_command);
                                }
                                fr.Dispose();
                            }
                            fcon.Dispose();
                        }
                        ft.Commit();
                        ft.Dispose();
                    }
                }
                catch {  }
                finally
                {
                    fb.Close();
                }
                fb.Dispose();
            }
            #endregion
            #region TRIGGERS
            using (FbConnection fb = new FbConnection(fc.ConnectionString))
            {
                try
                {
                    fb.Open();
                    using (FbTransaction ft = fb.BeginTransaction())
                    {
                        using (FbCommand fcon = new FbCommand(sql_triggers, fb, ft))
                        {
                            using (FbDataReader fr = fcon.ExecuteReader())
                            {
                                while (fr.Read())
                                {
                                    string dll_command = "create or alter trigger " + fr[0].ToString().Trim() + " FOR " + fr[1].ToString();
                                    //активность
                                    if (fr[5].ToString().Trim() == "1")
                                    {
                                        dll_command += "\n" + "INACTIVE";
                                    }
                                    else
                                    {
                                        dll_command += "\n" + "ACTIVE";
                                    }
                                    //теперь узнаем что за тип триггера
                                    switch (fr[3].ToString().Trim())
                                    {
                                        case "1":
                                            dll_command += " before insert ";
                                            break;
                                        case "2":
                                            dll_command += " after insert ";
                                            break;
                                        case "3":
                                            dll_command += " before update ";
                                            break;
                                        case "4":
                                            dll_command += " after update ";
                                            break;
                                        case "5":
                                            dll_command += " before delete ";
                                            break;
                                        case "6":
                                            dll_command += " after delete ";
                                            break;
                                        case "17":
                                            dll_command += " before insert or update ";
                                            break;
                                        case "25":
                                            dll_command += " before insert or delete";
                                            break;
                                        case "113":
                                            dll_command += " before insert or update or delete ";
                                            break;
                                        case "27":
                                            dll_command += " before update or delete ";
                                            break;
                                        case "18":
                                            dll_command += " after insert or update ";
                                            break;
                                        case "26":
                                            dll_command += " after insert or delete";
                                            break;
                                        case "114":
                                            dll_command += " after insert or update or delete ";
                                            break;
                                        case "28":
                                            dll_command += " after update or delete ";
                                            break;
                                    }
                                    //позиция триггера
                                    dll_command += " position " + fr[2].ToString().Trim();
                                    //Добавляем содержимое
                                    using (FbCommand fcon_b = new FbCommand(sql_triggers_b, fb, ft))
                                    {
                                        fcon_b.Parameters.Add("@a", FbDbType.VarChar, 31);
                                        fcon_b.Parameters[0].Value = fr[0].ToString();
                                        using (FbDataReader fr_b = fcon_b.ExecuteReader())
                                        {
                                            while (fr_b.Read())
                                            {
                                                dll_command += "\n" + fr_b.GetString(0);
                                            }
                                            fr_b.Dispose();
                                        }
                                        fcon_b.Dispose();
                                    }
                                    dll_command = Regex.Replace(dll_command, @"\bTrim\b", "TRIM_", RegexOptions.IgnoreCase);
                                    dll_command = Regex.Replace(dll_command, @"\bIIF\b", "IIF_", RegexOptions.IgnoreCase);
                                    com.Add(dll_command);
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
                finally
                {
                    fb.Close();
                }
                fb.Dispose();
            }
            #endregion
            com.Add("DROP EXTERNAL FUNCTION TRIM");
            com.Add("DROP EXTERNAL FUNCTION IIF");
            #region execute com
            using (FbConnection fb = new FbConnection(fc.ConnectionString))
            {
                try
                {
                    fb.Open();
                    using (FbTransaction ft = fb.BeginTransaction())
                    {
                        using (FbCommand fcon = new FbCommand("",fb,ft))
                        {
                            foreach (string cmd in com)
                            {
                                fcon.CommandText = cmd;
                                fcon.ExecuteNonQuery();
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
            #endregion

            #region SAVE
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("set term ^;");
            foreach (string cmd in com)
            {
                sb.AppendLine(cmd + "^");
            }
            com.Clear();           
            try
            {
                using (StreamWriter outfile = new StreamWriter(File.Create(@"d:\data.sql"), Encoding.Default))
                {
                    sb.Replace("{", "{{");
                    sb.Replace("}", "}}");
                    outfile.Write(sb.ToString(), Encoding.Default);
                }
            }
            catch {  }
            #endregion
        }
    }
}
