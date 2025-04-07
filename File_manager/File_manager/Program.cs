using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace File_manager
{
    internal class Program
    {
        private static string selectedFile = "";
        private static string currentPath = "";

        static void Main(string[] args)
        {
            var elems = new[]
            {
                new Element("Получить содержимое директории") { Command = InfoFile },
                new Element("       \n Выход \n       ") { Command = Exit }
            };

            Menu menu = new Menu(elems);
            while (true)
            {
                menu.Draw();
                var key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        menu.SelectPrev();
                        break;
                    case ConsoleKey.DownArrow:
                        menu.SelectNext();
                        break;
                    case ConsoleKey.Enter:
                        menu.ExecuteSelected();
                        break;
                    default:
                        return;
                }
            }
        }

        private static void InfoFile()
        {
            Console.Clear();
            Console.WriteLine("Вставьте путь директории в формате (C:\\папка\\папка):");

            try
            {
                var path = Console.ReadLine();
                if (Directory.Exists(path))
                {
                    currentPath = path;
                    NavigateDirectory(path);
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Директорий не найден.");
                }
            }
            catch (Exception ex)
            {
                Console.Clear();
                Console.WriteLine($"Произошла ошибка: {ex.Message}");
            }
        }

        private static void NavigateDirectory(string path)
        {
            while (true)
            {
                var directories = Directory.GetDirectories(path);
                var files = Directory.GetFiles(path);

                Console.Clear();
                Console.WriteLine($"Содержимое директории: {path}");

                List<Element> elements = new List<Element>();

                if (path != @"C:\" && path != Directory.GetDirectoryRoot(path))
                {
                    elements.Add(new Element(".. (вернуться в предыдущую директорию)") { Command = () => NavigateDirectory(Directory.GetParent(path).FullName) });
                }

                Console.WriteLine("Подкаталоги:");
                foreach (var directory in directories)
                {
                    Console.WriteLine($"{Path.GetFileName(directory)}");
                    elements.Add(new Element(Path.GetFileName(directory)) { Command = () => NavigateDirectory(directory) });
                }

                Console.WriteLine("\nФайлы:");
                foreach (var file in files)
                {
                    Console.WriteLine($"{Path.GetFileName(file)}");
                    elements.Add(new Element(Path.GetFileName(file)) { Command = () => SelectFile(file) });
                }

                Console.WriteLine("Введите название файла или подкаталога для навигации, или 'exit' для выхода.");

                string input = Console.ReadLine();
                if (input.ToLower() == "exit") return;

                bool found = false;

                foreach (var directory in directories)
                {
                    if (Path.GetFileName(directory).Equals(input, StringComparison.OrdinalIgnoreCase))
                    {
                        NavigateDirectory(directory);
                        found = true;
                        break;
                    }
                }

                foreach (var file in files)
                {
                    if (Path.GetFileName(file).Equals(input, StringComparison.OrdinalIgnoreCase))
                    {
                        SelectFile(file);
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    Console.WriteLine("Файл или папка не найдены.");
                }
            }
        }

        private static void SelectFile(string filePath)
        {
            selectedFile = filePath;
            Console.Clear();
            Console.WriteLine($"Вы выбрали файл: {filePath}");
            Console.ReadKey();
            ShowMainMenu();
        }

        private static void ShowMainMenu()
        {
            var elems = new[]
            {
                new Element($"Выбран файл: {selectedFile}"),
                new Element("Копировать файл") { Command = CopyFile },
                new Element("Удалить файл") { Command = DeleteFile },
                new Element("Информация о файле") { Command = InformateFile },
                new Element("       \n Выход \n       ") { Command = Exit }
            };

            Menu menu = new Menu(elems);
            while (true)
            {
                menu.Draw();
                var key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        menu.SelectPrev();
                        break;
                    case ConsoleKey.DownArrow:
                        menu.SelectNext();
                        break;
                    case ConsoleKey.Enter:
                        menu.ExecuteSelected();
                        break;
                    default:
                        return;
                }
            }
        }

        private static void CopyFile()
        {
            Console.Clear();
            Console.WriteLine("Введите путь куда скопировать файл");
            var put = Console.ReadLine();

            bool copy = false;

            if (Directory.Exists(put))
            {
                try
                {
                    string destinationFile = Path.Combine(put, Path.GetFileName(selectedFile));
                    string putFile = Path.GetFileName(selectedFile);
                    File.Copy(selectedFile, destinationFile, overwrite: true);
                    Console.WriteLine($"Файл {putFile} скопирован в {destinationFile}.");
                    copy = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при копировании файла: {ex.Message}");
                }
            }
            else
            {
                Console.Clear();
                Console.WriteLine("Директорий не найден.");
            }

            if (copy)
            {
                Console.ReadKey();
                ShowMainMenu();
            }
        }

        private static void DeleteFile()
        {
            Console.Clear();
            Console.WriteLine("Точно хотите удалить файл?");

            string response;
            bool delete = false;
            string Pat = selectedFile;

            do
            {
                Console.WriteLine("Введите ответ y/n");
                response = Console.ReadLine();
            } while (response != "y" && response != "n");

            switch (response)
            {
                case "n":
                    Console.WriteLine("Ну ладно");
                    ShowMainMenu();
                    break;
                case "y":
                    try
                    {
                        File.Delete(Pat);
                        Console.WriteLine($"Файл {Pat} удалён.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка: {ex.Message}");
                    }
                    Console.ReadKey();
                    ShowMainMenu();
                    break;
            }
        }
        private static void InformateFile()
        {
            Console.Clear();
            string Pat = selectedFile;
            FileInfo fileInfo = new FileInfo(Pat);

            Console.WriteLine("Информация о файле:");
            Console.WriteLine($"Полный путь: {fileInfo.FullName}");
            Console.WriteLine($"Размер: {fileInfo.Length} байт");
            Console.WriteLine($"Дата создания: {fileInfo.CreationTime}");
            Console.WriteLine($"Дата последнего изменения: {fileInfo.LastWriteTime}");
            Console.ReadKey();
            ShowMainMenu();
        }

        private static void Exit()
        {
            Environment.Exit(0);
        }
    }

    delegate void CommandHandler();

    class Menu
    {
        public Element[] Elements { get; set; }
        public int Index { get; set; }

        public Menu(Element[] elems)
        {
            this.Index = 0;
            this.Elements = elems;
            Elements[Index].IsSelected = true;
        }

        public void Draw()
        {
            Console.Clear();
            foreach (var element in Elements)
            {
                element.Print();
            }
        }

        public void SelectNext()
        {
            if (Index < Elements.Length - 1)
            {
                Elements[Index].IsSelected = false;
                Elements[++Index].IsSelected = true;
            }
        }

        public void SelectPrev()
        {
            if (Index > 0)
            {
                Elements[Index].IsSelected = false;
                Elements[--Index].IsSelected = true;
            }
        }

        public void ExecuteSelected()
        {
            Elements[Index].Execute();
        }
    }

    class Element
    {
        public string Text { get; set; }
        public ConsoleColor SelectedForeColor { get; set; }
        public ConsoleColor SelectedBackColor { get; set; }
        public bool IsSelected { get; set; }
        public CommandHandler Command;

        public Element(string text)
        {
            this.Text = text;
            this.SelectedForeColor = ConsoleColor.Black;
            this.SelectedBackColor = ConsoleColor.Gray;
            this.IsSelected = false;
        }

        public void Print()
        {
            if (this.IsSelected)
            {
                Console.BackgroundColor = this.SelectedBackColor;
                Console.ForegroundColor = this.SelectedForeColor;
            }
            Console.WriteLine(this.Text);
            Console.ResetColor();
        }

        public void Execute()
        {
            Command?.Invoke();
        }
    }
}
