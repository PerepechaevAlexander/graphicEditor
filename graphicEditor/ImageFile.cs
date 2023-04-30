using Microsoft.VisualBasic;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace graphicEditor
{
    internal class ImageFile
    {
        #region Fields & Getters & Setters
        // Путь к файлу
        string? fileName;
        // Формат файла
        string? fileExtension;
        // Изображение (модель данных), полученная из существующего файла или созданная в программе
        BitmapModel bitmapModel;

        public string FileName
        {
            get { return fileName; }
        }
        public string FileExtension
        {
            get { return fileExtension; }
        }
        public BitmapModel BitmapModel
        {
            get { return bitmapModel; }
        }
        #endregion





        #region Construcors
        private ImageFile()
        {
            // Создаётся изображение по-умолчанию
            this.bitmapModel = new BitmapModel();
            // Задаём значение каждому пикселю изображения, ибо изначально они то ли null, то ли ещё что с ними не так
            bitmapModel.GetWhiteWriteableBitmap();
        }
        private ImageFile(string name, string extension, BitmapModel bitmapModel)
        {
            this.fileName = name;
            this.fileExtension = extension.ToLower();
            this.bitmapModel = bitmapModel;
        }
        #endregion





        #region MethodsForMenuButtons
        public static ImageFile CreateImageFile()
        {
            ImageFile imageFile = new ImageFile();
            return imageFile;
        }

        // Может вернуть NULL
        public static ImageFile OpenImageFile()
        {
            // Объявляем объект ImageFile
            ImageFile imageFile = null;
            // Открываем стандартный OpenFileDialog.
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "All(*.png;*.jpeg;*.jpg;*.bmp)|*.png;*.jpeg;*.jpg;*.bmp|PNG Files (*.png)|*.png|JPEG Files (*.jpeg)|*.jpeg|JPG Files (*.jpg)|*.jpg|BMP Files (*.bmp)|*.bmp";
            Nullable<bool> result = dlg.ShowDialog();
            // Если изображение успешно получено -> возвращаемImageFile.
            if (result == true)
            {
                string fileName = dlg.FileName;
                string fileExtension = Path.GetExtension(fileName).ToLower();
                BitmapModel bitmapModel = new BitmapModel(fileName);
                imageFile = new ImageFile(fileName, fileExtension, bitmapModel);
            }
            // Иначе вернём null.
            return imageFile;
        }

        public void SaveImageFile()
        {
            if (fileExtension == "png")
            {
                this.SaveAsPng(fileName);
            }
            else if (fileExtension == "jpeg")
            {
                this.SaveAsJpeg(fileName);
            }
            else if (fileExtension == "bmp")
            {
                this.SaveAsBmp(fileName);
            }
            else
            {
                SaveImageFileInNewFile();
            }
        }

        public void SaveImageFileInNewFile()
        {
            // Если существует модель изображения, то открываем стандартный SaveFileDialog,
            if (bitmapModel != null)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "png Image|*.png|jpeg Image|*.jpeg|bmp Image|*.bmp";
                saveFileDialog.Title = "Сохранить как изображение";
                saveFileDialog.ShowDialog();
                if (saveFileDialog.FileName != "")
                {
                    // Задаём для ImageFile путь и формат
                    fileName = saveFileDialog.FileName;
                    fileExtension = Path.GetExtension(fileName).ToLower();
                    switch (saveFileDialog.FilterIndex)
                    {
                        case 1:
                            {
                                this.SaveAsPng(fileName);
                            }
                            break;
                        case 2:
                            {
                                this.SaveAsJpeg(fileName);
                            }
                            break;
                        case 3:
                            {
                                this.SaveAsBmp(fileName);
                            }
                            break;
                    }
                }
            }
            else
            {
                MessageBox.Show("Изображение не найдено.", "Ошибка сохранения", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion





        #region MethodsForSave
        // Сохранение изображений в png|jpeg|bmp форматах. Используется стандартный класс encoder
        private void SaveAsPng(string newFileName)
        {
            // Объявляем соответсвующий encoder, добавляем в него изображение
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmapModel.WriteableBitmap));
            // Запускаем FileStream
            using (FileStream stream = new FileStream(fileName, FileMode.Create))
            {
                encoder.Save(stream);
            }
        }
        private void SaveAsJpeg(string newFileName)
        {
            var encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmapModel.WriteableBitmap));
            using (FileStream stream = new FileStream(fileName, FileMode.Create))
            {
                encoder.Save(stream);
            }
        }
        private void SaveAsBmp(string newFileName)
        {
            var encoder = new BmpBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmapModel.WriteableBitmap));
            using (FileStream stream = new FileStream(fileName, FileMode.Create))
            {
                encoder.Save(stream);
            }
        }
        #endregion



    }
}
