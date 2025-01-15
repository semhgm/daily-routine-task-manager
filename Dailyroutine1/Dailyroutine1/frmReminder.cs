using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Drawing;

namespace Dailyroutine1
{
    public partial class frmReminder : Form
    {
        // Veritabanı bağlantı dizesi
        SqlConnection conn = new SqlConnection(@"Data Source=SEMIH\SQLEXPRESS;Initial Catalog=DailyRoutineDb1;Integrated Security=True;");

        // Kullanıcının ID'si
        public string CurrentUserID { get; set; }

        // Timer
        private System.Windows.Forms.Timer reminderTimer;

        public frmReminder()
        {
            InitializeComponent();
        }

        // Form yüklendiğinde çalışacak metod
        private void frmReminder_Load(object sender, EventArgs e)
        {
            try
            {
                // DataGridView görünümünü ayarla
                ConfigureDataGridView();
                AddMissingReminders(); // Önce eksik hatırlatıcıları ekle
                LoadReminders(); // Sonra tüm hatırlatıcıları yükle
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veri yüklenirken hata oluştu: " + ex.Message);
            }
        }

        private void ConfigureDataGridView()
        {
            // DataGridView genel ayarları
            dataGridViewReminders.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewReminders.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewReminders.AllowUserToAddRows = false;
            dataGridViewReminders.AllowUserToDeleteRows = false;
            dataGridViewReminders.ReadOnly = true;
            dataGridViewReminders.MultiSelect = false;
            dataGridViewReminders.BackgroundColor = Color.White;

            // Sütunları temizle ve yeniden oluştur
            dataGridViewReminders.Columns.Clear();
            dataGridViewReminders.Columns.Add("reminder_id", "Hatırlatıcı ID");
            dataGridViewReminders.Columns.Add("title", "Görev Adı");
            dataGridViewReminders.Columns.Add("dateof_beginning", "Görev Başlangıç");
            dataGridViewReminders.Columns.Add("dateof_reminder", "Hatırlatma Tarihi");

            // Sütun genişliklerini ayarla
            dataGridViewReminders.Columns["reminder_id"].Width = 100;
            dataGridViewReminders.Columns["title"].Width = 200;
            dataGridViewReminders.Columns["dateof_beginning"].Width = 150;
            dataGridViewReminders.Columns["dateof_reminder"].Width = 150;

            // Alternatif satır renkleri
            dataGridViewReminders.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;
            dataGridViewReminders.DefaultCellStyle.SelectionBackColor = Color.LightBlue;
            dataGridViewReminders.DefaultCellStyle.SelectionForeColor = Color.Black;
        }

        // Hatırlatıcıları yükleyen metod
        private void LoadReminders()
        {
            string query = @"SELECT r.reminder_id, t.title, 
                            FORMAT(t.dateof_beginning, 'dd.MM.yyyy HH:mm') as dateof_beginning,
                            FORMAT(r.dateof_reminder, 'dd.MM.yyyy HH:mm') as dateof_reminder
                            FROM tblReminder r 
                            INNER JOIN tblTasks t ON r.task_id = t.task_id 
                            WHERE t.user_id = @userId
                            ORDER BY r.dateof_reminder DESC";

            try
            {
                using (SqlConnection connection = new SqlConnection(conn.ConnectionString))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    adapter.SelectCommand.Parameters.AddWithValue("@userId", CurrentUserID);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dataGridViewReminders.DataSource = dt;

                    if (dt.Rows.Count == 0)
                    {
                        MessageBox.Show("Henüz hatırlatıcı bulunmuyor.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hatırlatıcılar yüklenirken hata oluştu: " + ex.Message);
            }
        }

        // Eksik hatırlatıcıları tblReminder'a ekleyen metod
        private void AddMissingReminders()
        {
            string query = @"SELECT task_id, dateof_beginning 
                             FROM tblTasks 
                             WHERE user_id = @userId
                             AND task_id NOT IN (SELECT task_id FROM tblReminder)";

            try
            {
                using (SqlConnection connection = new SqlConnection(conn.ConnectionString))
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@userId", CurrentUserID);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int taskId = Convert.ToInt32(reader["task_id"]);
                                DateTime taskDate = Convert.ToDateTime(reader["dateof_beginning"]);
                                CreateReminder(taskId, taskDate);
                            }
                        }
                    }
                }
                LoadReminders();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Eksik hatırlatıcılar eklenirken hata oluştu: " + ex.Message);
            }
        }

        // Yeni hatırlatıcı oluşturan metod
        private void CreateReminder(int taskId, DateTime taskDate)
        {
            DateTime reminderDate = taskDate.AddDays(-1);  // Hatırlatıcı bir gün öncesinde
            string query = "INSERT INTO tblReminder (task_id, dateof_reminder) VALUES (@taskId, @reminderDate)";

            try
            {
                using (SqlConnection connection = new SqlConnection(conn.ConnectionString))
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@taskId", taskId);
                        cmd.Parameters.AddWithValue("@reminderDate", reminderDate);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hatırlatıcı eklenirken hata oluştu: " + ex.Message);
            }
        }

        // Hatırlatıcıları kontrol eden metod
        private void CheckReminders()
        {
            string query = @"SELECT t.title 
                            FROM tblReminder r 
                            INNER JOIN tblTasks t ON r.task_id = t.task_id 
                            WHERE CONVERT(date, r.dateof_reminder) = CONVERT(date, GETDATE())
                            AND t.user_id = @userId";

            try
            {
                using (SqlConnection connection = new SqlConnection(conn.ConnectionString))
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@userId", CurrentUserID);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string taskTitle = reader["title"].ToString();
                                MessageBox.Show($"Hatırlatma: '{taskTitle}' göreviniz yarın başlayacak!",
                                    "Hatırlatma", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hatırlatıcı kontrol edilirken hata oluştu: " + ex.Message);
            }
        }

        // Hatırlatıcı timer ayarlama
        private void SetupReminderTimer()
        {
            if (reminderTimer == null)
            {
                reminderTimer = new System.Windows.Forms.Timer();
                reminderTimer.Interval = 60000; // Her dakika kontrol et
                reminderTimer.Tick += (s, e) => CheckReminders();
                reminderTimer.Start();
            }
        }

        // Ayarlar butonu
        private void btnSettings_Click(object sender, EventArgs e)
        {
            frmSettings settings = new frmSettings
            {
                CurrentUserID = this.CurrentUserID
            };
            settings.Show();
            this.Close();  // Mevcut formu kapat
        }

        // Profil butonu
        private void btnProfile_Click(object sender, EventArgs e)
        {
            frmProfile profile = new frmProfile
            {
                CurrentUserID = this.CurrentUserID
            };
            profile.Show();
            this.Close();  // Mevcut formu kapat
        }

        // Kategoriler butonu
        private void btnCategories_Click(object sender, EventArgs e)
        {
            frmCategories categories = new frmCategories
            {
                CurrentUserID = this.CurrentUserID
            };
            categories.Show();
            this.Close();  // Mevcut formu kapat
        }

        // Görevler butonu
        private void btnTasks_Click(object sender, EventArgs e)
        {
            frmTasks tasks = new frmTasks
            {
                CurrentUserID = this.CurrentUserID
            };
            tasks.Show();
            this.Close();  // Mevcut formu kapat
        }

        // Dashboard butonu
        private void btnDashboard_Click(object sender, EventArgs e)
        {
            frmMain dashboard = new frmMain
            {
                CurrentUserID = this.CurrentUserID
            };
            dashboard.Show();
            this.Close();  // Mevcut formu kapat
        }

        // Hatırlatıcılar butonu
        private void btnReminders_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Zaten hatırlatıcılar sayfasındasınız.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}