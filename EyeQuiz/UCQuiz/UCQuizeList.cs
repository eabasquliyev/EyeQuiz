﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using EyeQuiz.Entities;
using EyeQuiz.Helpers.FileHelpers;
using Guna.UI2.WinForms;

namespace EyeQuiz.UCQuiz
{
    public partial class UCQuizeList : UserControl
    {
        public UserControl LastUc;
        public Panel SelectedQuize;
        public int QuizCount = 0;
        public List<QuestionsBlock> Quizzes { get; set; }

        public UCQuizeList()
        {
            InitializeComponent();

            Quizzes = new List<QuestionsBlock>();
        }

        private void UCQuizeList_Load(object sender, EventArgs e)
        {
            // PanelQuiz Size: 152, 172
            // PanelQuiz Location: 16, 24
            //MessageBox.Show($"{PanelQuiz.Size}");
            //MessageBox.Show($"{PanelQuiz.Location}");

            if (!LoadQuizzes())
                return;

            RadioButtonAll.Checked = true;
        }

        private void ButtonBack_MouseEnter(object sender, EventArgs e)
        {
            ButtonBack.Image = Properties.Resources.back_to_100px;
        }

        private void ButtonBack_MouseLeave(object sender, EventArgs e)
        {
            ButtonBack.Image = Properties.Resources.back_to_light_100px;
        }

        private void PanelQuiz_MouseLeave(object sender, EventArgs e)
        {
            var panelQuiz = sender as Panel;

            var mousePosition = GetMousePosition(panelQuiz);

            if (mousePosition.X >= panelQuiz.Location.X &&
                 mousePosition.X <= panelQuiz.Location.X + panelQuiz.Size.Width &&
                 mousePosition.Y >= panelQuiz.Location.Y &&
                 mousePosition.Y <= panelQuiz.Location.Y + panelQuiz.Size.Height)
            {
                return;
            }

            if (SelectedQuize == panelQuiz)
                return;

            panelQuiz.BackColor = Color.FromArgb(83, 141, 216);
        }

        private void PanelQuiz_MouseEnter(object sender, EventArgs e)
        {
            var panelQuiz = sender as Panel;

            if (SelectedQuize == panelQuiz)
                return;

            panelQuiz.BackColor = Color.FromArgb(73, 128, 212);
        }

        private Point GetMousePosition(Panel panel)
        {
            return new Point()
            {
                X = Control.MousePosition.X - Form2.Instance.Location.X - Form2.Instance.Controls["PanelUserControls"].Location.X - panel.Location.X - 8,
                Y = Control.MousePosition.Y - Form2.Instance.Location.Y - Form2.Instance.Controls["PanelUserControls"].Location.Y - panel.Location.Y - 30,
            };
        }

        private void PanelQuiz_Click(object sender, EventArgs e)
        {
            Panel panelQuiz = null;

            if (sender is Panel pnl)
                panelQuiz = pnl;
            else if (sender is Label lbl)
                panelQuiz = lbl.Parent as Panel;

            if (SelectedQuize != null)
            {
                SelectedQuize.BackColor = Color.FromArgb(83, 141, 216);
            }

            panelQuiz.BackColor = Color.FromArgb(137, 251, 162);
            SelectedQuize = panelQuiz;

            var quizIndex = (int)panelQuiz.Tag;

            TextBoxQuestionCount.PlaceholderText = $"Total question count {Quizzes[quizIndex].Questions.Count}";
        }

        private void ButtonBack_Click(object sender, EventArgs e)
        {
            this.Dispose();
            LastUc.BringToFront();
        }

        private void ResetQuizListScrollValue()
        {
            this.PanelQuizeList.VerticalScroll.Value = 0;
        }

        private void QuizListScrollValueMax()
        {
            this.PanelQuizeList.AutoScrollPosition = new Point(0, GetVerticalQuizCount() * 442);
        }
        private int GetVerticalQuizCount()
        {
            var count = this.PanelQuizeList.Controls.Count;

            var rowCount = count / 4;
            return rowCount;
        }

        private int GetHorizontalQuizCount()
        {
            var count = this.PanelQuizeList.Controls.Count;

            var verticalQuizCount = GetVerticalQuizCount();

            var columnCount = count - (4 * verticalQuizCount);

            return columnCount;
        }

        private Panel CreateNewQuizPanel(int quizId)
        {
            var panelQuiz = new Panel();

            panelQuiz.Name = $"PanelQuiz{QuizCount}";
            panelQuiz.BackColor = Color.FromArgb(83, 141, 216);
            panelQuiz.Size = new Size(152, 172);
            panelQuiz.Tag = quizId;
            panelQuiz.MouseEnter += PanelQuiz_MouseEnter;
            panelQuiz.MouseLeave += PanelQuiz_MouseLeave;
            panelQuiz.Click += PanelQuiz_Click;

            var labelQuizName = new Label();

            labelQuizName.Name = $"LabelQuizName{QuizCount}";
            labelQuizName.Text = "Quiz name";
            labelQuizName.Font = new Font("Microsoft Sans Serif", 10, FontStyle.Regular);
            labelQuizName.Location = new Point(30, 68);
            labelQuizName.AutoSize = false;
            labelQuizName.Size = new Size(97, 81);
            labelQuizName.Click += PanelQuiz_Click;

            panelQuiz.Controls.Add(labelQuizName);

            var horizontalQuizCount = GetHorizontalQuizCount();
            var verticalQuizCount = GetVerticalQuizCount();

            var defaultLocation = new Point(16, 24);

            ResetQuizListScrollValue();

            var horizontalSpace = (horizontalQuizCount != 0) ? (horizontalQuizCount * 10) : 0;
            var verticalSpace = (verticalQuizCount != 0) ? (verticalQuizCount * 10) : 0;
            panelQuiz.Location = new Point((defaultLocation.X + horizontalQuizCount * panelQuiz.Size.Width + horizontalSpace),
                (defaultLocation.Y + verticalQuizCount * panelQuiz.Size.Height + verticalSpace));
            
            return panelQuiz;
        }

        private void ButtonAdd_Click(object sender, EventArgs e)
        {
            var newQuizPanel = CreateNewQuizPanel(0);

            QuizCount++;
            this.PanelQuizeList.Controls.Add(newQuizPanel);

            QuizListScrollValueMax();
        }

        private bool LoadQuizzes()
        {
            var dirName = @"Questions\";

            if (!Directory.Exists(dirName))
                return false;

            var dirInfo = new DirectoryInfo(dirName);

            var directories = dirInfo.GetDirectories();

            if (directories.Length == 0)
                return false;


            var status = false;

            foreach (var directoryInfo in directories)
            {
                var fileInfos = directoryInfo.GetFiles();

                if (fileInfos.Length > 0)
                {
                    status = true;

                    foreach (var fileInfo in fileInfos)
                    {
                        Guid ownerId = Guid.Empty;

                        try
                        {
                            ownerId = Guid.Parse(fileInfo.Directory.Name);
                        }
                        catch
                        {

                        }

                        var filePath = fileInfo.FullName;

                        var newQuestionsBlock = new QuestionsBlock()
                        {
                            OwnerId = ownerId,
                            FileName = fileInfo.Name,
                        };

                        XmlHelper.Deserialize(filePath, newQuestionsBlock);

                        if (newQuestionsBlock.Questions.Count == 0)
                            continue;

                        Quizzes.Add(newQuestionsBlock);
                    }
                }
            }


            return status;
        }

        private void LoadQuizzesToListView(List<QuestionsBlock> quizzes)
        {
            foreach (var quiz in quizzes)
            {
                var id = Quizzes.IndexOf(quiz);
                var newQuizPanel = CreateNewQuizPanel(id);

                newQuizPanel.Controls[$"LabelQuizName{QuizCount}"].Text = quiz.FileName.Substring(0, quiz.FileName.Length - 4);

                QuizCount++;
                this.PanelQuizeList.Controls.Add(newQuizPanel);
            }
        }

        private List<QuestionsBlock> FilterByName()
        {
            var filteredQuizzes = new List<QuestionsBlock>();

            return filteredQuizzes;
        }

        private List<QuestionsBlock> FilterByOwnerId()
        {
            var filteredQuizzes = new List<QuestionsBlock>();

            if (Quizzes.Count == 0)
                return filteredQuizzes;

            foreach (var quiz in Quizzes)
            {
                if (quiz.OwnerId == Form2.Instance.User.Guid)
                    filteredQuizzes.Add(quiz);
            }

            return filteredQuizzes;
        }

        private void RadioButtonOnlyMe_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is Guna2CustomRadioButton rd)
            {
                if(rd.Checked)
                    RadioButtonOnlyMeClick();
            }
        }

        private void RadioButtonAll_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is Guna2CustomRadioButton rd)
            {
                if (rd.Checked)
                    RadioButtonAllClick();
            }
        }

        private void RadioButtonAllClick()
        {
            this.PanelQuizeList.Controls.Clear();

            LoadQuizzesToListView(Quizzes);
        }

        private void RadioButtonOnlyMeClick()
        {
            var result = FilterByOwnerId();

            this.PanelQuizeList.Controls.Clear();

            if (result.Count == 0)
                return;

            QuizCount = 0;
            LoadQuizzesToListView(result);
        }

        private void LabelAllQuiz_Click(object sender, EventArgs e)
        {
            if(!RadioButtonAll.Checked)
                RadioButtonAll.Checked = true;
        }

        private void LabelMine_Click(object sender, EventArgs e)
        {
            if(!RadioButtonOnlyMe.Checked)
                RadioButtonOnlyMe.Checked = true;
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            QuizCount = 0;
            this.PanelQuizeList.Controls.Clear();
            Quizzes.Clear();

            SelectedQuize = null;

            if (!LoadQuizzes())
                return;

            if(RadioButtonAll.Checked)
                RadioButtonAllClick();
            else
                RadioButtonOnlyMeClick();

        }
    }
}