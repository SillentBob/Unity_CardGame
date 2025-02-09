using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public List<Card> Cards => _cardsInHand;
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