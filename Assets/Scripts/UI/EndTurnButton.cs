using DefaultNamespace;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class EndTurnButton : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(EndTurn);
    }

    private void EndTurn()
    {
        EventManager.Invoke(new EndTurnEvent());
    }
}