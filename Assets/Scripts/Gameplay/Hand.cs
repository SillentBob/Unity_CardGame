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
    
    public void ReorderCard(Card card, int oldIndex, int newIndex)
    {
        _cardsInHand.RemoveAt(oldIndex-1);  //Remove from the old index.
        _cardsInHand.Insert(newIndex-1, card); //Insert card into the new index.
        //card.transform.SetParent(this.transform); // ensure that it's in the hand.
        card.transform.SetSiblingIndex(newIndex);
    }
    
}