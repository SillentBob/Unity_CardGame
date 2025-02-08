using System.Collections.Generic;
using System.Linq;
using System.Text;
using DefaultNamespace;
using UnityEngine;
using Random = System.Random;

public class DeckManager : MonoBehaviour
{
    [Header("Mandatory")]
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private GameObject dragAnchor;
    [SerializeField] private Hand hand;
    [Header("Optional")]
    [SerializeField] private ParticleSystem particle;

    private CardsDatabase _cardDatabase;
    private GameSettings _gameSettings;

    private readonly List<CardModel> _deck = new();

    private void Awake()
    {
        EventManager.AddListener<EndTurnEvent>(OnEndTurnEvent);
        EventManager.AddListener<PlayCardEvent>(OnPlayCardEvent);
    }

    private void Start()
    {
        _cardDatabase = Game.Instance.CardDatabase;
        _gameSettings = Game.Instance.GameSettings;
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

    private void OnEndTurnEvent(EndTurnEvent e)
    {
        EndTurn();
    }
    
    private void OnPlayCardEvent(PlayCardEvent e)
    {
        if (particle == null)
        {
            return;
        }
        particle.gameObject.SetActive(true);
        particle.Stop();
        particle.Play();
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
        for (int i = 0; i < _gameSettings.playerHandSize; i++)
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

            GameObject cardObject = Instantiate(cardPrefab, hand.transform);
            Card card = cardObject.GetComponent<Card>();
            card.Setup(cardData, dragAnchor);
            // _hand.AddCardToHand(card);
            EventManager.Invoke(new AddCardToHandEvent(card));
        }
        else
        {
            Debug.Log("Deck to draw from is empty!");
        }
    }


    private void EndTurn()
    {
        // playArea.RemoveAllCards();
        EventManager.Invoke(new RemoveCardsFromPlayAreaEvent());

        int cardsToDraw = _gameSettings.playerHandSize - hand.GetCardCount();
        for (int i = 0; i < cardsToDraw; i++)
        {
            DrawCard();
        }
    }

    private void OnDestroy()
    {
        EventManager.RemoveListener<EndTurnEvent>(OnEndTurnEvent);
        EventManager.RemoveListener<PlayCardEvent>(OnPlayCardEvent);
    }
}