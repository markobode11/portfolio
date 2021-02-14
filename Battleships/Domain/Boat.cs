using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class Boat
    {
        public int BoatId { get; set; }

        [Range(1, 5)] public int Size { get; set; }

        [MaxLength(32)] public string Name { get; set; } = null!;

        public ICollection<GameOptionBoat> GameOptionBoats { get; set; } = null!;
        
    }
}