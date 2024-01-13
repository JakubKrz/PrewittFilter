using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {
        [DllImport("C:\\Users\\krzyw\\Source\\Repos\\PrewittFilter\\x64\\Debug\\Prewitt.dll")]
        public static extern void FiltrCpp(byte[] byteArray, byte[] byteArrayOriginal, int width, int height, int start, int end);
        [DllImport("C:\\Users\\krzyw\\Source\\Repos\\PrewittFilter\\x64\\Debug\\PrewittAsm.dll")]
        public static extern byte FiltrAsm(byte[] byteArray, byte[] byteArrayOriginal, int width, int height, int start, int end);

        private string imagePath;
        int processorCount;
        public Form1()
        {
            InitializeComponent();
            processorCount = Environment.ProcessorCount;
            for (int i = 0; i <= 6; i++) //maja byc raczej od 1-64 a nie tylko potegi 2
            {
                int value = (int)Math.Pow(2, i);
                comboBox1.Items.Add(value);
            }
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.SelectedItem = processorCount;

            comboBox2.Items.Add("C++");
            comboBox2.Items.Add("ASM");
            comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox2.SelectedIndex = 0;

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


            //TODO podzielic na czesci dla watkow

            byte[] pixelData = new byte[image.Length - 54];
            byte[] pixelDataOriginal = new byte[image.Length - 54];
            Array.Copy(image, 54, pixelData, 0, pixelData.Length);
            Array.Copy(image, 54, pixelDataOriginal, 0, pixelData.Length);// bez sensu kopiwoac mozna przekazac pusta TODO!

            Stopwatch stopwatch = Stopwatch.StartNew();

            //Przekazywanie w formacie bgr
            //TODO zmienic dodac osobne funkcje ladujace dynamiczne biblioteki
            int height = imageBitmap.Height;
            int width = imageBitmap.Width;
            int rowsPerThread = height / processorCount;
            if (comboBox2.SelectedIndex == 0)
            {
                Parallel.For(0, processorCount , threadIndex => //TO DO: upewnic sie czy dobrze dzielone sa watki ( na razie dziala) ( czy sa wszytkie wiersze? i czy dla nieparzystych dobrze?)
                {
                    int startRow = threadIndex * rowsPerThread + 1;
                    int endRow = (threadIndex == processorCount - 1) ? imageBitmap.Height - 1 : (threadIndex + 1) * rowsPerThread + 1; //pomijanie wyslania ostatniego wierszu( TO DO: zmienic w cpp)

                    FiltrCpp(pixelData, pixelDataOriginal, width, height, startRow, endRow);
                });
                //FiltrCpp(pixelData, pixelDataOriginal, imageBitmap.Width, imageBitmap.Height, 1, imageBitmap.Height); // do usuniecia
            } else if(comboBox2.SelectedIndex == 1)
            {
                Parallel.For(0, processorCount, threadIndex => //TO DO: upewnic sie czy dobrze dzielone sa watki ( na razie dziala) ( czy sa wszytkie wiersze? i czy dla nieparzystych dobrze?)
                {
                    int startRow = threadIndex * rowsPerThread + 1;
                    int endRow = (threadIndex == processorCount - 1) ? imageBitmap.Height - 1 : (threadIndex + 1) * rowsPerThread + 1;//pomijanie wyslania ostatniego wierszu (TO DO: zmienic w asm, nie pomijac ostatniego)

                    FiltrAsm(pixelData, pixelDataOriginal, width, height, startRow, endRow);
                });
                //Task task1 = Task.Run(() => FiltrAsm(pixelData, pixelDataOriginal, width, height, 0, height / 3));
                //Task task2 = Task.Run(() => FiltrAsm(pixelData, pixelDataOriginal, width, height, height / 3, height/3 *2));
                //Task task3 = Task.Run(() => FiltrAsm(pixelData, pixelDataOriginal, width, height, height / 3 * 2, height - 1));

                // Wait for both tasks to complete
                //Task.WaitAll(task1, task2, task3);
                //FiltrAsm(pixelData, pixelDataOriginal, imageBitmap.Width, imageBitmap.Height, 0 , imageBitmap.Height/2 ); // przy duzych plikach i ponownym uruchomieniu asm rzuca wyjatek sprawdzic dlaczeg TO DO
                //FiltrAsm(pixelData, pixelDataOriginal, imageBitmap.Width, imageBitmap.Height, imageBitmap.Height / 2 , imageBitmap.Height-1);
            }
    
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


            System.Runtime.InteropServices.Marshal.Copy(pixelData, 0, ptr, pixelData.Length); //- 54);


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
            if (int.TryParse(comboBox1.SelectedItem.ToString(), out int selectedValue))
            {
                processorCount = selectedValue;
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
