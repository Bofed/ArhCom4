using System;
using System.IO;
using System.Linq;

class Program
{
    static int Main()
    {
        static void CopyFiles(string sourceDirectory, string targetDirectory, bool enablePrompt, string[] fileExtensions)
        {
            if (!Directory.Exists(sourceDirectory))
            {
                throw new DirectoryNotFoundException($"Исходный каталог '{sourceDirectory}' не существует.");
            }

            if (!Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
                Console.WriteLine("Целевой каталог не существовал и был создан.");
            }

            string[] files = Directory.GetFiles(sourceDirectory);

            foreach (string filePath in files)
            {
                string fileName = Path.GetFileName(filePath);
                string targetFilePath = Path.Combine(targetDirectory, fileName);

                FileAttributes attributes = File.GetAttributes(filePath);

                if (enablePrompt)
                {
                    Console.WriteLine($"Копирование файла: {fileName}");
                    Console.WriteLine("Атрибуты файла:");   
                    Console.WriteLine($"  Скрытый: {((attributes & FileAttributes.Hidden) == FileAttributes.Hidden)}");
                    Console.WriteLine($"  Только для чтения: {((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)}");
                    Console.WriteLine($"  Архив: {((attributes & FileAttributes.Archive) == FileAttributes.Archive)}");
                }

                string fileExtension = Path.GetExtension(filePath);
                if (!fileExtensions.Contains(fileExtension))
                {
                    // Расширение файла не входит в выбранные типы файлов, пропустить файл
                    continue;
                }

                File.Copy(filePath, targetFilePath, true);

                // Применение атрибутов файла к скопированному файлу
                FileAttributes targetAttributes = File.GetAttributes(targetFilePath);

                if ((attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                {
                    targetAttributes |= FileAttributes.Hidden;
                }

                if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    targetAttributes |= FileAttributes.ReadOnly;
                }

                if ((attributes & FileAttributes.Archive) == FileAttributes.Archive)
                {
                    targetAttributes |= FileAttributes.Archive;
                }

                File.SetAttributes(targetFilePath, targetAttributes);
            }
        }

        Console.WriteLine("Введіть шлях до каталогу, з якого будемо копіювати:");
        string sourceDirectory = Console.ReadLine();

        Console.WriteLine("Введіть шлях до каталогу, в який будемо копіювати.:");
        string targetDirectory = Console.ReadLine();

        Console.WriteLine("Введіть тип файлів, які будуть копіюватися. Роздільники - пробіл:");
        string fileExtensionsInput = Console.ReadLine();
        string[] fileExtensions = fileExtensionsInput.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

        Console.WriteLine("Хотите ли вы использовать режим подсказок? (Да/Нет)");
        bool enablePrompt = Console.ReadLine().Equals("Да", StringComparison.OrdinalIgnoreCase);

        try
        {
            CopyFiles(sourceDirectory, targetDirectory, enablePrompt, fileExtensions);
            Console.WriteLine("Копирование файлов завершено.");
            return 1;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Произошла ошибка при копировании файлов:");
            Console.WriteLine(ex.Message);
            return 0;
        }
    }
}
