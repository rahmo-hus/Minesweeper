using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper.Models
{
    class Tile
    {
        public bool IsMine { get; set; }
        public bool IsClicked { get; set; }
        public bool IsFlagged { get; set; }

        public bool IsAmbigous { get; set; }

        //row where tile lays
        public int Row { get; set; }
        public int Column { get; set; }

        //list of 8 tiles around of center tile
        public List<Tile> Surroundings { get; set; }

        public int MinesAround => Surroundings.Count(x => x.IsMine);

        public Tile(int row, int column)
        {
            Surroundings = new List<Tile>();
            Row = row;
            Column = column;
        }
    }
}
