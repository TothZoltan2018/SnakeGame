using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeGame.Model
{
    /// <summary>
    /// Az iranyitott kigyot reprezentalo osztaly
    /// </summary>
    class Snake
    {
        public Snake(int rowPosition, int columnPosition)
        {
            HeadPosition = new ArenaPosition(rowPosition, columnPosition);
            Heading = SnakeHeadingEnum.InPlace;
        }

        //Tudnia kell, hogy hol van a feje
        public ArenaPosition HeadPosition { get; set; }

        //Tudnia kell, hogy merre megy eppen
        public SnakeHeadingEnum Heading { get; set; }

        //Tudnia kell, hogy milyen hosszu
        /// <summary>
        /// Ehhez nyilvan kell tartanunk a kigyo farkanak pontjait egy listaban
        /// </summary>
        public List<ArenaPosition> Tail { get; set; }

        //Ket property: a kigyo farok vege es a nyaka, ami az eleje.


    }
}
