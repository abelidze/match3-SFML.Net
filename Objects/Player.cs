namespace Match3.Objects
{
    public sealed class Player : Actor
    {
        public Player(float x, float y, float maxHealth = 100f) : base("overlord", x, y, maxHealth)
        {
            // ...
        }
    }
}