using UnityEngine;

namespace LANSHEN_SCRIPTS
{
    public class DialogueInteractionObject : InteractionBaseObject
    {
        [Header("对话设置")]
        public string itemName;

        [Header("位置偏移设置")]
        public float xOffset = 10f;  // X轴偏移量
        public float yOffset = 60f;  // Y轴偏移量

        [Header("交互设置")]
        public bool oneTimeOnly = true;

        private bool hasInteracted = false;

        public override void Interaction()
        {
            if (hasInteracted && oneTimeOnly) return;
            
            TriggerDialogue();
            OnInteract();
            
            hasInteracted = true;
        }

        protected virtual void OnInteract()
        {
            // Override in child classes for specific behavior
        }

        void TriggerDialogue()
        {
            if (DialogueManager.Instance != null && DialogueData.Instance != null)
            {
                // Freeze player during dialogue
                if (PlayerManager.instance != null)
                {
                    PlayerManager.instance.freezed = true;
                }
                
                string[] dialogueLines = DialogueData.Instance.GetDialogue(itemName);
                DialogueManager.Instance.StartDialogue(dialogueLines, transform, xOffset, yOffset);
            }
        }
    }
}