using System.Collections.Generic;
using UnityEngine;

public class DiscardPile : MonoBehaviour
{
    public List<Card> DiscardedCards => _discardedCards;
    private readonly List<Card> _discardedCards = new();

    public void AddCardToPile(List<Card> cards)
    {
        cards.ForEach(AddCardToPile);
    }

    private void AddCardToPile(Card c)
    {
        _discardedCards.Add(c);
        c.transform.SetParent(transform, true);
    }

    public void DeactivateCardsOnPile()
    {
        _discardedCards.Clear();
    }
}