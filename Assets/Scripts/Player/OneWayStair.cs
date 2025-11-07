using Guizan.Player;
using System;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Faz com que o jogador só colida com a escada se vier de cima.
/// A escada deve ter:
///  - Um collider físico (isTrigger = false)
///  - Um trigger de detecção (isTrigger = true)
/// O jogador deve ter um Rigidbody2D e um Collider2D.
/// </summary>
public class OneWayStair : MonoBehaviour
{
    [SerializeField] private Collider2D platformCollider; // Collider físico (colisão real)

    private Rigidbody2D playerRb;
    private Collider2D playerCol;

    PlayerMovement my_player;

    private void Start()
    {
        my_player = FindAnyObjectByType<PlayerMovement>();
        var playerCollider = my_player.GetComponent<Collider2D>();
        Physics2D.IgnoreCollision(playerCollider, platformCollider, true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerCol = other;
        playerRb = other.attachedRigidbody;

        // Ao entrar no trigger, decide se ignora a colisão
        Vector2 touchPoint = platformCollider.ClosestPoint(GetPlayerBoundsMin());
        CheckCollisionState(touchPoint);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        Vector2 touchPoint = platformCollider.ClosestPoint(GetPlayerBoundsMin());
        CheckCollisionState(touchPoint);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // Saiu da área da escada → desliga colisão
        Physics2D.IgnoreCollision(playerCol, platformCollider, true);
        playerCol = null;
        playerRb = null;
    }

    private Vector2 GetPlayerBoundsMin()
    {
        return new Vector2(
            playerCol.bounds.center.x,
            playerCol.bounds.min.y
        );
    }

    private void CheckCollisionState(Nullable<Vector2> contactPoint = null)
    {
        if (playerCol == null || playerRb == null) return;

        bool playerBelow = false;
        if(contactPoint != null)
        {
            playerBelow = contactPoint.Value.y > GetPlayerBoundsMin().y;
            Debug.DrawLine(contactPoint.Value, GetPlayerBoundsMin(), Color.green, 1f);
        }

        bool jumping = false;
        if (my_player != null)
        {
            jumping = playerRb.velocity.y > .05f || playerRb.velocity.y < -.05f || my_player.IsJumping || playerRb.IsSleeping();
        }

        bool ignore = !jumping || playerBelow;
        //Debug.Log($"below = {playerBelow}, jumping = {jumping}, ignore = {ignore}");

        if (my_player.DownPressed)
            ignore = true;

        Physics2D.IgnoreCollision(playerCol, platformCollider, ignore);

        if (!ignore && playerRb != null)
            GravityAdjust();
    }

    private void GravityAdjust()
    {
        if (my_player.DownPressed)
        {
            playerRb.WakeUp();
            return;
        }
        if(!my_player.IsMoving && my_player.OnFloor)
        {
            playerRb.velocity = Vector2.zero;
            playerRb.Sleep();
            return;
        }
        playerRb.WakeUp();
    }
}
