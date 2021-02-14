using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Domain
{
    public class GameOption
    {
        public int GameOptionId { get; set; }

        [MaxLength(128)] public string Name { get; set; } = null!;

        [Display(Name = "Can boats touch")] public EBoatsCanTouch EBoatsCanTouch { get; set; }

        [Display(Name = "Next move after hit")]
        public ENextMoveAfterHit ENextMoveAfterHit { get; set; }

        public ICollection<GameOptionBoat> GameOptionBoats { get; set; } = null!;

        public ICollection<Game> Games { get; set; } = null!;
    }
}