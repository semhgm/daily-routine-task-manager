using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dailyroutine1
{
    public partial class frmSettings : Form
    {
        public string CurrentUserID { get; set; } // Kullanıcının ID'sini tutar
        public frmSettings()
        {
            InitializeComponent();
        }

        private void btnProfile_Click(object sender, EventArgs e)
        {
            frmProfile profile = new frmProfile();
            profile.CurrentUserID = this.CurrentUserID;
            profile.Show();
            this.Hide();
        }

        private void btnCategories_Click(object sender, EventArgs e)
        {
            frmCategories categories = new frmCategories();
            categories.CurrentUserID = this.CurrentUserID;
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
            tasks.CurrentUserID = this.CurrentUserID;
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

        private void pictureBox10_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
