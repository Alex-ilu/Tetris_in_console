using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Tetris
{
    internal class Program
    {
        private static int speed = 1;//Speed in units
        private static int speedMain = 1;//Initial speed
        private static int globalCount = 0;//Player score
        private static int record = 0;//Record
        private static int lines = 0;//Number of broken lines
        public static bool colorfulFigures = true;
        public static bool sound = false;//Sound setting
        public static byte xField = 10 + 1;//Field size along x
        public static byte yField = 22;//Field size along y
        public static short frame = 1200;//Delay between frames at minimum speed (in milliseconds)
        public static string[,] field = new string[127, 255];
        private static bool zebra = false;//Variable for creating checkered field
        private static bool figureInField = false;
        private static List<Figures> figures = new List<Figures>();//All created figures
        public static List<Pixel> pixels = new List<Pixel>();//All figure pixels on the field
        public static bool BgIsZebra = true;//Background setting

        private static string[,] chapters = new string[3, 8];//All menu sections and options
        private static int nowChapter = 0;//Current section
        private static int nowOption = 0;//Current option
        private static int[] maxOption = { 2, 4, 7 };//Maximum option in current section
        public static char symbol = '█';//Pixel drawing symbol
        public static string pixelSymbol = "";
        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            pixelSymbol = symbol + "" + symbol;
            Console.SetWindowSize(95, 25);
            SetChapterText();
            DrawChapter();
            while (true)//Menu controls
            {
                ConsoleKey Key = Console.ReadKey(true).Key;
                if (Key == ConsoleKey.DownArrow || Key == ConsoleKey.S)
                {
                    if (nowOption != maxOption[nowChapter]) nowOption++;
                    Sound(800, 200);
                }
                else if (Key == ConsoleKey.UpArrow || Key == ConsoleKey.W)
                {
                    if (nowOption != 0) nowOption--;
                    Sound(800, 200);
                }
                else if (Key == ConsoleKey.Escape)
                {
                    if (nowChapter != 0) nowOption = 0;
                    nowChapter = 0;
                }
                else if (Key == ConsoleKey.Spacebar)
                {
                    Sound(500, 200);
                    if (nowChapter == 0)
                    {
                        if (nowOption == 0) StartGame();
                        else if (nowOption == 1) nowChapter = 1;
                        else if (nowOption == 2) nowChapter = 2;
                        nowOption = 0;
                    }
                    else if (nowChapter == 1)
                    {
                        if (nowOption == 1)
                        {
                            chapters[1, 1] = sound ? "Sound: off" : "Sound: on";
                            sound = !sound;
                        }
                        else if (nowOption == 3)
                        {
                            chapters[1, 3] = BgIsZebra ? "Background: solid" : "Background: checkered";
                            BgIsZebra = !BgIsZebra;
                        }
                        else if (nowOption == 0)
                        {
                            chapters[1, 0] = colorfulFigures ? "Colorful figures: off" : "Colorful figures: on ";
                            colorfulFigures = !colorfulFigures;
                        }
                        else if (nowOption == 2)
                        {
                            chapters[1, 2] = speedMain < 9 ? $"Initial speed: {speedMain + 1}" : "Initial speed: 1";
                            speedMain = chapters[1, 2][15] - 48;
                        }
                    }
                }
                else if (Key == ConsoleKey.Tab)
                {
                    if (nowChapter == 1)
                    {
                        if (nowOption == 4)
                        {
                            Console.CursorVisible = true;
                            pixelSymbol = Console.ReadLine();
                            Console.CursorVisible = false;
                            chapters[1, 4] = $"Drawing symbol: {pixelSymbol} (Press Tab to change)";
                        }
                    }
                }
                else { continue; }
                DrawChapter();
            }
        }
        static void StartGame()//Start the game
        {
            bool end = false;
            globalCount = 0;
            lines = 0;
            speed = speedMain;
            figureInField = false;
            pixels.Clear();
            figures.Clear();
            Console.Clear();
            Random rnd = new Random();
            Console.CursorVisible = false;
            CreateField();
            CreateBarriers();
            Stopwatch timer = new Stopwatch();
            Console.SetCursorPosition(25, 12);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Press 'Esc' button to");
            Console.SetCursorPosition(25, 13);
            Console.WriteLine("exit to main menu.");
            while (true)
            {
                if (!figureInField)
                {
                    Sound(1000, 200);
                    byte v = LayerCount();
                    globalCount = globalCount + (v * v);
                    lines = lines + v;
                    figures.Add(new Figures(10, 3, rnd.Next(1, 8)));
                    if (!figures.Last().Down())
                    {
                        break;
                    }
                    figureInField = true;
                    globalCount++;
                    if (record < globalCount)
                        record = globalCount;
                }

                if (lines > 9)
                    speed = (int)(Math.Floor((double)(lines / 10)) + speedMain);
                if (speed == 10) speed = 9;
                frame = (short)Math.Round(1200 / Math.Pow(1.4, speed - 1));

                Console.ForegroundColor = ConsoleColor.White;
                Console.SetCursorPosition(28, 4);
                Console.Write("Count: " + globalCount);
                Console.SetCursorPosition(28, 6);
                Console.Write("Line: " + lines);
                Console.SetCursorPosition(28, 8);
                Console.Write($"Speed: {speed}, {frame}ms");
                Console.SetCursorPosition(28, 10);
                Console.Write("Record: " + record);

                timer.Restart();
                while (timer.ElapsedMilliseconds <= frame)
                {
                    if (!Console.KeyAvailable) continue;
                    ConsoleKey key = Console.ReadKey(true).Key;
                    if (key == ConsoleKey.DownArrow || key == ConsoleKey.S)
                    {
                        while (figures.Last().Down()) { }
                        break;
                    }
                    else if (key == ConsoleKey.RightArrow || key == ConsoleKey.D) figures.Last().Right();
                    else if (key == ConsoleKey.LeftArrow || key == ConsoleKey.A) figures.Last().Left();
                    else if (key == ConsoleKey.Spacebar) figures.Last().Rotate();
                    else if (key == ConsoleKey.Escape)
                    {
                        end = true;
                        break;
                    }
                }
                if (end) break;
                figureInField = figures.Last().Down();
            }
            EndGame();
        }
        private static void CreateField()//Create the game field
        {
            for (byte x = 1; x < xField; x++)
            {
                for (byte y = 1; y < yField; y++)
                {
                    if (BgIsZebra)
                    {
                        if (!zebra)
                            new Pixel(Size(x), y, ConsoleColor.Gray, "Background").Draw();
                        else
                            new Pixel(Size(x), y, ConsoleColor.White, "Background").Draw();
                        zebra = !zebra;
                    }
                    else new Pixel(Size(x), y, ConsoleColor.White, "Background").Draw();
                }
            }
        }
        private static void CreateBarriers()//Create field barriers
        {
            for (byte y = 0; y < yField; y++)
                new Pixel(0, y, ConsoleColor.DarkGray, "Barrier").Draw();
            for (byte y = 0; y < yField; y++)
                new Pixel(Size(xField), y, ConsoleColor.DarkGray, "Barrier").Draw();
            for (byte x = 0; x < xField + 1; x++)
                new Pixel(Size(x), 0, ConsoleColor.DarkGray, "Barrier").Draw();
            for (byte x = 0; x < xField + 1; x++)
                new Pixel(Size(x), yField, ConsoleColor.DarkGray, "Barrier").Draw();
        }
        static byte Size(byte x)
        {
            return Convert.ToByte(x * 2);
        }
        public static byte LayerCount()//Count the number of broken layers and clear them
        {
            List<Pixel> list = new List<Pixel>();
            List<int> raws = new List<int>();
            byte count = 0;
            for (int u = 1; u < 5; u++)
            {
                for (int j = 0; j <= yField; j++)
                {
                    byte countPixel = 0;
                    for (int i = 0; i < 22; i = i + 2)
                    {
                        if (field[i, j] == "Figure")
                            countPixel++;
                    }
                    if (countPixel == 10)
                    {
                        count++;
                        foreach (Pixel el in pixels)
                        {
                            if (el.Y == j)
                            {
                                el.Clear();
                                list.Add(el);
                            }
                        }
                        Sound(600, 200);
                        raws.Add(j);
                    }
                }
            }
            if (list.Any())
            {
                Thread.Sleep(200);
                foreach (Pixel el in list)
                    el.Draw();
                Sound(800, 200);
                Thread.Sleep(200);
                foreach (Pixel el in list)
                    el.Clear();
                Sound(600, 200);
                Thread.Sleep(200);
                foreach (Pixel el in list)
                    el.Draw();
                Sound(800, 200);
                Thread.Sleep(200);
                foreach (Pixel el in list)
                    el.Clear();
                Sound(600, 200);
                foreach (Pixel el in list)
                    pixels.Remove(el);
                foreach (int raw in raws)
                {
                    foreach (Pixel el in pixels)
                        el.DownLayers((byte)raw);
                }
                foreach (Pixel el in pixels)
                {
                    if (el.Type == "Figure")
                        el.Draw();
                }
            }
            return count;
        }
        public static void EndGame()//Game over screen
        {
            Sound(400, 800);
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(6, 8);
            Console.Write("              Count: " + globalCount);
            Console.SetCursorPosition(6, 9);
            Console.WriteLine("              Line: " + lines);
            Console.Write("Press 'Esc' to return to menu");
            while (true)
            {
                ConsoleKey g;
                if (Console.KeyAvailable)
                {
                    g = Console.ReadKey(true).Key;
                    if (g == ConsoleKey.Escape)
                    {
                        nowChapter = 0;
                        nowOption = 0;
                        DrawChapter();
                        break;
                    }
                }
                Thread.Sleep(80);
            }
        }
        public static void DrawChapter()//Draw menu
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(14, 1);
            if (nowChapter == 0)
                Console.Write("TETRIS");
            if (nowChapter == 1)
                Console.Write("Settings");
            if (nowChapter == 2)
                Console.Write("Instructions");
            int count = 3;
            for (int i = 0; i <= maxOption[nowChapter]; i++)
            {
                Console.SetCursorPosition(8, count);
                if (i == nowOption && nowChapter != 2)
                    Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(chapters[nowChapter, i]);
                Console.ForegroundColor = ConsoleColor.White;
                count = count + 2;
            }
        }
        private static void SetChapterText()
        {
            chapters[0, 0] = "Play";//Section and option names
            chapters[0, 1] = "Settings";
            chapters[0, 2] = "How to Play";
            chapters[1, 0] = "Colorful figures: on ";
            chapters[1, 1] = "Sound: off";
            chapters[1, 2] = "Initial speed: 1";
            chapters[1, 3] = "Background: checkered";
            chapters[1, 4] = $"Drawing symbol: {pixelSymbol} (Press Tab to change)";
            chapters[2, 0] = "Arrow keys Left-Right or keys A, D";
            chapters[2, 1] = "are used to move the figure accordingly.";
            chapters[2, 2] = "";
            chapters[2, 3] = "Arrow Down or key S to quickly";
            chapters[2, 4] = "move the figure down.";
            chapters[2, 5] = "Spacebar to rotate the figure clockwise.";
            chapters[2, 6] = "Press 'Esc' to stop the game";
            chapters[2, 7] = "and return to the main menu.";
        }
        private static void Sound(int a, int b)
        {
            if (sound) Task.Run(() => Console.Beep(a, b));
        }
    }
}
