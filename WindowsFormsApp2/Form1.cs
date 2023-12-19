using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;


namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {
        [DllImport("C:\\Users\\krzyw\\Source\\Repos\\PrewittFilter\\x64\\Debug\\Prewitt.dll")]
        public static extern void FiltrCpp(byte[] byteArray, byte[] byteArrayOriginal, int width, int height);
        [DllImport("C:\\Users\\krzyw\\Source\\Repos\\PrewittFilter\\x64\\Debug\\PrewittAsm.dll")]
        public static extern byte FiltrAsm(byte[] byteArray, byte[] byteArrayOriginal, int width, int height);

        private string imagePath;

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
                openFileDialog.Filter = "BMP files (*.bmp)|*.bmp";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        imagePath = openFileDialog.FileName;
                        Image image = Image.FromFile(openFileDialog.FileName);

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
            if (string.IsNullOrEmpty(imagePath))
            {
                MessageBox.Show("Nie wybrano obrazu.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Bitmap imageBitmap = new Bitmap(imagePath);
            byte[] image = ImageToByteArray(pictureBox1.Image);




            byte[] pixelData = new byte[image.Length - 54];
            byte[] pixelDataOriginal = new byte[image.Length - 54];
            Array.Copy(image, 54, pixelData, 0, pixelData.Length);
            Array.Copy(image, 54, pixelDataOriginal, 0, pixelData.Length);// bez sensu kopiwoac mozna przekazac pusta TODO!

            Stopwatch stopwatch = Stopwatch.StartNew(); // cos zrobic bo czasami sie psuje

            //Przekazywanie w formacie bgr
            FiltrCpp(pixelData, pixelDataOriginal, imageBitmap.Width,imageBitmap.Height);
            //FiltrAsm(pixelData, pixelDataOriginal, imageBitmap.Width, imageBitmap.Height); 

            stopwatch.Stop();
            MessageBox.Show(stopwatch.ElapsedMilliseconds.ToString() + "ms", "Czas w ms");

            //byte[] modifiedImageWithHeader = AddBmpHeader(pixelData, pictureBox1.Image.Width, pictureBox1.Image.Height); niepotrzebne do wyswietlania

            Bitmap modifiedBitmap = ConstructBitmap(pixelData, pictureBox1.Image.Width, pictureBox1.Image.Height);

            modifiedBitmap.RotateFlip(RotateFlipType.RotateNoneFlipY); //pbrot bo zdjecie z jakiegos powodu wychodzi do gory nogami
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.Image = modifiedBitmap;
        }



        private Bitmap ConstructBitmap(byte[] pixelData, int width, int height)
        {

            Bitmap modifiedBitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);

            Rectangle rect = new Rectangle(0, 0, modifiedBitmap.Width, modifiedBitmap.Height);
            System.Drawing.Imaging.BitmapData bmpData =
                modifiedBitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                modifiedBitmap.PixelFormat);


            IntPtr ptr = bmpData.Scan0;


            System.Runtime.InteropServices.Marshal.Copy(pixelData, 54, ptr, pixelData.Length - 54);


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

        //private byte[] AddBmpHeader(byte[] pixelData, int width, int height)
        //{

        //    int imageSize = pixelData.Length;


        //    int headerSize = 54;

        //    int fileSize = headerSize + imageSize;

        //    byte[] header = new byte[headerSize];

        //    header[0] = (byte)'B';
        //    header[1] = (byte)'M';

        //    // Rozmiar pliku
        //    header[2] = (byte)(fileSize);
        //    header[3] = (byte)(fileSize >> 8);
        //    header[4] = (byte)(fileSize >> 16);
        //    header[5] = (byte)(fileSize >> 24);

        //    // Offset danych pikseli w pliku
        //    header[10] = 54; // offset danych pikseli zaczyna się od bajtu 54

        //    // Rozmiar nagłówka informacyjnego
        //    header[14] = 40; // Rozmiar nagłówka informacyjnego to 40 bajtów

        //    // Szerokość obrazu
        //    header[18] = (byte)width;
        //    header[19] = (byte)(width >> 8);
        //    header[20] = (byte)(width >> 16);
        //    header[21] = (byte)(width >> 24);

        //    // Wysokość obrazu
        //    header[22] = (byte)height;
        //    header[23] = (byte)(height >> 8);
        //    header[24] = (byte)(height >> 16);
        //    header[25] = (byte)(height >> 24);

        //    // Liczba płaszczyzn
        //    header[26] = 1;

        //    // Bitów na piksel (24 bitów na piksel)
        //    header[28] = 24;

        //    // Kompresja (bez kompresji)
        //    header[30] = 0;

        //    // Rozmiar obrazu (0 dla niekompresowanego BMP)
        //    header[34] = (byte)imageSize;
        //    header[35] = (byte)(imageSize >> 8);
        //    header[36] = (byte)(imageSize >> 16);
        //    header[37] = (byte)(imageSize >> 24);

        //    // Skonkatenuj nagłówek z danymi pikseli
        //    byte[] result = new byte[fileSize];
        //    Array.Copy(header, result, headerSize);
        //    Array.Copy(pixelData, 0, result, headerSize, imageSize);

        //    return result;
        //}

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

    }
}
