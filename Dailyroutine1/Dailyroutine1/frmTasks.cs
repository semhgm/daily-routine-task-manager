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
    public partial class frmTasks : Form
    {
        public string CurrentUserID { get; set; } // Kullanıcının ID'sini tutar
        SqlConnection conn = new SqlConnection(@"Data Source=SEMIH\SQLEXPRESS;Initial Catalog=DailyRoutineDb1;Integrated Security=True;");
        public frmTasks()
        {
            InitializeComponent();
        }

        private void btnDashboard_Click(object sender, EventArgs e)
        {
            frmMain dashboard = new frmMain();
            dashboard.CurrentUserID = this.CurrentUserID; // frmMain'deki kullanıcı ID'sini frmTasks'a aktar
            dashboard.Show();
            this.Hide();
        }

        private void btnReminders_Click(object sender, EventArgs e)
        {
            frmReminder reminder = new frmReminder();
            reminder.CurrentUserID = this.CurrentUserID;
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
            frmProfile settings = new frmProfile();
            settings.CurrentUserID = this.CurrentUserID;
            settings.Show();
            this.Hide();
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void frmTasks_Load(object sender, EventArgs e)
        {
            try
            {
                conn.Open();

                // Kullanıcıya özel verileri çek
                SqlCommand cmd = new SqlCommand("SELECT * FROM tblTasks WHERE user_id = @UserID", conn);
                cmd.Parameters.AddWithValue("@UserID", CurrentUserID);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                // DataGridView'e bağla
                dataGridView1.DataSource = dataTable;
                
                // DataGridView görünümünü düzenle
                ConfigureDataGridView();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
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

            // Sütun başlıklarını ve genişliklerini düzenle
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                switch (column.Name.ToLower())
                {
                    case "task_id":
                        column.HeaderText = "Görev ID";
                        column.Width = 80;
                        break;
                    case "user_id":
                        column.HeaderText = "Kullanıcı ID";
                        column.Width = 80;
                        break;
                    case "title":
                        column.HeaderText = "Görev Başlığı";
                        column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        break;
                    case "comment":
                        column.HeaderText = "Açıklama";
                        column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        break;
                    case "category_id":
                        column.HeaderText = "Kategori";
                        column.Width = 100;
                        break;
                    case "value":
                        column.HeaderText = "Değer";
                        column.Width = 80;
                        break;
                    case "dateof_beginning":
                        column.HeaderText = "Başlangıç Tarihi";
                        column.Width = 120;
                        break;
                    case "dateof_end":
                        column.HeaderText = "Bitiş Tarihi";
                        column.Width = 120;
                        break;
                    case "status":
                        column.HeaderText = "Durum";
                        column.Width = 80;
                        break;
                }
            }
        }

        private void panel12_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel12_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                conn.Open();

                // Görev ekleme sorgusuna kullanıcı ID'sini ekleyelim
                SqlCommand add = new SqlCommand(
                    "INSERT INTO tblTasks (title, comment, category_id, value, dateof_beginning, dateof_end, status, user_id) " +
                    "VALUES (@TaskName, @Comment, @Category, @Value, @StartDate, @EndDate, @Status, @UserID)", conn);

                // Parametreleri ekle
                add.Parameters.AddWithValue("@TaskName", txtTask.Text);
                add.Parameters.AddWithValue("@Comment", txtComment.Text);
                add.Parameters.AddWithValue("@Category", txtCategory.Text);
                add.Parameters.AddWithValue("@Value", txtValue.Text);
                add.Parameters.AddWithValue("@StartDate", DateTime.Parse(mskStart.Text));
                add.Parameters.AddWithValue("@EndDate", DateTime.Parse(mskEnd.Text));
                add.Parameters.AddWithValue("@Status", mskStatus.Text);
                add.Parameters.AddWithValue("@UserID", CurrentUserID); // Kullanıcı ID'sini ekle

                // Sorguyu çalıştır
                add.ExecuteNonQuery();

                MessageBox.Show("Görev başarıyla eklendi!");

                // Görev listesini güncelle
                frmTasks_Load(sender, e);
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
        private void LoadTasks()
        {
            try
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                SqlCommand cmd = new SqlCommand("SELECT * FROM tblTasks WHERE user_id = @UserID", conn);
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
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            LoadTasks(); // Görev listesini yenile
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];

                // TextBox'lara verileri doldurun
                txtTaskId.Text = selectedRow.Cells[0].Value?.ToString(); // Task ID
                txtid.Text = selectedRow.Cells[1].Value?.ToString(); // User ID
                txtTask.Text = selectedRow.Cells[2].Value?.ToString(); // Title
                txtComment.Text = selectedRow.Cells[3].Value?.ToString(); // Comment
                txtCategory.Text = selectedRow.Cells[4].Value?.ToString(); // Category ID
                txtValue.Text = selectedRow.Cells[5].Value?.ToString(); // Value

                // Tarihleri uygun formata çevir
                if (DateTime.TryParse(selectedRow.Cells[6].Value?.ToString(), out DateTime startDate))
                {
                    mskStart.Text = startDate.ToString("dd-MM-yyyy HH:mm"); // MaskedTextBox formatına uygun
                }
                else
                {
                    mskStart.Text = string.Empty; // Geçersiz tarih
                }

                if (DateTime.TryParse(selectedRow.Cells[7].Value?.ToString(), out DateTime endDate))
                {
                    mskEnd.Text = endDate.ToString("dd-MM-yyyy HH:mm"); // MaskedTextBox formatına uygun
                }
                else
                {
                    mskEnd.Text = string.Empty; // Geçersiz tarih
                }

                // Status
                mskStatus.Text = selectedRow.Cells[8].Value?.ToString(); // Status
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];

                // TextBox'lara verileri doldurun
                txtTaskId.Text = selectedRow.Cells[0].Value?.ToString(); // Task ID
                txtid.Text = selectedRow.Cells[1].Value?.ToString(); // User ID
                txtTask.Text = selectedRow.Cells[2].Value?.ToString(); // Title
                txtComment.Text = selectedRow.Cells[3].Value?.ToString(); // Comment
                txtCategory.Text = selectedRow.Cells[4].Value?.ToString(); // Category ID
                txtValue.Text = selectedRow.Cells[5].Value?.ToString(); // Value

                // Tarihleri uygun formata çevir
                if (DateTime.TryParse(selectedRow.Cells[6].Value?.ToString(), out DateTime startDate))
                {
                    mskStart.Text = startDate.ToString("dd-MM-yyyy HH:mm"); // MaskedTextBox formatına uygun
                }
                else
                {
                    mskStart.Text = string.Empty; // Geçersiz tarih
                }

                if (DateTime.TryParse(selectedRow.Cells[7].Value?.ToString(), out DateTime endDate))
                {
                    mskEnd.Text = endDate.ToString("dd-MM-yyyy HH:mm"); // MaskedTextBox formatına uygun
                }
                else
                {
                    mskEnd.Text = string.Empty; // Geçersiz tarih
                }

                // Status
                mskStatus.Text = selectedRow.Cells[8].Value?.ToString(); // Status
            }
        }
        private void LoadData()
        {
            try
            {
                conn.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM tblTasks WHERE user_id = @UserID", conn);
                adapter.SelectCommand.Parameters.AddWithValue("@UserID", CurrentUserID);

                DataTable dt = new DataTable();
                adapter.Fill(dt);

                dataGridView1.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Veriler yüklenirken bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtTaskId.Text))
            {
                MessageBox.Show("Lütfen silmek istediğiniz görevi seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                conn.Open();

                // Silme sorgusu
                SqlCommand delete = new SqlCommand("DELETE FROM tblTasks WHERE task_id = @TaskID", conn);
                delete.Parameters.AddWithValue("@TaskID", txtTaskId.Text); // Seçilen görevin ID'sini kullan

                int result = delete.ExecuteNonQuery();

                if (result > 0)
                {
                    MessageBox.Show("Görev başarıyla silindi!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // DataGridView'i yenile
                    LoadData();
                }
                else
                {
                    MessageBox.Show("Silme işlemi başarısız oldu. Görev bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();
            }
        }



        private void btnClear_Click(object sender, EventArgs e)
        {
            txtTaskId.Clear();
            txtid.Clear();
            txtTask.Clear();
            txtComment.Clear();
            txtCategory.Clear();
            txtValue.Clear();
            mskStart.Clear();
            mskEnd.Clear();
            mskStatus.Clear();
        }

        private void btnUpdateAll_Click(object sender, EventArgs e)
        {
            try
            {
                conn.Open();

                // Güncelleme sorgusu
                SqlCommand update = new SqlCommand(
                    "UPDATE tblTasks " +
                    "SET title = @TaskName, comment = @Comment, category_id = @Category, value = @Value, " +
                    "dateof_beginning = @StartDate, dateof_end = @EndDate, status = @Status " +
                    "WHERE task_id = @TaskID AND user_id = @UserID", conn);

                // Tarihleri kontrol et ve ata
                if (!DateTime.TryParse(mskStart.Text, out DateTime startDate))
                {
                    MessageBox.Show("Başlangıç tarihi geçersiz! Lütfen doğru formatta giriniz.");
                    return;
                }

                if (!DateTime.TryParse(mskEnd.Text, out DateTime endDate))
                {
                    MessageBox.Show("Bitiş tarihi geçersiz! Lütfen doğru formatta giriniz.");
                    return;
                }

                // Parametreleri ekle
                update.Parameters.AddWithValue("@TaskName", txtTask.Text);
                update.Parameters.AddWithValue("@Comment", txtComment.Text);
                update.Parameters.AddWithValue("@Category", txtCategory.Text);
                update.Parameters.AddWithValue("@Value", txtValue.Text);
                update.Parameters.AddWithValue("@StartDate", startDate);
                update.Parameters.AddWithValue("@EndDate", endDate);
                update.Parameters.AddWithValue("@Status", mskStatus.Text);
                update.Parameters.AddWithValue("@TaskID", txtTaskId.Text); // Task ID
                update.Parameters.AddWithValue("@UserID", CurrentUserID);  // Kullanıcı ID'si

                // Sorguyu çalıştır
                int rowsAffected = update.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Görev başarıyla güncellendi!");
                }
                else
                {
                    MessageBox.Show("Güncelleme işlemi başarısız. Lütfen bilgileri kontrol edin.");
                }

                // Görev listesini güncelle
                frmTasks_Load(sender, e);
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

        private void btnClear_Click_1(object sender, EventArgs e)
        {
            txtTaskId.Clear();
            txtid.Clear();
            txtTask.Clear();
            txtComment.Clear();
            txtCategory.Clear();
            txtValue.Clear();
            mskStart.Clear();
            mskEnd.Clear();
            mskStatus.Clear();
        }

        private void btnUpdateAll_Click_1(object sender, EventArgs e)
        {
            try
            {
                conn.Open();

                // Güncelleme sorgusu
                SqlCommand update = new SqlCommand(
                    "UPDATE tblTasks " +
                    "SET title = @TaskName, comment = @Comment, category_id = @Category, value = @Value, " +
                    "dateof_beginning = @StartDate, dateof_end = @EndDate, status = @Status " +
                    "WHERE task_id = @TaskID AND user_id = @UserID", conn);

                // Tarihleri kontrol et ve ata
                if (!DateTime.TryParse(mskStart.Text, out DateTime startDate))
                {
                    MessageBox.Show("Başlangıç tarihi geçersiz! Lütfen doğru formatta giriniz.");
                    return;
                }

                if (!DateTime.TryParse(mskEnd.Text, out DateTime endDate))
                {
                    MessageBox.Show("Bitiş tarihi geçersiz! Lütfen doğru formatta giriniz.");
                    return;
                }

                // Parametreleri ekle
                update.Parameters.AddWithValue("@TaskName", txtTask.Text);
                update.Parameters.AddWithValue("@Comment", txtComment.Text);
                update.Parameters.AddWithValue("@Category", txtCategory.Text);
                update.Parameters.AddWithValue("@Value", txtValue.Text);
                update.Parameters.AddWithValue("@StartDate", startDate);
                update.Parameters.AddWithValue("@EndDate", endDate);
                update.Parameters.AddWithValue("@Status", mskStatus.Text);
                update.Parameters.AddWithValue("@TaskID", txtTaskId.Text); // Task ID
                update.Parameters.AddWithValue("@UserID", CurrentUserID);  // Kullanıcı ID'si

                // Sorguyu çalıştır
                int rowsAffected = update.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Görev başarıyla güncellendi!");
                }
                else
                {
                    MessageBox.Show("Güncelleme işlemi başarısız. Lütfen bilgileri kontrol edin.");
                }

                // Görev listesini güncelle
                frmTasks_Load(sender, e);
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

        private void btnUpdate_Click_1(object sender, EventArgs e)
        {
            LoadTasks(); // Görev listesini yenile
        }

        private void btnAdd_Click_1(object sender, EventArgs e)
        {
            try
            {
                conn.Open();

                // Görev ekleme sorgusuna kullanıcı ID'sini ekleyelim
                SqlCommand add = new SqlCommand(
                    "INSERT INTO tblTasks (title, comment, category_id, value, dateof_beginning, dateof_end, status, user_id) " +
                    "VALUES (@TaskName, @Comment, @Category, @Value, @StartDate, @EndDate, @Status, @UserID)", conn);

                // Parametreleri ekle
                add.Parameters.AddWithValue("@TaskName", txtTask.Text);
                add.Parameters.AddWithValue("@Comment", txtComment.Text);
                add.Parameters.AddWithValue("@Category", txtCategory.Text);
                add.Parameters.AddWithValue("@Value", txtValue.Text);
                add.Parameters.AddWithValue("@StartDate", DateTime.Parse(mskStart.Text));
                add.Parameters.AddWithValue("@EndDate", DateTime.Parse(mskEnd.Text));
                add.Parameters.AddWithValue("@Status", mskStatus.Text);
                add.Parameters.AddWithValue("@UserID", CurrentUserID); // Kullanıcı ID'sini ekle

                // Sorguyu çalıştır
                add.ExecuteNonQuery();

                MessageBox.Show("Görev başarıyla eklendi!");

                // Görev listesini güncelle
                frmTasks_Load(sender, e);
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

        private void btnRemove_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtTaskId.Text))
            {
                MessageBox.Show("Lütfen silmek istediğiniz görevi seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                conn.Open();

                // Silme sorgusu
                SqlCommand delete = new SqlCommand("DELETE FROM tblTasks WHERE task_id = @TaskID", conn);
                delete.Parameters.AddWithValue("@TaskID", txtTaskId.Text); // Seçilen görevin ID'sini kullan

                int result = delete.ExecuteNonQuery();

                if (result > 0)
                {
                    MessageBox.Show("Görev başarıyla silindi!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // DataGridView'i yenile
                    LoadData();
                }
                else
                {
                    MessageBox.Show("Silme işlemi başarısız oldu. Görev bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();
            }
        }
    }
}
