using System;
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
        public static extern void ExampleFunction(byte[] byteArray, int length);
        [DllImport("C:\\Users\\krzyw\\Source\\Repos\\PrewittFilter\\x64\\Debug\\PrewittAsm.dll")]
        public static extern int Myproc();

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
                        // Wczytaj obraz z pliku
                        Image image = Image.FromFile(openFileDialog.FileName);

                        using (Bitmap bmp = new Bitmap(openFileDialog.FileName))
                        {
                            int bitDepth = Image.GetPixelFormatSize(bmp.PixelFormat);
                            //MessageBox.Show("Plik BMP ma " + bitDepth + " bitów na piksel. " + bmp.Width + " wysokosc " + bmp.Height + " " + bmp.PixelFormat.ToString(), "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
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
            if (string.IsNullOrEmpty(imagePath))
            {
                MessageBox.Show("Nie wybrano obrazu.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Bitmap imageBitmap = new Bitmap(imagePath);
            byte[] image = ImageToByteArray(pictureBox1.Image);



            // Pomijaj nagłówek (pierwsze 54 bajty)
            byte[] pixelData = new byte[image.Length - 54];
            Array.Copy(image, 54, pixelData, 0, pixelData.Length);

            ExampleFunction(pixelData, pixelData.Length);

            byte[] modifiedImageWithHeader = AddBmpHeader(pixelData, pictureBox1.Image.Width, pictureBox1.Image.Height);

            Bitmap modifiedBitmap = ConstructBitmap(modifiedImageWithHeader, pictureBox1.Image.Width, pictureBox1.Image.Height);
            // Wyświetlamy zmodyfikowany obraz
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.Image = modifiedBitmap;
        }



        private Bitmap ConstructBitmap(byte[] pixelData, int width, int height)
        {
            // Konstruujemy nowy obraz na podstawie zmodyfikowanych danych pikseli
            Bitmap modifiedBitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);

            // Lock the bitmap's bits.
            Rectangle rect = new Rectangle(0, 0, modifiedBitmap.Width, modifiedBitmap.Height);
            System.Drawing.Imaging.BitmapData bmpData =
                modifiedBitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                modifiedBitmap.PixelFormat);

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;

            // Copy the modified RGB values back to the bitmap
            System.Runtime.InteropServices.Marshal.Copy(pixelData, 54, ptr, pixelData.Length - 54);

            // Unlock the bits.
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

        private byte[] AddBmpHeader(byte[] pixelData, int width, int height)
        {
            // Oblicz rozmiar obrazu
            int imageSize = pixelData.Length;

            // Rozmiar nagłówka BMP
            int headerSize = 54;

            // Rozmiar pliku = rozmiar nagłówka + rozmiar danych pikseli
            int fileSize = headerSize + imageSize;

            // Tworzymy bufor na nagłówek
            byte[] header = new byte[headerSize];

            // "BM" to standardowy sygnatury nagłówka BMP
            header[0] = (byte)'B';
            header[1] = (byte)'M';

            // Rozmiar pliku
            header[2] = (byte)(fileSize);
            header[3] = (byte)(fileSize >> 8);
            header[4] = (byte)(fileSize >> 16);
            header[5] = (byte)(fileSize >> 24);

            // Offset danych pikseli w pliku
            header[10] = 54; // offset danych pikseli zaczyna się od bajtu 54

            // Rozmiar nagłówka informacyjnego
            header[14] = 40; // Rozmiar nagłówka informacyjnego to 40 bajtów

            // Szerokość obrazu
            header[18] = (byte)width;
            header[19] = (byte)(width >> 8);
            header[20] = (byte)(width >> 16);
            header[21] = (byte)(width >> 24);

            // Wysokość obrazu
            header[22] = (byte)height;
            header[23] = (byte)(height >> 8);
            header[24] = (byte)(height >> 16);
            header[25] = (byte)(height >> 24);

            // Liczba płaszczyzn
            header[26] = 1;

            // Bitów na piksel (24 bitów na piksel)
            header[28] = 24;

            // Kompresja (bez kompresji)
            header[30] = 0;

            // Rozmiar obrazu (0 dla niekompresowanego BMP)
            header[34] = (byte)imageSize;
            header[35] = (byte)(imageSize >> 8);
            header[36] = (byte)(imageSize >> 16);
            header[37] = (byte)(imageSize >> 24);

            // Skonkatenuj nagłówek z danymi pikseli
            byte[] result = new byte[fileSize];
            Array.Copy(header, result, headerSize);
            Array.Copy(pixelData, 0, result, headerSize, imageSize);

            return result;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

    }
}
