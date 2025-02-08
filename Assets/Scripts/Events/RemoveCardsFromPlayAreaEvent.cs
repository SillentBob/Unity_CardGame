namespace DefaultNamespace
{
    public class DiscardCardToPileEvent : BaseEvent
    {
        public readonly Card DiscardedCard;
        
        public DiscardCardToPileEvent(Card c)
        {
            DiscardedCard = c;
        }
    }
}