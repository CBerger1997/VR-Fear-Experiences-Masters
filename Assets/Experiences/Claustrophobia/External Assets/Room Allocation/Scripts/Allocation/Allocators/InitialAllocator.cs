using System.Collections.Generic;
using UnityEngine;

namespace RoomAllocation
{
    /// <summary>
    /// Handles the allocation of the initial room, and its subsequent children
    /// </summary>
    public class InitialAllocator : MonoBehaviour
    {
        /// <summary>
        /// A list of all archetype prefabs that results in a complete generation failure
        /// </summary>
        public List<string> UnviableArchetypeNames = new List<string>();

        public RoomAllocator MainAllocator { get; set; }
        public PathManager PathManager { get; set; }
        public CameraManager CameraManager { get; set; }
        public List<GameObject> UnassignedDoors { get; set; }

        public void Awake()
        {
            UnviableArchetypeNames = new List<string>();
            UnassignedDoors = new List<GameObject>();
            MainAllocator = GetComponent<RoomAllocator>();
            PathManager = GetComponent<PathManager>();
            CameraManager = GetComponent<CameraManager>();
        }

        /// <summary>
        /// Allocates the initial room archetype in the geometry, at the current position of the camera
        /// </summary>
        /// <returns>The initial room archetype, or null if no viable archetypes could be found</returns>
        public RoomArchetype DoInitialGeneration()
        {
            //All archetypes are inviable, so return null
            if (UnviableArchetypeNames.Count >= MainAllocator.Archetypes.Count)
            {
                Debug.LogError("No initial or subsequent archetypes can be generated with the current requirements");
                return null;
            }

            //Obtain viable archetypes for the initial room generation
            List<RoomArchetype> viableArchetypes = GetViableInitialArchetypes();
            if (viableArchetypes.Count > 0)
            {
                //Choose one randomly using weightings
                RoomArchetype initial = UtilityHelper.ChooseWeightedArchetype(viableArchetypes);
                //If the initial archetype fits, then we need to generate its children
                if (initial != null)
                {
                    UtilityHelper.StandardiseObjectName(initial.gameObject);
                    PathManager.InitialisePath(initial.gameObject, null);
                    //Generate the initial children
                    List<RoomArchetype> initialChildren = GenerateInitialChildren(initial.Doors.Count - 2, initial.Doors, initial);
                    if (initialChildren != null)
                        return initial;
                    else
                    {
                        //Add the unviable archetype to the list
                        UnviableArchetypeNames.Add(initial.gameObject.name);
                        //Destroy the invalid initial archetype, then recursively call the function again
                        DestroyImmediate(initial.gameObject);
                        return DoInitialGeneration();
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Does generation loop for each door in the parent archetype.
        /// If exceeds the maximum dead ends, need to pick a different archetype
        /// </summary>
        /// <param name="maxDeadEnds">The maximum number of dead ends the parent can have</param>
        /// <param name="doors">The doors to be considered for generation</param>
        /// <param name="parent">The archetype the doors belong to</param>
        /// <returns>A list of all generated room archetypes, or null if they do not fit the criteria</returns>
        public List<RoomArchetype> GenerateInitialChildren(int maxDeadEnds, List<GameObject> doors, RoomArchetype parent)
        {
            //Do subsequent generation for all doors in the archetype
            List<RoomArchetype> archetypes = MainAllocator.DoGenerationForDoors(doors, parent);
            int deadEndCount = 0;
            //Count all dead ends in the parent
            foreach (GameObject doorObj in parent.Doors)
            {
                Door door = doorObj.GetComponent<Door>();
                if (door.IsDeadEnd)
                    deadEndCount++;
            }
            //If dead end count exceeds the maximum, we need to delete all generated archetypes
            //And also return null to signal to the initial generator that we need a new archetype
            if (deadEndCount > maxDeadEnds)
            {
                foreach (RoomArchetype archetype in archetypes)
                    DestroyImmediate(archetype.gameObject);
                return null;
            }
            return archetypes;
        }

        /// <summary>
        /// Tries to pack all reference archetypes at the camera location
        /// </summary>
        /// <returns>A list of all viable archetypes that fit</returns>
        private List<RoomArchetype> GetViableInitialArchetypes()
        {
            List<RoomArchetype> viableArchetypes = new List<RoomArchetype>();
            //Iterate through all archetype prefabs
            foreach (GameObject archetypeObj in MainAllocator.Archetypes)
            {
                if (!UnviableArchetypeNames.Contains(archetypeObj.name))
                {
                    RoomArchetype copyArchetype = Instantiate(archetypeObj).GetComponent<RoomArchetype>();
                    //Instantiate each in turn, checking that they fit into the geometry
                    if (TryInitialPack(copyArchetype))
                    {
                        viableArchetypes.Add(copyArchetype);
                    }
                    else
                    {
                        UnviableArchetypeNames.Add(UtilityHelper.GetStandardisedObjectName(copyArchetype.gameObject));
                        Destroy(copyArchetype.gameObject);
                    }
                }
            }
            return viableArchetypes;
        }

        /// <summary>
        /// Attempts to fit the archetype, moving a random spawn point to the camera position
        /// </summary>
        /// <param name="archetype">The archetype to be packed</param>
        /// <returns>True if the archetype fits, false if it doesn't</returns>
        private bool TryInitialPack(RoomArchetype archetype)
        {
            CameraManager.AlignCameraAndSpawnInArchetype(archetype);
            float rotation = 0;
            while (rotation < 360)
            {
                //Check if it can be placed in the geometry
                if (MainAllocator.GeometryManager.CheckRoomCanBePlaced(archetype))
                    return true;
                else
                {
                    rotation += 60;
                    archetype.gameObject.transform.Rotate(Vector3.up, 60);
                }
            }
            return false;
        }
    }
}