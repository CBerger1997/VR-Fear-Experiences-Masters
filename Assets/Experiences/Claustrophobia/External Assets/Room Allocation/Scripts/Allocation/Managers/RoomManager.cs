using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;

namespace RoomAllocation
{
    /// <summary>
    /// Handles the management of rooms, including linking archetypes to rooms and room instantiation.
    /// </summary>
    public class RoomManager : MonoBehaviour
    {
        /// <summary>
        /// How many times the manager will attempt to find a viable room
        /// It will use an unviable room if this limit is exceeded
        /// </summary>
        public int roomAttemptLimit = 20;

        /// <summary>
        /// A list of all loaded room prefabs
        /// </summary>
        public List<GameObject> Rooms { get; set; }

        /// <summary>
        /// Records all archetype prefabs, and what room prefabs they correspond to for easy access
        /// </summary>
        public List<Tuple<GameObject, List<GameObject>>> ArchetypeRoomList = null;

        public void Awake()
        {
            ArchetypeRoomList = new List<Tuple<GameObject, List<GameObject>>>();
            Rooms = new List<GameObject>(Resources.LoadAll<GameObject>(AllocationConstants.ROOM_PREFAB_PATH));
            if (Rooms.Count <= 0)
                Debug.LogError("Rooms are null or none have been loaded");
        }

        /// <summary>
        /// Links all unique archetypes to all rooms that relate to that archetype
        /// </summary>
        /// <param name="referenceArchetypes">A list of generated reference archetypes</param>
        public void LinkArchetypesAndRooms(List<GameObject> referenceArchetypes)
        {
            ArchetypeRoomList.Clear();

            //Iterate through all reference archetypes
            foreach (GameObject archetypeObj in referenceArchetypes)
            {
                List<GameObject> associatedRooms = new List<GameObject>();
                foreach (GameObject roomObj in Rooms)
                {
                    //Compare names, if the same then link by the archetype
                    GameObject roomArchetype = roomObj.GetComponent<Room>().assignedArchetype.gameObject;
                    if (roomArchetype != null)
                    {
                        if (UtilityHelper.StandardiseObjectName(roomArchetype)
                            == archetypeObj.name)
                        {
                            associatedRooms.Add(roomObj);
                        }
                    }
                }

                //Add the link between the archetype object and all of its corresponding room prefabs
                ArchetypeRoomList.Add(new Tuple<GameObject, List<GameObject>>(archetypeObj, associatedRooms));
            }

            Debug.Log("Number of archetypes added to RoomManager: " + ArchetypeRoomList.Count);
            int roomCount = 0;
            foreach (Tuple<GameObject, List<GameObject>> tpl in ArchetypeRoomList)
            {
                foreach (GameObject roomObj in tpl.Item2)
                    roomCount++;
            }

            Debug.Log("Number of rooms added to RoomManager: " + roomCount);
        }

        /// <summary>
        /// Finds the corresponding archetype entry.
        /// Instantiates a random room from the rooms associated with the found archetypes
        /// </summary>
        /// <param name="archetypeObj">The archetype that needs to be generated for</param>
        /// <returns>the generated object if successful, null otherwise</returns>
        public GameObject InstantiateRandomRoomForArchetype(GameObject archetypeObj)
        {
            foreach (Tuple<GameObject, List<GameObject>> tpl in ArchetypeRoomList)
            {
                //Find the tuple entry associated with the archetype object
                if (archetypeObj.name == tpl.Item1.name)
                {
                    GameObject archetypeFound = tpl.Item1;
                    List<GameObject> roomsFound = new List<GameObject>(tpl.Item2);
                    if (roomsFound != null && archetypeFound != null)
                    {
                        //Track the number of unviable rooms generated
                        int notViableCount = 0;
                        while (notViableCount < roomAttemptLimit)
                        {
                            //Choose a random room
                            GameObject randomRoomPrefab = UtilityHelper.ChooseRandomObject(roomsFound, false);
                            if (randomRoomPrefab != null)
                            {
                                RoomArchetype archetype = archetypeObj.GetComponent<RoomArchetype>();
                                GameObject instantiatedRoom = Instantiate(randomRoomPrefab, archetypeObj.transform.GetChild(1));
                                if (instantiatedRoom != null)
                                {
                                    //Test that the generated room is viable
                                    Room r = instantiatedRoom.GetComponent<Room>();
                                    if (!r.isViable && roomsFound.Count > 1)
                                    {
                                        //Keep attempting to generate viable rooms whilst not viable rooms are generated
                                        //And the count has not met the attempt limit
                                        notViableCount++;
                                        //Remove from the copy list
                                        roomsFound.Remove(randomRoomPrefab);
                                        Destroy(instantiatedRoom);
                                    }
                                    else
                                    {
                                        //Update the archetype's assigned room
                                        //Link all doorpoints in the room to doors in the archetype
                                        archetype.AssociatedRoom = instantiatedRoom;
                                        instantiatedRoom.GetComponent<Room>().LinkDoorsToDoorPoints(archetype.Doors);
                                        if (instantiatedRoom.GetComponent<Room>().unique)
                                        {
                                            //Remove from the stored list
                                            tpl.Item2.Remove(randomRoomPrefab);
                                            Debug.Log("Removed unique room, there are now " + tpl.Item2.Count + " rooms of type " + archetypeObj.name);
                                        }
                                        return instantiatedRoom;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }
    }
}