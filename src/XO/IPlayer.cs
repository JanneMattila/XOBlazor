namespace XO
{
    public interface IPlayer
    {
        bool IsHuman { get; }

        Move MakeMove(Board board);
    }
}