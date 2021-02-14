using System;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class Game
    {
        public int GameId { get; set; }

        public int GameOptionId { get; set; }
        public GameOption? GameOption { get; set; }

        [Display(Name = "game name")]
        [StringLength(30)]
        [Required]
        [MaxLength(30)]
        [MinLength(3)]
        public string Description { get; set; } = DateTime.Now.ToLongDateString();
        
        public string? History { get; set; }

        public int Player1Id { get; set; }
        public Player Player1 { get; set; } = null!;

        public int Player2Id { get; set; }
        public Player Player2 { get; set; } = null!;

        public int Player1BoardStateId { get; set; }
        public BoardState? Player1BoardState { get; set; }

        public int Player2BoardStateId { get; set; }
        public BoardState? Player2BoardState { get; set; } 
    }
}