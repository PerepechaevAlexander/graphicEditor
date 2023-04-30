using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
// Долгий ящик:
// ??? Возможно, так должно быть ???   Поправить конструктор BitmapModel(string fileName)
// Поправить и потестить кучу проверок при сохранении файла

//                  НАПОМИНАЛКА
// CollorCorrectionWindow -> textBox_gamma -> разобраться, как исправить ощибку изменения поля Text до начала инициализации
// Поправить фильтры при открытии файла


namespace graphicEditor
{
    public partial class MainWindow : Window
    {
        // Текущий ФАЙЛ изображения. После сохранения текущего файла или
        // открытия нового, imageFile обновляется и хранит данные о последнем использованном файле.
        ImageFile imageFile;


        public MainWindow()
        {
            InitializeComponent();
            // При запуске программы создаётся новый imageFile,
            // его fileName и fileExtension = null до первого сохранения.
            imageFile = ImageFile.CreateImageFile();
            // Обновляем Image и Canvas
            UpdateCanvasAndImage();
        }





        #region Menu
        private void MenuItem_Create_Click(object sender, RoutedEventArgs e)
        {
            // Запрос на утерю изменений в текущем изображении
            if (RequestToUnsafeImageFile("создать новое изображение?") == true)
            {
                // Создаём изображение
                imageFile = ImageFile.CreateImageFile();
                // Обновляем Image и Canvas
                UpdateCanvasAndImage();
            }
        }

        private void MenuItem_Open_Click(object sender, RoutedEventArgs e)
        {
            if (RequestToUnsafeImageFile("открыть новое изображение?") == true)
            {
                ImageFile newImage = ImageFile.OpenImageFile();
                if (newImage != null)
                {
                    // Отрываем изображение
                    imageFile = newImage;
                    // Обновляем Image и Canvas
                    UpdateCanvasAndImage();
                }
            }
        }

        private void MenuItem_Save_Click(object sender, RoutedEventArgs e)
        {
            imageFile.SaveImageFile();
        }

        private void MenuItem_SaveAs_Click(object sender, RoutedEventArgs e)
        {
            imageFile.SaveImageFileInNewFile();
        }

        private void MenuItem_Close_Click(object sender, RoutedEventArgs e)
        {
            // Запрос на утерю изменений в текущем изображении
            if (RequestToUnsafeImageFile("выйти?") == true)
            {
                Close();
            }
        }
        #endregion





        #region Buttons

        // Запуск окна коррекции цветов
        private void button_ColorCorrection_Click(object sender, RoutedEventArgs e)
        {
            ColorCorrectionWindow colorCorrectionWindow = new ColorCorrectionWindow(imageFile.BitmapModel);
            colorCorrectionWindow.ShowDialog();
        }

        #endregion





        // Обновление размеров Canvas/Image, ресурса Image
        private void UpdateCanvasAndImage()
        {
            // Задаём для Canvas длину/ширину
            //canvas.Width = imageFile.BitmapModel.Width;
            //canvas.Height = imageFile.BitmapModel.Height;
            // Задаём для Image длину/ширину, а также ресурс
            image.Width = imageFile.BitmapModel.Width;
            image.Height = imageFile.BitmapModel.Height;
            image.Source = imageFile.BitmapModel.WriteableBitmap;
        }

        // Запрос пользователю для удаления текущего изображения
        private bool RequestToUnsafeImageFile(string action)
        {
            if (MessageBox.Show("Вы потеряете все изменения, сделанные в текущем изображении.\nВы уверены, что хотите " + action, "Внимание!",
            MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                return true;
            else return false;
        }





        #region Mouse
        private void image_MouseMove(object sender, MouseEventArgs e)
        {
            
            int X = (int)e.GetPosition(image).X;
            int Y = (int)e.GetPosition(image).Y;
            if (X < imageFile.BitmapModel.Width && Y< imageFile.BitmapModel.Height)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    imageFile.BitmapModel.DrawPixel(e, X, Y);
                }
                else if (e.RightButton == MouseButtonState.Pressed)
                {
                    imageFile.BitmapModel.ErasePixel(e, X, Y);
                }
            }
        }

        private void image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            int X = (int)e.GetPosition(image).X;
            int Y = (int)e.GetPosition(image).Y;
            if (X < imageFile.BitmapModel.Width && Y < imageFile.BitmapModel.Height)
            {
                imageFile.BitmapModel.DrawPixel(e, X, Y);
            }
        }

        private void image_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            int X = (int)e.GetPosition(image).X;
            int Y = (int)e.GetPosition(image).Y;
            if (X < imageFile.BitmapModel.Width && Y < imageFile.BitmapModel.Height)
            {
                imageFile.BitmapModel.ErasePixel(e, X, Y);
            }
        }
        #endregion





    }
}
