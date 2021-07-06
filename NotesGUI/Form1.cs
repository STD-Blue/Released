﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NotesDLL;
using System.IO;
namespace NotesGUI
{
    public partial class Form1 : Form
    {
        public string noteName { get; private set; }
        public int EmptyNote { get; private set; }

        public Form1()
        {
            InitializeComponent();
            Notes = new List<Button>();
            int x = -130, y = 100;
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    Notes.Add(new Button()
                    {
                        Width = 130,
                        Height = 100,
                        Location = new Point(x += 130, y),
                        Visible = false
                    });
                    Notes.Last().Click += Form1_Click;
                }
                x = -130;
                y += 100;
            }

            Controls.AddRange(Notes.ToArray());
        }
        public string test { get; set; }
        private void Form1_Click(object sender, EventArgs e)
        {

            File.WriteAllText("FindBtn.txt", $"{(sender as Button).Text}");
            Form note = new SelectedNote();
            this.Visible = false;
            note.ShowDialog();
            this.Visible = true;
        }

        private void notexText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (notexText.Text.Length < 3)
                {
                    MessageBox.Show("Название не может быть меньше 3 символов!");
                    notexText.Focus();
                }
                else
                {
                    if (notexText.Text.Equals("Enter note's name"))
                    {
                        MessageBox.Show("Не по правилам!");
                        notexText.Focus();
                    }
                    else
                    {
                        if (Notes.Find(x => x.Text == notexText.Text) == null)
                        {
                            noteName = $"{notexText.Text}";
                            notexText.Text = "Enter note's name";
                            notexText.Visible = false;
                            if (MessageBox.Show("Создать пустую заметку?", "INFO", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                Manager.CreateNote(noteName, "Welcome!", System.DateTime.UtcNow);
                                Notes.Find(x => x.Text == string.Empty).Visible = true;
                                Notes.Find(x => x.Text == string.Empty).Text = noteName;

                            }
                            else
                            {
                                MessageBox.Show("Выберите .txt файл который будет загружен в заметку");
                                OpenFileDialog openFile = new OpenFileDialog();
                                openFile.Filter = "Txt(.txt)|*.txt";
                                if (openFile.ShowDialog() == DialogResult.OK)
                                {
                                    Manager.CreateNote(noteName, File.ReadAllText(openFile.FileName), System.DateTime.Now);
                                    Notes.Find(x => x.Text == string.Empty).Visible = true;
                                    Notes.Find(x => x.Text == string.Empty).Text = noteName;
                                }
                            }
                        }
                        else
                        {

                            MessageBox.Show("Такая заметка уже есть");
                        }
                    }

               
                }
            }
            if(e.KeyCode == Keys.Escape)
            {
                notexText.Text = "Enter note's name";
                notexText.Visible = false;
            }
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(Manager.notes.Count == 20)
            {
                MessageBox.Show("Больше 20 заметок нельзя.Удалите 1 заметку");
            }
            else
            {
                notexText.Visible = true;
                notexText.Focus();
            }
           
        }

      
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Manager.SaveAllNotes("SaveNotes.txt");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (File.Exists("SaveNotes.txt"))
            {
                if (File.ReadAllText("SaveNotes.txt").Length != 0)
                {
                    Manager.ReadNotesFromFile("SaveNotes.txt");

                    for (int i = 0; i < Manager.notes.Count; i++)
                    {
                        Notes.Find(x => x.Text == string.Empty).Visible = true;
                        Notes.Find(x => x.Text == string.Empty).Text = Manager.notes[i].NoteName;
                    }
                }
            }
          

        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            if(Notes.Find(x => x.Text.Equals(searchTextBox.Text)) == null)
            {
                MessageBox.Show("Ничего не найдено");
            }
            else
            {
                Notes.Find(x => x.Text.Equals(searchTextBox.Text)).Focus();
            }
           
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(Manager.notes.Count == 0)
            {
                MessageBox.Show("Нечего удалть!");
            }
            else
            {
                deleteNote.Visible = true;
                deleteNote.Focus();
                deleteNote.KeyDown += DeleteNote_KeyDown;
            }
           
        }

        private void DeleteNote_KeyDown(object sender, KeyEventArgs e)
        {

            if(e.KeyCode == Keys.Enter)
            {
                if(Notes.Find(x=>x.Text == deleteNote.Text) == null)
                {
                   
                }
                else
                {
                    Manager.DeleteNote($"{deleteNote.Text}");
                    Notes.Find(x => x.Text == $"{deleteNote.Text}").Visible = false;
                    Notes.Find(x => x.Text == $"{deleteNote.Text}").Text = string.Empty;

                    deleteNote.Text = "Enter note's name";
                    deleteNote.Visible = false;
                }
              
            }
            if(e.KeyCode == Keys.Escape)
            {
                deleteNote.Text = "Enter note's name";
                deleteNote.Visible = false;
            }
        }

        private void backColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (ColorDialog color = new ColorDialog())
            {
                if(color.ShowDialog() == DialogResult.OK)
                {
                    this.BackColor = color.Color;
                }
            }
        }

        private void backImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFile = new OpenFileDialog())
            {
                openFile.Filter = "Png(.png)|*.png|Jpg(.jpg)|*.jpg";
                if(openFile.ShowDialog() == DialogResult.OK)
                {
                    this.BackgroundImage = Image.FromFile(openFile.FileName);
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Manager.SaveAllNotes("SaveNotes.txt");
            Environment.Exit(0);
        }

        private void deleteALLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(Manager.notes.Count == 0)
            {
                MessageBox.Show("Удалять нечего!");
            }
            else
            {
                if (MessageBox.Show("Удалить все заметки?", "WARNING", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    for (int i = 0; i < Manager.notes.Count; i++)
                    {
                        Notes.Find(x => x.Text == $"{Manager.notes[i].NoteName}").Visible = false;
                        Notes.Find(x => x.Text == $"{Manager.notes[i].NoteName}").Text = string.Empty;
                        Manager.DeleteNote($"{Manager.notes[i].NoteName}");


                    }
                }
            }
           
        }
    }
}
