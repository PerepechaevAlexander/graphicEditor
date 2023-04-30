using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace graphicEditor
{
    public class BitmapModel
    {


        #region Fields & Getters & Setters
        // Изображение, которое хранит ImageFile
        private WriteableBitmap writeableBitmap;
        // Ширина и высота изображения
        private int width;
        private int height;

        public WriteableBitmap WriteableBitmap
        {
            get { return writeableBitmap; }
        }
        public int Width
        {
            get { return width; }
        }
        public int Height
        {
            get { return height; }
        }
        #endregion





        # region Constructors
        // Модель по-умолчанию: изображение 1140*540 в формате RGBA32
        public BitmapModel()
        {
            this.writeableBitmap = new WriteableBitmap(1140, 540, 96, 96, PixelFormats.Bgra32, null);
            this.width = 1140;
            this.height = 540;
        }
        // Модель на основе уже существующего изображения, в параметрах передаётся путь к файлу
        public BitmapModel(string fileName)
        {
            // Надеюсь, что это временный костыль(но это не точно):
            // filName -> Uri -> BitmapImage -> WriteableBitmap.
            BitmapImage vedro = new BitmapImage(new Uri(fileName));
            this.writeableBitmap = new WriteableBitmap(vedro);
            this.width = vedro.PixelWidth;
            this.height = vedro.PixelHeight;
        }
        // Модель на основе BitmapModel, используется при вызове окон редактирования для создания копии изображения
        public BitmapModel(BitmapModel originalModel)
        {
            // Если оиригинальное изображение слишком большое - делаем уменьшенную копию
            if (originalModel.Width > 1500)
            {
                TransformedBitmap transformedBitmap = new TransformedBitmap(originalModel.WriteableBitmap, new ScaleTransform(0.5, 0.5/*newWidth / bitmapSource.PixelWidth, newHeight / bitmapSource.PixelHeight*/));
                this.writeableBitmap = new WriteableBitmap(transformedBitmap);
            }
            else
            {
                this.writeableBitmap = new WriteableBitmap(originalModel.WriteableBitmap);
            }
            this.width = (int)writeableBitmap.Width;
            this.height = (int)writeableBitmap.Height;
        }
        public BitmapModel(BitmapSource original)
        {
            // Если оиригинальное изображение слишком большое - делаем уменьшенную копию
            this.writeableBitmap = new WriteableBitmap(original);
            this.width = (int)writeableBitmap.Width;
            this.height = (int)writeableBitmap.Height;
        }
        #endregion




        #region Methods For Create BitmapModel
        // Получаем белый холст. Используется при создании изображения в программе
        public void GetWhiteWriteableBitmap()
        {
            // Резервируем задний буфер для обновлений
            writeableBitmap.Lock();
            unsafe
            {
                // Получаем указатель на задний буфер
                IntPtr pBackBuffer = writeableBitmap.BackBuffer;

                byte* pbuff = (byte*)pBackBuffer.ToPointer();

                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        // Вычисляем место текущего пикселя в массиве байтов
                        int loc = y * writeableBitmap.BackBufferStride + x * 4;
                        //Производим необходимые изменения
                        pbuff[loc] = 255; // Blue
                        pbuff[loc + 1] = 255; // Green
                        pbuff[loc + 2] = 255; // Red
                        pbuff[loc + 3] = 255; // Alpha
                    }
                }
            }
            // Указываем зону, затронутую задним буфером
            writeableBitmap.AddDirtyRect(new Int32Rect(0, 0, width, height));
            // Освобождаем задний буфер и делаем его доступным для отображения
            writeableBitmap.Unlock();
        }
        #endregion





        #region Methods For Color Correction

        public void Red(int red, ref byte[] pixelBytes)
        {
            //MessageBox.Show("Ширина" + writeableBitmap.Width + "Высота" + writeableBitmap.Height, "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question);
            // Резервируем задний буфер для обновлений
            writeableBitmap.Lock();
            unsafe
            {
                // Получаем указатель на задний буфер
                IntPtr pBackBuffer = writeableBitmap.BackBuffer;

                byte* pbuff = (byte*)pBackBuffer.ToPointer();

                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        // Вычисляем место текущего пикселя в массиве байтов
                        int loc = y * writeableBitmap.BackBufferStride + x * 4;
                        byte oldRed = pixelBytes[(y * width + x) * 4 + 2];
                        //byte oldRed = pixelBytes[loc + 2];
                        if (red > 0)
                        {
                            if(oldRed < red)
                            {
                                pbuff[loc + 2] = (byte)red;
                            }
                            else
                            {
                                
                            }
                        }
                        else
                        {
                            if (oldRed > 255 + red)
                            {
                                pbuff[loc + 2] = (byte)(255 + red);
                            }
                            else
                            {

                            }
                        }
                    }
                }
            }
            // Указываем зону, затронутую задним буфером
            writeableBitmap.AddDirtyRect(new Int32Rect(0, 0, width, height));
            // Освобождаем задний буфер и делаем его доступным для отображения
            writeableBitmap.Unlock();
        }



        #endregion









        // Метод Red через буфер
        public void RedByBuffer()
        {
            try
            {
                // Резервируем задний буфер для обновлений
                writeableBitmap.Lock();
                unsafe
                {
                    // Get a pointer to the back buffer.
                    IntPtr pBackBuffer = writeableBitmap.BackBuffer;

                    byte* pbuff = (byte*)pBackBuffer.ToPointer();

                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            // Вычисляем место текущего пикселя в массиве байтов
                            int loc = y * writeableBitmap.BackBufferStride + x * 4;
                            //Производим необходимые изменения
                            //pbuff[loc] = c.B; // Blue
                            //pbuff[loc + 1] = c.G; // Green
                            pbuff[loc + 2] = 255; // Red
                            //pbuff[loc + 3] = c.A; // Alpha
                        }
                    }
                }
                // Указываем зону, затронутую задним буфером
                writeableBitmap.AddDirtyRect(new Int32Rect(0, 0, width, height));
            }
            finally
            {
                // Release the back buffer and make it available for display.
                writeableBitmap.Unlock();
            }
        }





        #region Methods For Drawing
        // Рисование - долгий ящик
        public void DrawPixel(MouseEventArgs e, int X, int Y)
        {
            try
            {
                // Резервируем задний буфер для обновлений
                writeableBitmap.Lock();
                unsafe
                {
                    // Get a pointer to the back buffer.
                    IntPtr pBackBuffer = writeableBitmap.BackBuffer;

                    // Find the address of the pixel to draw.
                    pBackBuffer += Y * writeableBitmap.BackBufferStride;
                    pBackBuffer += X * 4;

                    // Compute the pixel's color.
                    int color_data = 255 << 16; // R
                    color_data |= 128 << 8;   // G
                    color_data |= 255 << 0;   // B

                    // Assign the color data to the pixel.
                    *((int*)pBackBuffer) = color_data;
                }

                // Specify the area of the bitmap that changed.
                writeableBitmap.AddDirtyRect(new Int32Rect(X, Y, 1, 1));
            }
            finally
            {
                // Release the back buffer and make it available for display.
                writeableBitmap.Unlock();
            }
        }

        public void ErasePixel(MouseEventArgs e, int X, int Y)
        {
            byte[] ColorData = { 255, 255, 255, 255 }; // B G R

            Int32Rect rect = new Int32Rect(X, Y, 1, 1);

            writeableBitmap.WritePixels(rect, ColorData, 4, 0);
        }
        #endregion





        // Попиксельная обработка - не уверен, что нужно, но пусть будет
        public void RedByPixels()
        {
            // Массив цветов каждого пикселя изображения
            byte[] pixelBytes = new byte[height * width * 4];
            writeableBitmap.CopyPixels(pixelBytes, width * 4, 0);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    // Получаем значения цвета пикселя (x,y) из массива
                    byte blue = pixelBytes[(y * width + x) * 4 + 0];
                    byte green = pixelBytes[(y * width + x) * 4 + 1];
                    byte red = pixelBytes[(y * width + x) * 4 + 2];
                    byte alpha = pixelBytes[(y * width + x) * 4 + 3];

                    // Set the pixel value.                    
                    byte[] colorData = { blue, green, 255, alpha }; // B G R

                    Int32Rect rect = new Int32Rect(x, y, 1, 1);
                    //int stride = (width * bitmap.Format.BitsPerPixel) / 8;
                    writeableBitmap.WritePixels(rect, colorData, 4, 0);

                    //bitmap.WritePixels(.[y * bitmap.PixelWidth + x] = pixelColorValue;
                }
            }


            //  Закраска
            /*  byte blue = 100;
                byte green = 50;
                byte red = 50;
                byte alpha = 255;
                byte[] colorData = { blue, green, red, alpha };
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        // Обновить одиночный пиксель. Область начинается с (x,y)
                        //и имеет размер в 1 пиксель в ширину и 1 пиксель в высоту.
                        Int32Rect rect = new Int32Rect(x, y, 1, 1);
                        // Записать 4 байта из массива в растровое изображение.
                        writeableBitmap.WritePixels(rect, colorData, 4, 0);
                    }
                }*/


        }



    }
}
