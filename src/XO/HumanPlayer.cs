using System;

namespace XO
{
    public class HumanPlayer : IPlayer
    {
        public bool IsHuman => true;

        public Move MakeMove(Board board)
        {
            throw new NotImplementedException();
        }
    }
}
