using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A list of all archetype prefabs that results in a complete generation failure
/// </summary>
/// <param name="variablename">info about variable</param>
/// <returns>if your method is not void, you can use this to describe what it returns</returns>
namespace RoomAllocation
{
    /// <summary>
    /// Allocates archetypes and rooms based on the supplied geometry
    /// </summary>
    public class RoomAllocator : MonoBehaviour
    {
        [Tooltip("How often the system will test for generation conditions")]
        public float generationLoopRate = 0.1f;

        [Tooltip("If true, stops rooms being generated and only renders the line renderer in the archetypes")]
        public bool forceArchetypeRendering = false;

        public InitialAllocator InitialAllocator { get; set; }
        public RoomManager RoomManager { get; set; }
        public PathManager PathManager { get; set; }
        public CameraManager CameraManager { get; set; }
        public GeometryManager GeometryManager { get; set; }
        public Logger Logger { get; set; }

        /// <summary>A list of all reference archetypes</summary>

        public List<GameObject> Archetypes { get; set; }

        /// <summary>The parent transform by which generated archetypes are placed</summary>
        public Transform GeneratedParent { get; set; }

        public void Awake()
        {
            //Obtain references to all relevant components

            RoomManager = gameObject.GetComponent<RoomManager>();
            PathManager = gameObject.GetComponent<PathManager>();
            GeometryManager = gameObject.GetComponent<GeometryManager>();
            Logger = gameObject.GetComponent<Logger>();
            CameraManager = gameObject.GetComponent<CameraManager>();
            InitialAllocator = gameObject.GetComponent<InitialAllocator>();

            //Create the parent object for the generated archetypes
            GameObject parentObj = GameObject.Find(AllocationConstants.GENERATED_ARCHETYPE_PARENT);
            //If not found, create the object and set the transform
            if (parentObj == null)
            {
                GeneratedParent = new GameObject(AllocationConstants.GENERATED_ARCHETYPE_PARENT).transform;
                GeneratedParent.transform.position = GeometryManager.areaOrigin;
            }
            else // If found, then just set the position and transform
            {
                parentObj.transform.position = GeometryManager.areaOrigin;
                GeneratedParent = parentObj.transform;
            }
        }

        /// <summary>
        /// Performs the overarching generation loop.
        /// Updates the archetype that the camera is currently in.
        /// Finds the closest door to the camera, and checks if it is viable for generation.
        /// Generates archetypes, and renders rooms if enabled.
        /// Opens doors.
        /// Disables inactive archetypes, and also manages door logic to get them to display properly.
        /// </summary>
        public void DoGenerationLoop()
        {
            RoomArchetype current = CameraManager.GetArchetypeThatContainsCamera();
            if (current != null)
            {
                //Update the current archetype, to ensure it is always correct every update
                UpdateCurrentArchetype(current);
                //Find the closest door
                Door viableDoor = CameraManager.FindClosestDoorInArchetype(PathManager.CurrentArchetype);
                if (viableDoor != null)
                {
                    if (viableDoor.ArchetypeAssignedTo != null)
                    {
                        RenderMirrorDoor(viableDoor.gameObject);
                        //Update next predicted archetype
                        PathManager.NextArchetype = viableDoor.ArchetypeAssignedTo;
                        //Generate all children for the assigned archetype if not already done
                        if (!viableDoor.ArchetypeAssignedTo.HasChildrenGenerated)
                        {
                            StartCoroutine(DoDefaultGeneration(viableDoor.ArchetypeAssignedTo.Doors, viableDoor.ArchetypeAssignedTo));
                            viableDoor.ArchetypeAssignedTo.HasChildrenGenerated = true;
                        }
                        //Check to see if we want rooms to be instantiated and rendered
                        if (!forceArchetypeRendering)
                        {
                            //If no room is associated, then we generated one
                            if (viableDoor.ArchetypeAssignedTo.AssociatedRoom == null)
                            {
                                GameObject roomObj = RoomManager.InstantiateRandomRoomForArchetype(viableDoor.ArchetypeAssignedTo.gameObject);
                                if (roomObj != null)
                                {
                                    viableDoor.ArchetypeAssignedTo.AssociatedRoom = roomObj;
                                    //Update the path entry of this archetype
                                    PathManager.Path.UpdateRoomObject(viableDoor.ArchetypeAssignedTo, roomObj);
                                    roomObj.GetComponent<Room>().LinkDoorsToDoorPoints(viableDoor.ArchetypeAssignedTo.Doors);
                                    viableDoor.ArchetypeAssignedTo.Drawer.LineRenderer.enabled = false;
                                    //Log the final generation with the room
                                    LogGeneration(viableDoor.ArchetypeAssignedTo);
                                }
                            }
                        }
                        //Open viable door
                        if (!viableDoor.Open)
                        {
                            viableDoor.OpenDoor();
                        }
                        //Set the "current archetype" to be the newly loaded archetype
                        //Need to ensure the the right door is opened by checking that the current is the owner
                        if (PathManager.CurrentArchetype.gameObject == viableDoor.Owner.gameObject)
                        {
                            DisableInactiveArchetypes();
                            //Enable the archetype the door is assigned to
                            viableDoor.ArchetypeAssignedTo.EnableRendering();
                        }
                    }
                    else //Last resort - force the generation for the door
                        DoGenerationForDoor(viableDoor, viableDoor.Owner);
                }
            }
        }

        /// <summary>
        /// Performs the initial actions to pack the initial room archetype.
        /// Creates the reference archetypes and links the archetypes to their associated rooms.
        /// </summary>
        /// <returns>The initial room archetype generated, or null if nothing could be generated</returns>
        public RoomArchetype PackInitialRoom()
        {
            //Ensure that geometry is somewhat defined
            if (GeometryManager.Geometry == null || GeometryManager.Geometry.Count <= 0)
            {
                Debug.LogError("Geometry not defined correctly");
                return null;
            }

            CreateReferenceArchetypes();
            //Links all unique archetypes to their corresponding room prefabs
            RoomManager.LinkArchetypesAndRooms(Archetypes);

            RoomArchetype initial = InitialAllocator.DoInitialGeneration();
            //Null if initial archetype, and its subsequent children, do not satisfy dead end conditions
            if (initial == null)
            {
                Debug.LogError("No initial archetype could be found for the current camera location");
                //Reset reference archetypes
                foreach (GameObject refArchetypeObj in Archetypes)
                {
                    GeometryManager.ResetReferenceArchetype(refArchetypeObj);
                }
            }
            else
            {
                if (!forceArchetypeRendering)
                    PathManager.Path.UpdateRoomObject(initial, RoomManager.InstantiateRandomRoomForArchetype(initial.gameObject));
                initial.HasChildrenGenerated = true;
                //Set the parent of the initial
                initial.gameObject.transform.parent = GeneratedParent;
                //Log the initial archetype generated
                LogGeneration(initial);
                InvokeRepeating(nameof(DoGenerationLoop), 0, generationLoopRate);
            }

            return initial;
        }

        /// <summary>Loads all archetype prefabs and instantiates them at their default positions</summary>
        private void CreateReferenceArchetypes()
        {
            Archetypes = new List<GameObject>();
            //Load all archetype prefabs in Resources/Prefabs/Room Archetypes
            List<GameObject> archetypePrefabs = new List<GameObject>(Resources.LoadAll<GameObject>(AllocationConstants.ARCHETYPE_PREFAB_PATH));
            //Create a parent object to bind all the reference archetypes to
            GameObject parentObject = GameObject.Find(AllocationConstants.REFERENCE_ARCHETYPE_PARENT);
            if (parentObject == null)
            {
                parentObject = new GameObject(AllocationConstants.REFERENCE_ARCHETYPE_PARENT);
            }

            foreach (GameObject prefab in archetypePrefabs)
            {
                GameObject archetype = Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity, parentObject.transform);
                UtilityHelper.StandardiseObjectName(archetype);
                Archetypes.Add(archetype);
            }

            Debug.Log(Archetypes.Count + " reference archetypes generated");

            if (Archetypes == null || Archetypes.Count <= 0)
            {
                Debug.LogError("Archetypes are null or none have been loaded");
                return;
            }
        }

        ///<summary> Disables rendering for archetypes that are not the current or previous </summary>
        private void DisableInactiveArchetypes()
        {
            if (PathManager.CurrentArchetype != null
                && PathManager.NextArchetype != null)
            {
                foreach (RoomArchetype archetype in FindObjectsOfType<RoomArchetype>())
                {
                    if (archetype != PathManager.CurrentArchetype
                        && archetype != PathManager.NextArchetype)
                    {
                        archetype.DisableRendering();
                    }
                }
            }
        }

        /// <summary> Coroutine for normal generation, does generation for each door supplied</summary>
        /// <param name="doors">The doors to be generated for</param>
        /// <param name="parent">The archetype the doors belong to</param>
        /// <returns>The IEnumerator for the coroutine, yield return null</returns>
        public IEnumerator DoDefaultGeneration(List<GameObject> doors, RoomArchetype parent)
        {
            DoGenerationForDoors(doors, parent);

            yield return null;
        }

        /// <summary> Does the generation for a particular door.
        /// Also adds the generated archetype to the path, with a null room object reference.
        /// </summary>
        /// <param name="door">The door to be generated for</param>
        /// <param name="parent">The archetype the door belongs to</param>
        /// <returns>The archetype generated if successful, null otherwise</returns>
        private RoomArchetype DoGenerationForDoor(Door door, RoomArchetype parent)
        {
            if (door.ArchetypeAssignedTo == null)
            {
                List<RoomArchetype> viable = GeometryManager.GetViableArchetypes(door.gameObject, parent, Archetypes);
                RoomArchetype generated = UtilityHelper.ChooseWeightedArchetype(viable);
                if (generated != null)
                {
                    door.ArchetypeAssignedTo = generated;
                    //Disable rendering of newly generated archetype
                    generated.DisableRendering();
                    generated.transform.parent = GeneratedParent;
                    //Add new archetype to path
                    AddToPath(parent, generated, null);
                    //If we are forcing archetype rendering then rooms won't be generated,
                    //So we can just log here
                    if (forceArchetypeRendering)
                        LogGeneration(generated);
                    return generated;
                }
                else
                {
                    door.IsDeadEnd = true; //No archetypes can be generated
                    door.gameObject.SetActive(false);
                }
            }
            return null;
        }

        /// <summary> Does generation for all doors in the provided parent archetype, returning all generated.</summary>
        /// <param name="doorObjects">The doors to be generated for</param>
        /// <param name="parent">The archetype the doors belong to</param>
        /// <returns>A list of all archetypes generated</returns>
        public List<RoomArchetype> DoGenerationForDoors(List<GameObject> doorObjects, RoomArchetype parent)
        {
            //Shuffle doors to improve random distribution of which doors are chosen
            List<GameObject> shuffledDoors = UtilityHelper.ShuffleList(doorObjects);
            //Doors that have been successfully generated for
            List<GameObject> acceptedDoors = new List<GameObject>();
            //All archetypes generated for the parent archetype
            List<RoomArchetype> archetypes = new List<RoomArchetype>();
            if (shuffledDoors != null && shuffledDoors.Count > 0)
            {
                //Obtain a random amount of doors to generate between min and max
                int randomNumberOfDoors = Random.Range(parent.minDoorCount, parent.maxDoorCount);
                //Iterate through all doors
                foreach (GameObject doorObject in shuffledDoors)
                {
                    //If less than the random number, then we generate for the door
                    if (acceptedDoors.Count <= randomNumberOfDoors)
                    {
                        if (!doorObject.GetComponent<Door>().IsDeadEnd)
                        {
                            RoomArchetype archetype = DoGenerationForDoor(doorObject.GetComponent<Door>(), parent);
                            if (archetype != null)
                            {
                                //Add to generated, and also add the doorObject to update the count of how many doors are generated
                                archetypes.Add(archetype);
                                UtilityHelper.StandardiseObjectName(archetype.gameObject);
                                acceptedDoors.Add(doorObject);
                            }
                        }
                    }
                    else
                    {
                        //Set the rest of the doors to be disabled
                        Door door = doorObject.GetComponent<Door>();
                        if (door.ArchetypeAssignedTo == null)
                        {
                            door.IsDeadEnd = true;
                            doorObject.SetActive(false);
                        }
                    }
                }
            }
            return archetypes;
        }

        /// <summary>
        /// Updates the current archetype, if it is different.
        /// Also updates the previous archetype, and appropriately renders the mirrored door
        /// </summary>
        /// <param name="archetype">The archetype which is the 'new' current archetype</param>
        /// <param name="renderDoor">The door that is used as the reference point for swapping the rendering between the two doors at that doorpoint</param>
        /// <returns>The current archetype</returns>
        public RoomArchetype UpdateCurrentArchetype(RoomArchetype archetype)
        {
            if (archetype != PathManager.CurrentArchetype)
            {
                PathManager.PreviousArchetype = PathManager.CurrentArchetype;
                PathManager.CurrentArchetype = archetype;
            }

            return PathManager.CurrentArchetype;
        }

        /// <summary>
        /// Sets the provided door to active, and disables the mirror door if available.
        /// Also disables the closest door from the archetype the door is connected to.
        /// </summary>
        /// <param name="doorObj">The door that is being made active</param>
        private void RenderMirrorDoor(GameObject doorObj)
        {
            if (doorObj != null)
            {
                Door door = doorObj.GetComponent<Door>();
                DoorPoint doorDP = door.AssociatedDoorPoint;
                if (doorDP != null)
                {
                    doorObj.GetComponent<Door>().AssociatedDoorPoint.SetAsClosedDoor();
                }

                if (door.DoorMirror != null)
                {
                    Door mirrorDoor = door.DoorMirror.GetComponent<Door>();
                    DoorPoint mirrorDP = mirrorDoor.AssociatedDoorPoint;
                    if (mirrorDP != null)
                    {
                        mirrorDP.SetAsOpenDoor();
                    }
                }
                else
                {
                    //Manually set the door mirror
                    GameObject closest = GeometryManager.FindClosestDoorToOrigin(doorObj, door.ArchetypeAssignedTo);
                    if (closest != null)
                    {
                        DoorPoint prevDP = closest.GetComponent<Door>().AssociatedDoorPoint;
                        if (prevDP != null)
                        {
                            prevDP.SetAsOpenDoor();
                            //Set door mirror so can be used again
                            door.DoorMirror = closest;
                        }
                    }
                }
            }
        }

        /// <summary>Adds a new node to the path with the specified parameters. </summary>
        /// <param name="parent">The parent archetype of the node</param>
        /// <param name="childArchetype">The archetype to be added to the path</param>
        /// <param name="childRoom">The associated room object</param>
        /// <returns>Returns true if succesful, false otherwise</returns>
        public bool AddToPath(RoomArchetype parent, RoomArchetype childArchetype, GameObject childRoom)
        {
            AllocationTree node = PathManager.Path.AddNodeToParentArchetype(parent,
                 childArchetype.gameObject,
                 childRoom);

            if (node != null)
                return true;
            return false;
        }

        /// <summary>Uses the logger to log a particular generation </summary>
        /// <param name="generated">The generated archetype to be logged</param>
        public void LogGeneration(RoomArchetype generated)
        {
            if (Logger != null && generated != null
                && !GeometryManager.isVR)
            {
                Logger.LogEvent(
                    Logger.ParseEventData(
                        PathManager.Path,
                        PathManager.Path.FindNodeByArchetype(generated.gameObject),
                        GeometryManager.GetSizeVector())
                );
            }
        }
    }
}