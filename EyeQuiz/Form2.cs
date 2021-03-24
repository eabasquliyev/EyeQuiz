﻿using System;
using System.Drawing;
using System.Windows.Forms;
using EyeQuiz.Helpers;
using EyeQuiz.UCQuiz;

namespace EyeQuiz
{
    public partial class Form2 : Form
    {
        private static Form2 Obj;

        public static Form2 Instance => Obj ?? (Obj = new Form2());

        public Form2()
        {
            InitializeComponent();
        }

        private void PictureBoxMinimize_MouseEnter(object sender, EventArgs e)
        {
            PictureBoxMinimize.Image = Properties.Resources.minimize_window_green_50px;
        }

        private void PictureBoxMinimize_MouseLeave(object sender, EventArgs e)
        {
            PictureBoxMinimize.Image = Properties.Resources.macos_minimize_24px;
        }

        private void PictureBoxDrag_MouseDown(object sender, MouseEventArgs e)
        {
            FormHelper.MouseDown(Cursor.Position, this.Location);
        }

        private void PictureBoxDrag_MouseMove(object sender, MouseEventArgs e)
        {
            var newLocation = FormHelper.MouseMove();

            this.Location = (newLocation != Point.Empty) ? newLocation : this.Location;
        }

        private void PictureBoxDrag_MouseUp(object sender, MouseEventArgs e)
        {
            FormHelper.MouseUp();
        }

        private void PictureBoxMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            Obj = this;
            var dashboard = new UCDashboard();
            this.PanelUserControls.Controls.Add(dashboard);
        }

        private void ButtonLogout_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void ButtonDashboard_Click(object sender, EventArgs e)
        {
            this.PanelUserControls.Controls["UCDashboard"].BringToFront();
            ClearUserControls("UCDashboard");
        }

        private void ClearUserControls(string except = "")
        {
            foreach (Control control in this.PanelUserControls.Controls)
            {
                if (control.Name != except)
                    control.Dispose();
            }
        }
    }
}