using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.UIElements.Runtime;

using Interactions;
using Interactions.Dialogue;

using UnityEngine.InputSystem;

using Data;
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public InputAction move;
    public InputAction interact;
    public InputAction quit;

    public float speed;
    private InteractionsGraph activeInteraction;
    private InteractionContext context;
    public UIController UIController;

    private Rigidbody2D body;
    private HashSet<string> flags;
    private List<InteractionsGraph> interactables;
    public List<InteractionsGraph> onFlagChangeHandlers;
    private Vector2 start_pos;
    void Awake() {
        body = this.GetComponent<Rigidbody2D>();
        my_animator = this.GetComponent<Animator>();
        
        start_pos = this.transform.position;
        interact.started += OnInteract;
        interact.performed += OnEndInteracting;
        quit.performed += (ctx) => Application.Quit();
    }

    public void AddFlagChangeHandler(InteractionsGraph graph) {
        if (onFlagChangeHandlers == null) {
            onFlagChangeHandlers = new List<InteractionsGraph>();
        }
        onFlagChangeHandlers.Add(graph);
    }

    private List<InteractionsGraph> toRemoveHandlers;
    public void RemoveFlagChangeHandler(InteractionsGraph graph) {
        if (toRemoveHandlers == null) {
            toRemoveHandlers = new List<InteractionsGraph>();
        }
        toRemoveHandlers.Add(graph);
    }

    void OnEnable() {
        move.Enable();
        interact.Enable();
        quit.Enable();
    }
    void OnDisable() {
        move.Disable();
        interact.Disable();
        quit.Disable();
    }
    // Start is called before the first frame update
    void Start()
    {
        flags = new HashSet<string>();
        interactables = new List<InteractionsGraph>();
        context.complete = true;
        move_time = time_to_flip_to_target;
    }

    void UpdateFlagChangeListeners(HashSet<string> flags_added, HashSet<string> flags_removed) {
        foreach (string flag in flags_added) {
            Debug.Log($"FLAG ADDED {flag}");
        }
        foreach (string flag in flags_removed) {
            Debug.Log($"FLAG REMOVED {flag}");
        }
        if (onFlagChangeHandlers == null) {
            onFlagChangeHandlers = new List<InteractionsGraph>();
        }
        foreach (InteractionsGraph graph in onFlagChangeHandlers) {
            Debug.Log($"GRAPH NAMES: {graph.name}");
            foreach (string flag_added in flags_added) {
                InteractionEntryNode node = graph.GetEntryPoint($"FlagAdded{flag_added}");
                if (node != null) {
                    Debug.Log("STARTING FLAG ADD LISTENER");
                    graph.StartInteraction(node, flags, UIController, out var context);
                }
            }

            foreach (string flag_removed in flags_removed) {
                InteractionEntryNode node = graph.GetEntryPoint($"FlagRemoved{flag_removed}");
                if (node != null) {
                    Debug.Log("STARTING FLAG REMOVE LISTENER");
                    graph.StartInteraction(node, flags, UIController, out var context);
                }
            }
        }
    }

    private bool swung_sword;
    void OnInteract(InputAction.CallbackContext action_context) {
        Debug.Log("INTERACTING!");
        if (context.complete) {
            foreach (var interactions in interactables) {
                if (interactions == null) continue;
                if (interactions.gameObject == null) continue;
                foreach (var entry in interactions.GetEntryPoints()) {
                    if (entry.Action != "Interact") continue;
                    if (entry.FlagsMeetRequirements(flags)) {
                        // If we start the interaction, but it does not complete
                        // then we want to zero velocity as we will likely be in dialogue
                        if (!interactions.StartInteraction(
                            entry,
                            flags,
                            UIController,
                            out context
                        )) {
                            activeInteraction = interactions;
                            body.velocity = Vector2.zero;
                        }
                        UpdateFlagChangeListeners(context.flagsAdded, context.flagsRemoved);
                        return;
                    }
                    break;
                }
            }

            // Trigger animation for sword swing
            if (flags.Contains("keyboard")) {
                swung_sword = true;
                my_animator.SetTrigger("Slash");
            }

        } else {
            if (activeInteraction.FollowInteraction(ref context)) {
                activeInteraction = null;
                if (queue != null) {
                    while (queue.Count > 0) {
                        UpdateFlagChangeListeners(context.flagsAdded, context.flagsRemoved);
                        var next = queue[0];
                        queue.RemoveAt(0);
                        if (!next.Item1.StartInteraction(
                            next.Item2,
                            flags,
                            UIController,
                            out context
                        )) {
                            activeInteraction = next.Item1;
                            break;
                        }
                    }
                }
            }
            UpdateFlagChangeListeners(context.flagsAdded, context.flagsRemoved);
        }
    }

    void OnEndInteracting(InputAction.CallbackContext action_context) {
        if (swung_sword) {
            my_animator.SetTrigger("Sheath");
            swung_sword = false;
        }
    }

    public float time_to_flip_to_target = 1.0f;
    private float move_time = 1.0f;
    private Vector2 move_target_transform;
    private Vector2 move_start_pos;
    public void AnimateFlipTo(Transform trans) {
        move_time = 0;
        move_start_pos = this.transform.position;
        move_target_transform = trans.position;
    }

    Animator my_animator;

    // Update is called once per frame
    void Update()
    {
        if (move_time < time_to_flip_to_target) {
            move_time += Time.deltaTime;
            if (move_time >= time_to_flip_to_target) {
                move_time = time_to_flip_to_target;
            }
            float parameter = move_time / time_to_flip_to_target;
            this.transform.position = Vector2.Lerp(
                move_start_pos,
                move_target_transform,
                parameter
            );
            this.transform.rotation = Quaternion.Euler(360.0f * parameter, 0.0f, 360.0f * parameter);
        } else {
            if (context.complete) {
                body.velocity = speed * move.ReadValue<Vector2>();
                float currentSpeed = body.velocity.magnitude;
                if (currentSpeed > speed) {
                    body.velocity *= speed / currentSpeed;
                }
            }
            my_animator.SetFloat("Horizontal", body.velocity.x);
            my_animator.SetFloat("Vertical", body.velocity.y);
            if (body.velocity.magnitude > 0.001) {
                my_animator.SetFloat("Facing Horizontal", body.velocity.x < -0.5 ? -1 : body.velocity.x > 0.5 ? 1 : 0);
                my_animator.SetFloat("Facing Vertical", body.velocity.y < -0.5 ? -1 : body.velocity.y > 0.5 ? 1 : 0);
            }
        }

        if (toRemoveHandlers != null) {
            foreach (InteractionsGraph g in toRemoveHandlers) {
                onFlagChangeHandlers.Remove(g);
            }
            toRemoveHandlers = null;
        }
    }

    private List<(InteractionsGraph, InteractionEntryNode)> queue;
    void QueueInteraction(InteractionsGraph graph, InteractionEntryNode node) {
        if (context.complete) {
            if (!graph.StartInteraction(
                node,
                flags,
                UIController,
                out context
            )) {
                activeInteraction = graph;
            }
            UpdateFlagChangeListeners(context.flagsAdded, context.flagsRemoved);
        } else {
            if (queue == null) {
                queue = new List<(InteractionsGraph, InteractionEntryNode)>();
                queue.Add((graph, node));
            }
        }
    }
    public void TriggerHit(InteractionsGraph g) {
        if (g == null) return;
        var entry = g.GetEntryPoint("WasHit");
        if (entry != null) {
            QueueInteraction(g, entry);
        }
    }

    void OnTriggerEnter2D(Collider2D collider) {
        InteractionsGraph interactable = collider.GetComponent<InteractionsGraph>();
        if (interactable) {
            interactables.Add(interactable);
        }
    }

    void OnTriggerExit2D(Collider2D collider) {
        InteractionsGraph interactable = collider.GetComponent<InteractionsGraph>();
        if (interactable) {
            interactables.Remove(interactable);
        }
    }
}
