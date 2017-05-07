using UnityEngine;

/// <summary>
/// Routes control from player input to hover engines
/// </summary>
public class HoverControl : MonoBehaviour
{
    /// <summary>
    /// Turning script
    /// </summary>
    public HoverOrientation Orientation;
    /// <summary>
    /// Movement script
    /// </summary>
    public MovementEngine Movement;

    // To tell which player this is
    public MyPlayer myPlayer;
    // To tell which team this player belongs
    public Team myTeam;

    private void Start()
    {
     
    }

    void Update()
    {
        float vertical = Input.GetAxis("Vertical" + myPlayer.ToString());
        float horizontal = Input.GetAxis("Horizontal" + myPlayer.ToString());

        if (vertical < 0)
        {
            // Invert the horizontal movement if moving in reverse
            horizontal *= -1;
        }

        Movement.Thrust = vertical;
        Orientation.Turn = horizontal;
    }
}
