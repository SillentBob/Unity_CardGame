namespace DefaultNamespace
{
    public class AddCardToHandEvent: BaseEvent
    {
        public readonly Card Card;
        
        public AddCardToHandEvent(Card c)
        {
            Card = c;
        }
    }
}