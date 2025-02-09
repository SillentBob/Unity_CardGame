using System.Collections.Generic;
using UnityEngine;

public class DiscardPile : MonoBehaviour
{
    private readonly List<Card> _discardedCards = new();

    public void AddCardToPile(List<Card> cards)
    {
        cards.ForEach(AddCardToPile);
    }

    public void AddCardToPile(Card c)
    {
        _discardedCards.Add(c);
        c.transform.SetParent(transform);
        c.gameObject.SetActive(false); //TODO play animation moving to discard pile, then deactivate and move to pool
    }
}