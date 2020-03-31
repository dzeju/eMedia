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
using AForge.Imaging;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;

namespace e_media0_2
{
    /// <summary>
    /// Logika interakcji dla klasy fftWindow.xaml
    /// </summary>
    public partial class fftWindow : Window
    {
        public fftWindow(string file)
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
                Bitmap grayScaleBP = ToGrayscale(myBmp);
                System.Drawing.Rectangle cloneRect = new System.Drawing.Rectangle(0, 0, grayScaleBP.Width, grayScaleBP.Height);
                Bitmap clone = myBmp.Clone(cloneRect, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
                
                ComplexImage complexImage = ComplexImage.FromBitmap(clone);
                myBmp.Dispose();

                complexImage.ForwardFourierTransform();
                Bitmap fourierImage = complexImage.ToBitmap();

                imgFFT.Height = fourierImage.Height*2;
                imgFFT.Width = fourierImage.Width*2;
                imgFFT.Source = BitmapToImageSource(fourierImage);
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

        public static unsafe Bitmap ToGrayscale(Bitmap colorBitmap)
        {
            int Width = colorBitmap.Width;
            int Height = colorBitmap.Height;

            Bitmap grayscaleBitmap = new Bitmap(Width, Height, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);

            grayscaleBitmap.SetResolution(colorBitmap.HorizontalResolution,
                                 colorBitmap.VerticalResolution);

            ///////////////////////////////////////
            // Set grayscale palette
            ///////////////////////////////////////
            ColorPalette colorPalette = grayscaleBitmap.Palette;
            for (int i = 0; i < colorPalette.Entries.Length; i++)
            {
                colorPalette.Entries[i] = System.Drawing.Color.FromArgb(i, i, i);
            }
            grayscaleBitmap.Palette = colorPalette;
            ///////////////////////////////////////
            // Set grayscale palette
            ///////////////////////////////////////
            BitmapData bitmapData = grayscaleBitmap.LockBits(
                new System.Drawing.Rectangle(System.Drawing.Point.Empty, grayscaleBitmap.Size),
                ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);

            Byte* pPixel = (Byte*)bitmapData.Scan0;

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    System.Drawing.Color clr = colorBitmap.GetPixel(x, y);

                    Byte byPixel = (byte)((30 * clr.R + 59 * clr.G + 11 * clr.B) / 100);

                    pPixel[x] = byPixel;
                }

                pPixel += bitmapData.Stride;
            }

            grayscaleBitmap.UnlockBits(bitmapData);

            return grayscaleBitmap;
        }

        public static Bitmap MakeGrayscale3(Bitmap original)
        {
            //create a blank bitmap the same size as original
            Bitmap newBitmap = new Bitmap(original.Width, original.Height);

            //get a graphics object from the new image
            using (Graphics g = Graphics.FromImage(newBitmap))
            {

                //create the grayscale ColorMatrix
                ColorMatrix colorMatrix = new ColorMatrix(
                   new float[][]
                   {
             new float[] {.3f, .3f, .3f, 0, 0},
             new float[] {.59f, .59f, .59f, 0, 0},
             new float[] {.11f, .11f, .11f, 0, 0},
             new float[] {0, 0, 0, 1, 0},
             new float[] {0, 0, 0, 0, 1}
                   });

                //create some image attributes
                using (ImageAttributes attributes = new ImageAttributes())
                {

                    //set the color matrix attribute
                    attributes.SetColorMatrix(colorMatrix);

                    //draw the original image on the new image
                    //using the grayscale color matrix
                    g.DrawImage(original, new System.Drawing.Rectangle(0, 0, original.Width, original.Height),
                                0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);
                }
            }
            return newBitmap;
        }
    }
}
