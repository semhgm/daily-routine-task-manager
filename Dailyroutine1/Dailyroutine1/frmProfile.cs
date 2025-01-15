using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Dailyroutine1
{

    public partial class frmProfile : Form
    {
        public string CurrentUserID { get; set; } // Kullanıcının ID'sini tutar
        SqlConnection conn = new SqlConnection(@"Data Source=SEMIH\SQLEXPRESS;Initial Catalog=DailyRoutineDb1;Integrated Security=True;");
        
        public frmProfile()
        {
            InitializeComponent();
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            frmSettings settings = new frmSettings();
            settings.CurrentUserID=this.CurrentUserID;
            settings.Show();
            this.Hide();
        }

        private void btnCategories_Click(object sender, EventArgs e)
        {
            frmCategories categories = new frmCategories();
            categories.CurrentUserID=this.CurrentUserID;
            categories.Show();
            this.Hide();
        }

        private void btnReminders_Click(object sender, EventArgs e)
        {
            frmReminder reminder = new frmReminder();
            reminder.CurrentUserID = this.CurrentUserID;
            reminder.Show();
            this.Hide();
        }

        private void btnTasks_Click(object sender, EventArgs e)
        {
            frmTasks tasks = new frmTasks();
            tasks.CurrentUserID=this.CurrentUserID;
            tasks.Show();
            this.Hide();
        }

        private void btnDashboard_Click(object sender, EventArgs e)
        {
            frmMain dashboard = new frmMain();
            dashboard.CurrentUserID = this.CurrentUserID;
            dashboard.Show();
            this.Hide();
        }

        private void frmProfile_Load(object sender, EventArgs e)
        {
            LoadUserData();
            ConfigureDataGridView();
        }

        private void LoadUserData()
        {
            try
            {
                conn.Open();
                
                SqlCommand cmd = new SqlCommand("SELECT user_id, name_surname, email, password FROM tblUser WHERE user_id = @UserID", conn);
                cmd.Parameters.AddWithValue("@UserID", CurrentUserID);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                dataGridView1.DataSource = dt;
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

        private void ConfigureDataGridView()
        {
            // DataGridView genel ayarları
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.BackgroundColor = Color.White;
            dataGridView1.BorderStyle = BorderStyle.None;
            dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridView1.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.ReadOnly = true;

            // Başlık satırı ayarları
            dataGridView1.EnableHeadersVisualStyles = false;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(50, 50, 50);
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dataGridView1.ColumnHeadersHeight = 40;

            // Satır ayarları
            dataGridView1.RowTemplate.Height = 35;
            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(238, 239, 249);
            dataGridView1.DefaultCellStyle.Font = new Font("Segoe UI", 9);
            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.FromArgb(87, 87, 87);
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.White;

            // Sütun başlıklarını düzenle
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                switch (column.Name.ToLower())
                {
                    case "user_id":
                        column.HeaderText = "Kullanıcı ID";
                        column.Width = 100;
                        break;
                    case "name_surname":
                        column.HeaderText = "Ad Soyad";
                        column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        break;
                    case "email":
                        column.HeaderText = "E-posta";
                        column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        break;
                    case "password":
                        column.HeaderText = "Şifre";
                        column.Width = 150;
                        break;
                }
            }
        }

        // DataGridView'dan seçilen satırı TextBox'lara aktar
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                txtUserName.Text = row.Cells["name_surname"].Value.ToString();
                txtMail.Text = row.Cells["email"].Value.ToString();
                txtPassword.Text = row.Cells["password"].Value.ToString();
            }
        }

        // Güncelleme butonu için
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtUserName.Text) || 
                    string.IsNullOrWhiteSpace(txtMail.Text) || 
                    string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    MessageBox.Show("Lütfen tüm alanları doldurun!");
                    return;
                }

                conn.Open();
                
                SqlCommand cmd = new SqlCommand(
                    "UPDATE tblUser SET name_surname = @NameSurname, email = @Email, password = @Password " +
                    "WHERE user_id = @UserID", conn);

                cmd.Parameters.AddWithValue("@NameSurname", txtUserName.Text.Trim());
                cmd.Parameters.AddWithValue("@Email", txtMail.Text.Trim());
                cmd.Parameters.AddWithValue("@Password", txtPassword.Text.Trim());
                cmd.Parameters.AddWithValue("@UserID", CurrentUserID);

                cmd.ExecuteNonQuery();
                
                MessageBox.Show("Bilgileriniz başarıyla güncellendi!");
                
                // Tabloyu yenile
                LoadUserData();
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

        // Temizle butonu için
        private void btnClear_Click(object sender, EventArgs e)
        {
            txtUserName.Clear();
            txtMail.Clear();
            txtPassword.Clear();
        }



        private void btnDelete_Click(object sender, EventArgs e)
        {
            // Kullanıcıdan onay al
            DialogResult result = MessageBox.Show(
                "Hesabınızı silmek istediğinizden emin misiniz?\nBu işlem geri alınamaz ve tüm görevleriniz silinecektir.",
                "Hesap Silme Onayı",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    conn.Open();

                    // Transaction başlat
                    SqlTransaction transaction = conn.BeginTransaction();

                    try
                    {
                        // Önce kullanıcının görevlerini sil
                        SqlCommand deleteTasksCmd = new SqlCommand(
                            "DELETE FROM tblTasks WHERE user_id = @UserID", conn, transaction);
                        deleteTasksCmd.Parameters.AddWithValue("@UserID", CurrentUserID);
                        deleteTasksCmd.ExecuteNonQuery();

                        // Sonra kullanıcıyı sil
                        SqlCommand deleteUserCmd = new SqlCommand(
                            "DELETE FROM tblUser WHERE user_id = @UserID", conn, transaction);
                        deleteUserCmd.Parameters.AddWithValue("@UserID", CurrentUserID);
                        deleteUserCmd.ExecuteNonQuery();

                        // İşlemleri onayla
                        transaction.Commit();

                        MessageBox.Show("Hesabınız başarıyla silindi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Giriş formunu aç
                        frmSignIn signInForm = new frmSignIn();
                        signInForm.Show();
                        this.Hide();
                    }
                    catch (Exception ex)
                    {
                        // Hata durumunda işlemleri geri al
                        transaction.Rollback();
                        throw new Exception("İşlem sırasında bir hata oluştu: " + ex.Message);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadUserData(); // Tabloyu yenile
            
            // TextBox'ları temizle
            txtUserName.Clear();
            txtMail.Clear();
            txtPassword.Clear();
            
            MessageBox.Show("Tablo başarıyla güncellendi!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnUpdateTable_Click(object sender, EventArgs e)
        {
            LoadUserData(); // Tabloyu yenile

            // TextBox'ları temizle
            txtUserName.Clear();
            txtMail.Clear();
            txtPassword.Clear();

            MessageBox.Show("Tablo başarıyla güncellendi!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
