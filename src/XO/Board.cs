using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace XO
{
    public class Board
    {
        public const int StraightLength = 5;
        public const char NewLine = '\n';

        private readonly Stack<Move> _moves = new Stack<Move>();

        private string _state = BoardState.Unknown;
        private string _subState = BoardSubState.Unknown;
        private EvaluationResult _evaluationResult = null;

        private Piece[][] _pieces;

        public Board(Player player = Player.X, int width = 10, int heigth = 10)
        {
            CleanUp(player, width, heigth);
        }

        private Board(Player firstPlayer, Player currentPlayer, int width, int heigth, string state, string subState, Stack<Move> moves, Piece[][] pieces)
        {
            CleanUp(firstPlayer, width, heigth);

            CurrentPlayer = currentPlayer;
            _state = state;
            _subState = subState;
            _moves = new Stack<Move>(moves);
            for (var row = 0; row < Height; row++)
            {
                for (var column = 0; column < Width; column++)
                {
                    _pieces[column][row] = pieces[column][row];
                }
            }
        }

        public int Width { get; private set; }

        public int Height { get; private set; }

        public Player FirstPlayer { get; internal set; }

        public Player CurrentPlayer { get; internal set; }

        public Move PreviousMove
        {
            get
            {
                if (_moves.Count > 0)
                {
                    return _moves.Peek();
                }

                return null;
            }
        }

        public int Count
        {
            get
            {
                return _moves.Count;
            }
        }

        public List<int> WinningMoves
        {
            get
            {
                if (_evaluationResult != null)
                {
                    var winningStraights = _evaluationResult.GetWinningStraights();
                    return winningStraights.SelectMany(s => s.Indexes).ToList();
                }

                return null;
            }
        }

        public string State
        {
            get
            {
                return _state;
            }
        }

        public string SubState
        {
            get
            {
                return _subState;
            }
        }

        public BoardData Serialize()
        {
            return new BoardData()
            {
                Size = $"{Width}x{Height}",
                State = BoardState.Running,
                Rules = null,
                FirstPlayer = FirstPlayer.ToString().ToLower()[0],
                CurrentPlayer = CurrentPlayer.ToString().ToLower()[0],
                Board = ToString(),
                Data = ConvertPiecesToDataArray(_pieces),
                Text = new List<string>(),
                Moves = _moves.Select(m => m.Column + (m.Row * Width)).Reverse().ToList(),
                Version = 1
            };
        }

        private int[][] ConvertPiecesToDataArray(Piece[][] pieces)
        {
            var data = new int[pieces[0].Length][];
            for (var column = 0; column < pieces[0].Length; column++)
            {
                data[column] = new int[pieces[0].Length];
                for (var row = 0; row < pieces.Length; row++)
                {
                    var piece = pieces[column][row];
                    if (piece == Piece.X)
                    {
                        data[column][row] = MoveHightlight.NormalMove;
                    }
                    else if (piece == Piece.O)
                    {
                        data[column][row] = -MoveHightlight.NormalMove;
                    }
                }
            }

            if (this.WinningMoves != null)
            {
                foreach (var moveIndex in this.WinningMoves)
                {
                    var move = Move.FromIndex(this, moveIndex);
                    data[move.Column][move.Row] *= MoveHightlight.SelectedMove;
                }
            }

            return data;
        }

        public void Deserialize(BoardData data)
        {
            var size = data.Size.Split(new char[] { 'x' }, StringSplitOptions.RemoveEmptyEntries);
            var width = Convert.ToInt32(size[0]);
            var height = Convert.ToInt32(size[1]);
            var player = Player.X;
            if (data?.FirstPlayer == Player.O.ToString().ToLower()[0])
            {
                player = Player.O;
            }

            CleanUp(player, width, height);

            if (data.Moves != null)
            {
                foreach (var move in data.Moves)
                {
                    MakeMove(move);
                }
            }
        }

        public void Deserialize(string data)
        {
            Deserialize(JsonSerializer.Deserialize<BoardData>(data));
        }

        public void SetBoard(string board)
        {
            var rows = board
                .Replace("\r", string.Empty)
                .Split(
                    new char[] { NewLine },
                    StringSplitOptions.RemoveEmptyEntries);

            if (rows.Length == 0)
            {
                throw new ArgumentException("Invalid board", nameof(board));
            }

            CleanUp(CurrentPlayer, rows[0].Length, rows.Length);
            for (var column = 0; column < rows[0].Length; column++)
            {
                for (var row = 0; row < rows.Length; row++)
                {
                    var piece = rows[row][column];
                    var boardPiece = Piece.Empty;
                    switch (piece)
                    {
                        case '-':
                            break;
                        case 'x':
                            boardPiece = Piece.X;
                            break;
                        case 'o':
                            boardPiece = Piece.O;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException("board");
                    }

                    _pieces[column][row] = boardPiece;
                }
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            for (var row = 0; row < Height; row++)
            {
                if (row > 0)
                {
                    sb.Append(NewLine);
                }

                for (var column = 0; column < Width; column++)
                {
                    var piece = _pieces[column][row];
                    var pieceChar = '-';

                    switch (piece)
                    {
                        case Piece.X:
                            pieceChar = 'x';
                            break;
                        case Piece.O:
                            pieceChar = 'o';
                            break;
                        default:
                            break;
                    }

                    sb.Append(pieceChar);
                }
            }

            return sb.ToString();
        }

        public Board Clone()
        {
            return new Board(FirstPlayer, CurrentPlayer, Width, Height, _state, _subState, _moves, _pieces);
        }

        public Piece GetPiece(int moveIndex)
        {
            return GetPiece(Move.FromIndex(this, moveIndex));
        }

        public Piece GetPiece(Move move)
        {
            return GetPiece(move.Column, move.Row);
        }

        public Piece GetPiece(int column, int row)
        {
            return _pieces[column][row];
        }

        public EvaluationResult MakeMove(int moveIndex)
        {
            return MakeMove(Move.FromIndex(this, moveIndex));
        }

        public EvaluationResult MakeMove(Move move)
        {
            return MakeMove(move.Column, move.Row);
        }

        public IEnumerable<Move> GetAvailableMovesInRadius(int column, int row, int radius)
        {
            if (_state != BoardState.Running)
            {
                yield break;
            }

            var minColumn = Math.Max(column - radius, 0);
            var maxColumn = Math.Min(column + radius + 1, Width);
            var minRow = Math.Max(row - radius, 0);
            var maxRow = Math.Min(row + radius + 1, Height);

            for (var c = minColumn; c < maxColumn; c++)
            {
                for (var r = minRow; r < maxRow; r++)
                {
                    if (GetPiece(c, r) == Piece.Empty)
                    {
                        yield return Move.FromCoordinates(this, c, r);
                    }
                }
            }
        }

        public IEnumerable<Move> GetAvailableMovesInRadius(int index, int radius)
        {
            var move = Move.FromIndex(this, index);
            return GetAvailableMovesInRadius(move.Column, move.Row, radius);
        }

        public IEnumerable<int> GetAvailableMovesInRadiusAsNumbers(int column, int row, int radius)
        {
            var moves = GetAvailableMovesInRadius(column, row, radius);
            return moves.Select(move => move.ToInt(this));
        }

        public IEnumerable<int> GetAvailableMovesInRadiusAsNumbers(int index, int radius)
        {
            var move = Move.FromIndex(this, index);
            return GetAvailableMovesInRadiusAsNumbers(move.Column, move.Row, radius);
        }

        public IEnumerable<Move> GetNearbyAvailableMoves()
        {
            var moves = new List<Move>();
            var indexes = new HashSet<int>();
            var moveOffsets = new List<int[]>()
            {
                new int[] { 1, 0 }, // East
                new int[] { -1, 0 }, // West
                new int[] { 0, 1 }, // South
                new int[] { 0, -1 }, // North
                new int[] { 1, -1 }, // North-East
                new int[] { -1, 1 }, // South-West
                new int[] { -1, -1 }, // North-West
                new int[] { 1, 1 } // South-East
            };
            foreach (var move in GetMoves())
            {
                foreach (var moveOffset in moveOffsets)
                {
                    var column = move.Column + moveOffset[0];
                    var row = move.Row + moveOffset[1];
                    if (column < 0 || column >= Width || row < 0 || row >= Height)
                    {
                        continue;
                    }

                    if (GetPiece(column, row) == Piece.Empty)
                    {
                        var index = column + (row * Width);
                        indexes.Add(index);
                    }
                }
            }

            foreach (var index in indexes)
            {
                moves.Add(Move.FromIndex(this, index));
            }
            return moves;
        }

        public IEnumerable<Move> GetAvailableMoves()
        {
            return GetAvailableMoves(0, 0, Width, Height);
        }

        public IEnumerable<Move> GetAvailableMoves(int startColumn, int startRow, int endColumn, int endRow)
        {
            if (_state != BoardState.Running)
            {
                yield break;
            }

            for (var column = startColumn; column < endColumn; column++)
            {
                for (var row = startRow; row < endRow; row++)
                {
                    if (GetPiece(column, row) == Piece.Empty)
                    {
                        yield return Move.FromCoordinates(this, column, row);
                    }
                }
            }
        }

        public IEnumerable<int> GetAvailableMovesAsNumbers()
        {
            var moves = GetAvailableMoves();
            return moves.Select(move => move.ToInt(this));
        }

        public IEnumerable<Move> GetMoves()
        {
            return _moves.Reverse();
        }

        public IEnumerable<int> GetMovesAsNumbers()
        {
            var moves = GetMoves();
            return moves.Select(move => move.ToInt(this));
        }

        public EvaluationResult MakeMove(int column, int row)
        {
            if (!IsAvailable(column, row))
            {
                throw new ArgumentException();
            }

            if (_state != BoardState.Running)
            {
                throw new ArgumentException();
            }

            if (CurrentPlayer == Player.X)
            {
                _pieces[column][row] = Piece.X;
                CurrentPlayer = Player.O;
            }
            else if (CurrentPlayer == Player.O)
            {
                _pieces[column][row] = Piece.O;
                CurrentPlayer = Player.X;
            }
            else
            {
                throw new ArgumentException();
            }

            _moves.Push(new Move(this) { Column = column, Row = row });

            var evaluationResult = Evaluate(column, row);
            if (evaluationResult.HasWinningStraight())
            {
                _evaluationResult = evaluationResult;
                _state = BoardState.Finished;
                _subState = CurrentPlayer != Player.X ? BoardSubState.XWon : BoardSubState.OWon;
            }
            else if (_moves.Count == Width * Height)
            {
                // Board is full!
                _state = BoardState.Finished;
                _subState = BoardSubState.Draw;
            }
            else
            {
                _state = BoardState.Running;
                _subState = CurrentPlayer == Player.X ? BoardSubState.XTurn : BoardSubState.OTurn;
            }

            return evaluationResult;
        }

        public bool IsAvailable(int column, int row)
        {
            if (this.State == BoardState.Running)
            {
                return _pieces[column][row] == Piece.Empty;
            }
            return false;
        }

        public EvaluationResult[,] Evaluation()
        {
            var values = new EvaluationResult[Width, Height];
            for (var c = 0; c < Width; c++)
            {
                for (var r = 0; r < Height; r++)
                {
                    values[c, r] = Evaluate(c, r);
                }
            }

            return values;
        }

        public int[,] EvaluationAggregate()
        {
            var values = new int[Width, Height];
            var evaluations = Evaluation();
            for (var c = 0; c < Width; c++)
            {
                for (var r = 0; r < Height; r++)
                {
                    values[c, r] = evaluations[c, r].Max();
                }
            }

            return values;
        }

        public Move Undo()
        {
            if (_moves.Count > 0)
            {
                var move = _moves.Pop();
                _pieces[move.Column][move.Row] = Piece.Empty;
                if (CurrentPlayer == Player.X)
                {
                    CurrentPlayer = Player.O;
                }
                else if (CurrentPlayer == Player.O)
                {
                    CurrentPlayer = Player.X;
                }
                else
                {
                    throw new ArgumentException();
                }

                _state = BoardState.Running;
                _subState = CurrentPlayer == Player.X ? BoardSubState.XTurn : BoardSubState.OTurn;
                _evaluationResult = null;
                return move;
            }

            return null;
        }

        internal EvaluationResult Evaluate(int column, int row)
        {
            var playerPiece = GetPiece(column, row);
            var evaluationResult = new EvaluationResult(playerPiece);
            if (playerPiece == Piece.Empty)
            {
                return evaluationResult;
            }

            var opponentPiece = playerPiece == Piece.X ? Piece.O : Piece.X;
            var moves = new List<int[]>()
            {
                new int[] { 1, 0 }, // East
                new int[] { -1, 0 }, // West
                new int[] { 0, 1 }, // South
                new int[] { 0, -1 }, // North
                new int[] { 1, -1 }, // North-East
                new int[] { -1, 1 }, // South-West
                new int[] { -1, -1 }, // North-West
                new int[] { 1, 1 } // South-East
            };

            for (var i = 0; i < moves.Count; i += 2)
            {
                var evaluationValuePlayer = new EvaluationValue();
                evaluationValuePlayer.Indexes.Add(column + (row * Width));

                var evaluationValueOpponent = new EvaluationValue();
                evaluationValueOpponent.Indexes.Add(column + (row * Width));

                var move = moves[i];
                EvaluateScan(
                    ref evaluationValuePlayer,
                    column,
                    row,
                    move[0] /* columnStep */,
                    move[1] /* rowStep */,
                    playerPiece);

                EvaluateScan(
                    ref evaluationValueOpponent,
                    column,
                    row,
                    move[0] /* columnStep */,
                    move[1] /* rowStep */,
                    opponentPiece);

                move = moves[i + 1];
                EvaluateScan(
                    ref evaluationValuePlayer,
                    column,
                    row,
                    move[0] /* columnStep */,
                    move[1] /* rowStep */,
                    playerPiece);

                EvaluateScan(
                    ref evaluationValueOpponent,
                    column,
                    row,
                    move[0] /* columnStep */,
                    move[1] /* rowStep */,
                    opponentPiece);

                evaluationResult.Evaluations.Add(evaluationValuePlayer);

                evaluationValueOpponent.Value = -evaluationValueOpponent.Value;
                evaluationResult.Evaluations.Add(evaluationValueOpponent);
            }

            return evaluationResult;
        }

        internal void EvaluateScan(
            ref EvaluationValue evaluationValue,
            int column,
            int row,
            int columnStep,
            int rowStep,
            Piece piece)
        {
            var targetColumn = column + (columnStep * StraightLength);
            if (targetColumn < 0)
            {
                targetColumn = -1;
            }
            else if (targetColumn > Width)
            {
                targetColumn = Width;
            }

            var targetRow = row + (rowStep * StraightLength);
            if (targetRow < 0)
            {
                targetRow = -1;
            }
            else if (targetRow > Height)
            {
                targetRow = Height;
            }

            column += columnStep;
            row += rowStep;
            for (int c = column, r = row;
                (c != targetColumn || r != targetRow) &&
                (c >= 0 && c < Width && r >= 0 && r < Height);
                c += columnStep, r += rowStep)
            {
                if (GetPiece(c, r) == piece)
                {
                    evaluationValue.Value += 1;
                    evaluationValue.Indexes.Add(c + (r * Width));
                }
                else
                {
                    return;
                }
            }
        }

        private void CleanUp(Player player, int width, int heigth)
        {
            Width = width;
            Height = heigth;
            CurrentPlayer = player;
            FirstPlayer = player;
            _pieces = new Piece[Width][];
            _moves.Clear();

            _state = BoardState.Running;
            _subState = FirstPlayer == Player.X ? BoardSubState.XTurn : BoardSubState.OTurn;

            for (var column = 0; column < Width; column++)
            {
                _pieces[column] = new Piece[Height];
                for (var row = 0; row < Height; row++)
                {
                    _pieces[column][row] = Piece.Empty;
                }
            }
        }
    }
}
