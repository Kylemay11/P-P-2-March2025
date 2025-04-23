using System.Collections;
using UnityEngine;

public class PuzzleButton : MonoBehaviour
{
    public PuzzleColor color;
    public RepairConsole targetConsole;

    private Renderer buttonRenderer;
    private Color originalColor;
    public float flashTime;

    private bool isAnimating;
    void Start()
    {
        buttonRenderer = GetComponentInChildren<Renderer>();
        if (buttonRenderer != null)
        {
            originalColor = buttonRenderer.material.color;
        }
    }

    void Update()
    {
        if (PlayerIsClose() && Input.GetKeyDown(KeyCode.E) && isAnimating == false)
        {
            targetConsole.SubmitColor(color);
            StartCoroutine(PressDownAnimation());
            StartCoroutine(FlashButton());
           
        }
    }

    private bool PlayerIsClose()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return false;

        return Vector3.Distance(player.transform.position, transform.position) <= 2.5f;
    }

    private IEnumerator FlashButton()
    {
        if (buttonRenderer != null)
        {
            buttonRenderer.material.color = Color.clear;
            yield return new WaitForSeconds(flashTime);
            buttonRenderer.material.color = originalColor;
        }
    }
    private IEnumerator PressDownAnimation()
    {
        isAnimating = true;

        Vector3 originalPos = transform.localPosition;
        Vector3 pressedPos = originalPos + new Vector3(0f, -0.1f, 0f);

        transform.localPosition = pressedPos;
        yield return new WaitForSeconds(0.3f);
        transform.localPosition = originalPos;
        isAnimating = false;
    }
}