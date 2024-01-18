using UnityEngine;

namespace Core.Scripts
{
    public class Tile
    {
        public Vector3Int Position;
        public Fruit Fruit;
        
        public Tile(Vector3Int position)
        {
            Position = position;
        }

        public void SetFruit(Fruit fruit)
        {
            Fruit = fruit;
        }
    }
}