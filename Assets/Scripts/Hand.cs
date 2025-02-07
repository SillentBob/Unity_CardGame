using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    private List<Card> _cardsInHand = new();

    public void AddCardToHand(Card card)
    {
        _cardsInHand.Add(card);
    }

    public void RemoveCardFromHand(Card card)
    {
        _cardsInHand.Remove(card);
    }

    public int GetCardCount()
    {
        return _cardsInHand.Count;
    }

}