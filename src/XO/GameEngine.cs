using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XO
{
    public class GameEngine
    {
        public Action<BoardData> ExecuteDraw;

        private Board _board;
        private Move _selectedMove;

        public GameEngine()
        {
            _board = new Board();
        }

        public async Task CanvasClickAsync(int column, int row)
        {
            Console.WriteLine($"CanvasClickAsync: {column} {row}");

            if (_board.State == BoardState.Finished)
            {
                // Initiate new game
                var firstPlayer = _board.FirstPlayer == Player.X ? Player.O : Player.X;
                _board = new Board(firstPlayer);
                ExecuteDraw(_board.Serialize());
            }
            else if (_board.IsAvailable(column, row))
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
                }
                else
                {
                    // Make move for this position
                    Console.WriteLine($"CanvasClickAsync: Make move {column} {row}");

                    _selectedMove = null;
                    _board.MakeMove(column, row);
                    boardData = _board.Serialize();
                    if (_board.State == BoardState.Running)
                    {
                        boardData.Data[column][row] *= MoveHightlight.SelectedMove;
                    }
                }

                ExecuteDraw(boardData);
            }
            else if (_selectedMove != null)
            {
                // Clear selection
                _selectedMove = null;
                ExecuteDraw(_board.Serialize());
            }

            await Task.CompletedTask;
        }
    }
}
