using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DefaultNamespace;
using Gameplay;
using UnityEngine;
using Random = System.Random;

public partial class DeckManager : MonoBehaviour
{
    [Header("Mandatory")] 
    [SerializeField] private GameObject cardSpawnRoot;
    [SerializeField] private GameObject dragAnchor;
    [SerializeField] private Hand hand;
    [SerializeField] private PlayArea playArea;
    [SerializeField] private DiscardPile discardPile;
    [SerializeField] private EndTurnButton endTurnButton;
    [Header("Optional")] 
    [SerializeField] private ParticleSystem particle;

    private CardsDatabase _cardDatabase;
    private GameSettings _gameSettings;

    private readonly DeckManagerAnimator _animations = new();
    private readonly List<CardModel> _deck = new();

    private void Start()
    {
        _cardsPool = GetComponent<CardsPool>().Pool;
        _cardDatabase = Game.Instance.CardDatabase;
        _gameSettings = Game.Instance.GameSettings;
        endTurnButton.Button.onClick.AddListener(EndTurn);
        InitializeDeck();
        ShuffleDeck();
        DrawStartingHand();
    }

    private void InitializeDeck()
    {
        var variousCardsCount = _cardDatabase.cardModels.Count;
        for (int i = 0; i < _gameSettings.initialDeckSize; i++)
        {
            _deck.Add(_cardDatabase.cardModels[i % variousCardsCount]);
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
        List<Card> newCards = new();
        for (int i = 0; i < _gameSettings.playerHandSize; i++)
        {
            var drawnCard = TryDrawCardToHand();
            if (drawnCard != null)
            {
                newCards.Add(drawnCard);
                hand.AddCardToHand(drawnCard);
            }
        }

        if (newCards.Count == 0)
        {
            Debug.LogError("No cards to start drawing! Specify correct amount in settings");
            return;
        }

        _animations.DrawCardsSequence(newCards, cardSpawnRoot.transform, () => EnableInput(false),
            () => EnableInput(true)).Forget();
    }

    private void OnPlayCard(Card c)
    {
        PlayCardSequence(c).Forget();
    }

    private void OnSwapCardPlaces(Card draggedCard, int draggedCardIdx, Card stationaryCard, int stationaryCardIdx)
    {
        OnSwapCardPlacesSequence(draggedCard, draggedCardIdx, stationaryCard, stationaryCardIdx).Forget();
    }

    private Card TryDrawCardToHand()
    {
        Card card = null;
        if (_deck.Count > 0)
        {
            CardModel cardData = _deck[0];
            _deck.RemoveAt(0);
            
            card = _cardsPool.Get();
            card.transform.SetParent(hand.transform);
            card.Setup(cardData, dragAnchor);
            card.OnPlayCard += OnPlayCard;
            card.OnSwapCardPlaces += OnSwapCardPlaces;
        }

        return card;
    }
    
    private void EndTurn()
    {
        EndTurnSequence().Forget();
    }

    private void EnableInput(bool value)
    {
        hand.Cards.ForEach(c => c.SetDraggable(value));
        endTurnButton.Button.interactable = value;
    }

    private void SetDrawDeckVisible(bool visible)
    {
        cardSpawnRoot.gameObject.SetActive(visible);
    }

    private void OnDestroy()
    {
        endTurnButton.Button.onClick.RemoveListener(EndTurn);
    }
}