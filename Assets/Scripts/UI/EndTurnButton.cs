using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class EndTurnButton : MonoBehaviour
{
    public Button Button => _button;
    private Button _button;

    private void Start()
    {
        _button = GetComponent<Button>();
    }
}