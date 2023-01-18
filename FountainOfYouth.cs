FountainOfYouth game = new();

public class FountainOfYouth
{
    private string _moveSeparator = "-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-";

    private MapTile[,] _map;
    private int _smallMap = 4;
    private int _mediumMap = 6;
    private int _largeMap = 8;
    private int _mapSize;
    bool _debugging = true;

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
    

    public FountainOfYouth()
    {
        Console.Title = "Fountain of Youth";
        Console.ForegroundColor = ConsoleColor.Gray;

        if (_debugging)
        {
            AskForDifficulty();
            AskForMapSize();
            SetResourcesByMapSize();
            _map = new MapTile[_mapSize, _mapSize];
            PopulateMap();
        }
        else
        {
            Console.WriteLine("Welcome to the Fountain of Youth game!");
            Console.WriteLine("It is a text-based adventure game where you navigate a grid world in search of the fountain of youth.");
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

            Console.WriteLine("\nYou have travelled far to find this dungeon, which legend says holds the fountain of youth!");
            Console.WriteLine("The entrance is but a humble hole in the ground.");

            Thread.Sleep(5000);

            Console.CursorVisible = true;
            Console.Write("\nYou fasten your rope to a nearby tree.\nDo you dare drop it into the abyss below? [ENTER] ");
            Console.ReadKey(true);
            Console.WriteLine();

            PopulateMap();
            Console.Write("There's no going back now. Take the plunge... [ENTER] ");
            Console.ReadKey(true);
            Console.WriteLine();

            Console.CursorVisible = false;
            Console.WriteLine("\nArmed with a musket and a satchel of smoked oysters, you enter the unknown.");
            Thread.Sleep(4000);
            Console.CursorVisible = true;
        }

        Run();
    }


    private void PrintHelpText()
    {
        Console.WriteLine("\n\t* Choose an action ('move', 'shoot', 'pray').");
        Console.WriteLine("\t* If you move or shoot, also type a direction ('north', 'south', 'east', 'west').");
        Console.WriteLine("\t\t- Ex. 'move east', 'shoot north'");
        Console.WriteLine("\t* Prayers are a scarce resource that guide you back to the start.");
        Console.WriteLine("\t* Typing 'help' returns to this screen.\n");
    }


    private void Run() 
    {
        bool victory = false;
        bool gameOver = false;
        while (!gameOver) (gameOver, victory) = Update();

        Console.ForegroundColor = ConsoleColor.Yellow;
        if (victory == true) Console.WriteLine("On to the next adventure...");
        if (victory == false) Console.WriteLine("In your quest for eternal youth, you found eternal death.");
        Console.Write("Press ENTER to quit: ");
        Console.ReadKey();
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine();
        Environment.Exit(0);
    }


    private (bool GameOver, bool DidYouWin) Update()
    {
        Console.WriteLine();
        Console.WriteLine(_moveSeparator);

        if (_turnsRemaining == 0)
        {
            Console.WriteLine("\nYou have run out of oysters and subsequently your will to live.");
            return (true, false);
        }

        MapTile currentTile = _map[_playerPosition.Row, _playerPosition.Col];
        currentTile.View();             // see current room picture
        currentTile.PrintRoomText();    // get current room spiel

        // room interaction logic
        (bool gameOver, bool didYouWin, bool teleportCheck, string gameOverMessage) = InteractWithRoom(currentTile);
        if (teleportCheck) return (false, false);  // reset in a different room

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
        string wallBump = "You walk straight into the wall. There's nothing that way!";

        switch (direction)
        {
            case Direction.North:
                if (row != 0) { _playerPosition.Row = row - 1; return true; }
                else { Console.WriteLine(wallBump); return false; }
            case Direction.South:
                if (row != _mapSize - 1) { _playerPosition.Row = row + 1; return true; }
                else { Console.WriteLine(wallBump); return false; }
            case Direction.East:
                if (col != _mapSize - 1) { _playerPosition.Col = col + 1; return true; }
                else { Console.WriteLine(wallBump); return false; }
            case Direction.West:
                if (col != 0) { _playerPosition.Col = col - 1; return true; }
                else { Console.WriteLine(wallBump); return false; }
            default:
                return false;
        }
    }
    
    
    private bool Shoot(Direction direction) 
    {
        int col = _playerPosition.Col;
        int row = _playerPosition.Row;

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine();

        if (_bulletsRemaining > 0)
        {
            switch (direction)
            {
                case Direction.North:
                    if (row != 0) { Console.Write("\t"); _map[row-1, col].Shoot(); _bulletsRemaining--; break; }
                    else { Console.WriteLine("\tBang! You just shot at a wall."); _bulletsRemaining--; break; }
                case Direction.South:
                    if (row != _mapSize - 1) { Console.Write("\t"); _map[row+1, col].Shoot(); _bulletsRemaining--; break; }
                    else { Console.WriteLine("\tBang! You just shot at a wall."); _bulletsRemaining--; break; }
                case Direction.East:
                    if (col != _mapSize - 1) { Console.Write("\t"); _map[row, col+1].Shoot(); _bulletsRemaining--; break; }
                    else { Console.WriteLine("\tBang! You just shot at a wall."); _bulletsRemaining--; break; }
                case Direction.West:
                    if (col != 0) { Console.Write("\t"); _map[row, col-1].Shoot(); _bulletsRemaining--; break; }
                    else { Console.WriteLine("\tBang! You just shot at a wall."); _bulletsRemaining--; break; }
                default:
                    break;
            }
        }
        else Console.WriteLine("\tYou aim your empty musket and say \"blammo!\"");

        Console.WriteLine($"\tYou have {_bulletsRemaining} musket balls remaining.");
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Gray;
        return false;
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

                    currentTile.ToggleActive();  // bathe in fountain of youth
                    _map[_startPosition.Row, _startPosition.Col].ToggleActive();  // unlock start
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
                        Console.WriteLine("Praise! The ogre crashes to the ground with a musket ball between its eyes.");
                        return (false, false, false, "shouldn't be over");
                    }
                }
                else return (true, false, false, "\nClick! You are out of musket balls, and the ogre devours you!");
            
            // gamble with oysters
            case 1:
                double oysterMean = 0;
                double oysterStd = 0;

                Console.WriteLine("\nYou think back to your monsters unit in Magical Zoology 101.");

                switch (_difficulty)
                {
                    case Difficulty.Easy:
                        oysterMean = 8;
                        oysterStd = 4;
                        Console.WriteLine("An ogre this size wants between 2 and 14 oysters, 90% of the time.");
                        break;

                    case Difficulty.Normal:
                        oysterMean = 12;
                        oysterStd = 4;
                        Console.WriteLine("An ogre this size wants between 6 and 18 oysters, 90% of the time.");
                        break;

                    case Difficulty.Hard:
                        oysterMean = 16;
                        oysterStd = 4;
                        Console.WriteLine("An ogre this size wants between 10 and 22 oysters, 90% of the time.");
                        break;
                }

                int oysterBet;

                while (true)
                {
                    Console.Write("It's a gamble. How many oysters do you throw? ");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    try { oysterBet = Math.Clamp(Convert.ToInt32(Console.ReadLine()), 0, _turnsRemaining - 1); }
                    catch (FormatException) { return (true, false, false, "\nYou freeze up, and the ogre devours you!"); }
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
                }

                int oysterDraw = Math.Clamp(DrawFromNormalDist(oysterMean, oysterStd), 0, Int32.MaxValue);
                if (oysterBet >= oysterDraw)
                {
                    Console.WriteLine("\nYeehaw! The ogre falls for your oyster gambit.\nYou scramble away in the dark.");
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

        Console.WriteLine("\nWhat difficulty would you like?");
        Console.WriteLine($"\t1 - Easy");
        Console.WriteLine($"\t2 - Normal");
        Console.WriteLine($"\t3 - Hard");

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

        ShuffleMap();
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
                _prayersRemaining = 2;
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
}
