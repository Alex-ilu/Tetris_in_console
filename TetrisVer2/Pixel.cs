using System;
namespace Tetris
{
    internal class Pixel
    {
        private readonly string charPixel = Program.pixelSymbol;
        private ConsoleColor color { get; }
        private byte x;
        private byte y;
        private string type;//Pixel type
        public Pixel(byte x, byte y, ConsoleColor color, string type)
        {
            this.color = color;
            this.x = x;
            this.y = y;
            this.type = type;
        }
        public Pixel(byte x, byte y, ConsoleColor color)
        {
            this.color = color;
            this.x = x;
            this.y = y;
        }
        public string Type
        {
            get { return type; }
            set { this.type = value; }
        }
        public byte X
        {
            get { return x; }
            set { this.x = value; }
        }
        public byte Y
        {
            get { return y; }
            set { this.y = value; }
        }
        public void Draw()//Draw pixel
        {
            Console.ForegroundColor = color;
            Console.SetCursorPosition(x, y);
            Console.Write(charPixel);
            Program.field[x, y] = type;
        }
        public void Clear()//Clear pixel
        {
            Console.SetCursorPosition(x, y);
            if (Program.BgIsZebra)
            {
                double xx = (x / 2);
                if ((Math.Round(xx) + y) % 2 == 0)
                    new Pixel(x, y, ConsoleColor.Gray, "Background").Draw();
                else
                    new Pixel(x, y, ConsoleColor.White, "Background").Draw();
            }
            else new Pixel(x, y, ConsoleColor.White, "Background").Draw();
        }
        public void DownLayers(byte y)//Move pixels down
        {
            if (Type != "Figure") return;
            if (this.y < y)
            {
                Clear();
                this.y++;
            }
        }
    }
}
