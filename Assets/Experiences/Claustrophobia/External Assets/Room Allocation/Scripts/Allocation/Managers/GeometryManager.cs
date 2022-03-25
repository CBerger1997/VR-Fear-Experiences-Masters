using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace RoomAllocation
{
    /// <summary>
    /// Handles the geometry of the area and functions pertaining to it
    /// </summary>
    public class GeometryManager : MonoBehaviour
    {
        public float AreaX = 2;
        public float AreaZ = 2;

        [Tooltip("Denote whether or not the space you are using is in VR or not")]
        public bool isVR = false;

        [Tooltip("How many vectors make up the geometry for area creation")]
        public int numberOfGeometryPoints = 36;

        [Tooltip("The origin point by which the geometry is created")]
        public Vector3 areaOrigin = new Vector3(0, 0, 0);

        [Tooltip("If checked, will render spheres at each position generated for the area")]
        public bool renderGeometryPoints = true;

        /// <summary>
        /// Different shape geometry types for the area
        /// </summary>
        public enum AreaShape
        {
            Rect,
            Square,
            Triangle,
            VR
        }

        [Tooltip("The shape the area geometry will take")]
        public AreaShape shape;

        [Tooltip("The offset from the origin for the top point in the x axis if using a triangle area shape")]
        public float trianglePointOffset = 1;

        /// <summary>
        /// A list of all vectors in the defined geometry
        /// </summary>
        public List<Vector3> Geometry { get; set; }

        public float XMin { get; set; }
        public float XMax { get; set; }
        public float ZMin { get; set; }
        public float ZMax { get; set; }

        //The min/max positions of the vectors in the geometry
        public Vector3 XMinZMin { get; set; }

        public Vector3 XMinZMax { get; set; }

        public Vector3 XMaxZMin { get; set; }
        public Vector3 XMaxZMax { get; set; }

        /// <summary>
        /// Obtains all min/max positions and stores them accordingly
        /// Essentially forms a Rect, but can be used on more complex shapes
        /// </summary>
        public void SetMinMaxPositions()
        {
            if (Geometry != null && Geometry.Count > 0)
            {
                XMin = Geometry[0].x;
                XMax = Geometry[0].x;
                ZMin = Geometry[0].z;
                ZMax = Geometry[0].z;

                for (int i = 1; i < Geometry.Count; i++)
                {
                    float vecX = Geometry[i].x;
                    float vecZ = Geometry[i].z;
                    if (vecX < XMin) { XMin = vecX; }
                    else if (vecX > XMax) { XMax = vecX; }

                    if (vecZ < ZMin) { ZMin = vecZ; }
                    else if (vecZ > ZMax) { ZMax = vecZ; }
                }

                XMinZMin = new Vector3(XMin, AllocationConstants.FLOOR_LEVEL, ZMin);
                XMinZMax = new Vector3(XMin, AllocationConstants.FLOOR_LEVEL, ZMax);
                XMaxZMin = new Vector3(XMax, AllocationConstants.FLOOR_LEVEL, ZMin);
                XMaxZMax = new Vector3(XMax, AllocationConstants.FLOOR_LEVEL, ZMax);
            }
        }

        /// <summary>
        /// Calculates the centroid of the geometry
        /// </summary>
        /// <returns>The centre of the geometry</returns>
        public Vector3 GetCentreOfGeometry()
        {
            return UtilityHelper.CalculateCentroidOfVectors(Geometry);
        }

        /// <summary>
        /// Calculates a vector that evaluates the width, in x, and height, in z, of the geometry
        /// </summary>
        /// <returns>A vector containing the max width and max height of the geometry</returns>
        public Vector2 GetSizeVector()
        {
            return new Vector2(XMaxZMin.x - XMinZMin.x, XMaxZMax.z - XMaxZMin.z);
        }

        /// <summary>
        /// Checks if a particular room archetype can be placed in the geometry
        /// </summary>
        /// <param name="archetype">The archetype to be tested</param>
        /// <returns>True if it fits, false otherwise</returns>
        public bool CheckRoomCanBePlaced(RoomArchetype archetype)
        {
            foreach (Vector3 vec in archetype.GetCornerPositions())
            {
                //Must round to avoid very small deviancy with position
                float roundedX = (float)System.Math.Round(vec.x, 1);
                float roundedZ = (float)System.Math.Round(vec.z, 1);
                Vector3 roundedVec = new Vector3(roundedX, AllocationConstants.FLOOR_LEVEL, roundedZ);

                //If assumerect, then will only judge based on the min max of the geometry
                if (shape == AreaShape.Rect || shape == AreaShape.Square)
                {
                    if (!(roundedVec.x >= XMin && roundedVec.x <= XMax
                        && roundedVec.z >= ZMin && roundedVec.z <= ZMax))
                    {
                        return false;
                    }
                }
                else if (!UtilityHelper.IsPositionWithinVectors(roundedVec, Geometry))
                {
                    //Otherwise will go through each and ensure it is WITHIN the geometry
                    //I.e. fringe vectors will NOT be in the geometry, so this is checking exclusively of the border
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Finds all viable archetypes and their variations for a particular door origin
        /// </summary>
        /// <param name="doorOrigin">The origin door that is being generated for</param>
        /// <param name="parent">The owner of the door origin</param>
        /// <param name="instantiatedArchetypes">A list of all reference archetypes</param>
        /// <returns>A list of all viable archetypes that can be generated for the door</returns>
        public List<RoomArchetype> GetViableArchetypes(GameObject doorOrigin,
            RoomArchetype parent,
            List<GameObject> instantiatedArchetypes)
        {
            List<RoomArchetype> viableArchetypes = new List<RoomArchetype>();
            foreach (GameObject archetypeObj in instantiatedArchetypes)
            {
                //Check to see if the archetype is viable for this door
                if (!doorOrigin.GetComponent<Door>()
                    .UnviableArchetypeNames
                    .Contains(archetypeObj.name))
                {
                    RoomArchetype viableArchetype = GetViableArchetypeVariation(doorOrigin, archetypeObj, parent);
                    //If there are any viable variations of this archetype, then add to the list
                    if (viableArchetype != null)
                    {
                        viableArchetypes.Add(viableArchetype);
                    }
                }
            }
            return viableArchetypes;
        }

        /// <summary>
        /// Uses reference archetypes to find all viable variations of a specific archetype for a particular door origin.
        /// Tests all doors in the archetype
        /// </summary>
        /// <param name="doorOrigin">The origin door that is being generated for</param>
        /// <param name="archetypeObj">The reference archetype being tested</param>
        /// <param name="parent">The owner of the origin door</param>
        /// <returns>A randomly selected viable archetype variation</returns>
        //Returns a viable archetype variation by testing all doors and randomly selecting one that fits
        public RoomArchetype GetViableArchetypeVariation(GameObject doorOrigin, GameObject archetypeObj, RoomArchetype parent)
        {
            archetypeObj.SetActive(true);
            RoomArchetype instantiatedArchetype = archetypeObj.GetComponent<RoomArchetype>();
            if (instantiatedArchetype.selectionWeighting <= 0)
            {
                ResetReferenceArchetype(archetypeObj);
                return null;
            }

            Transform pointsParent = archetypeObj.transform.GetChild(0);
            //Each integer represents the door id that can be connected
            //The vector is the position the ARCHETYPE is in, so can be easily moved back there without calculating again
            List<System.Tuple<int, Vector3, Quaternion>> viableDoorIds = new List<System.Tuple<int, Vector3, Quaternion>>();
            //Iterating through all the doors gives a significant perfomance hit
            //Choose a random door and see if it fits in
            for (int i = 0; i < instantiatedArchetype.Doors.Count; i++)
            {
                GameObject currentDoor = instantiatedArchetype.Doors[i];
                AlignArchetypeToDoor(pointsParent, doorOrigin, currentDoor, instantiatedArchetype.gameObject);
                //Check if the room can be placed inside the play area geometry
                if (CheckRoomCanBePlaced(instantiatedArchetype))
                {
                    //Add the door id to the list
                    viableDoorIds.Add(new System.Tuple<int, Vector3, Quaternion>(i, archetypeObj.transform.position, archetypeObj.transform.rotation));
                }
            }

            //If no variations are applicable, then we should add this to unviable archetypes for the door
            //So, if regeneration occurs, this process is not repeated unneccesarily
            if (viableDoorIds.Count <= 0)
            {
                ResetReferenceArchetype(archetypeObj);
                doorOrigin.GetComponent<Door>().UnviableArchetypeNames.Add(archetypeObj.name);
                return null;
            }

            //Choose a random id and obtain the corresponding transform
            System.Tuple<int, Vector3, Quaternion> randomSelection = viableDoorIds[Random.Range(0, viableDoorIds.Count)];
            GameObject selectedDoor = instantiatedArchetype.Doors[randomSelection.Item1];

            //Set the position and rotation from the selected values
            archetypeObj.transform.position = randomSelection.Item2;
            archetypeObj.transform.rotation = randomSelection.Item3;

            //Copy archetype and its doors
            GameObject copyArchetypeObj = GetCopyOfArchetype(archetypeObj, selectedDoor, parent);
            RoomArchetype copyArchetype = copyArchetypeObj.GetComponent<RoomArchetype>();

            GameObject closestDoorToOrigin = FindClosestDoorToOrigin(doorOrigin, copyArchetype);
            if (closestDoorToOrigin != null)
            {
                //Set parent in new archetype
                copyArchetype.ParentDoor = closestDoorToOrigin;

                //Update door mirrors
                closestDoorToOrigin.GetComponent<Door>().DoorMirror = doorOrigin;
                doorOrigin.GetComponent<Door>().DoorMirror = closestDoorToOrigin;

                //Reset the reference object to ensure consistent future testing
                ResetReferenceArchetype(archetypeObj);
                return copyArchetype;
            }

            Debug.LogError("Could not find closest door to origin in newly created archetype - oops!");
            return null;
        }

        /// <summary>
        /// Moves an archetype so that it aligns with the door origin
        /// </summary>
        /// <param name="pointsParent">The parent transform where the door object should be placed</param>
        /// <param name="doorOrigin">The reference object that provides the position</param>
        /// <param name="doorObj">The door object that needs to align with the origin</param>
        /// <param name="archetypeObj">The archetype that owns the doorObj</param>
        private void AlignArchetypeToDoor(Transform pointsParent, GameObject doorOrigin, GameObject doorObj, GameObject archetypeObj)
        {
            //NOTE - setting parents is very inefficient, though if a better way is found this should be implemented
            //Make door parent in order to transform central to the door
            //Moves the archetype to attach door-door rather than calculating vector difference
            doorObj.transform.SetParent(null);
            archetypeObj.transform.SetParent(doorObj.transform);
            doorObj.transform.position = doorOrigin.transform.position;

            Vector3 originEuler = doorOrigin.transform.eulerAngles;
            Vector3 doorEuler = doorObj.transform.eulerAngles;
            Vector3 oppositeOriginEuler = new Vector3(0, UtilityHelper.GetOppositeAngle(originEuler.y), 0);

            //Rotate to fit the room such that it is opposite to the origin door
            doorObj.transform.Rotate(Vector3.up, oppositeOriginEuler.y - doorEuler.y);

            //Reset parents
            archetypeObj.transform.SetParent(null);
            doorObj.transform.SetParent(pointsParent);
        }

        /// <summary>
        /// Resets a reference archetype to its original position and rotation
        /// </summary>
        /// <param name="archetypeObj">The reference archetype to be reset</param>
        public void ResetReferenceArchetype(GameObject archetypeObj)
        {
            archetypeObj.transform.position = new Vector3(-50, -10, -50);
            archetypeObj.transform.parent = GameObject.Find(AllocationConstants.REFERENCE_ARCHETYPE_PARENT).transform;
            archetypeObj.transform.rotation = Quaternion.identity;
        }

        /// <summary>
        /// Finds the closest door  in the supplied archetype to the door origin point.
        /// Used to match parent door and door mirrors when connecting archetypes
        /// </summary>
        /// <param name="doorOrigin">The origin door to act as a reference position</param>
        /// <param name="archetype">The archetype whose doors are being checked to find which one is closest to the origin</param>
        /// <returns>The closest door from the archetype, or null if none could be found</returns>
        public GameObject FindClosestDoorToOrigin(GameObject doorOrigin, RoomArchetype archetype)
        {
            GameObject closestDoor = null;
            float closestDist = float.MaxValue;
            foreach (GameObject doorObj in archetype.Doors)
            {
                float dist = Vector3.Distance(doorObj.transform.position, doorOrigin.transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closestDoor = doorObj;
                }
            }

            return closestDoor;
        }

        /// <summary>
        /// Copies an archetype into a new object, setting the values of the origin door also
        /// </summary>
        /// <param name="archetypeObj">The archetype to be copied</param>
        /// <param name="door">The door to be set in the newly generated archetype</param>
        /// <param name="parent">The parent archetype that owns the door</param>
        /// <returns></returns>
        private GameObject GetCopyOfArchetype(GameObject archetypeObj, GameObject door, RoomArchetype parent)
        {
            //Disable the door on newly generated archetype to ensure only 1 door between rooms
            door.SetActive(false);
            if (parent != null)
                door.GetComponent<Door>().ArchetypeAssignedTo = parent;

            //Instantiate to obtain a copy
            GameObject copyArchetypeObj = Instantiate(archetypeObj);
            UtilityHelper.StandardiseObjectName(copyArchetypeObj);
            //Reset selected door so that it doesn't affect further generations
            door.SetActive(true);
            door.GetComponent<Door>().IsDeadEnd = false;
            door.GetComponent<Door>().ArchetypeAssignedTo = null;

            return copyArchetypeObj;
        }

        /// <summary>
        /// Creates the simulated geometry based on chosen geometry shape, size and origin
        /// </summary>
        public void CreateArea()
        {
            switch (shape)
            {
                case (AreaShape.Rect):
                    {
                        Geometry = TestAreaCreator.CreateRectGeometry(
                            numberOfGeometryPoints,
                            areaOrigin,
                            AreaX, AreaZ,
                            renderGeometryPoints
                        );
                        break;
                    }

                case (AreaShape.Square):
                    {
                        Geometry = TestAreaCreator.CreateSquareGeometry(
                            numberOfGeometryPoints,
                            areaOrigin,
                            AreaX,
                            renderGeometryPoints
                        );
                        break;
                    }

                case (AreaShape.Triangle):
                    {
                        Geometry = TestAreaCreator.CreateTriangleGeometry(
                            numberOfGeometryPoints,
                            areaOrigin,
                            AreaX,
                            new Vector3(trianglePointOffset, areaOrigin.y, areaOrigin.z + AreaZ),
                            renderGeometryPoints
                        );
                        break;
                    }
            }
        }

        /// <summary>
        /// Performs sanity checks on inspector values for Geometry Manager
        /// </summary>
        public void DoSanityChecks()
        {
            if (AreaX <= 0)
            {
                Debug.LogError("Area x size cannot be 0 or less, automatically changed to 2");
                AreaX = 2;
            }

            if (AreaZ <= 0)
            {
                Debug.LogError("Area z size cannot be 0 or less, automatically changing to 2");
                AreaZ = 2;
            }

            if (numberOfGeometryPoints <= 2)
            {
                Debug.LogError("Geometry with 2 or less points is a line. Resetting to 36");
                numberOfGeometryPoints = 36;
            }

            //Check that cuboid areas have the appropriate amount of points
            if ((shape == AreaShape.Rect
                || shape == AreaShape.Square)
                && numberOfGeometryPoints % 4 != 0)
            {
                Debug.LogError("You have incorrectly defined the number of geometry points " +
                    "for a rect or square area, which should be a multiple of 4. Resetting to 4.");
                numberOfGeometryPoints = 4;
            }
        }
    }
}