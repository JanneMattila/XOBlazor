using System.Collections.Generic;

namespace XO
{
    public class BoardData
    {
        public BoardData() => Rules = new List<string>();

        public int Version { get; set; }

        /// <summary>
        /// Gets or sets size as string using "x" separator.
        /// </summary>
        /// <example>10x10</example>
        /// <example>19x19</example>
        /// <example>20x20</example>
        /// <example>100x100</example>
        public string Size { get; set; }

        /// <summary>
        /// Gets or sets list of rules in this games.
        /// </summary>
        /// <example>Basic</example>
        /// <example>Swap2</example>
        public List<string> Rules { get; set; }

        /// <summary>
        /// Gets or sets 'x' or 'o' indicating player who started game.
        /// </summary>
        public char FirstPlayer { get; set; }

        /// <summary>
        /// Gets or sets 'x' or 'o' indicating players turn.
        /// </summary>
        public char CurrentPlayer { get; set; }

        /// <summary>
        /// Gets or sets board serialized as string.
        /// </summary>
        public string Board { get; set; }

        public List<int> Moves { get; set; }

        public List<int> WinningStraight { get; set; }

        public string State { get; set; }

        public string StateReason { get; set; }
    }
}
