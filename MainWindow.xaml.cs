using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
//using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using System.Windows.Forms;

namespace e_media0_2
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string file = string.Empty;
        byte[] myFile = null;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnFileOpen_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog();
            var result = fileDialog.ShowDialog();
            switch (result)
            {
                case System.Windows.Forms.DialogResult.OK:
                    file = fileDialog.FileName;
                    TxtFile.Content = file;
                    MainFunction(file);
                    break;
                case System.Windows.Forms.DialogResult.Cancel:
                default:
                    TxtFile.Content = null;
                    break;
            }
        }

        
        private void BtnViewImg_Click(object sender, RoutedEventArgs e)
        {
            pictureWindow pw = new pictureWindow(file);
            pw.DataContext = file;
            try
            {
                pw.Show();
            }
            catch (System.InvalidOperationException)
            {
                pw.Close();
            }
        }

        private void BtnFFT_Click(object sender, RoutedEventArgs e)
        {
            fftWindow ft = new fftWindow(file);
            ft.DataContext = file;
            try
            {
                ft.Show();
            }
            catch (System.InvalidOperationException)
            {
                ft.Close();
            }
        }

        private void BtnAnonImg_Click(object sender, RoutedEventArgs e)
        {
            if (myFile == null)
            {
                System.Windows.MessageBox.Show("No file here", "Error");
            }
            else
            {
                Anon(myFile);
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void MainFunction(string name)
        {
            //MainWindow wndow = new MainWindow();
            try
            {
                Image myBmp = LoadImg(name);
                myFile = ConvertToByte(myBmp);
                DisplayData(myFile);
            }
            catch (OutOfMemoryException)
            {
                System.Windows.MessageBox.Show("Pick BMP u fool", "Error");
            }
        }

        /// <summary>
        /// Display data from image
        /// </summary>
        /// <param name="myFile">array with image data</param>
        private void DisplayData(byte[] myFile)
        {
            if (myFile[0] == 0x42 & myFile[1] == 0x4D)
            {
                labelBTMhead.Content = "Bitmap header file:";
                labelType.Content = "Type: BMP";
                labelSize.Content = "Size in bytes: " + ReadData(myFile, 2, 6);
                labelOffset.Content = "Starting point (byte): " + ReadData(myFile, 10, 13);

                labelDIB.Content = "DIB header:";
                labelDIBsize.Content = "Header size: " + ReadData(myFile, 14, 17);
                labelWidth.Content = "Width: " + ReadData(myFile, 18, 21);
                labelHeight.Content = "Height: " + ReadData(myFile, 22, 25);
                labelColorPlanes.Content = "Color planes: " + ReadData(myFile, 26, 27);
                labelBtsPerPxl.Content = "Bytes per pixel: " + ReadData(myFile, 28, 29);
                labelCompr.Content = "Compression method (0 is none): " + ReadData(myFile, 30, 33);
                labelImSize.Content = "Img size (0 if BI_RGB bitmap): " + ReadData(myFile, 34, 37);
                labelPxlH.Content = "Pixel per metre (horizontal): " + ReadData(myFile, 38, 41);
                labelPxlV.Content = "Pixel per metre (vertical): " + ReadData(myFile, 42, 45);
                labelColPal.Content = "Colors in the color palette: " + ReadData(myFile, 46, 49);
                labelImpCol.Content = "Important colors (if 0 none or all equal): " + ReadData(myFile, 50, 53);
            }
            else
                System.Windows.MessageBox.Show("Pick BMP u fool");
        }

        /// <summary>
        /// Reads piece of data from array and converts to decimal
        /// </summary>
        /// <param name="myFile">array of data</param>
        /// <param name="begin">start point for reading</param>
        /// <param name="end">end point</param>
        /// <returns>piece of data from image</returns>
        private static Int32 ReadData(byte[] myFile, int begin, int end)
        {
            string data = "";
            for (int i = begin; i < end; i++)
            {
                if (myFile[i] != 0)
                {
                    data += myFile[i].ToString("X");
                }
            }
            if (data == "")
                return 0;
            return Convert.ToInt32(data, 16);
        }

        private static void WriteData(byte[] myFile, int begin, int end, byte data)
        {
            for (int i = begin; i < end; i++)
            {
                myFile[i] = data;
            }
        }

        /// <summary>
        /// Loads an image (idk if necessary)
        /// </summary>
        /// <param name="name">name of the file</param>
        /// <returns>image</returns>
        private static Image LoadImg(string name)
        {
            Image myBmp = Image.FromFile(name);

            return myBmp;
        }

        /// <summary>
        /// Converts image to byte array
        /// </summary>
        /// <param name="myBmp">Image to convert</param>
        /// <returns>byte array with image contents</returns>
        private static byte[] ConvertToByte(Image myBmp)
        {
            ImageConverter converter = new ImageConverter();

            return (byte[])converter.ConvertTo(myBmp, typeof(byte[]));
        }

        private static Image ConvertToImage(byte[] myFile)
        {
            ImageConverter converter = new ImageConverter();

            return (Bitmap)converter.ConvertFrom(myFile);
        }

        private void Anon(byte[] myFile)
        {
            WriteData(myFile, 38, 53, 0);
            DisplayData(myFile);
            Image tmp = ConvertToImage(myFile);

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "All files (*.*)|*.*|BMP files (*.bmp)|*.bmp";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                System.IO.File.WriteAllBytes(saveFileDialog1.FileName, myFile);
            }
            

        }
    }
}
