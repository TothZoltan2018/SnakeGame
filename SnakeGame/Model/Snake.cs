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
            //ilyenkor (a jatek elejen) nincs megjelenitve semmi, igy nincs ilyen adatunk, ezert a null
            HeadPosition = new CanvasPosition(rowPosition, columnPosition, null);
            Heading = SnakeHeadingEnum.InPlace;
            Length = 6;
            //gondoskodom arrol, hogy a lista valtozom listat tartalmazzon, nehogy 
            //object reference null kivetel tortenjen
            //https://netacademia.blog.hu/2017/05/30/miert_ne_hasznaljunk_null-t
            Tail = new List<CanvasPosition>();
        }

        //Tudnia kell, hogy hol van a feje
        public CanvasPosition HeadPosition { get; set; }

        //Tudnia kell, hogy merre megy eppen
        public SnakeHeadingEnum Heading { get; set; }

        //Tudnia kell, hogy milyen hosszu
        /// <summary>
        /// Ehhez nyilvan kell tartanunk a kigyo farkanak pontjait egy listaban
        /// </summary>
        public List<CanvasPosition> Tail { get; set; }

        public int Length { get; set; }
        

        //Ket property: a kigyo farok vege es a nyaka, ami az eleje.


    }
}
