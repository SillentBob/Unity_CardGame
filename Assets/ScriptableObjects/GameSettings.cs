using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "ScriptableObjects/GameSettings", order = 0)]
public class GameSettings : ScriptableObject
{
    public int playerHandSize = 5;
    public int initialDeckSize = 11;
}
