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


    public partial class frmCategories : Form
    {
        public string CurrentUserID { get; set; } // Kullanıcının ID'sini tutar
        public frmCategories()
        {
            InitializeComponent();
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            frmSettings settings = new frmSettings();
            settings.CurrentUserID = this.CurrentUserID;
            settings.Show();
            this.Hide();

        }

        private void btnProfile_Click(object sender, EventArgs e)
        {
            frmProfile profile = new frmProfile();
            profile.CurrentUserID= this.CurrentUserID;
            profile.Show();
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
            tasks.CurrentUserID = this.CurrentUserID;
            tasks.Show();
            this.Hide();
        }

        private void btnDashboard_Click(object sender, EventArgs e)
        {
            frmMain main = new frmMain();
            main.CurrentUserID = this.CurrentUserID;
            main.Show();
            this.Hide();
        }

        private void frmCategories_Load(object sender, EventArgs e)
        {
            // Tabloyu doldur
            this.tblCategoryTableAdapter.Fill(this.dailyRoutineDb1DataSet3.tblCategory);
            
            // DataGridView görünümünü düzenle
            ConfigureDataGridView();
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
            if (dataGridView1.Columns["categoryidDataGridViewTextBoxColumn"] != null)
            {
                dataGridView1.Columns["categoryidDataGridViewTextBoxColumn"].HeaderText = "Kategori ID";
                dataGridView1.Columns["categoryidDataGridViewTextBoxColumn"].Width = 100;
            }

            if (dataGridView1.Columns["categorynameDataGridViewTextBoxColumn"] != null)
            {
                dataGridView1.Columns["categorynameDataGridViewTextBoxColumn"].HeaderText = "Kategori Adı";
                dataGridView1.Columns["categorynameDataGridViewTextBoxColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }

            // Gereksiz sütunları gizle (varsa)
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                if (column.Name != "categoryidDataGridViewTextBoxColumn" && 
                    column.Name != "categorynameDataGridViewTextBoxColumn")
                {
                    column.Visible = false;
                }
            }
        }

        // Column isimlerini görmek için debug amaçlı
        private void ListColumnNames()
        {
            string columnNames = "";
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                columnNames += column.Name + ", ";
            }
            MessageBox.Show(columnNames);
        }

        private void btnAdd_Click_1(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(@"Data Source=SEMIH\SQLEXPRESS;Initial Catalog=DailyRoutineDb1;Integrated Security=True;"))
                {
                    conn.Open();
                    
                    // Kategori adının boş olup olmadığını kontrol et
                    if (string.IsNullOrWhiteSpace(txtCategory.Text))
                    {
                        MessageBox.Show("Kategori adı boş olamaz!");
                        return;
                    }

                    SqlCommand cmd = new SqlCommand(
                        "INSERT INTO tblCategory (category_name) VALUES (@CategoryName)", conn);
                    
                    cmd.Parameters.AddWithValue("@CategoryName", txtCategory.Text.Trim());
                    
                    cmd.ExecuteNonQuery();
                    
                    // Tabloyu yenile
                    this.tblCategoryTableAdapter.Fill(this.dailyRoutineDb1DataSet3.tblCategory);
                    
                    MessageBox.Show("Kategori başarıyla eklendi!");
                    txtCategory.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.CurrentRow == null)
                {
                    MessageBox.Show("Lütfen silinecek kategoriyi seçin!");
                    return;
                }

                if (MessageBox.Show("Seçili kategoriyi silmek istediğinizden emin misiniz?", "Onay", 
                    MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    using (SqlConnection conn = new SqlConnection(@"Data Source=SEMIH\SQLEXPRESS;Initial Catalog=DailyRoutineDb1;Integrated Security=True;"))
                    {
                        conn.Open();
                        
                        int categoryId = Convert.ToInt32(dataGridView1.CurrentRow.Cells["categoryidDataGridViewTextBoxColumn"].Value);
                        
                        SqlCommand cmd = new SqlCommand(
                            "DELETE FROM tblCategory WHERE category_id = @CategoryId", conn);
                        
                        cmd.Parameters.AddWithValue("@CategoryId", categoryId);
                        
                        cmd.ExecuteNonQuery();
                        
                        // Tabloyu yenile
                        this.tblCategoryTableAdapter.Fill(this.dailyRoutineDb1DataSet3.tblCategory);
                        
                        MessageBox.Show("Kategori başarıyla silindi!");
                        txtCategory.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.CurrentRow == null)
                {
                    MessageBox.Show("Lütfen güncellenecek kategoriyi seçin!");
                    return;
                }

                using (SqlConnection conn = new SqlConnection(@"Data Source=SEMIH\SQLEXPRESS;Initial Catalog=DailyRoutineDb1;Integrated Security=True;"))
                {
                    conn.Open();
                    
                    int categoryId = Convert.ToInt32(dataGridView1.CurrentRow.Cells["categoryidDataGridViewTextBoxColumn"].Value);
                    
                    SqlCommand cmd = new SqlCommand(
                        "UPDATE tblCategory SET category_name = @CategoryName WHERE category_id = @CategoryId", conn);
                    
                    cmd.Parameters.AddWithValue("@CategoryName", txtCategory.Text.Trim());
                    cmd.Parameters.AddWithValue("@CategoryId", categoryId);
                    
                    cmd.ExecuteNonQuery();
                    
                    // Tabloyu yenile
                    this.tblCategoryTableAdapter.Fill(this.dailyRoutineDb1DataSet3.tblCategory);
                    
                    MessageBox.Show("Kategori başarıyla güncellendi!");
                    txtCategory.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }

        // DataGridView'dan seçilen satırı TextBox'a aktarmak için
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                txtCategory.Text = row.Cells["categorynameDataGridViewTextBoxColumn"].Value.ToString();
            }
        }
    }
}
