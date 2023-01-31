bool keepPlaying = true;
bool quickStart = false;

while (keepPlaying)
{
    FountainOfYouth game = new(quickStart: quickStart);
    (keepPlaying, quickStart) = game.Run();
}

Environment.Exit(0);


public class FountainOfYouth
{
    private string _moveSeparator = "-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-";

    private MapTile[,] _map;
    private int _smallMap = 4;
    private int _mediumMap = 6;
    private int _largeMap = 8;
    private int _mapSize;
    bool _debugging = false;
    bool _shotYourself = false;

    private (int Row, int Col) _playerPosition, _startPosition;
    private int _bulletsRemaining;
    private int _turnsRemaining;
    private int _prayersRemaining;

    // implications of difficulty are set in SetTileDistByDifficulty() and SetResourcesByDifficulty()
    private enum Difficulty { Easy, Normal, Hard, Null}
    private Difficulty _difficulty;
    readonly Random _rand = new Random();  // used to generate tile distributions

    private enum Action { Move, Shoot, Pray, Null }
    private enum Direction { North, South, East, West, Null }
    

    public FountainOfYouth(bool quickStart = false)
    {
        Console.Title = "Fountain of Youth";
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.BackgroundColor = ConsoleColor.Black;

        if (_debugging || quickStart)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n[NEW GAME CREATED]");
            Console.ForegroundColor = ConsoleColor.Gray;
            AskForDifficulty();
            AskForMapSize();
            SetResourcesByMapSize();
            _map = new MapTile[_mapSize, _mapSize];
            PopulateMap();
            Console.Clear();

            PrintPlayer();
            Console.WriteLine("Armed with a musket and a satchel of smoked oysters, you enter the unknown.");

        }
        else
        {
            Console.WriteLine("Welcome to the Fountain of Youth game!");
            Console.WriteLine("It is a text-based adventure game where you navigate a grid world in search of the Fountain of Youth.");
            Console.WriteLine("\nBefore we begin, here are the controls:");
            PrintHelpText();

            Console.Write("Press ENTER to advance: ");
            Console.ReadKey(true);
            Console.WriteLine();

            AskForDifficulty();
            // instantiate _map and populate with MapTile objects
            AskForMapSize();
            SetResourcesByMapSize();
            _map = new MapTile[_mapSize, _mapSize];

            Console.CursorVisible = false;

            Console.Write("\nThank you! Let the journey commence. ");

            Thread.Sleep(2500);

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n\tSCALLYWAG PRESENTS...");
            Thread.Sleep(1500);
            Console.WriteLine("\tTHE FOUNTAIN OF YOUTH");
            Thread.Sleep(2000);
            Console.ForegroundColor = ConsoleColor.Gray;

            Console.WriteLine("\nYou have travelled far to find this dungeon, which legend says holds the Fountain of Youth!");
            Console.WriteLine("The entrance is but a humble hole in the ground.");

            Thread.Sleep(4000);

            Console.CursorVisible = true;
            Console.Write("\nYou fasten your rope to a nearby tree.\nDo you dare drop it into the abyss below? [ENTER] ");
            Console.ReadKey(true);
            Console.WriteLine();

            PopulateMap();
            Console.Write("There's no going back now. Take the plunge... [ENTER] ");
            Console.ReadKey(true);
            Console.WriteLine();

            PrintPlayer();

            Console.CursorVisible = false;
            Console.WriteLine("Armed with a musket and a satchel of smoked oysters, you enter the unknown.");

            Thread.Sleep(4000);
            Console.CursorVisible = true;
        }
    }


    private void PrintHelpText()
    {
        Console.WriteLine("\n\t* Choose an action ('move', 'shoot', 'pray').");
        Console.WriteLine("\t\t- You can only shoot if you have musket balls.");
        Console.WriteLine("\t\t- You can only move if you have oysters (you are diabetic).");
        Console.WriteLine("\t* If you move or shoot, also type a direction ('north', 'south', 'east', 'west').");
        Console.WriteLine("\t\t- Ex. 'move east', 'shoot north'");
        Console.WriteLine("\t* Prayers ask the gods for guidance back to the start.");
        Console.WriteLine("\t* Typing 'help' returns to this screen.\n");
    }


    public (bool playAgain, bool quickStart) Run() 
    {
        bool victory = false;
        bool gameOver = false;
        while (!gameOver) (gameOver, victory) = Update();

        Console.ForegroundColor = ConsoleColor.Yellow;
        if (victory == true) { Console.WriteLine("On to the next adventure..."); PrintWin(); }
        if (victory == false) { Console.WriteLine("In your quest for eternal youth, you found eternal death."); PrintDeath(); }

        Console.ForegroundColor = ConsoleColor.Gray;

        while (true)
        {
            Console.Write("Type 'r' restart, 'q' to quickstart (no intro), or 'e' to exit: ");
            string? input = Console.ReadLine();
            switch (input)
            {
                case "r":
                    Console.Clear();
                    return (true, false);
                case "q":
                    Console.Clear();
                    return (true, true);
                case "e":
                    Console.Clear();
                    return (false, false);
                default:
                    break;
            }
        }
    }


    private void PrintDeath()
    {
        Console.ForegroundColor = ConsoleColor.Red;

        string l0, l1, l2, l3, l4;

        l0 = "\t\t#uDedFool";
        l1 = "\t\t __/V\\__ ";
        l2 = "\t\t* (;_;)*";
        l3 = "\t\t   / \\* ";
        l4 = "\t\t * ~~~  ";

        Console.WriteLine();  // buffer
        Console.WriteLine(l0);
        Console.WriteLine(l1);
        Console.WriteLine(l2);
        Console.WriteLine(l3);
        Console.WriteLine(l4);
        Console.WriteLine();  // buffer

        Console.ForegroundColor = ConsoleColor.Gray;  // reset
    }


    private void PrintWin()
    {
        Console.ForegroundColor = ConsoleColor.Yellow;

        string l0, l1, l2, l3, l4;

        l0 = "\t\t#4evrYung ";
        l1 = "\t\t __/V\\__ ";
        l2 = "\t\t  (^.^) ";
        l3 = "\t\t  b|_|b ";
        l4 = "\t\t   b b  ";

        Console.WriteLine();  // buffer
        Console.WriteLine(l0);
        Console.WriteLine(l1);
        Console.WriteLine(l2);
        Console.WriteLine(l3);
        Console.WriteLine(l4);
        Console.WriteLine();  // buffer

        Console.ForegroundColor = ConsoleColor.Gray;  // reset
    }


    private void PrintPlayer()
    {
        Console.ForegroundColor = ConsoleColor.Gray;

        string l0, l1, l2, l3, l4;

        l0 = "\t\t#Rdy2Rumbl ";
        l1 = "\t\t __/V\\__ ";
        l2 = "\t\t  (o_6) ";
        l3 = "\t\t  b|/|b";
        l4 = "\t\t   b b  ";

        Console.WriteLine();  // buffer
        Console.WriteLine(l0);
        Console.WriteLine(l1);
        Console.WriteLine(l2);
        Console.WriteLine(l3);
        Console.WriteLine(l4);
        Console.WriteLine();  // buffer

        Console.ForegroundColor = ConsoleColor.Gray;  // reset
    }


    private (bool GameOver, bool DidYouWin) Update()
    {
        Console.WriteLine();
        Console.WriteLine(_moveSeparator);

        if (_turnsRemaining == 0)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\nYou have run out of oysters and subsequently your will to live.");
            Console.ForegroundColor = ConsoleColor.Gray;
            return (true, false);
        }

        MapTile currentTile = _map[_playerPosition.Row, _playerPosition.Col];
        currentTile.View();             // see current room picture
        currentTile.PrintRoomText();    // get current room spiel

        // room interaction logic
        (bool gameOver, bool didYouWin, bool teleportCheck, string gameOverMessage) = InteractWithRoom(currentTile);
        if (teleportCheck)
        {
            Console.CursorVisible = false;
            Thread.Sleep(2500);
            Console.CursorVisible = true;
            return (false, false);  // reset in a different room
        }

        if (gameOver)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(gameOverMessage);
            Console.ForegroundColor = ConsoleColor.Gray;
            return (true, didYouWin);
        }

        // sense adjacent rooms
        Console.WriteLine("\n~~ You focus your senses in the darkness ~~");
        Console.ForegroundColor = ConsoleColor.DarkGray;
        ReportAdjacentTiles();  // sense adjacent rooms in a random order
        Console.ForegroundColor = ConsoleColor.Gray;

        // map and position if debugging turned on
        if (_debugging)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Debugging:");
            PrintMap();
            Console.WriteLine(_playerPosition);
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        // what to do next
        Console.WriteLine($"You have {_turnsRemaining} smoked oysters and {_bulletsRemaining} musket balls.");
        TakeAction();

        if (_shotYourself) return (true, false);

        _turnsRemaining--;
        return (false, false);
    }


    private void ReportAdjacentTiles()
    {
        // reports adjacent tiles in a pseudo-random order, cowboy style
        // shorten for ease of reference
        int row = _playerPosition.Row;
        int col = _playerPosition.Col;

        // drawing a random index
        bool[] printed = new bool[] { false, false, false, false };
        int rInt;

        // while something has yet to print
        while (printed.Contains(false))
        {
            // get a random index
            rInt = _rand.Next(0, printed.Length);

            // if that index hasn't been accessed yet...
            if (printed[rInt] == false)
            {
                // ...print it, based on this arbitrary but complete index/direction pairing
                switch (rInt)
                {
                    case 0:
                        if (col != 0) _map[row, col - 1].PrintAdjacentText();  // west
                        printed[rInt] = true;
                        break;
                    case 1:
                        if (col != _mapSize - 1) _map[row, col + 1].PrintAdjacentText();  // east
                        printed[rInt] = true;
                        break;
                    case 2:
                        if (row != _mapSize - 1) _map[row + 1, col].PrintAdjacentText();  // south
                        printed[rInt] = true;
                        break;
                    case 3:
                        if (row != 0) _map[row - 1, col].PrintAdjacentText();  // north
                        printed[rInt] = true;
                        break;
                    default:  // shouldn't ever trigger, but if it does, reset
                        break;
                }
            }

        }

        Console.WriteLine("You have learned all you will. It is time to act!");
        Console.WriteLine();
    }


    private (Action, Direction) AskForAction() 
    {
        string input, actionCommand, directionCommand;
        string[] commands;
        Action action;
        Direction direction;

        while (true)
        {
            Console.Write("What action would you like to take? ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            input = Console.ReadLine() ?? "reset";
            Console.ForegroundColor = ConsoleColor.Gray;
            input = input.ToLower();

            if (input.Contains("help"))
            {
                PrintHelpText();
                continue;
            }

            commands = input.Split(' ');  // 'move east' becomes [move, east]

            actionCommand = commands[0];
            action = actionCommand switch 
            {
                "move" => Action.Move,
                "shoot" => Action.Shoot,
                "pray" => Action.Pray,
                _ => Action.Null 
            };

            // restart loop if invalid entry occurs
            if (action == Action.Null) continue;

            // which direction to take action toward?
            if (action == Action.Move || action == Action.Shoot)
            {
                // check to make sure a second index exists
                if (commands.Length == 1) continue;

                directionCommand = commands[1];
                direction = directionCommand switch
                {
                    "north" => Direction.North,
                    "south" => Direction.South,
                    "east" => Direction.East,
                    "west" => Direction.West,
                    _ => Direction.Null
                };

                if (direction == Direction.Null) continue;
            }
            // actions that don't require a direction
            else direction = Direction.Null;

            break;
        }

        return (action, direction);
    }


    private void TakeAction()
    {
        Action action;
        Direction direction;
        bool success;

        while (true)
        {
            (action, direction) = AskForAction();
            // only moving returns true and moves on to the next turn
            success = action switch { Action.Move => Move(direction), Action.Shoot => Shoot(direction), Action.Pray => Pray(), _ => false };
            if (success == true) break;
        }
    }


    private bool Move(Direction direction)
    {
        int col = _playerPosition.Col;
        int row = _playerPosition.Row;
        string wallBump = "\n\tYou walk straight into the wall. There's nothing that way!\n";

        switch (direction)
        {
            case Direction.North:
                if (row != 0) { _playerPosition.Row = row - 1; return true; }
                else { Console.ForegroundColor = ConsoleColor.DarkGray; Console.WriteLine(wallBump); Console.ForegroundColor = ConsoleColor.Gray; return false; }
            case Direction.South:
                if (row != _mapSize - 1) { _playerPosition.Row = row + 1; return true; }
                else { Console.ForegroundColor = ConsoleColor.DarkGray; Console.WriteLine(wallBump); Console.ForegroundColor = ConsoleColor.Gray; return false; }
            case Direction.East:
                if (col != _mapSize - 1) { _playerPosition.Col = col + 1; return true; }
                else { Console.ForegroundColor = ConsoleColor.DarkGray; Console.WriteLine(wallBump); Console.ForegroundColor = ConsoleColor.Gray; return false; }
            case Direction.West:
                if (col != 0) { _playerPosition.Col = col - 1; return true; }
                else { Console.ForegroundColor = ConsoleColor.DarkGray; Console.WriteLine(wallBump); Console.ForegroundColor = ConsoleColor.Gray; return false; }
            default:
                return false;
        }
    }
    
    
    private bool Shoot(Direction direction) 
    {
        int col = _playerPosition.Col;
        int row = _playerPosition.Row;
        bool endTurn = false;

        bool isSpecial = false;
        (int, int) specialCoords = (-1, -1);

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine();

        if (_bulletsRemaining > 0)
        {
            switch (direction)
            {
                case Direction.North:
                    if (row != 0) 
                    {
                        Console.Write("\t");
                        isSpecial = _map[row-1, col].Shoot();
                        if (isSpecial) specialCoords = (row - 1, col);
                        _bulletsRemaining--;
                        break; 
                    }
                    else { Console.WriteLine("\tBang! You just shot at a wall."); _bulletsRemaining--; break; }
                
                case Direction.South:
                    if (row != _mapSize - 1) 
                    {
                        Console.Write("\t");
                        isSpecial = _map[row+1, col].Shoot();
                        if (isSpecial) specialCoords = (row + 1, col);
                        _bulletsRemaining--;
                        break;
                    }
                    else { Console.WriteLine("\tBang! You just shot at a wall."); _bulletsRemaining--; break; }
                
                case Direction.East:
                    if (col != _mapSize - 1) 
                    {
                        Console.Write("\t");
                        isSpecial = _map[row, col+1].Shoot();
                        if (isSpecial) specialCoords = (row, col + 1);
                        _bulletsRemaining--;
                        break; 
                    }
                    else { Console.WriteLine("\tBang! You just shot at a wall."); _bulletsRemaining--; break; }
                
                case Direction.West:
                    if (col != 0) 
                    { 
                        Console.Write("\t");
                        isSpecial = _map[row, col-1].Shoot();
                        if (isSpecial) specialCoords = (row, col - 1);
                        _bulletsRemaining--; 
                        break; 
                    }
                    else { Console.WriteLine("\tBang! You just shot at a wall."); _bulletsRemaining--; break; }
                
                default:
                    break;
            }
        }
        else Console.WriteLine("\tYou aim your empty musket and say \"blammo!\"");

        if (isSpecial) endTurn = HandleSpecialShot(specialCoords);
        if (!endTurn) Console.WriteLine($"\tYou have {_bulletsRemaining} musket balls remaining.");

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Gray;
        return endTurn;
    }
    
    
    private bool Pray() 
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine();
        // always returns false
        if (_prayersRemaining > 0)
        {
            int rowDiff, colDiff;
            rowDiff = _playerPosition.Row - _startPosition.Row;
            colDiff = _playerPosition.Col - _startPosition.Col;

            if (rowDiff > 0 && colDiff > 0) Console.WriteLine("\tAn idea pops into your head! Freedom is vaguely northwest.");
            else if (rowDiff < 0 && colDiff > 0) Console.WriteLine("\tAn idea pops into your head! Freedom is vaguely southwest.");
            else if (rowDiff == 0 && colDiff > 0) Console.WriteLine("\tAn idea pops into your head! Freedom is a straight shot west.");

            else if (rowDiff > 0 && colDiff == 0) Console.WriteLine("\tAn idea pops into your head! Freedom is a straight shot north.");
            else if (rowDiff < 0 && colDiff == 0) Console.WriteLine("\tAn idea pops into your head! Freedom is a straight shot south.");
            else if (rowDiff == 0 && colDiff == 0)
            {
                Console.WriteLine("\tFriends don't let friends pray at the cave entrance.");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine();
                return false;  // exit without losing a prayer
            }

            else if (rowDiff > 0 && colDiff < 0) Console.WriteLine("\tAn idea pops into your head! Freedom is vaguely northeast.");
            else if (rowDiff < 0 && colDiff < 0) Console.WriteLine("\tAn idea pops into your head! Freedom is vaguely southeast.");
            else if (rowDiff == 0 && colDiff < 0) Console.WriteLine("\tAn idea pops into your head! Freedom is a straight shot east.");
            
            _prayersRemaining--;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine();
            return false;
        }
        Console.WriteLine("\tIt appears you have been forsaken. Oof.");
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine();
        return false;
    }


    private (bool GameOver, bool DidYouWin, bool teleportCheck, string GameOverMessage) InteractWithRoom(MapTile currentTile)
    {
        switch (currentTile)
        {
            case OgreTile:
                if (currentTile.IsActive == true) return OgreEncounter(currentTile);
                break;
            
            case GoblinTile:
                if (currentTile.IsActive == true) return GoblinEncounter(currentTile);
                break;
            
            case WarpTile:
                return Teleport();
            
            case PitTile:
                return (true, false, false, "\nYou were impaled on a spike!");
            
            case StartTile:
                if (currentTile.IsActive == true)
                {
                    Console.Write("At last! Shall you embrace your destiny and face the world? [ENTER] ");
                    Console.ReadKey(true);
                    Console.WriteLine("\n");

                    return (true, true, false, "You shimmy up the rope and are greeted by birdsong.");
                }
                break;

            case FountainTile:
                if (currentTile.IsActive == false)
                {
                    Console.Write("You strip to your undies. Shall you achieve eternal youth? [ENTER] ");
                    Console.ReadKey(true);
                    Console.WriteLine("\n");

                    Console.ForegroundColor = ConsoleColor.Cyan;
                    currentTile.ToggleActive();  // bathe in fountain of youth
                    Console.ForegroundColor = ConsoleColor.Gray;
                    _map[_startPosition.Row, _startPosition.Col].ToggleActive();  // unlock start
                    Console.CursorVisible = false;
                    Thread.Sleep(3000);
                    Console.CursorVisible = true;
                }
                break;

            default:
                break;
        }

        // returns unless overridden above
        return (false, false, false, "Uh Oh! The game shouldn't be over!");
    }


    private (bool GameOver, bool DidYouWin, bool teleportCheck, string GameOverMessage) GoblinEncounter(MapTile currentTile) 
    {
        double bulletMean = 0;
        double bulletStd = 0;
        double turnMean = 0;
        double turnStd = 0;

        switch (_difficulty)
        {
            case Difficulty.Easy:
                bulletMean = 2;
                bulletStd = 1;  // 15%ish of losing 3+

                turnMean = 5;
                turnStd = 1.5;  // 25%ish of losing 6+
                break;

            case Difficulty.Normal:
                bulletMean = 3;
                bulletStd = 1.5;  // 25%ish of losing 4+

                turnMean = 8;
                turnStd = 3;  // 25%ish of losing 10+
                break;

            case Difficulty.Hard:
                bulletMean = 4;
                bulletStd = 3;  // 25%ish of losing 6+

                turnMean = 10;
                turnStd = 6;  // 25%ish of losing 14+
                break;
        }

        _bulletsRemaining -= Math.Clamp(DrawFromNormalDist(bulletMean, bulletStd), 0, _bulletsRemaining);
        _turnsRemaining -= Math.Clamp(DrawFromNormalDist(turnMean, turnStd), 0, _turnsRemaining - 1);

        currentTile.IsActive = false;

        Console.CursorVisible = false;
        Thread.Sleep(2500);
        Console.CursorVisible = true;

        return (false, false, false, "goblin");
    }


    private (bool GameOver, bool DidYouWin, bool teleportCheck, string GameOverMessage) OgreEncounter(MapTile currentTile) 
    {
        int choice;
        Console.WriteLine("\nIt gears up to charge, giving you a split second to react.");
            
        while (true)
        {
            Console.Write("Do you try to shoot [0], or throw some oysters and run [1]? ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            try { choice = Convert.ToInt32(Console.ReadLine()); }
            catch (FormatException) { choice = 2; }
            Console.ForegroundColor = ConsoleColor.Gray;
            break;
        }

        switch (choice)
        {
            // try to shoot it
            case 0:
                if(_bulletsRemaining > 0)
                {
                    int hitChance = 10;
                    switch (_difficulty)
                    {
                        case Difficulty.Easy:
                            hitChance = 8;
                            break;

                        case Difficulty.Normal:
                            hitChance = 6;
                            break;

                        case Difficulty.Hard:
                            hitChance = 4;
                            break;
                    }

                    if (_rand.Next(1,11) > hitChance) return (true, false, false, "\nYour shot just barely misses, and the ogre devours you!");
                    else
                    {
                        currentTile.IsActive = false;
                        _bulletsRemaining--;
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\n\tPraise! The ogre crashes to the ground with a musket ball between its eyes.");
                        Console.ForegroundColor = ConsoleColor.Gray;
                        return (false, false, false, "shouldn't be over");
                    }
                }
                else return (true, false, false, "\nClick! You are out of musket balls, and the ogre devours you!");
            
            // gamble with oysters
            case 1:
                double oysterMean = 0;
                double oysterStd = 0;

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\n\tYou think back to your monsters unit in Magical Zoology 101.");

                switch (_difficulty)
                {
                    case Difficulty.Easy:
                        oysterMean = 8;
                        oysterStd = 4;
                        Console.WriteLine("\tAn ogre this size wants between 2 and 14 oysters, 90% of the time.");
                        break;

                    case Difficulty.Normal:
                        oysterMean = 12;
                        oysterStd = 4;
                        Console.WriteLine("\tAn ogre this size wants between 6 and 18 oysters, 90% of the time.");
                        break;

                    case Difficulty.Hard:
                        oysterMean = 16;
                        oysterStd = 4;
                        Console.WriteLine("\tAn ogre this size wants between 10 and 22 oysters, 90% of the time.");
                        break;
                }

                int oysterBet;

                while (true)
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write("\nIt's a gamble. How many oysters do you throw? ");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    try { oysterBet = Math.Clamp(Convert.ToInt32(Console.ReadLine()), 0, _turnsRemaining - 1); }
                    catch (FormatException) { return (true, false, false, "\nYou freeze up, and the ogre devours you!"); }
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
                }

                int oysterDraw = Math.Clamp(DrawFromNormalDist(oysterMean, oysterStd), 0, Int32.MaxValue);
                if (oysterBet >= oysterDraw)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("\n\tYeehaw! The ogre falls for your oyster gambit.\n\tYou scramble away in the dark.");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    _turnsRemaining -= oysterBet;

                    int escapeValOne = _rand.Next(0, 2);  // row or column?
                    int escapeValTwo = _rand.Next(0, 2);  // which direction?

                    if (escapeValOne == 0 && escapeValTwo == 0 && _playerPosition.Row != 0) { _playerPosition.Row--; }                  // go north
                    else if (escapeValOne == 0 && escapeValTwo == 1 && _playerPosition.Row != _mapSize - 1) { _playerPosition.Row++; }  // go south
                    else if (escapeValOne == 1 && escapeValTwo == 0 && _playerPosition.Col != 0) { _playerPosition.Col--; }             // go west
                    else if (escapeValOne == 1 && escapeValTwo == 1 && _playerPosition.Col != _mapSize - 1) { _playerPosition.Col++; }  // go east
                    else return (true, false, false, "\nIn your frenzy to escape, you run into a wall!\nThe ogre snaps back to attention and devours you.");

                    return (false, false, true, "It shouldn't be over.");  // teleport is active
                }
                else return (true, false, false, "\nThe ogre scoffs at your oyster offering and devours you.");
            
            // if the player mistypes
            default:
                return (true, false, false, "\nYou freeze up, and the ogre devours you!");
        }
    }


    private (bool GameOver, bool DidYouWin, bool teleportCheck, string GameOverMessage) Teleport()
    {
        // occasional minor bug where you hit another teleport (including this one)
        _playerPosition.Row = _rand.Next(0, _mapSize);
        _playerPosition.Col = _rand.Next(0, _mapSize);
        return (false, false, true, "Game shouldn't be over");
    }


    private void AskForMapSize()
    {
        int choice;

        Console.WriteLine("\nWhat size dungeon would you like?");
        Console.WriteLine($"\t1 - Small ({_smallMap}x{_smallMap})");
        Console.WriteLine($"\t2 - Medium ({_mediumMap}x{_mediumMap})");
        Console.WriteLine($"\t3 - Large ({_largeMap}x{_largeMap})");

        while (true)
        {
            Console.Write("Please choose [1], [2], or [3]: ");
            try { choice = Convert.ToInt32(Console.ReadLine()); }
            catch (FormatException) { continue; }
            _mapSize = choice switch { 1 => _smallMap, 2 => _mediumMap, 3 => _largeMap, _ => 0 };
            if (_mapSize != 0) break;
        }
    }


    private void AskForDifficulty()
    {
        int choice;

        Console.WriteLine("\nWhat sort of adventurer are you?");
        Console.WriteLine($"\t1 - Novice");
        Console.WriteLine($"\t2 - Hobbyist");
        Console.WriteLine($"\t3 - Professional");

        while (true)
        {
            Console.Write("Please choose [1], [2], or [3]: ");

            try { choice = Convert.ToInt32(Console.ReadLine()); }
            catch (FormatException) { continue; }

            _difficulty = choice switch { 1 => Difficulty.Easy, 2 => Difficulty.Normal, 3 => Difficulty.Hard, _ => Difficulty.Null };
            if (_difficulty != Difficulty.Null) break;
        }
    }


    private void PopulateMap()
    {
        int ogreCount, goblinCount, pitCount, warpCount, emptyCount;
        (ogreCount, goblinCount, pitCount, warpCount, emptyCount) = SetTileDistByDifficulty();
        int startCount = 1;
        int fountainCount = 1;

        if (ogreCount + goblinCount + pitCount + warpCount + emptyCount + startCount + fountainCount != _mapSize * _mapSize) { Console.WriteLine("doesn't add up"); }

        // double for loop iterates over entire array
        for (int i = 0; i < _mapSize; i++)
        {
            for (int j = 0; j < _mapSize; j++)
            {
                if (startCount > 0)
                {
                    _map[i, j] = new StartTile();
                    startCount--;
                }
                else if (fountainCount > 0)
                {
                    _map[i, j] = new FountainTile();
                    fountainCount--;
                }
                else if (ogreCount > 0)
                {
                    _map[i, j] = new OgreTile();
                    ogreCount--;
                }
                else if (goblinCount > 0)
                {
                    _map[i, j] = new GoblinTile();
                    goblinCount--;
                }
                else if (pitCount > 0)
                {
                    _map[i, j] = new PitTile();
                    pitCount--;
                }
                else if (warpCount > 0)
                {
                    _map[i, j] = new WarpTile();
                    warpCount--;
                }
                else if (emptyCount > 0)
                {
                    _map[i, j] = new EmptyTile();
                    emptyCount--;
                }
            }

        }

        while (true)
        {
            ShuffleMap();
            if (ManhattanDistance(FindStartTile(), FindGoalTile()) >= 2) break;  // make sure start and goal aren't adjacent
        }

        _startPosition = _playerPosition = FindStartTile();
        Console.WriteLine("\nYou feel a deep rumbling. The dungeon has taken shape and awaits its challenger.");
    }


    private void ShuffleMap()
    {
        // Using the Fisher-Yates algorithm
        // https://stackoverflow.com/questions/30164019/shuffling-2d-array-of-cards
        for (int i = _map.Length - 1; i > 0; i--)
        {
            int i0 = i / _mapSize;
            int i1 = i % _mapSize;

            int j = _rand.Next(i + 1);
            int j0 = j / _mapSize;
            int j1 = j % _mapSize;

            MapTile temp = _map[i0, i1];
            _map[i0, i1] = _map[j0, j1];
            _map[j0, j1] = temp;
        }
    }


    private (int, int, int, int, int) SetTileDistByDifficulty()
    {
        int numTiles = _mapSize * _mapSize - 2;  // possible tiles, after counting start and fountain

        // does not compile without initialization due to prior initialization in switch statement
        double ogreMean = 0.0;
        double goblinMean = 0.0;
        double pitMean = 0.0;
        double warpMean = 0.0;
        double std = 1.0;

        int ogreCount, goblinCount, pitCount, warpCount;

        switch (_difficulty)
        {
            case Difficulty.Easy:
                // on a 4x4 (16 tiles), you would have...
                ogreMean = numTiles / 10.66;    // 1.5 ogres
                goblinMean = numTiles / 8;      // 2 goblins
                pitMean = numTiles / 16;        // 1 pit
                warpMean = numTiles / 10.66;    // 1.5 warps
                // for a total of 6/16 = 37.5% dangerous tiles

                std = 1.0;
                break;
            
            case Difficulty.Normal:
                // on a 4x4 (16 tiles), you would have...
                ogreMean = numTiles / 8;        // 2 ogres
                goblinMean = numTiles / 5.33;   // 3 goblins
                pitMean = numTiles / 10.66;     // 1.5 pits
                warpMean = numTiles / 10.66;    // 1.5 warps
                // for a total of 8/16 = 50% dangerous tiles

                std = 1.5;
                break;
            
            case Difficulty.Hard:
                // on a 4x4 (16 tiles), you would have...
                ogreMean = numTiles / 6.4;  // 2.5 ogres
                goblinMean = numTiles / 4;  // 4 goblins
                pitMean = numTiles / 8;     // 2 pits
                warpMean = numTiles / 8;    // 2 warps
                // for a total of 10.5/16 = 65.6% dangerous tiles

                std = 2.0;
                break;
        }

        // loop resets in case numTiles runs out before last draw, guaranteeing a min of 1 rather than 0
        while (true)
        {
            numTiles = _mapSize * _mapSize - 2;  // re-initialize

            try
            {
                ogreCount = Math.Clamp(DrawFromNormalDist(ogreMean, std), 1, numTiles);
                numTiles -= ogreCount;

                goblinCount = Math.Clamp(DrawFromNormalDist(goblinMean, std), 1, numTiles);
                numTiles -= goblinCount;

                pitCount = Math.Clamp(DrawFromNormalDist(pitMean, std), 1, numTiles);
                numTiles -= pitCount;

                warpCount = Math.Clamp(DrawFromNormalDist(warpMean, std), 1, numTiles);
                numTiles -= warpCount;
                break;
            }
            catch (ArgumentException) { continue; }
        }

        return (ogreCount, goblinCount, pitCount, warpCount, numTiles);
    }


    private int DrawFromNormalDist(double mean, double std)
    {
        // https://stackoverflow.com/questions/218060/random-gaussian-variables
        // get two random variables
        double u1 = 1.0 - _rand.NextDouble(); //uniform(0,1] random doubles
        double u2 = 1.0 - _rand.NextDouble();
        
        // Box-Muller transformation
        double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
        double randNormal = mean + std * randStdNormal; //random normal(mean,std^2)
        
        // convert to an int and return
        return (int)randNormal;
    }

    
    private void SetResourcesByMapSize()
    {
        // scale resources to map size
        int numTiles = _mapSize * _mapSize;
        double monstersPerTile, bulletsPerMonster;

        

        switch (_difficulty)
        {
            case Difficulty.Easy:
                _prayersRemaining = 3;
                _turnsRemaining = Convert.ToInt32(numTiles * 1.2);

                // BULLETS
                monstersPerTile = 0.2;  // see SetTileDistByDifficulty()
                bulletsPerMonster = 1.0;
                // t/1 * m/t => m, and m/1 * b/m => b, therefore t/1 * m/t * b/m => b
                _bulletsRemaining = Convert.ToInt32(numTiles * monstersPerTile * bulletsPerMonster);
                break;

            case Difficulty.Normal:
                _prayersRemaining = 1;
                _turnsRemaining = Convert.ToInt32(numTiles);

                // BULLETS
                monstersPerTile = 0.3;  // see SetTileDistByDifficulty()
                bulletsPerMonster = 0.75;
                // t/1 * m/t => m, and m/1 * b/m => b, therefore t/1 * m/t * b/m => b
                _bulletsRemaining = Convert.ToInt32(numTiles * monstersPerTile * bulletsPerMonster);
                break;

            case Difficulty.Hard:
                _prayersRemaining = 1;
                _turnsRemaining = Convert.ToInt32(numTiles * 0.8);

                // BULLETS
                monstersPerTile = 0.4;  // see SetTileDistByDifficulty()
                bulletsPerMonster = 0.5;
                // t/1 * m/t => m, and m/1 * b/m => b, therefore t/1 * m/t * b/m => b
                _bulletsRemaining = Convert.ToInt32(numTiles * monstersPerTile * bulletsPerMonster);
                break;
        }
    }


    private void PrintMap()
    {
        for (int i = 0; i < _mapSize; i++)
        {
            for (int j = 0; j < _mapSize; j++)
            {
                Console.Write(string.Format("{0}\t", _map[i, j].ID));
            }
            Console.Write(Environment.NewLine);
        }
    }


    private (int, int) FindStartTile()
    {
        for (int i = 0; i < _mapSize; i++)
        {
            for (int j = 0; j < _mapSize; j++)
            {
                if (_map[i,j].ID == MapTile.RoomType.Start) { return (i, j); }
            }
        }

        Environment.Exit(2);
        return (0, 0);
    }


    private (int, int) FindGoalTile()
    {
        for (int i = 0; i < _mapSize; i++)
        {
            for (int j = 0; j < _mapSize; j++)
            {
                if (_map[i, j].ID == MapTile.RoomType.Goal) { return (i, j); }
            }
        }

        Environment.Exit(2);
        return (0, 0);
    }


    private int ManhattanDistance((int, int) pt1, (int, int) pt2)
    {
        int manDist = Math.Abs(pt1.Item1 - pt2.Item1) + Math.Abs(pt1.Item2 - pt2.Item2);
        return manDist;
    }


    private bool HandleSpecialShot((int, int) coords)
    {
        if (_map[coords.Item1, coords.Item2].ID == MapTile.RoomType.Goal)
        {
            return FountainShoot();  // always returns true
        }
        else if (_map[coords.Item1, coords.Item2].ID == MapTile.RoomType.Warp)
        {
            return WarpShoot();
        }
        return false;  // whether or not to end the turn
    }


    private bool WarpShoot()
    {
        bool endTurn = false;

        // Selects a random tile to shoot
        int randRow = _rand.Next(_mapSize);
        int randCol = _rand.Next(_mapSize);

        // If it isn't the player's tile, shoot it
        if ((randRow, randCol) != _playerPosition)
        {
            Console.Write("\t");  // matching formatting
            bool isSpecial = _map[randRow, randCol].Shoot();
            if(isSpecial) endTurn = HandleSpecialShot((randRow, randCol));  // recursively call HandleSpecial if warp or fountain
        }

        else
        {
            // handle player death
            endTurn = _shotYourself = true;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nA bullet appears out of thin air and explodes your face!");
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        return endTurn;
    }


    private bool FountainShoot()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n\tA deep rumbling builds, rattling your bones. The dungeon is angry.");
        Console.WriteLine("\tIn your hubris, you damaged what you sought.");
        Console.WriteLine("\tThe world begins to spin. Is the dungeon transforming?");

        (int, int) ftnCoords = FindGoalTile();
        bool alreadyBathed = _map[ftnCoords.Item1, ftnCoords.Item2].IsActive;

        if (alreadyBathed) Console.WriteLine("\tOh well, you've already bathed in the fountain...\n\tTime to get the hell out of dodge!");
        else Console.WriteLine("\tYou'll have plenty of time to mope once you're immortal...\n\tTime to find that fountain and dance with infinity!");

        Console.ForegroundColor = ConsoleColor.Gray;
        Console.Write("\nYou brace yourself as the room takes on a new shape.\nWhat does the dungeon have in store for you? [ENTER] ");

        Console.ReadKey(true);

        _prayersRemaining = 0;  // lose the favor of the gods

        ReactivateAllMonsters();

        while (true)
        {
            ShuffleMap();  // player stays where they are, make sure they aren't next to or on start or goal
            if (ManhattanDistance(FindStartTile(), _playerPosition) >= 2 && ManhattanDistance(FindGoalTile(), _playerPosition) >= 2) break;
        }

        return true;
    }


    private void ReactivateAllMonsters()
    {
        for (int i = 0; i < _mapSize; i++)
        {
            for (int j = 0; j < _mapSize; j++)
            {
                if (_map[i, j].ID == MapTile.RoomType.Goblin) _map[i, j].IsActive = true;
                else if (_map[i, j].ID == MapTile.RoomType.Ogre) _map[i, j].IsActive = true;
            }
        }
    }
}
