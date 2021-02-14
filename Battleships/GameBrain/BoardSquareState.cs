namespace GameBrain
{
    public class BoardSquareState
    {
        public int? BoatId { get; set; }
        public bool Bomb { get; set; }

        public override string ToString()
        {
            if (BoatId != null && Bomb)
            {
                return "X";
            }

            if (BoatId != null && !Bomb)
            {
                return "B";
            }

            return !Bomb ? " " : "O";
        }
    }
}