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

        public BoardData BoardData
        {
            get
            {
                return _board.Serialize();
            }
        }

        public GameEngine()
        {
            _board = new Board();
        }

        public async Task CanvasClickAsync(int column, int row)
        {
            Console.WriteLine($"CanvasClickAsync: {column} {row}");

            if (_board.IsAvailable(column, row))
            {
                _board.MakeMove(column, row);

                ExecuteDraw(BoardData);
            }

            await Task.CompletedTask;
        }
    }
}
