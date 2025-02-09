namespace DefaultNamespace
{
    public class PlayCardEvent: BaseEvent
    {
        public readonly Card PlayedCard;
        
        public PlayCardEvent(Card c)
        {
            PlayedCard = c;
        }
    }
}