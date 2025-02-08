using DefaultNamespace;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class QuitButton : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(Quit);
    }

    private void Quit()
    {
        Game.Instance.Quit();
    }
}