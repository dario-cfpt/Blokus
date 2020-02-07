using System.Collections.Generic;

namespace Assets
{
    /// <summary>
    /// Contains the list of players when switching the current scene
    /// </summary>
    public static class PlayerList
    {
        public static List<Player> Players { get; set; } = new List<Player> {
            new Player(BlokusColor.BLUE, "Blue"),
            new Player(BlokusColor.GREEN, "Green"),
            new Player(BlokusColor.RED, "Red"),
            new Player(BlokusColor.YELLOW, "Yellow")
        };
    }
}
