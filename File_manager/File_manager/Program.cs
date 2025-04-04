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
        static void Main(string[] args)
        {
            var elems = new[]
            {
                new Element("Получить содержимое директории") { Command = InfoFile },
                new Element("Получить содержимое директории"), 
                new Element("Получить содержимое директории"),
                new Element("Получить содержимое директории"),
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
            Console.WriteLine("Вставьте путь директрии в формате (C:\\папка\\папка)");

            try
            {
                var path = Console.ReadLine();

                if (Directory.Exists(path))
                {
                    var files = Directory.GetFiles(path);
                    var directories = Directory.GetDirectories(path);

                    Console.WriteLine("Содержимое директории:");

                    if (files.Length > 0)
                    {
                        Console.Clear();
                        Console.WriteLine("Файлы:");
                        foreach (var file in files)
                        {
                            Console.WriteLine(Path.GetFileName(file));
                        }
                    }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("Нет файлов.");
                    }

                    if (directories.Length > 0)
                    {
                        Console.WriteLine("\nПодкаталоги:");
                        foreach (var directory in directories)
                        {
                            Console.WriteLine(Path.GetFileName(directory));
                        }
                    }
                    else
                    {
                        Console.WriteLine("Нет подкаталогов.");
                    }
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
            Console.ReadKey();
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
