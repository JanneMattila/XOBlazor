namespace XO.Tests.Stubs
{
    public class PlayerStub : IPlayer
    {
        public bool IsHuman => _isHuman;
        public Move Move;

        private bool _isHuman;

        public PlayerStub(bool isHuman)
        {
            _isHuman = isHuman;
        }
        
        public Move MakeMove(Board board)
        {
            return Move;
        }
    }
}
