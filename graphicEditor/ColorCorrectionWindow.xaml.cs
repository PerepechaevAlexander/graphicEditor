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
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using static System.Collections.Specialized.BitVector32;

namespace graphicEditor
{
    /// <summary>
    /// Логика взаимодействия для ColorCorrectionWindow.xaml
    /// </summary>
    public partial class ColorCorrectionWindow : Window
    {
        // Оригинальная модель изображения
        BitmapModel originalModel;
        // Текущая модель изображения
        BitmapModel newModel;
        // Масив пикселей изображения
        byte[] pixelBytes;


        // Коэффициенты настроек
        byte brightness = 0; // Яркость [-100..100]
        byte contrast = 0; // Контрастность [-100..100]
        double gamma = 1.00; // Гамма [0 .. 10,00]

        int red = 0; // Красный [-255..255]
        byte green = 0; // Зелёный [-255..255]
        byte blue = 0; // Синий [-255..255]

        byte shade = 0; // Оттенок [-180..180]
        byte saturation = 0; // Насыщенность [-100..100]

        public ColorCorrectionWindow(BitmapModel originalModel)
        {
            InitializeComponent();

            this.originalModel = originalModel;
            image.Source = originalModel.WriteableBitmap;

            // Через TransformedBitmap
            //this.newModel = new BitmapModel(originalModel);

            // Через метод CreateResizedImage
            BitmapFrame bitmapFrame = CreateResizedImage(image.Source, (int)image.Source.Width/2, (int)image.Source.Height/2, 0);
            this.newModel = new BitmapModel(bitmapFrame);

            UpdateImage();

            pixelBytes = new byte[newModel.Height * newModel.Width * 4];
            newModel.WriteableBitmap.CopyPixels(pixelBytes, newModel.Width * 4, 0);
        }


        // Создание уменьшенного изображения
        private static BitmapFrame CreateResizedImage(ImageSource source, int width, int height, int margin)
        {
            var rect = new Rect(margin, margin, width - margin * 2, height - margin * 2);

            var group = new DrawingGroup();
            RenderOptions.SetBitmapScalingMode(group, BitmapScalingMode.HighQuality);
            group.Children.Add(new ImageDrawing(source, rect));

            var drawingVisual = new DrawingVisual();
            using (var drawingContext = drawingVisual.RenderOpen())
                drawingContext.DrawDrawing(group);

            var resizedImage = new RenderTargetBitmap(
                width, height,         // Resized dimensions
                96, 96,                // Default DPI values
                PixelFormats.Default); // Default pixel format
            resizedImage.Render(drawingVisual);

            return BitmapFrame.Create(resizedImage);
        }

        private void UpdateImage()
        {
            // Задаём для Image длину/ширину, а также ресурс
            //image.Width = newModel.Width;
            //image.Height = newModel.Height;
            image.Source = newModel.WriteableBitmap;
        }





        #region Buttons
        private void button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void button_Reset_Click(object sender, RoutedEventArgs e)
        {
            // Ползунки по-умолчанию
            slider_Brightness.Value = 0;
            slider_Contrast.Value = 0;
            slider_Gamma.Value = 1;
            slider_Red.Value = 0;
            slider_Green.Value = 0;
            slider_Blue.Value = 0;
            slider_Shade.Value = 0;
            slider_Saturation.Value = 0;
        }
        private void button_OK_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion





        #region Sliders_ValueChanged
        private void slider_Brightness_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            brightness = (byte)slider_Brightness.Value;
        }

        private void slider_Contrast_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            contrast = (byte)slider_Contrast.Value;
        }
        private void slider_Gamma_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // Посмотреть тип данных
            gamma = (double)slider_Gamma.Value;
        }



        private void slider_Red_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            red = (int)slider_Red.Value;
            //this.newModel = new BitmapModel(originalModel);
            newModel.Red(red, ref pixelBytes);
            UpdateImage();
        }

        private void slider_Green_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            green = (byte)slider_Green.Value;
        }

        private void slider_Blue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            blue = (byte)slider_Blue.Value;
        }



        private void slider_Shade_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            shade = (byte)slider_Shade.Value;
        }

        private void slider_Saturation_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            saturation = (byte)slider_Saturation.Value;
        }



        #endregion

        private void slider_Red_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (slider_Red != null)
            {
                slider_Red.Value += slider_Red.SmallChange * e.Delta / 12;
            }
        }





    }
}
