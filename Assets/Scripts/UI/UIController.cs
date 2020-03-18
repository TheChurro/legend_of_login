using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.UIElements.Runtime;
using UnityEngine.UIElements;
using Interactions;
using Interactions.Dialogue;
using BlueGraph;
using UnityEngine.InputSystem;
public class UIController : MonoBehaviour
{
    public PanelRenderer UIRenderer;
    private ListView ChoiceList;
    private VisualElement ChoicePanel;
    private Label ChoiceLabel;
    private VisualElement DialoguePanel;
    private Label NameLabel;
    private VisualElement PortraitImage;
    private VisualElement PortraitContainer;
    private Label DialogueLabel;
    private List<string> Choices;

    public InputAction up;
    public InputAction down;

    void Awake() {
        up.started += (context) => UpChoice();
        down.started += (context) => DownChoice();
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        Choices = new List<string>();
        UIRenderer.postUxmlReload = BindUI;
        up.Enable();
        down.Enable();
    }

    void OnDisable() {
        up.Disable();
        down.Disable();
    }

    private int current_choice;
    public void UpdateChoices(IEnumerable<string> newChoices) {
        Choices.Clear();
        Choices.AddRange(newChoices);
        current_choice = 0;
        if (ChoiceList != null) {
            ChoiceList.style.height = Mathf.Min(
                ChoiceList.itemHeight * 3,
                ChoiceList.itemHeight * Choices.Count
            );
            ChoiceList.Refresh();
            ChoiceList.ScrollToItem(current_choice);
        }
    }

    private bool ui_loaded;
    public bool UIReady { get { return ui_loaded; } }

    private IEnumerable<Object> BindUI() {
        UIRenderer.visualTree.style.display = DisplayStyle.Flex;
        UIRenderer.visualTree.style.visibility = Visibility.Visible;
        UIRenderer.enabled = true;

        ChoicePanel = UIRenderer.visualTree.Q<VisualElement>("choice-panel");
        ChoiceList = UIRenderer.visualTree.Q<ListView>("choice-list");
        if (ChoiceList != null) {
            ChoiceList.selectionType = SelectionType.Single;
            // Hide the vertical scroll bar...
            var scrollView = ChoiceList.Q<ScrollView>();
            if (scrollView != null) {
                scrollView.verticalScroller.style.display = DisplayStyle.None;
            }
            ChoiceList.itemsSource = Choices;
            ChoiceList.makeItem = MakeChoice;
            ChoiceList.bindItem = BindChoice;
            UpdateChoices(new string[]{});
        }
        ChoiceLabel = UIRenderer.visualTree.Q<Label>("choice-preview");

        DialoguePanel = UIRenderer.visualTree.Q<VisualElement>("dialogue-panel");
        NameLabel = UIRenderer.visualTree.Q<Label>("character-name");
        PortraitContainer = UIRenderer.visualTree.Q<VisualElement>("portrait-container");
        PortraitImage = UIRenderer.visualTree.Q<VisualElement>("character-portrait");
        DialogueLabel = UIRenderer.visualTree.Q<Label>("dialogue");
        
        ChoicePanel.visible = false;
        DialoguePanel.visible = false;
        NameLabel.visible = false;
        PortraitContainer.style.display = DisplayStyle.None;
        DialogueLabel.text = "This is some basic dialogue text. It is nice to have, right?";
        
        ui_loaded = true;
        // TESTING!
        // ShowChoices();
        return null;
    }

    public VisualTreeAsset ListItemTemplate;
    public VisualElement MakeChoice() {
        return ListItemTemplate.CloneTree();
    }

    public void BindChoice(VisualElement element, int index) {
        var display = element.Q<Label>("text");
        display.text = Choices[index];
    }

    private DialogueNode dialogue_node;
    private DialogueChoiceNode choice_node;
    private List<int> valid_choices;
    private List<string> choice_responses;
    public void Hide() {
        DialoguePanel.visible = false;
        PortraitContainer.visible = false;
        NameLabel.visible = false;
        ChoicePanel.visible = false;
        ChoiceLabel.visible = false;
        UpdateChoices(new string[0]);
        dialogue_node = null;
        choice_node = null;
    }
    public void Show(DialogueNode node, HashSet<string> flag_set) {
        dialogue_node = node;
        ShowDialogue();
        if (node is DialogueChoiceNode) {
            choice_node = node as DialogueChoiceNode;
            ShowChoices(flag_set);
        }
    }

    private void ShowCharacter() {
        if (dialogue_node.Character != null) {
            NameLabel.text = dialogue_node.Character.name;
            NameLabel.style.color = dialogue_node.Character.color;
            NameLabel.visible = true;
            if (dialogue_node.Character.portrait != null) {
                PortraitImage.style.backgroundImage = new StyleBackground(dialogue_node.Character.portrait);
                PortraitContainer.visible = true;
            }
            else
            {
                PortraitContainer.visible = false;
            }
        }
        else
        {
            NameLabel.visible = false;
        }
    }
    private void ShowChoices(HashSet<string> flag_set) {
        if (choice_node.answers.Count > 0) {
            valid_choices = new List<int>();
            var new_choices = new List<string>();
            choice_responses = new List<string>();
            for (int i = 0; i < choice_node.answers.Count; i++) {
                Port port = choice_node.GetPort($"answers[{i}]");
                if (port.IsConnected && port.ConnectedPorts[0].node is InteractionNode) {
                    if ((port.ConnectedPorts[0].node as InteractionNode).FlagsMeetRequirements(flag_set)) {
                        valid_choices.Add(i);
                        var answer = choice_node.answers[i];
                        new_choices.Add(answer.list_text);
                        if (answer.unique_preview) {
                            choice_responses.Add(answer.preview);
                        } else if (answer.show_preview && port.ConnectedPorts[0].node is DialogueNode) {
                            choice_responses.Add((port.ConnectedPorts[0].node as DialogueNode).Dialogue);
                        } else {
                            choice_responses.Add(null);
                        }
                    }
                } else {
                    valid_choices.Add(i);
                    var answer = choice_node.answers[i];
                    new_choices.Add(answer.list_text);
                    if (answer.show_preview) {
                        choice_responses.Add(answer.preview);
                    } else {
                        choice_responses.Add(null);
                    }
                }
            }
            if (valid_choices.Count > 0) {
                ChoicePanel.visible = true;
                UpdateChoices(new_choices);
                current_choice = 0;
                ScrollAndShowChoice();
                return;
            }
        }
        ChoicePanel.visible = false;
        choice_node = null;
    }

    private void ShowDialogue() {
        DialoguePanel.visible = true;
        ShowCharacter();
        DialogueLabel.text = dialogue_node.Dialogue;
    }

    private void ScrollAndShowChoice() {
        ChoiceList.ScrollToItem(current_choice);
        ChoiceList.selectedIndex = current_choice;
        if (choice_responses[current_choice] != null) {
            ChoiceLabel.visible = true;
            ChoiceLabel.text = choice_responses[current_choice];
        } else {
            ChoiceLabel.visible = false;
        }
    }
    public void DownChoice() {
        if (choice_node == null) return;
        current_choice++;
        if (current_choice >= Choices.Count) {
            current_choice = Choices.Count - 1;
        } else {
            ScrollAndShowChoice();
        }
    }

    public void UpChoice() {
        if (choice_node == null) return;
        current_choice--;
        if (current_choice < 0) {
            current_choice = 0;
        } else {
            ScrollAndShowChoice();
        }
    }

    public bool IsChoice() {
        return choice_node != null && valid_choices.Count > 0;
    }

    public InteractionNode Select() {
        InteractionNode nextNode = null;
        if (choice_node != null) {
            nextNode = choice_node.GetNextNodeForChoice(valid_choices[current_choice]);
        } else {
            nextNode = dialogue_node.GetNextNode();
        }
        Hide();
        return nextNode;
    }
}
