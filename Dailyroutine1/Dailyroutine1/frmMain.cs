using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Markup;
using System.Windows.Media;

namespace Dailyroutine1
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();

        }
        public string CurrentUserID { get; set; }

        SqlConnection conn = new SqlConnection(@"Data Source=SEMIH\SQLEXPRESS;Initial Catalog=DailyRoutineDb1;Integrated Security=True;");



        private (int nextWeekTasks, int otherTasks) GetWeeklyTaskCounts()
        {
            int nextWeekTasks = 0;
            int otherTasks = 0;

            try
            {
                conn.Open();

                // SQL sorgusu
                SqlCommand cmd = new SqlCommand(
                    "SELECT " +
                    "SUM(CASE WHEN dateof_beginning BETWEEN GETDATE() AND DATEADD(day, 7, GETDATE()) THEN 1 ELSE 0 END) AS NextWeekTasks, " +
                    "SUM(CASE WHEN dateof_beginning < GETDATE() OR dateof_beginning > DATEADD(day, 7, GETDATE()) THEN 1 ELSE 0 END) AS OtherTasks " +
                    "FROM tblTasks " +
                    "WHERE user_id = @UserID AND dateof_beginning IS NOT NULL;", conn);

                cmd.Parameters.AddWithValue("@UserID", CurrentUserID); // Giriş yapan kullanıcının ID'si

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    nextWeekTasks = reader.GetInt32(0);    // Gelecek bir haftadaki görev sayısı
                    otherTasks = reader.GetInt32(1);       // Diğer görevler
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }
            return (nextWeekTasks, otherTasks);
        }




        private void UpdateWeeklyChart()
        {
            var (nextWeekTasks, otherTasks) = GetWeeklyTaskCounts();

            chartWeeklyTasks.Series.Clear();

            chartWeeklyTasks.Series.Add("Görevler");
            chartWeeklyTasks.Series["Görevler"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Pie;

            chartWeeklyTasks.Series["Görevler"].Points.AddXY("Gelecek Haftadaki Görevler", nextWeekTasks);
            chartWeeklyTasks.Series["Görevler"].Points.AddXY("Diğer Görevler", otherTasks);

            chartWeeklyTasks.Series["Görevler"]["PieLabelStyle"] = "Outside";
            chartWeeklyTasks.Series["Görevler"].IsValueShownAsLabel = true;
        }






        private (int doneTasks, int notDoneTasks) GetTaskStatusCounts()
        {
            int doneTasks = 0;
            int notDoneTasks = 0;

            try
            {
                conn.Open();

                // SQL sorgusu: Status'a göre görev sayısını al
                SqlCommand cmd = new SqlCommand(
                    "SELECT " +
                    "SUM(CASE WHEN status = 1 THEN 1 ELSE 0 END) AS DoneTasks, " +
                    "SUM(CASE WHEN status = 0 THEN 1 ELSE 0 END) AS NotDoneTasks " +
                    "FROM tblTasks WHERE user_id = @UserID;", conn);

                cmd.Parameters.AddWithValue("@UserID", CurrentUserID); // Giriş yapan kullanıcının ID'si

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    doneTasks = reader.GetInt32(0);    // Yapılmış görev sayısı
                    notDoneTasks = reader.GetInt32(1); // Yapılmamış görev sayısı
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return (doneTasks, notDoneTasks);
        }


        private void UpdateChart()
        {
            // Görev durum sayılarını al
            var (doneTasks, notDoneTasks) = GetTaskStatusCounts();

            // Chart verilerini temizle
            chartTasks.Series.Clear();

            // Yeni bir seri ekle
            chartTasks.Series.Add("Görevler");
            chartTasks.Series["Görevler"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Pie;

            // Pie chart verilerini ekle
            chartTasks.Series["Görevler"].Points.AddXY("Yapılmış Görevler", doneTasks);
            chartTasks.Series["Görevler"].Points.AddXY("Yapılmamış Görevler", notDoneTasks);

            // Pie chart için ayarları düzenle
            chartTasks.Series["Görevler"]["PieLabelStyle"] = "Outside"; // Etiketleri dışarıya koy
            chartTasks.Series["Görevler"].IsValueShownAsLabel = true; // Dilim değerlerini göster

        }

        private void panel9_Paint(object sender, PaintEventArgs e)
        {
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'dailyRoutineDb1DataSet6.tblCategory' table. You can move, or remove it, as needed.
            this.tblCategoryTableAdapter.Fill(this.dailyRoutineDb1DataSet6.tblCategory);
            UpdateChart(); // Mevcut pie chart
            UpdateWeeklyChart(); // Günlük görev grafiği
            UpdateTotalTaskCount(); // Toplam görev sayısı
            UpdateUpcomingTaskCount(); // Yaklaşan görevlerin sayısı
        }

        private void btnTasks_Click(object sender, EventArgs e)
        {
            frmTasks tasksForm = new frmTasks();
            tasksForm.CurrentUserID = this.CurrentUserID; // frmMain'deki kullanıcı ID'sini frmTasks'a aktar
            tasksForm.Show();
            this.Hide();
        }

        private void btnReminders_Click(object sender, EventArgs e)
        {
            frmReminder reminder = new frmReminder();
            reminder.CurrentUserID = this.CurrentUserID; // frmMain'deki kullanıcı ID'sini frmTasks'a aktar
            reminder.Show();
            this.Hide();

        }

        private void btnCategories_Click(object sender, EventArgs e)
        {
            frmCategories categories = new frmCategories();
            categories.CurrentUserID = this.CurrentUserID;
            categories.Show();
            this.Hide();
        }

        private void btnProfile_Click(object sender, EventArgs e)
        {
            frmProfile profile = new frmProfile();
            profile.CurrentUserID = this.CurrentUserID;
            profile.Show();
            this.Hide();

        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            frmSettings settings = new frmSettings();
            settings.CurrentUserID = this.CurrentUserID;
            settings.Show();
            this.Hide();
        }

        private void lblExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void lblMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void panel8_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel9_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void btnDashboard_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox10_Click(object sender, EventArgs e)
        {
            frmTasks tasksForm = new frmTasks();
            tasksForm.CurrentUserID = this.CurrentUserID; // frmMain'deki kullanıcı ID'sini frmTasks'a aktar
            tasksForm.Show();
            this.Hide();
        }

        private void UpdateTotalTaskCount()
        {
            try
            {
                conn.Open();
                
                SqlCommand cmd = new SqlCommand(
                    "SELECT COUNT(*) FROM tblTasks WHERE user_id = @UserID", conn);
                    
                cmd.Parameters.AddWithValue("@UserID", CurrentUserID);
                
                int totalTasks = (int)cmd.ExecuteScalar();
                lblUserTasks.Text = totalTasks.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            frmReminder reminder = new frmReminder();
            reminder.CurrentUserID = this.CurrentUserID;
            reminder.Show();
            this.Hide();
        }

        private void pictureBox1_Click_1(object sender, EventArgs e)
        {
            frmReminder reminder = new frmReminder();
            reminder.CurrentUserID = this.CurrentUserID;
            reminder.Show();
            this.Hide();
        }

        private void UpdateUpcomingTaskCount()
        {
            try
            {
                conn.Open();
                
                SqlCommand cmd = new SqlCommand(
                    @"SELECT COUNT(*) 
                    FROM tblTasks 
                    WHERE user_id = @UserID 
                    AND dateof_beginning BETWEEN GETDATE() AND DATEADD(day, 7, GETDATE())", conn);
                    
                cmd.Parameters.AddWithValue("@UserID", CurrentUserID);
                
                int upcomingTasks = (int)cmd.ExecuteScalar();
                lblUpcoming.Text = upcomingTasks.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }
    }
}
