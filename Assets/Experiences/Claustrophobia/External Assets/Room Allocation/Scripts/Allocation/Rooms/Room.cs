using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoomAllocation
{
    /// <summary>
    /// Represents and manages functions pertaining to rooms
    /// </summary>
    public class Room : MonoBehaviour
    {
        public Color cornerGizmoColour = Color.green,
        spawnGizmoColour = Color.red,
        doorGizmoColour = Color.yellow;

        /// <summary>
        /// Whether a room can appear multiple times, or only once
        /// </summary>
        public bool unique = false;

        /// <summary>
        /// Whether a room can be generated at the current time
        /// </summary>
        public bool isViable = true;

        /// <summary>
        /// A list of all doorpoints in the room
        /// </summary>
        public List<DoorPoint> DoorPoints { get; set; }

        /// <summary>
        /// The archetype this room is assigned to -
        /// Important to set this manually in the inspector
        /// </summary>
        public RoomArchetype assignedArchetype;

        public void Awake()
        {
            //Obtain all door points in the actual room
            DoorPoints = new List<DoorPoint>(gameObject.GetComponentsInChildren<DoorPoint>());
        }

        /// <summary>
        /// Creates links between the doorpoints in the room and the corresponding doors in the archetype instance.
        /// Also disables the doors that are currently disabled in the archetype instance
        /// </summary>
        /// <param name="doors">The doors to be linked to doorpoints</param>
        public void LinkDoorsToDoorPoints(List<GameObject> doors)
        {
            //Finds the closest door to each doorpoint
            //Relies on doors being structure properly in the room
            foreach (DoorPoint dp in DoorPoints)
            {
                float closestDist = float.MaxValue;
                GameObject closestDoor = null;
                Vector3 dpPos = dp.unmaskedDoor.transform.position;
                foreach (GameObject door in doors)
                {
                    float dist = Vector3.Distance(dpPos, door.transform.position);
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        closestDoor = door;
                    }
                }

                dp.AssociatedDoor = closestDoor;
                Door closest = closestDoor.GetComponent<Door>();
                //Update associated door point for both closest and its mirror
                closest.AssociatedDoorPoint = dp;
                if (!dp.AssociatedDoor.activeInHierarchy)
                {
                    Door door = dp.AssociatedDoor.GetComponent<Door>();
                    //A door that connects to rooms together, but is not the parent door and so both needs to be disabled
                    if (door.ArchetypeAssignedTo != null)
                        dp.SetAsOpenDoor();
                    else if (door.IsDeadEnd)
                        dp.SetAsDeadEnd();
                }
            }
        }

        public void OnDrawGizmos()
        {
            if (!Application.isPlaying)
            {
                if (assignedArchetype != null)
                    UtilityHelper.RenderArchetype(assignedArchetype.gameObject, spawnGizmoColour, cornerGizmoColour, doorGizmoColour);
            }
        }
    }
}