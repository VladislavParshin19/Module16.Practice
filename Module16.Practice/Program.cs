using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Module16.Practice
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Введите путь к отслеживаемой директории:");
            string directoryPath = Console.ReadLine();

            Console.WriteLine("Введите путь к лог-файлу:");
            string logFilePath = Console.ReadLine();

            try
            {
                DirectoryWatcher directoryWatcher = new DirectoryWatcher(directoryPath, logFilePath);
                directoryWatcher.StartWatching();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

    }
    class DirectoryWatcher
    {
        private FileSystemWatcher watcher;
        private string logFilePath;

        public DirectoryWatcher(string directoryPath, string logFilePath)
        {
            this.logFilePath = logFilePath;

            // Инициализация FileSystemWatcher
            watcher = new FileSystemWatcher
            {
                Path = directoryPath,
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastWrite | NotifyFilters.CreationTime,
                EnableRaisingEvents = true
            };

            // Подписка на события
            watcher.Created += OnFileOrDirectoryChanged;
            watcher.Deleted += OnFileOrDirectoryChanged;
            watcher.Renamed += OnFileRenamed;

            Console.WriteLine($"Отслеживание директории: {directoryPath}");
            Console.WriteLine($"Лог-файл: {logFilePath}");
            Console.WriteLine("Для завершения работы нажмите 'Q'.\n");
        }

        public void StartWatching()
        {
            Console.WriteLine("Отслеживание начато...");
            Console.WriteLine("Для завершения работы нажмите 'Q'.\n");

            // Ожидание нажатия 'Q' для завершения работы
            while (Console.ReadKey().Key != ConsoleKey.Q) { }

            // Остановка FileSystemWatcher
            watcher.EnableRaisingEvents = false;
            watcher.Dispose();

            Console.WriteLine("Отслеживание завершено.");
        }

        private void OnFileOrDirectoryChanged(object sender, FileSystemEventArgs e)
        {
            LogChange($"Изменение: {e.ChangeType}, Путь: {e.FullPath}");
        }

        private void OnFileRenamed(object sender, RenamedEventArgs e)
        {
            LogChange($"Переименование: {e.ChangeType}, Старое имя: {e.OldFullPath}, Новое имя: {e.FullPath}");
        }

        private void LogChange(string message)
        {
            try
            {
                // Запись в лог-файл с указанием времени и типа изменения
                string logMessage = $"{DateTime.Now} - {message}";
                File.AppendAllText(logFilePath, logMessage + Environment.NewLine);
                Console.WriteLine(logMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при записи в лог-файл: {ex.Message}");
            }
        }
    }
}
