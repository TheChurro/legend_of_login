using UnityEngine;

using Interactions;
public class KeyboardHit : MonoBehaviour
{
    private PlayerController player;
    void Awake() {
        player = this.transform.parent.gameObject.GetComponent<PlayerController>();
    }

    void OnTriggerEnter2D(Collider2D collider) {
        Debug.Log("Trigger entered");
        InteractionsGraph interactable = collider.GetComponent<InteractionsGraph>();
        if (interactable) {
            player.TriggerHit(interactable);
        }
    }
}
