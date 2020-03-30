using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Drawing;
using System.IO;

namespace e_media0_2
{
    /// <summary>
    /// Logika interakcji dla klasy Window1.xaml
    /// </summary>
    public partial class pictureWindow : Window
    {
        public pictureWindow(string file)
        {
            InitializeComponent();

            if (string.IsNullOrEmpty(file))
            {
                MessageBox.Show("Did not specify file, nerd");
                this.Close();
            }
            else
            {
                Bitmap myBmp = new Bitmap(file);
                imgDisp.Source = BitmapToImageSource(myBmp);
                imgDisp.Height = myBmp.Height;
                imgDisp.Width = myBmp.Width;
                myBmp.Dispose();
            }
        }

        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }
    }
}
