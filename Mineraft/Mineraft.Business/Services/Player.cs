using Mineraft.Business.Services.Contracts;

namespace Mineraft.Business.Services
{
    public class Player : IPlayer
    {
        public int PositionX { get; private set; }
        public int PositionY { get; private set; }
        public bool OnTheBoard { get; set; }

        public void UpdatePosition(int x, int y)
        {
            //note, no safety here in case we want to position the player off the board temporarily

            PositionX = x;
            PositionY = y;
        }
    }
}
