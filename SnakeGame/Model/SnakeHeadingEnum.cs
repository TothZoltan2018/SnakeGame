namespace SnakeGame.Model
{
    /// <summary>
    /// Ez a tipus jelzi a kigyo fejenek az iranyat
    /// </summary>
    enum SnakeHeadingEnum
    {
        Up, Down, Left, Right,
        /// <summary>
        /// A jatek kezdeten a kigyo nem mozog
        /// </summary>
        InPlace
    }
}
