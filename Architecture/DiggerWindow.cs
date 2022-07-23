using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Digger
{
    public class DiggerWindow : Form
    {
        private readonly Dictionary<string, Bitmap> bitmaps = new Dictionary<string, Bitmap>();
        private readonly GameState gameState;
        private readonly HashSet<Keys> pressedKeys = new HashSet<Keys>();
        private int tickCount;

        // Отсюда начинается игра
        public DiggerWindow(DirectoryInfo imagesDirectory = null)
        {
            // Создает экземпляр класса GameState
            gameState = new GameState();
            
            // Создает размер формы            
            ClientSize = new Size(
                GameState.ElementSize * Game.MapWidth,
                GameState.ElementSize * Game.MapHeight + GameState.ElementSize);
            
            // Создает стиль границ формы - толстая граница
            FormBorderStyle = FormBorderStyle.FixedDialog;

            // Устанавливает ссылку на папку с картинками
            // Если она не указана, то по умолчанию находится в корне проекта, в папке Images
            if (imagesDirectory == null)
                imagesDirectory = new DirectoryInfo("Images");

            // Берет все картинки из папки и через цикл переводит в битмапы каждую из них
            foreach (var e in imagesDirectory.GetFiles("*.png"))
                bitmaps[e.Name] = (Bitmap) Image.FromFile(e.FullName);
            
            // Дальше что-то происходит по таймингу
            var timer = new Timer();
            timer.Interval = 15;
            timer.Tick += TimerTick;
            timer.Start();
        }


        // При загрузке формы вызывает загрузчик формы и использует двойную буферизацию для отрисовки формы
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Text = "Digger";
            DoubleBuffered = true;
        }

        // Запоминает клавишу при нажатии
        protected override void OnKeyDown(KeyEventArgs e)
        {
            pressedKeys.Add(e.KeyCode);
            Game.KeyPressed = e.KeyCode;
        }

        // Забывает нажатую клавишу, при отпускании клавиши
        protected override void OnKeyUp(KeyEventArgs e)
        {
            pressedKeys.Remove(e.KeyCode);
            Game.KeyPressed = pressedKeys.Any() ? pressedKeys.Min() : Keys.None;
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.TranslateTransform(0, GameState.ElementSize);
            e.Graphics.FillRectangle(
                Brushes.Black, 0, 0, GameState.ElementSize * Game.MapWidth,
                GameState.ElementSize * Game.MapHeight);
            foreach (var a in gameState.Animations)
                e.Graphics.DrawImage(bitmaps[a.Creature.GetImageFileName()], a.Location);
            e.Graphics.ResetTransform();
            e.Graphics.DrawString(Game.Scores.ToString(), new Font("Arial", 16), Brushes.Green, 0, 0);
        }

        private void TimerTick(object sender, EventArgs args)
        {
            if (tickCount == 0) gameState.BeginAct();
            foreach (var e in gameState.Animations)
                e.Location = new Point(e.Location.X + 4 * e.Command.DeltaX, e.Location.Y + 4 * e.Command.DeltaY);
            if (tickCount == 7)
                gameState.EndAct();
            tickCount++;
            if (tickCount == 8) tickCount = 0;
            Invalidate();
        }
    }
}