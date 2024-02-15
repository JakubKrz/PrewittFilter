/////////////////////////////////////////////
//
// Topic:            Edge Detection using Prewitt Operator
// Year:             2023/2024
// Semester:         5
// Author:           Jakub Krzywoń
// 
//////////////////////////////////////////////


using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {
        [DllImport("C:\\Users\\krzyw\\Source\\Repos\\PrewittFilter\\x64\\Debug\\Prewitt.dll")]
        public static extern void FiltrCpp(byte[] byteArray, byte[] byteArrayOriginal, int width, int height, int start, int end);
        [DllImport("C:\\Users\\krzyw\\Source\\Repos\\PrewittFilter\\x64\\Debug\\PrewittAsm.dll")]
        public static extern void FiltrAsm(byte[] byteArray, byte[] byteArrayOriginal, int width, int height, int start, int end);

        private string imagePath;
        int processorCount;
        public Form1()
        {
            InitializeComponent();

            processorCount = Environment.ProcessorCount;
            for (int i = 0; i <= 6; i++)
            {
                comboBox1.Items.Add((int)Math.Pow(2,i));
            }
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            if (comboBox1.Items.Contains(processorCount))
            {
                comboBox1.SelectedIndex = comboBox1.Items.IndexOf(processorCount);
            } else
            {
                comboBox1.Items.Add(processorCount);
                comboBox1.SelectedItem = processorCount;
            }

            comboBox2.Items.Add("C++");
            comboBox2.Items.Add("ASM");
            comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox2.SelectedIndex = 0;

            label1.Text = "";

            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        private void ImageSelectionButtonClicked(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "BMP files (*.bmp)|*.bmp";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        imagePath = openFileDialog.FileName;
                        Image image = Image.FromFile(imagePath);
                        pictureBox1.Image = image;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Image loading error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                }

            }
        }

        private async void ProcessImageButtonClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(imagePath))
            {
                MessageBox.Show("Please select an image", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            button2.Enabled = false;
            int procCount = processorCount;
            int libraryIndex = comboBox2.SelectedIndex;

            Bitmap imageBitmap = new Bitmap(imagePath);
            byte[] image = ImageToByteArray(pictureBox1.Image);

            byte[] pixelData = new byte[image.Length - 54]; //input array
            byte[] pixelDataOriginal = new byte[image.Length - 54]; //output array
            Array.Copy(image, 54, pixelDataOriginal, 0, pixelData.Length);

            int height = imageBitmap.Height;
            int width = imageBitmap.Width;
            int rowsPerThread = (height - 2) / procCount;
            int remainingRows = (height - 2) % procCount;

            Stopwatch stopwatch = Stopwatch.StartNew();
            await Task.Run(() =>
            {
                if (libraryIndex == 0) //c++
                {
                    Parallel.For(0, procCount, threadIndex =>
                    {
                        int startRow = threadIndex * rowsPerThread + 1 + (threadIndex == 0 ? 0 : (threadIndex - 1 < remainingRows ? 1 : 0));
                        int endRow = (threadIndex == procCount - 1) ? imageBitmap.Height - 1 : (threadIndex + 1) * rowsPerThread + 1 + (threadIndex < remainingRows ? 1 : 0);
                        FiltrCpp(pixelData, pixelDataOriginal, width, height, startRow, endRow);
                    });
                }
                else if (libraryIndex == 1) // asm
                {
                    Parallel.For(0, procCount, threadIndex =>
                    {
                        int startRow = threadIndex * rowsPerThread + 1 + (threadIndex == 0 ? 0 : (threadIndex - 1 < remainingRows ? 1 : 0));
                        int endRow = (threadIndex == procCount - 1) ? imageBitmap.Height - 1 : (threadIndex + 1) * rowsPerThread + 1 + (threadIndex < remainingRows ? 1 : 0);
                        FiltrAsm(pixelData, pixelDataOriginal, width, height, startRow, endRow);
                    });
                }
            });
            stopwatch.Stop();
            label1.Text = "Czas: " + stopwatch.ElapsedMilliseconds.ToString() + " ms";

            Bitmap modifiedBitmap = ConstructBitmap(pixelData, pictureBox1.Image.Width, pictureBox1.Image.Height);
            modifiedBitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
            pictureBox2.Image = modifiedBitmap;

            modifiedBitmap.Save(imagePath.Substring(0, imagePath.Length - 4) + "_Prewitt.bmp", ImageFormat.Bmp);
            button2.Enabled = true;
        }

        private Bitmap ConstructBitmap(byte[] pixelData, int width, int height)
        {
            Bitmap modifiedBitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);

            Rectangle rect = new Rectangle(0, 0, modifiedBitmap.Width, modifiedBitmap.Height);
            System.Drawing.Imaging.BitmapData bmpData =
                modifiedBitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                modifiedBitmap.PixelFormat);

            IntPtr ptr = bmpData.Scan0;

            System.Runtime.InteropServices.Marshal.Copy(pixelData, 0, ptr, pixelData.Length);

            modifiedBitmap.UnlockBits(bmpData);

            return modifiedBitmap;
        }

        private byte[] ImageToByteArray(Image image)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                image.Save(stream, ImageFormat.Bmp);
                return stream.ToArray();
            }
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (int.TryParse(comboBox1.SelectedItem.ToString(), out int selectedValue))
            {
                processorCount = selectedValue;
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}