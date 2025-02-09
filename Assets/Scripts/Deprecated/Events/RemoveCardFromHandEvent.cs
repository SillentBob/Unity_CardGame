namespace DefaultNamespace
{
    public class RemoveCardFromHandEvent: BaseEvent
    {
        public readonly Card Card;
        
        public RemoveCardFromHandEvent(Card c)
        {
            Card = c;
        }
    }
}