using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class BoardState
    {
        public int BoardStateId { get; set; }

        [MaxLength(100000)] public string PlayerBoardState { get; set; } = null!;

        public int PlayerId { get; set; }
        public Player Player { get; set; } = null!;

        [InverseProperty("Player1BoardState")] public ICollection<Game> Games1 { get; set; } = null!;

        [InverseProperty("Player2BoardState")] public ICollection<Game> Games2 { get; set; } = null!;
    }
}