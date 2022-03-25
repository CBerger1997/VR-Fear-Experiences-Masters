using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoomAllocation
{
    /// <summary>
    /// A point in a room that holds an unmasked and masked door, the physical manifestation of archetype doors
    /// </summary>
    public class DoorPoint : MonoBehaviour
    {
        /// <summary>
        /// The object that represents when a door is active in the hierarchy
        /// </summary>
        public GameObject unmaskedDoor;

        /// <summary>
        /// The object that represents when a door is inactive in the hierarchy
        /// , likely when it is a dead end
        /// </summary>
        public GameObject maskedDoor;

        /// <summary>
        /// The archetype door this doorpoint is associated with
        /// </summary>
        public GameObject AssociatedDoor { get; set; }

        /// <summary>
        /// Allows for opening and closing animations
        /// </summary>
        public AnimateDoor DoorAnimator { get; set; }

        public void Awake()
        {
            DoorAnimator = GetComponent<AnimateDoor>();
        }

        /// <summary>
        /// Disables the unmaskedDoor, enables the maskedDoor
        /// </summary>
        public void SetAsDeadEnd()
        {
            maskedDoor.SetActive(true);
            unmaskedDoor.SetActive(false);
        }

        /// <summary>
        /// Disables both the masked and unmasked doors
        /// </summary>
        public void SetAsOpenDoor()
        {
            maskedDoor.SetActive(false);
            unmaskedDoor.SetActive(false);
        }

        /// <summary>
        /// Disables the masked door, enables the unmasked door
        /// </summary>
        public void SetAsClosedDoor()
        {
            maskedDoor.SetActive(false);
            unmaskedDoor.SetActive(true);
        }
    }
}