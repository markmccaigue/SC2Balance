namespace SC2Balance.ViewModels
{
    public class WinRate
    {
        public float TerranWinRate { get; set; }
        public float ProtossWinRate { get; set; }
        public float ZergWinRate { get; set; }

        public bool IsEmpty()
        {
            return TerranWinRate.Equals(0) && ProtossWinRate.Equals(0) && ZergWinRate.Equals(0);
        }
    }
}