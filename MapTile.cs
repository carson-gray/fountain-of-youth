public abstract class MapTile
{
    // does tile have active/nonactive states?
    protected bool CanToggle { get; set; }

    public enum RoomType { Goal, Start, Ogre, Goblin, Pit, Empty, Warp }
    public RoomType ID { get; init; }

    // tracking active states
    public bool IsActive { get; set; } = true;  // default for non toggleable tiles
    protected string? ActivateText { get; init; } = null;
    protected string? DeactivateText { get; init; } = null;

    // flavor text when you are in the room
    protected string? ActiveRoomText { get; init; } = null;  // default for non toggleable tiles
    protected string? NonActiveRoomText { get; init; } = null;

    // flavor text when you are next to the room
    protected string? ActiveAdjacentText { get; init; } = null;  // default for non toggleable tiles
    protected string? NonActiveAdjacentText { get; init; } = null;

    public void PrintRoomText()
    {
        if (IsActive == true && ActiveRoomText != null) Console.WriteLine(ActiveRoomText);
        else if (IsActive == false && NonActiveRoomText != null) Console.WriteLine(NonActiveRoomText);
    }

    public void PrintAdjacentText()
    {
        if (IsActive == true && ActiveAdjacentText != null) Console.WriteLine(ActiveAdjacentText);
        else if (IsActive == false && NonActiveAdjacentText != null) Console.WriteLine(NonActiveAdjacentText);
    }

    public void ToggleActive()
    {
        // toggles if possible, otherwise does nothing
        if (CanToggle == true)
        {
            if (IsActive == true)
            {
                if (DeactivateText != null) Console.WriteLine(DeactivateText);
                IsActive = false;
            }
            else
            {
                if (ActivateText != null) Console.WriteLine(ActivateText);
                IsActive = true;
            }

            // once toggled, does not retoggle
            CanToggle = false;
        }
    }

    public abstract void View();

    public abstract bool Shoot();
}

class FountainTile : MapTile
{
    public FountainTile()
    {
        ID = RoomType.Goal;
        CanToggle = true;
        IsActive = false;
        ActivateText = "\tYou leap into the shimmering pool and are overcome by a surge of energy.\n\tHave you become immortal, or is the water just cold?\n\tEither way, it is time to retrace your steps and return to the surface.";
        ActiveRoomText = "You are back at the fountain, as beautiful as ever.";
        NonActiveRoomText = "At last, the fountain! You are filled with wonder as you gaze upon the sapphire cascade.";
        ActiveAdjacentText = "You sense the familiar freshness of the fountain. It is nearby.";
        NonActiveAdjacentText = "The energy in this room feels fresher somehow, and you hear the trickling of water.";
    }

    public override void View()
    {
        Console.ForegroundColor = ConsoleColor.Blue;

        string l0 = "\t\t    ~~    ";
        string l1 = "\t\t   ~~~~   ";
        string l2 = "\t\t  ~~||~~  ";
        string l3 = "\t\t <~~~~~~> ";
        string l4 = "\t\t ======== ";

        Console.WriteLine();  // buffer
        Console.WriteLine(l0);
        Console.WriteLine(l1);
        Console.WriteLine(l2);
        Console.WriteLine(l3);
        Console.WriteLine(l4);
        Console.WriteLine();  // buffer

        Console.ForegroundColor = ConsoleColor.Gray;  // reset
    }

    public override bool Shoot() 
    {
        if (IsActive == false) Console.WriteLine("You hear a splash. Could it be the fountain?");
        else Console.WriteLine("You hear a splash. Did you really just shoot at the Fountain of Youth?");

        return true;
    }
}

class OgreTile : MapTile
{
    // can gamble turns (throw oysters as a distraction) to escape
    // can shoot with musket balls from adjacent room (100% hit rate)
    // can gamble a shot in the room as well
    public OgreTile()
    {
        ID = RoomType.Ogre;
        CanToggle = true;
        IsActive = true;
        DeactivateText = "You hear a mighty thud and the waning bellows of a dying ogre.";
        ActiveRoomText = "You have walked into an ogre's lair!\nIts piercing gaze fixates on your slight frame with a murderous hunger.";
        NonActiveRoomText = "You enter the dead ogre's lair. While the danger is gone, the smell remains.";
        ActiveAdjacentText = "A rancid stench pervades the room, paired with a cacophony of breathy grunts.";
        NonActiveAdjacentText = "The stale air and scampering of rats indicate a dead ogre is nearby.";
    }

    public override void View()
    {
        Console.ForegroundColor = ConsoleColor.Red;

        string l0, l1, l2, l3, l4;
        if (IsActive)
        {
            l0 = "\t\t   ____   ";
            l1 = "\t\t ( o`'0 ) ";
            l2 = "\t\t   VVVV   ";
            l3 = "\t\t />___/>  ";
            l4 = "\t\t |_   |_  ";
        }
        else
        {
            l0 = "\t\t   *      ";
            l1 = "\t\t   ____  *";
            l2 = "\t\t ( x'`# ) ";
            l3 = "\t\t * VVVV * ";
            l4 = "\t\t /  ~o~|  ";
        }

        Console.WriteLine();  // buffer
        Console.WriteLine(l0);
        Console.WriteLine(l1);
        Console.WriteLine(l2);
        Console.WriteLine(l3);
        Console.WriteLine(l4);
        Console.WriteLine();  // buffer

        Console.ForegroundColor = ConsoleColor.Gray;  // reset
    }

    public override bool Shoot()
    {
        if (IsActive) ToggleActive();
        else Console.WriteLine("Your ears are greeted with a splatter. You shot a corpse!");
        return false;
    }
}

class GoblinTile : MapTile
{
    // pickpockets you of oysters and musket balls, but leaves you unharmed
    // can shoot with musket balls from adjacent room (100% hit rate)
    public GoblinTile()
    {
        ID = RoomType.Goblin;
        CanToggle = true;
        IsActive = true;
        DeactivateText = "You hear a shriek, a bang, and the furious scurry of retreat.";
        ActiveRoomText = "You have walked into a goblin trap!\nThe first goblin detonates a smokebomb while the second grabs at you, squealing with glee.\nIt is over in an instant, and you are left alone with watering eyes and a lighter satchel.";
        NonActiveRoomText = "The air is filled with a suffocating residue left over from smokebombs.\nGoblins were here, but they aren't coming back.";
        ActiveAdjacentText = "You hear muffled snickering and scampering. You better watch your satchel!";
        NonActiveAdjacentText = "You catch the subtle smell of lingering smoke. Goblins were in a nearby room, but are now long gone.";
    }

    public override void View()
    {
        Console.ForegroundColor = ConsoleColor.Green;

        string l0, l1, l2, l3, l4;
        if (IsActive)
        {
            l0 = "\t\t  v       ";
            l1 = "\t\t (oo)  v  ";
            l2 = "\t\t /()> (oo)";
            l3 = "\t\t $bb  /()>";
            l4 = "\t\t      $bb ";
        }
        else
        {
            l0 = "\t\t   *    ~ ";
            l1 = "\t\t  *   *   ";
            l2 = "\t\t   ~  *   ";
            l3 = "\t\t   *     *";
            l4 = "\t\t ~  ~  ~~ ";
        }

        Console.WriteLine();  // buffer
        Console.WriteLine(l0);
        Console.WriteLine(l1);
        Console.WriteLine(l2);
        Console.WriteLine(l3);
        Console.WriteLine(l4);
        Console.WriteLine();  // buffer

        Console.ForegroundColor = ConsoleColor.Gray;  // reset
    }

    public override bool Shoot() 
    {
        if(IsActive) ToggleActive(); 
        else Console.WriteLine("The blast echoes, but nothing was hit.");
        return false;
    }
}

class PitTile : MapTile
{
    // instakill
    public PitTile()
    {
        ID = RoomType.Pit;
        CanToggle = false;
        ActiveRoomText = "\tAAAAAAAAAHHHHHHHHH!!!!!!!";
        ActiveAdjacentText = "A high-pitch wind whistles through the caverns. Be very careful, you are close to a treacherous pit!";
    }

    public override void View()
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;

        string l0 = "\t\t|        |";
        string l1 = "\t\t|        |";
        string l2 = "\t\t|        |";
        string l3 = "\t\t|        |";
        string l4 = "\t\t|^|^|^|^||";

        Console.WriteLine();  // buffer
        Console.WriteLine(l0);
        Console.WriteLine(l1);
        Console.WriteLine(l2);
        Console.WriteLine(l3);
        Console.WriteLine(l4);
        Console.WriteLine();  // buffer

        Console.ForegroundColor = ConsoleColor.Gray;  // reset
    }

    public override bool Shoot() 
    {
        Console.WriteLine("The blast echoes, but nothing was hit.");
        return false;
    }
}

class EmptyTile : MapTile
{
    public EmptyTile()
    {
        ID = RoomType.Empty;
        CanToggle = false;
        ActiveRoomText = "The room is empty except for the occasional boulder.";
    }

    public override void View()
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;

        string l0 = "\t\t----------";
        string l1 = "\t\t          ";
        string l2 = "\t\t--_-~-n--_";
        string l3 = "\t\t-~n--_-~--";
        string l4 = "\t\t-_--~--_--";

        Console.WriteLine();  // buffer
        Console.WriteLine(l0);
        Console.WriteLine(l1);
        Console.WriteLine(l2);
        Console.WriteLine(l3);
        Console.WriteLine(l4);
        Console.WriteLine();  // buffer

        Console.ForegroundColor = ConsoleColor.Gray;  // reset
    }

    public override bool Shoot() 
    {
        Console.WriteLine("The blast echoes, but nothing was hit.");
        return false;
    }
}

class WarpTile : MapTile
{
    // teleports you to a random room
    public WarpTile()
    {
        ID = RoomType.Warp;
        CanToggle = false;
        ActiveRoomText = "Zip! Zap! Zoop! You stretch and convulse and contract then BAM!";
        ActiveAdjacentText = "You hear the faint crackle of magic.";
    }

    public override void View()
    {
        Console.ForegroundColor = ConsoleColor.Magenta;

        string l0 = "\t\t0  f v  < ";
        string l1 = "\t\t #  / s 8 ";
        string l2 = "\t\t _q  - $  ";
        string l3 = "\t\t@ *  >  = ";
        string l4 = "\t\t / } c p +";

        Console.WriteLine();  // buffer
        Console.WriteLine(l0);
        Console.WriteLine(l1);
        Console.WriteLine(l2);
        Console.WriteLine(l3);
        Console.WriteLine(l4);
        Console.WriteLine();  // buffer

        Console.ForegroundColor = ConsoleColor.Gray;  // reset
    }

    public override bool Shoot() 
    {
        Console.WriteLine("The musket ball teleports with a sizzle and a zap...");
        return true;
    }
}

class StartTile : MapTile
{
    // entrance and exit
    public StartTile()
    {
        ID = RoomType.Start;
        CanToggle = true;
        IsActive = false;
        NonActiveRoomText = "Light pours in from the cave entrance, and the freedom is tempting.\nBut first, you need to find the fountain.";
        ActiveAdjacentText = "This room is slightly brighter than the rest.";
        NonActiveAdjacentText = "This room is slightly brighter than the rest.";
    }

    public override void View()
    {
        Console.ForegroundColor = ConsoleColor.Yellow;

        string l0 = "\t\t----  ----";
        string l1 = "\t\t   *  |   ";
        string l2 = "\t\t      |*  ";
        string l3 = "\t\t *    l   ";
        string l4 = "\t\t__________";

        Console.WriteLine();  // buffer
        Console.WriteLine(l0);
        Console.WriteLine(l1);
        Console.WriteLine(l2);
        Console.WriteLine(l3);
        Console.WriteLine(l4);
        Console.WriteLine();  // buffer

        Console.ForegroundColor = ConsoleColor.Gray;  // reset
    }

    public override bool Shoot() 
    {
        Console.WriteLine("The blast echoes, but nothing was hit.");
        return false;
    }
}
