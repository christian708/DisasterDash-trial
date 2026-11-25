using UnityEngine;

/// <summary>
/// Temporarily slows the player, then restores normal speed.
/// Added to the Player automatically by Apply(), no setup needed.
/// </summary>
public class PlayerSlowEffect : MonoBehaviour
{
    private PlayerController controller;
    private float timeLeft;

    public static void Apply(GameObject player, float multiplier, float duration)
    {
        PlayerSlowEffect effect = player.GetComponent<PlayerSlowEffect>();
        if (effect == null) effect = player.AddComponent<PlayerSlowEffect>();
        effect.Begin(multiplier, duration);
    }

    private void Begin(float multiplier, float duration)
    {
        if (controller == null) controller = GetComponent<PlayerController>();
        if (controller == null) return;

        controller.SetSpeedMultiplier(multiplier);
        timeLeft = Mathf.Max(timeLeft, duration); // a second hit refreshes, never stacks
    }

    private void Update()
    {
        if (timeLeft <= 0f) return;

        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0f && controller != null)
        {
            controller.SetSpeedMultiplier(1f);
        }
    }
}