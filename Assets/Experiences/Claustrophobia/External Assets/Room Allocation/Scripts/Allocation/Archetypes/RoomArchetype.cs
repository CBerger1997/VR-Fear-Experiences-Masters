using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RoomAllocation
{
    ///<summary>Represents the overarching structure of a room</summary>
    public class RoomArchetype : MonoBehaviour
    {
        [Tooltip("Determines how many instances " +
         "the archetype has in the selection array when archetypes are being selected, " +
         "where a greater number results in a greater chance")]
        public int selectionWeighting = 1;

        [Tooltip("Allows for random door enabling (after allocation) between certain limits. " +
            "The minimum amount of viable doors that will be enabled in the archetype.")]
        public int minDoorCount = 1;

        [Tooltip("Allows for random door enabling (after allocation) between certain limits. " +
            "The maximum amount of viable doors that will be enabled in the archetype.")]
        public int maxDoorCount = 2;

        ///<summary>The room currently generated for this specific archetype instant</summary>
        public GameObject AssociatedRoom { get; set; }

        ///<summary>If the system has already generated for the doors on this archetype or not</summary>
        public bool HasChildrenGenerated { get; set; }

        ///<summary>The door that connects the archetype to its parent archetype</summary>
        public GameObject ParentDoor { get; set; }

        ///<summary>Draws the archetype, if there is no associated room object</summary>
        public RoomArchetypeDrawer Drawer { get; set; }

        ///<summary>All corner objects in the archetype</summary>
        public List<GameObject> CornerPoints { get; set; }

        ///<summary>All door objects in the archetype</summary>
        public List<GameObject> Doors { get; set; }

        ///<summary>All spawn objects in the archetype</summary>
        public List<GameObject> SpawnPoints { get; set; }

        public void Awake()
        {
            Drawer = gameObject.GetComponent<RoomArchetypeDrawer>();
            CornerPoints = new List<GameObject>();
            SpawnPoints = new List<GameObject>();
            Doors = new List<GameObject>();
            HasChildrenGenerated = false;
            //Ensure all room points are set up appropriately
            UpdateRoomPoints();
        }

        public void FixedUpdate()
        {
            //Render the archetype using the line renderer if no room is associated
            if (AssociatedRoom == null)
                Drawer.DrawArchetype(CornerPoints);
        }

        ///<summary>Obtains room point objects and stores them in the corresponding lists (corners, doors, spawns)</summary>
        public void UpdateRoomPoints()
        {
            Transform roomPointsParent = gameObject.transform.GetChild(0);
            for (int i = 0; i < roomPointsParent.childCount; i++)
            {
                GameObject child = roomPointsParent.GetChild(i).gameObject;
                if (child.CompareTag(AllocationConstants.SPAWN_TAG_NAME))
                    SpawnPoints.Add(child);
                else if (child.CompareTag(AllocationConstants.DOOR_TAG_NAME))
                {
                    Doors.Add(child);
                    child.GetComponent<Door>().Owner = this; //Set owner
                }
                else if (child.CompareTag(AllocationConstants.CORNER_TAG_NAME))
                    CornerPoints.Add(child);
            }
        }

        ///<summary>Iterates through all corners and adds their positions to a list</summary>
        ///<returns>A list of all positions of all corners in the archetype</returns>
        public List<Vector3> GetCornerPositions()
        {
            List<Vector3> positions = new List<Vector3>();
            foreach (GameObject obj in CornerPoints)
            {
                positions.Add(obj.transform.position);
            }
            return positions;
        }

        ///<summary>Checks if the supplied vector is within the corners of the archetype</summary>
        ///<param name="pos">The position to be checked against the corners</param>
        ///<returns>True if the position is within the archetype bounds, false otherwise</returns>
        public bool IsPositionInArchetypeBounds(Vector3 pos)
        {
            return UtilityHelper.IsPositiontWithinPointPolygon(pos, CornerPoints);
        }

        ///<summary>
        ///Checks if the archetype is rendered.
        ///If no room is associated, will use the Drawer/Line Renderer to check
        ///</summary>
        ///<returns>True if rendered, false otherwise</returns>
        public bool IsRendered()
        {
            if (AssociatedRoom != null)
                return AssociatedRoom.activeInHierarchy;
            else
                return Drawer.LineRenderer.enabled;
        }

        ///<summary>
        ///Enables the rendering of the archetype.
        ///If no room is associated, will render using the Line Renderer.
        ///Otherwise, will set the room object to be active.
        ///</summary>
        public void EnableRendering()
        {
            if (AssociatedRoom != null)
                AssociatedRoom.SetActive(true);
            else
                Drawer.LineRenderer.enabled = true;
        }

        ///<summary>
        ///Disables the rendering of the archetype.
        ///If no room is associated, will render using the Line Renderer.
        ///Otherwise, will set the room object to be inactive.
        ///</summary>
        public void DisableRendering()
        {
            if (AssociatedRoom != null)
                AssociatedRoom.SetActive(false);
            else
                Drawer.LineRenderer.enabled = false;
        }
    }
}