using System;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using XO.ComputerPlayers;

namespace XO
{
    public class GameEngine
    {
        public Action<BoardData> ExecuteDraw;

        private Board _board;
        private Move _selectedMove;

        private IPlayer _playerX;
        private IPlayer _playerO;
        private System.Timers.Timer _timer;

        public GameEngine()
        {
            _board = new Board();

            _timer = new System.Timers.Timer(100);
            _timer.Elapsed += ComputerPlayersMove;
        }

        public void SetPlayers(string playerX, string playerO)
        {
            Console.WriteLine($"GameEngine: X: {playerX}, O: {playerO}");

            _playerX = LoadPlayer(playerX);
            _playerO = LoadPlayer(playerO);
            SetTimerForComputerPlayer();
        }

        private IPlayer LoadPlayer(string player)
        {
            switch (player)
            {
                case "Randomizer":
                    return new Randomizer();
                default:
                    return new HumanPlayer();
            }
        }

        public async Task CanvasClickAsync(int column, int row)
        {
            Console.WriteLine($"CanvasClickAsync: {column} {row}");

            if (_board.State == BoardState.Finished)
            {
                // Initiate new game and swap players
                var firstPlayer = _board.FirstPlayer == Player.X ? Player.O : Player.X;
                _board = new Board(firstPlayer);

                ExecuteDraw(_board.Serialize());
                SetTimerForComputerPlayer();
            }
            else if ((_board.CurrentPlayer == Player.X && _playerX.IsHuman) ||
                     (_board.CurrentPlayer == Player.O && _playerO.IsHuman))
            {
                if (_board.IsAvailable(column, row))
                {
                    BoardData boardData;
                    if (_selectedMove == null || _selectedMove.Column != column || _selectedMove.Row != row)
                    {
                        // Pre-select this position 
                        Console.WriteLine($"CanvasClickAsync: Preselect move {column} {row}");
                        _selectedMove = new Move(_board)
                        {
                            Column = column,
                            Row = row
                        };

                        boardData = _board.Serialize();
                        boardData.Data[column][row] = _board.CurrentPlayer == Player.X ? MoveHightlight.PreSelectedMove : -MoveHightlight.PreSelectedMove;
                        ExecuteDraw(boardData);
                    }
                    else
                    {
                        // Make move for this position
                        Console.WriteLine($"CanvasClickAsync: Make move {column} {row}");

                        _board.MakeMove(column, row);
                        HighlightMove(column, row);
                    }
                }
                else if (_selectedMove != null)
                {
                    // Clear selection
                    _selectedMove = null;
                    ExecuteDraw(_board.Serialize());
                }
            }

            await Task.CompletedTask;
        }

        private void HighlightMove(int column, int row)
        {
            _selectedMove = null;
            BoardData boardData;
            boardData = _board.Serialize();
            if (_board.State == BoardState.Running)
            {
                boardData.Data[column][row] *= MoveHightlight.SelectedMove;
                SetTimerForComputerPlayer();
            }

            ExecuteDraw(boardData);
        }

        private void SetTimerForComputerPlayer()
        {
            Console.WriteLine("SetTimerForComputerPlayer");

            if ((_board.CurrentPlayer == Player.X && !_playerX.IsHuman) ||
                (_board.CurrentPlayer == Player.O && !_playerO.IsHuman))
            {
                // Current player is computer. Let's call that logic in 100ms.
                _timer.Start();
            }
        }

        private void ComputerPlayersMove(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine("ComputerPlayersMove");
            _timer.Stop();

            Move move;
            if (_board.CurrentPlayer == Player.X)
            {
                move = _playerX.MakeMove(_board);
            }
            else
            {
                move = _playerO.MakeMove(_board);
            }

            _board.MakeMove(move);
            HighlightMove(move.Column, move.Row);
        }
    }
}
