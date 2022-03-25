using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoomAllocation
{
    /// <summary>
    /// Represents a door in an archetype (not a room)
    /// </summary>
    public class Door : MonoBehaviour
    {
        public bool Open = false;

        ///<summary>If the door can lead to another room</summary>
        public bool IsDeadEnd = false;

        ///<summary>The  archetype the door leads to</summary>
        public RoomArchetype ArchetypeAssignedTo = null;

        ///<summary>The archetype that owns this door</summary>
        public RoomArchetype Owner = null;

        ///<summary>The doorpoint in the room associated with this door</summary>
        public DoorPoint AssociatedDoorPoint = null;

        ///<summary>The door in the assigned archetype that relates to this door</summary>
        public GameObject DoorMirror = null;

        ///<summary>All archetype names that are not viable for this door</summary>
        public List<string> UnviableArchetypeNames = new List<string>();

        ///<summary>Performs all necessary actions to open a door.
        ///Sets Open to true in this door and its mirror.
        ///Forces the associated game object to be active.
        ///Starts the animation coroutine
        ///</summary>
        public void OpenDoor()
        {
            if (AssociatedDoorPoint != null)
            {
                //Set the door mirror open also
                if (DoorMirror != null)
                {
                    Door door = DoorMirror.GetComponent<Door>();
                    door.Open = false;
                }

                //Set the door as open
                Open = true;
                //Enforce that the door is active in the hierarchy
                if (!gameObject.activeInHierarchy)
                    gameObject.SetActive(true);
                //Start the animation coroutine
                StartCoroutine(AssociatedDoorPoint.DoorAnimator.AnimateOpen(this));
            }
        }

        ///<summary>Performs all necessary actions to closes a door.
        ///Sets Open to false in this door and its mirror.
        ///Forces the associated game object to be active.
        ///Starts the animation coroutine
        ///</summary>
        public void CloseDoor()
        {
            if (AssociatedDoorPoint != null)
            {
                //Set the door mirror closed also
                if (DoorMirror != null)
                {
                    Door door = DoorMirror.GetComponent<Door>();
                    door.Open = false;
                }

                //Set the door as closed
                Open = false;

                //Enforce that the door is active in the hierarchy
                if (!gameObject.activeInHierarchy)
                    gameObject.SetActive(true);

                //Disable rendering of the attached archetype, as no longer need it rendered
                if (ArchetypeAssignedTo != null)
                    ArchetypeAssignedTo.DisableRendering();
                //Start the animation coroutine
                StartCoroutine(AssociatedDoorPoint.DoorAnimator.AnimateClose(this));
            }
        }
    }
}