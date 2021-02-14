using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enums;

namespace Domain
{
    public class Player
    {
        public int PlayerId { get; set; }

        [Display(Name = "player name")]
        [StringLength(20)]
        [Required]
        [MaxLength(20)]
        [MinLength(3)]
        public string Name { get; set; } = null!;

        public bool PlayerTurn { get; set; }

        public EPlayerType EPlayerType { get; set; }

        public ICollection<BoardState>? BoardStates { get; set; }

        [InverseProperty("Player1")] public ICollection<Game>? Player1Games { get; set; }

        [InverseProperty("Player2")] public ICollection<Game>? Player2Games { get; set; }
    }
}