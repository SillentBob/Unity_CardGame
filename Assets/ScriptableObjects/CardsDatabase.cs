using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardsDatabase", menuName = "ScriptableObjects/CardsDatabase", order = 1)]
public class CardsDatabase : ScriptableObject
{
    public List<CardModel> cardModels = new();
}
