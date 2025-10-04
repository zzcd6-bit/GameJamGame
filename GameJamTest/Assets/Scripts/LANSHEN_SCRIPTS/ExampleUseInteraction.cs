using UnityEngine;

/*
 * Created By LANSHEN
 * 2025-10-4
 * This is an example use.
 */
namespace LANSHEN_SCRIPTS
{
    public class ExampleUseInteraction : InteractionBaseObject
    {
        public override void Interaction()
        {
            Debug.Log(gameObject.name);
        }
    }
}
