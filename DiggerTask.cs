using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Digger
{
    class Vector2
    {
        public int X;
        public int Y;

        public Vector2(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
    //Напишите здесь классы Player, Terrain и другие.
    class Terrain : ICreature
    {
        public CreatureCommand Act(int x, int y)
        {
            return new CreatureCommand();
        }

        public bool DeadInConflict(ICreature conflictedObject)
        {
            return conflictedObject.ToString() == "Digger.Player";            
        }

        public int GetDrawingPriority()
        {
            return 1;
        }

        public string GetImageFileName()
        {
            return "Terrain.png";
        }
    }

    class Player : ICreature
    {
        public CreatureCommand Act(int x, int y)
        {
            CreatureCommand command = new CreatureCommand();
            var creature = Game.Map[x, y];
            if (creature == this)
            {
                switch (Game.KeyPressed)
                {
                    case Keys.Left:
                        {
                            command.DeltaX = (x - 1 < 0) || (Game.Map[x - 1, y]?.ToString() == "Digger.Sack") ? 0 : -1;
                            command.DeltaY = 0;
                            break;
                        }
                    case Keys.Right:
                        {
                            command.DeltaX = (x + 1 >= Game.MapWidth) || (Game.Map[x + 1, y]?.ToString() == "Digger.Sack") ? 0 : 1;
                            command.DeltaY = 0;
                            break;
                        }
                    case Keys.Up:
                        {
                            command.DeltaX = 0;
                            command.DeltaY = (y - 1 < 0) || (Game.Map[x, y - 1]?.ToString() == "Digger.Sack") ? 0 : -1;
                            break;
                        }
                    case Keys.Down:
                        {
                            command.DeltaX = 0;
                            command.DeltaY = (y + 1 >= Game.MapHeight) || (Game.Map[x, y + 1]?.ToString() == "Digger.Sack") ? 0 : 1;
                            break;
                        }
                }
            }
            return command;
        }

        public bool DeadInConflict(ICreature conflictedObject)
        {
            return conflictedObject.ToString() == "Digger.Monster" || conflictedObject.ToString() == "Digger.Sack";
        }

        public int GetDrawingPriority()
        {
            return 0;
        }

        public string GetImageFileName()
        {
            return "Digger.png";
        }
    }

    class Sack : ICreature
    {
        private int falling;
        public CreatureCommand Act(int x, int y)
        {
            var command = new CreatureCommand();
            if(Game.Map[x, y] == this)
            {
                if ((y + 1) < Game.MapHeight)
                {
                    if (Game.Map[x, y + 1] == null || (falling > 0 && Game.Map[x, y + 1].ToString() == "Digger.Player"))
                    {
                        command.DeltaX = 0;
                        command.DeltaY = 1;                        
                    }
                }
                if (command.DeltaY == 0 && falling > 1)
                {
                    command.TransformTo = new Gold();                    
                }
                falling = command.DeltaY == 0 ? 0 : falling + 1;
            }
            return command;
        }

        public bool DeadInConflict(ICreature conflictedObject)
        {
            return false;
        }

        public int GetDrawingPriority()
        {
            return 2;
        }

        public string GetImageFileName()
        {
            return "Sack.png";
        }
    }

    class Gold : ICreature
    {
        public CreatureCommand Act(int x, int y)
        {
            return new CreatureCommand();
        }

        public bool DeadInConflict(ICreature conflictedObject)
        {
            if(conflictedObject.ToString() == "Digger.Player")
            {
                Game.Scores += 10;
            }
            return conflictedObject.ToString() == "Digger.Player";
        }

        public int GetDrawingPriority()
        {
            return 3;
        }

        public string GetImageFileName()
        {
            return "Gold.png";
        }
    }

    class Monster : ICreature
    {
        Stack<CreatureCommand> pathToPlayer = new Stack<CreatureCommand>();

        public CreatureCommand Act(int x, int y)
        {
            var command = new CreatureCommand();
            bool isPlayerFound = false;
            Vector2 playerPosition = FindPlayer(isPlayerFound);

            if(isPlayerFound)
            {
                var pathCount = pathToPlayer.Count;
                
            }
            return command;
        }

        public bool DeadInConflict(ICreature conflictedObject)
        {
            return conflictedObject.ToString() == "Digger.Monster" || conflictedObject.ToString() == "Digger.Sack";
        }

        public int GetDrawingPriority()
        {
            return 4;
        }

        public string GetImageFileName()
        {
            return "Monster.png";
        }

        private Vector2 FindPlayer(bool playerIsHere)
        {
            for (int i = 0; i < Game.MapWidth; i++)
                for (int j = 0; j < Game.MapHeight; j++)
                {
                    if(Game.Map[i, j]?.ToString() == "Digger.Player")
                    {
                        playerIsHere = true;
                        return new Vector2(i, j);                        
                    }
                }
            return new Vector2(0, 0);
        }
        
        private void FindPathToPlayer(Vector2 playerPosition, Vector2 monsterPosition, List<CreatureCommand> commands, List<Vector2> path = null)
        {
            if(playerPosition == monsterPosition)
            {

            }
            var command = new CreatureCommand();
            command.DeltaY = (playerPosition.Y > monsterPosition.Y) ? 1 : (playerPosition.Y < monsterPosition.Y) ? -1 : 0;
            FindPathToPlayer(playerPosition, new Vector2(monsterPosition.X + command.DeltaX, monsterPosition.Y), commands);
            command.DeltaX = (playerPosition.X > monsterPosition.X) ? 1 : (playerPosition.X < monsterPosition.X) ? -1 : 0;
            FindPathToPlayer(playerPosition, new Vector2(monsterPosition.X + command.DeltaX, monsterPosition.Y), commands);
            
        }
    }
}