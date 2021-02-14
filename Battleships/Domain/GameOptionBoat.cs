using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class GameOptionBoat
    {
        public int GameOptionBoatId { get; set; }
        
        [Range(0, 5, ErrorMessage = "Min amount is 0, max amount is 5")]
        public int Amount { get; set; }

        public int BoatId { get; set; }
        public Boat? Boat { get; set; }

        public int GameOptionId { get; set; }
        public GameOption? GameOption { get; set; }
    }
}