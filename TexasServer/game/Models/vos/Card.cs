
namespace Texas.Game.Models.vos
{
    public class Card
    {
        public int id { get; }
        public int attribute { get; }
        public int index { get; }

        public Card(int id, int attribute, int index)
        {
            this.id = id;
            this.attribute = attribute;
            this.index = index;
        }
    }
}
