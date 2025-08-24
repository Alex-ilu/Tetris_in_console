using System;
using System.Collections.Generic;

namespace Tetris
{
    internal class Figures
    {
        private static Pixel[,] Stages = new Pixel[4, 4];//Array containing the position of the figure's pixels relative to its center coordinates
        private byte x;//Coordinates of the figure's center
        private byte y;
        private int type;//Parameter defining the type of figure
        private static ConsoleColor color = ConsoleColor.Black;
        private static byte nowSt = 1;//Current rotation variation
        private static byte stagesAmount = 2;//Number of rotation variations for this figure

        public Figures(byte x, byte y, int type)
        {
            this.x = x;
            this.y = y;
            this.type = type;
            color = ConsoleColor.Black;
            if (type == 1 && Program.colorfulFigures)//Set the color of each figure according to settings
                color = ConsoleColor.Cyan;
            else if (type == 5 && Program.colorfulFigures)
                color = ConsoleColor.Green;
            else if (type == 7 && Program.colorfulFigures)
                color = ConsoleColor.Red;
            else if (type == 2 && Program.colorfulFigures)
                color = ConsoleColor.Blue;
            else if (type == 3 && Program.colorfulFigures)
                color = ConsoleColor.DarkYellow;
            else if (type == 6 && Program.colorfulFigures)
                color = ConsoleColor.Magenta;
            else if (type == 4 && Program.colorfulFigures)
                color = ConsoleColor.Yellow;

            if (type == 1 || type == 5 || type == 7)//Set the stages of this figure
            {
                stagesAmount = 2;
                ReDrawStage(0);
                ReDrawStage(1);
            }
            else if (type == 2 || type == 3 || type == 6)
            {
                stagesAmount = 4;
                ReDrawStage(0);
                ReDrawStage(1);
                ReDrawStage(2);
                ReDrawStage(3);
            }
            else if (type == 4)
            {
                stagesAmount = 1;
                ReDrawStage(0);
            }

            nowSt = 0;
            for (int i = 0; i < 4; i++)//Add all figure pixels to the pixel array
            {
                Stages[nowSt, i].Draw();
                Program.pixels.Add(Stages[nowSt, i]);
            }
        }
        public bool Down()//Move the figure down
        {
            List<Pixel> list = new List<Pixel>();

            for (int i = 0; i < 4; i++)
                list.Add(Stages[nowSt, i]);
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (Stages[nowSt, i].X == Stages[nowSt, j].X && Stages[nowSt, i].Y + 1 == Stages[nowSt, j].Y)
                        list.Remove(Stages[nowSt, i]);
                }
            }
            bool ClearIsDown = true;
            foreach (Pixel el in list)//Check if space is free in the direction
            {
                if (Program.field[el.X, el.Y + 1] != "Background")
                    ClearIsDown = false;
            }
            if (ClearIsDown)
            {
                for (int i = 0; i < 4; i++)
                {
                    Stages[nowSt, i].Clear();
                    for (int r = 0; r < stagesAmount; r++)
                    {
                        Stages[r, i].Y = (byte)(Stages[nowSt, i].Y + 1);
                    }
                    Program.pixels.Remove(Stages[nowSt, i]);
                }
                for (int i = 0; i < 4; i++)
                {
                    Stages[nowSt, i].Draw();
                    if (Program.pixels.Contains(Stages[nowSt, i]) == false)
                        Program.pixels.Add(Stages[nowSt, i]);
                }
                y++;
            }
            return ClearIsDown;
        }
        public void Right()//Move the figure right
        {
            List<Pixel> list = new List<Pixel>();

            for (int i = 0; i < 4; i++)
                list.Add(Stages[nowSt, i]);

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (Stages[nowSt, i].Y == Stages[nowSt, j].Y && Stages[nowSt, i].X + 2 == Stages[nowSt, j].X)
                        list.Remove(Stages[nowSt, i]);

                }
            }

            bool ClearIsRight = true;
            foreach (Pixel el in list)
            {
                if (Program.field[el.X + 2, el.Y] != "Background")
                    ClearIsRight = false;
            }
            if (ClearIsRight)
            {
                for (int i = 0; i < 4; i++)
                {
                    Stages[nowSt, i].Clear();
                    for (int r = 0; r < stagesAmount; r++)
                        Stages[r, i].X = (byte)(Stages[nowSt, i].X + 2);
                    Program.pixels.Remove(Stages[nowSt, i]);
                }
                for (int i = 0; i < 4; i++)
                {
                    Stages[nowSt, i].Draw();
                    if (Program.pixels.Contains(Stages[nowSt, i]) == false)
                        Program.pixels.Add(Stages[nowSt, i]);
                }
                x = (byte)(x + 2);
            }
        }
        public void Left()//Move the figure left
        {
            List<Pixel> list = new List<Pixel>();
            for (int i = 0; i < 4; i++)
                list.Add(Stages[nowSt, i]);
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (Stages[nowSt, i].Y == Stages[nowSt, j].Y && Stages[nowSt, i].X - 2 == Stages[nowSt, j].X)
                        list.Remove(Stages[nowSt, i]);
                }
            }
            bool ClearIsRight = true;
            foreach (Pixel el in list)
            {
                if (Program.field[el.X - 2, el.Y] != "Background") ClearIsRight = false;
            }
            if (ClearIsRight)
            {
                for (int i = 0; i < 4; i++)
                {
                    Stages[nowSt, i].Clear();
                    for (int r = 0; r < stagesAmount; r++)
                    {
                        Stages[r, i].X = (byte)(Stages[nowSt, i].X - 2);
                    }
                    Program.pixels.Remove(Stages[nowSt, i]);
                }
                for (int i = 0; i < 4; i++)
                {
                    Stages[nowSt, i].Draw();
                    if (Program.pixels.Contains(Stages[nowSt, i]) == false)
                        Program.pixels.Add(Stages[nowSt, i]);
                }
                x = (byte)(x - 2);
            }
        }
        public void Rotate()//Rotate the figure
        {
            if (type == 4) return;
            byte oldSt = nowSt;
            byte newSt = nowSt;
            byte lastX = x;
            bool ClearIsRotate = true;
            List<Pixel> list = new List<Pixel>();

            newSt++;
            if (newSt == stagesAmount) newSt = 0;
            ReDrawStage(newSt);

            int shift = 0;
            for (int i = 0; i < 4; i++)//Shift the figure if it goes out of bounds when rotating
            {
                if (Stages[newSt, i].X == 0) shift = 2;
                else if (Stages[newSt, i].X == 24) shift = -4;
                else if (Stages[newSt, i].X == 22) shift = -2;
            }
            x = (byte)(x + shift);
            ReDrawStage(newSt);

            for (int i = 0; i < 4; i++)
                list.Add(Stages[newSt, i]);

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (Stages[newSt, i].X == Stages[oldSt, j].X && Stages[newSt, i].Y == Stages[oldSt, j].Y)
                        list.Remove(Stages[newSt, i]);
                }
            }
            foreach (Pixel el in list)
            {
                if (Program.field[el.X, el.Y] != "Background")
                {
                    x = lastX;
                    ClearIsRotate = false; break;
                }
            }
            if (ClearIsRotate)
            {
                for (int i = 0; i < 4; i++)
                {
                    Stages[oldSt, i].Clear();
                    Program.pixels.Remove(Stages[oldSt, i]);
                }
                for (int i = 0; i < 4; i++)
                {
                    Stages[newSt, i].Draw();
                    if (Program.pixels.Contains(Stages[newSt, i]) == false)
                        Program.pixels.Add(Stages[newSt, i]);
                }
                nowSt = newSt;
            }
        }
        public void ReDrawStage(byte stage)//Set the shape of each figure
        {
            switch (type)
            {
                case 1:
                    if (stage == 0) SetStages(stage, new int[] { -1, 0, 1, 2 }, new int[] { -1, -1, -1, -1 });
                    else if (stage == 1) SetStages(stage, new int[] { 0, 0, 0, 0 }, new int[] { -1, 0, 1, 2 });
                    break;
                case 2:
                    if (stage == 0) SetStages(stage, new int[] { -1, -1, 0, 1 }, new int[] { -1, 0, 0, 0 });
                    else if (stage == 1) SetStages(stage, new int[] { 1, 0, 0, 0 }, new int[] { -1, -1, 0, 1 });
                    else if (stage == 2) SetStages(stage, new int[] { 1, 1, 0, -1 }, new int[] { 1, 0, 0, 0 });
                    else if (stage == 3) SetStages(stage, new int[] { -1, 0, 0, 0 }, new int[] { 1, 1, 0, -1 });
                    break;
                case 3:
                    if (stage == 0) SetStages(stage, new int[] { 1, 1, 0, -1 }, new int[] { -1, 0, 0, 0 });
                    else if (stage == 1) SetStages(stage, new int[] { 0, 0, 0, 1 }, new int[] { -1, 0, 1, 1 });
                    else if (stage == 2) SetStages(stage, new int[] { -1, -1, 0, 1 }, new int[] { 1, 0, 0, 0 });
                    else if (stage == 3) SetStages(stage, new int[] { -1, 0, 0, 0 }, new int[] { -1, -1, 0, 1 });
                    break;
                case 4:
                    SetStages(stage, new int[] { 0, 1, 1, 0 }, new int[] { -1, -1, 0, 0 });
                    break;
                case 5:
                    if (stage == 0) SetStages(stage, new int[] { -1, 0, 0, 1 }, new int[] { 0, 0, -1, -1 });
                    else if (stage == 1) SetStages(stage, new int[] { -1, -1, 0, 0 }, new int[] { -1, 0, 0, 1 });
                    break;
                case 6:
                    if (stage == 0) SetStages(stage, new int[] { 0, -1, 0, 1 }, new int[] { -1, 0, 0, 0 });
                    else if (stage == 1) SetStages(stage, new int[] { 1, 0, 0, 0 }, new int[] { 0, -1, 0, 1 });
                    else if (stage == 2) SetStages(stage, new int[] { 0, 1, 0, -1 }, new int[] { 1, 0, 0, 0 });
                    else if (stage == 3) SetStages(stage, new int[] { -1, 0, 0, 0 }, new int[] { 0, 1, 0, -1 });
                    break;
                case 7:
                    if (stage == 0) SetStages(stage, new int[] { -1, 0, 0, 1 }, new int[] { -1, -1, 0, 0 });
                    else if (stage == 1) SetStages(stage, new int[] { 0, 0, -1, -1 }, new int[] { -1, 0, 0, 1 });
                    break;
            }
        }
        private void SetStages(byte stageNumber, int[] xShift, int[] yShift)
        {
            for (int i = 0; i < 4; i++)
                Stages[stageNumber, i] = new Pixel((byte)(x + xShift[i] * 2), (byte)(y + yShift[i]), color, "Figure");
        }
    }
}
