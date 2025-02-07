using System.Collections.Generic;
using System.Linq;
using System.Text;
using Const;
using UnityEngine;
using Random = System.Random;

public class DeckManager : MonoBehaviour
{
    [SerializeField] private CardsDatabase cardDatabase;
    [SerializeField] private GameSettings gameSettings;
    [SerializeField] private GameObject cardPrefab;

    private Hand _hand;

    private readonly List<CardModel> _deck = new();

    private void Start()
    {
        _hand = GameObject.FindGameObjectWithTag(GameObjectTags.PLAYER_HAND).GetComponent<Hand>();
        InitializeDeck();

#if UNITY_EDITOR
        StringBuilder deckString = new();
        Debug.Log($"Deck before shuffle: {deckString.AppendJoin(',', _deck.Select(model => model.name))}");
#endif

        ShuffleDeck();

#if UNITY_EDITOR
        deckString.Clear();
        Debug.Log($"Deck before shuffle: {deckString.AppendJoin(',', _deck.Select(model => model.name))}");
#endif

        DrawStartingHand();
    }

    private void InitializeDeck()
    {
        var variousCardsCount = cardDatabase.cardModels.Count;
        for (int i = 0; i < gameSettings.initialDeckSize; i++)
        {
            _deck.Add(cardDatabase.cardModels[i % variousCardsCount]);
        }
    }

    private void ShuffleDeck()
    {
        Random rng = new Random();
        int n = _deck.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (_deck[k], _deck[n]) = (_deck[n], _deck[k]); //swap
        }
    }

    private void DrawStartingHand()
    {
        for (int i = 0; i < gameSettings.playerHandSize; i++)
        {
            DrawCard();
        }
    }

    private void DrawCard()
    {
        if (_deck.Count > 0)
        {
            CardModel cardData = _deck[0];
            _deck.RemoveAt(0);

            GameObject cardObject = Instantiate(cardPrefab, _hand.transform);
            Card card = cardObject.GetComponent<Card>();
            card.Setup(cardData);
            _hand.AddCardToHand(card);
        }
        else
        {
            Debug.Log("Deck to draw from is empty!");
        }
    }

    private void EndTurn()
    {
        int cardsToDraw = gameSettings.playerHandSize - _hand.GetCardCount();
        for (int i = 0; i < cardsToDraw; i++)
        {
            DrawCard();
        }
    }
}