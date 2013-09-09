using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FirebirdSql.Data.FirebirdClient;
using System.Threading;

using dataPump2.det.Arch_;
using dataPump2.det.Check_;
using dataPump2.det.Data_;

namespace dataPump2.det
{
    public partial class Form1 : Form
    {
        #region temp
        int i = 1;
        string temp_folder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
                            + @"\det\temp\Setup\DB\"
                            + DateTime.Today.ToString("yyyy")
                            + DateTime.Today.ToString("MM")
                            + DateTime.Today.ToString("dd")
                            + DateTime.Now.ToString("HH")
                            + DateTime.Now.ToString("mm");
        #endregion
        #region connections
        FbConnectionStringBuilder fc_old = new FbConnectionStringBuilder();
        FbConnectionStringBuilder fc_new = new FbConnectionStringBuilder();
        #endregion
        #region sub func
        public string get_install(string programm_)
        {
            string install_dir = "";
            RegistryKey key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall");
            string[] skeys = key.GetSubKeyNames(); // Все подключи из key
            int length = skeys.Length;
            // Проход по всем подключам
            for (int i = 0; i < length; i++)
            {
                // Получаем очередной подключ
                RegistryKey appKey = key.OpenSubKey(skeys[i]);
                string name;
                string install_;
                try // Пробуем получить значение DisplayName
                {
                    name = appKey.GetValue("DisplayName").ToString();
                    install_ = appKey.GetValue("InstallLocation").ToString();

                }
                catch (Exception)
                {
                    // Если не указано имя, то пропускаем ключ
                    continue;
                }

                if (name.ToUpper().StartsWith(programm_.ToUpper()))
                {
                    install_dir = install_;
                }
                appKey.Close();
            }
            key.Close();
            return install_dir;
        }
        public bool check_sysdba(string pass_)
        {
            bool yes_ = true;
            //проверим - установлен ли сервер FB
            string dir_ = get_install("Firebird 2.5");
            if (dir_ != "")
            {

                FbConnectionStringBuilder fc_ch = new FbConnectionStringBuilder();
                fc_ch.Database = temp_folder + @"\det.fdb";//база, которую нужно конвертировать                
                fc_ch.Pooling = false; //пул соединения - отсутствует - для более быстрого освобождения базы                   
                //fc_old.Charset = "win1251"; //кодировка для FB 1/5 не указывается - здесь нужно было переводить в форматы UTF
                fc_ch.UserID = "sysdba";//пользователь по умолчанию
                fc_ch.Password = pass_; //Пароль можно не указывать
                try
                {
                    if (!Directory.Exists(temp_folder))
                    {
                        Directory.CreateDirectory(temp_folder);
                    }
                    FbConnection.CreateDatabase(fc_ch.ConnectionString, true);
                }
                catch (FbException ex)
                {
                    MessageBox.Show(ex.Message, "Неверный пароль SYSDBA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    yes_ = false;
                }
            }
            return yes_;
        }
        #endregion

        public void set_tab()
        {
            switch (i)
            {
                case 1:
                    this.panel1.Visible = true;
                    this.panel2.Visible = false;
                    this.panel3.Visible = false;
                    this.panel4.Visible = false;
                    break;
                case 2:
                    this.panel1.Visible = false;
                    this.panel2.Visible = true;
                    this.panel3.Visible = false;
                    this.panel4.Visible = false;
                    break;
                case 3:
                    this.panel1.Visible = false;
                    this.panel2.Visible = false;
                    this.panel3.Visible = true;
                    this.panel4.Visible = false;
                    break;
                case 4:
                    this.panel1.Visible = false;
                    this.panel2.Visible = false;
                    this.panel3.Visible = false;
                    this.panel4.Visible = true;
                    break;
            }
        }
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            i++;
            set_tab();
            this.timer1.Enabled = true;
            bool is_ok = true;
            switch (i)
            {
                case 3:
                    if (this.is_database.Checked)
                    {
                        if (!File.Exists(this.t_database.Text))
                        {
                            is_ok = false;
                            MessageBox.Show("Файла не существует\nпроверте правильность пути", "Файл не найден", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    if (this.is_install.Checked && is_ok)
                    {
                        //проверим
                        //есть ли установленный сервер  FB25
                        is_ok = check_sysdba(this.t_sysdba.Text);
                    }
                    break;
            }
            if (is_ok)
            {
                switch (i)
                {
                    case 3:
                        Thread t_ = new Thread(run_);
                        t_.Start();
                        while (!t_.IsAlive) ; //ждем пока завершиться
                        break;
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            switch (i)
            {
                case 1:
                    this.tableLayoutPanel1.ColumnStyles[0].Width+=10;
                    if (this.tableLayoutPanel1.ColumnStyles[0].Width >= 90)
                    {
                        this.tableLayoutPanel1.ColumnStyles[0].Width = 100;
                        this.tableLayoutPanel1.ColumnStyles[1].Width = 0;
                        this.timer1.Enabled = false;
                    }
                    break;
                case 2:
                    this.tableLayoutPanel1.ColumnStyles[1].Width += 10;
                    if (this.tableLayoutPanel1.ColumnStyles[1].Width >= 90)
                    {
                        this.tableLayoutPanel1.ColumnStyles[1].Width = 100;
                        this.tableLayoutPanel1.ColumnStyles[0].Width = 0;
                        this.tableLayoutPanel1.ColumnStyles[2].Width = 0;
                        this.timer1.Enabled = false;
                    }
                    break;
                case 3:
                    this.tableLayoutPanel1.ColumnStyles[2].Width += 10;
                    if (this.tableLayoutPanel1.ColumnStyles[2].Width >= 90)
                    {
                        this.tableLayoutPanel1.ColumnStyles[2].Width = 100;
                        this.tableLayoutPanel1.ColumnStyles[1].Width = 0;
                        this.tableLayoutPanel1.ColumnStyles[3].Width = 0;
                        this.timer1.Enabled = false;
                    }
                    break;
                case 4:
                    this.tableLayoutPanel1.ColumnStyles[3].Width += 10;
                    if (this.tableLayoutPanel1.ColumnStyles[3].Width >= 90)
                    {
                        this.tableLayoutPanel1.ColumnStyles[3].Width = 100;
                        this.tableLayoutPanel1.ColumnStyles[2].Width = 0;
                        this.timer1.Enabled = false;
                    }
                    break;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.tableLayoutPanel1.ColumnStyles[0].Width = 100;
            this.tableLayoutPanel1.ColumnStyles[1].Width = 0;
            this.tableLayoutPanel1.ColumnStyles[2].Width = 0;
            this.tableLayoutPanel1.ColumnStyles[3].Width = 0;

            if (System.Environment.Is64BitOperatingSystem)
            {
                //x64
                this.t_install.Text = @"C:\Program Files (x86)\Firebird\Firebird_2_5";
            }
            else
            {
                //x86
                this.t_install.Text = @"C:\Program Files\Firebird\Firebird_2_5";
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            i--;
            set_tab();
            this.timer1.Enabled = true;
            
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        //Запуск процесса конвертации
        public void run_()
        {
            //перове - распакуем FB15
            bool res_ = false;
            res_ = Arch_.Arch_.Decompress(new FileInfo(".\\fb\f15.zip"),@"c:\temp\det\fb15");

            fc_old.UserID = "DBADMIN";
            fc_old.Password = "cnhfiysq";
            fc_old.Database = this.t_database.Text;
            fc_old.Pooling = false;
            fc_old.ServerType = FbServerType.Embedded;
            fc_old.ClientLibrary = @"c:\temp\det\fb15\fbembed.dll";
            
            //второе - проверка доступности функций TRIM and IIF            
            if (Check_.Check_.check_func(fc_old))
            {
                //функции существуют - значит заменим их
                Data_.Data_.run_replace(fc_old);
            }
            //теперь делаем B

        }
    }
}
