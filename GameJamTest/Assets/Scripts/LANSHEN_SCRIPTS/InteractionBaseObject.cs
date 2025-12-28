using UnityEngine;

/*
 * Created By LANSHEN
 * 2025-10-4
 * This is a base class extends MonoBehaviour.
 * If you want influences something when player is collision enter to the shadow provided by this object
 * You can create a new class extends the InteractionBaseObject.
 * Then overdrive function Interaction()
 * Add the behavior script to the object.
 */
namespace LANSHEN_SCRIPTS
{
    public class InteractionBaseObject : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        /// <summary>
        /// this function will invoke when player is collision enter to the shadow this object provided.
        /// Don’t write code here!
        /// Don't just use the base class!
        /// If you want to use this function,create a new class extends this class.
        /// </summary>
        public virtual void Interaction()
        {

        }
    }
}
