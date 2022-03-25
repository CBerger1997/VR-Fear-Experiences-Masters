using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoomAllocation
{
    /// <summary>
    /// Allows for the animation of doors when opening or closing
    /// </summary>
    public class AnimateDoor : MonoBehaviour
    {
        [Tooltip("The hinge by which the door will rotate in the y axis")]
        public GameObject hinge;

        [Tooltip("The object within the door frame if defined")]
        public GameObject innerDoorObject;

        [Tooltip("Whether the door is rendered or not when opened")]
        public bool disableOnOpen = false;

        /// <summary>
        /// Animates a door open
        /// </summary>
        /// <returns>The IEnumerator for the coroutine (yield return null)</returns>
        public IEnumerator AnimateOpen(Door associatedDoor)
        {
            if (disableOnOpen)
                innerDoorObject.SetActive(false);
            else
            {
                //INSERT OPEN ANIMATION HERE
            }

            yield return null;
        }

        /// <summary>
        /// Animates a door closed
        /// </summary>
        /// <returns>The IEnumerator for the coroutine (yield return null)</returns>
        public IEnumerator AnimateClose(Door associatedDoor)

        {
            if (disableOnOpen)
                innerDoorObject.SetActive(true);
            else
            {
                //INSERT OPEN ANIMATION HERE
            }

            yield return null;
        }
    }
}