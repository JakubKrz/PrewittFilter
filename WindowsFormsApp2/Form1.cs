﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {
        [DllImport("C:\\Users\\krzyw\\Source\\Repos\\PrewittFilter\\x64\\Debug\\Prewitt.dll")]
        public static extern void ExampleFunction();
        [DllImport("C:\\Users\\krzyw\\Source\\Repos\\PrewittFilter\\x64\\Debug\\PrewittAsm.dll")]
        public static extern int Myproc();
        public Form1()
        {
            InitializeComponent();
            int processorCount = Environment.ProcessorCount;
            for (int i = 0; i <= 6; i++)
            {
                int value = (int)Math.Pow(2, i);
                comboBox1.Items.Add(value);
            }
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.SelectedItem = processorCount;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files (*.bmp;*.jpg;*.png)|*.bmp;*.jpg;*.png|All files (*.*)|*.*";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Wczytaj obraz z pliku
                        Image image = Image.FromFile(openFileDialog.FileName);
                        
                        // Wyświetl obraz w PictureBox
                        pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                        pictureBox1.Image = image;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Błąd wczytywania obrazu: {ex.Message}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
           
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

    }
}
