using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoomAllocation
{
    public static class AllocationConstants
    {
        /// <summary>
        /// The y value at with the area is based on
        /// </summary>
        public static float FLOOR_LEVEL = 0;

        /// <summary>
        /// The height of the camera, with respect to the floor level
        /// </summary>
        public static float CAMERA_HEIGHT = FLOOR_LEVEL + 1;

        /// <summary>
        /// The name of the parent transform object for generated archetypes
        /// </summary>
        public static string GENERATED_ARCHETYPE_PARENT = "GeneratedArchetypes";

        /// <summary>
        /// The name of the parent transform object for reference archetypes
        /// </summary>
        public static string REFERENCE_ARCHETYPE_PARENT = "ReferenceArchetypes";

        //Tag names for different components used in the system
        //--- General ---//
        /// <summary>
        /// Tag name for test area objects
        /// </summary>
        public static string TESTAREA_TAG_NAME = "TestArea";

        /// <summary>
        /// Tag name for the camera/player to be used in the Camera Manager
        /// </summary>
        public static string CAMERA_TAG_NAME = "MainCamera";

        //--- Archetypes ---//
        /// <summary>
        /// Tag name for the corner in an archetype
        /// </summary>
        public static string CORNER_TAG_NAME = "Corner";

        /// <summary>
        /// Tag name for the doors in an archetype
        /// </summary>
        public static string DOOR_TAG_NAME = "Door";

        /// <summary>
        /// Tag name for the spawns in an archetype
        /// </summary>
        public static string SPAWN_TAG_NAME = "Spawn";

        //--- Rooms ---//
        /// <summary>
        /// Tag name for a room
        /// </summary>
        public static string ROOM_TAG_NAME = "Room";

        /// <summary>
        /// Tag name for a doorpoint in a room
        /// </summary>
        public static string DOORPOINT_TAG_NAME = "Doorpoint";

        /// <summary>
        /// Tag name for a doormask in a room
        /// </summary>
        public static string DOORMASK_TAG_NAME = "DoorMask";

        //Path names
        /// <summary>
        /// Path name that leads to all archetype prefabs
        /// </summary>
        public static string ARCHETYPE_PREFAB_PATH = "Prefabs/Room Archetypes";

        /// <summary>
        /// Path name that leads to all room prefabs
        /// </summary>
        public static string ROOM_PREFAB_PATH = "Prefabs/Rooms";
    }
}