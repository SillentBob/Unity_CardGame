using System.Collections.Generic;

namespace DefaultNamespace
{
    public class DiscardMultipleCardsToPileEvent: BaseEvent
    {
        public readonly List<Card> DiscardedCards;
        
        public DiscardMultipleCardsToPileEvent(List<Card> cards)
        {
            DiscardedCards = new(cards);
        }
    }
}