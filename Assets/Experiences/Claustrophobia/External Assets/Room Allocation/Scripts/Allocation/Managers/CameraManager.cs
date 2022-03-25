using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoomAllocation
{
    /// <summary>
    /// Manages and provides functionality relating to the camera (or the player)
    /// </summary>
    public class CameraManager : MonoBehaviour
    {
        /// <summary>
        /// Where the camera is placed in the area for the initial generation
        /// </summary>
        public enum StartingPosition
        {
            Centre,
            Corner,
            CameraPosition
        }

        [Tooltip("Where the camera will be placed in the geometry upon the initial generation. " +
                "\nCorner is offset from the minimum x and z vector in the geometry")]
        public StartingPosition startPos = StartingPosition.Corner;

        [Tooltip("The max distance away from a door the camera has to be to automatically open it. Recommended to set to around 1.5 for base settings" +
            ", or dependent on your camera height ")]
        public float doorOpenDistance = 1.5f;

        [Tooltip("How far the initial camera position is offset from the min x,z corner of the geometry")]
        public float cornerOffset = 0.5f;

        /// <summary>
        /// Camera here refers to the object that contains the camera, or is responsible for its movement
        /// </summary>
        public GameObject MainCamera { get; set; }

        public void Awake()
        {
            MainCamera = GameObject.FindGameObjectWithTag(AllocationConstants.CAMERA_TAG_NAME);
        }

        private void Update()
        {
            //Force camera to only be able to move in x or z
            //CAN SAFELY REMOVE
            MainCamera.transform.position = new Vector3(MainCamera.transform.position.x,
                AllocationConstants.CAMERA_HEIGHT,
                MainCamera.transform.position.z);
        }

        /// <summary>
        ///         //Moves the camera to a random position in the geometry
        /// </summary>
        /// <param name="gm">The associated geometry manager</param>
        public void MoveCameraToRandomPosition(GeometryManager gm)
        {
            SetCameraPosition(new Vector3(
                Random.Range(gm.XMin, gm.XMax),
                AllocationConstants.CAMERA_HEIGHT,
                Random.Range(gm.ZMin, gm.ZMax))
            );
        }

        /// <summary>
        /// Aligns a random spawn point in the room archetype to position of the camera
        /// </summary>
        /// <param name="archetype">The archetype to be moved</param>
        public void AlignCameraAndSpawnInArchetype(RoomArchetype archetype)
        {
            //Camera position is stationary - this is the mechanism to be used in the main system
            //In VR, the player will stand at a particular location, and the rooms will be generated around that
            SetCameraOnFloor();
            archetype.gameObject.transform.position = GetCameraPosition();
            GameObject randomSpawn = UtilityHelper.ChooseRandomObject(archetype.SpawnPoints, false);
            if (randomSpawn != null)
            {
                Vector3 spawnPosition = randomSpawn.transform.position;
                Vector3 spawnToCamera = GetCameraPosition() - spawnPosition;
                archetype.gameObject.transform.position += spawnToCamera;
            }
            SetCameraAtNormalHeight();
        }

        /// <summary>
        /// Moves the camera to a random spawn point in the archetype, as defined in its prefab
        /// </summary>
        /// <param name="archetype">The archetype the camera is being moved into</param>
        public void MoveCameraToRandomSpawnPoint(RoomArchetype archetype)
        {
            GameObject randomSpawn = UtilityHelper.ChooseRandomObject(archetype.SpawnPoints, false);
            if (randomSpawn != null)
            {
                SetCameraPosition(randomSpawn.transform.position);
            }
            SetCameraAtNormalHeight();
        }

        /// <summary>
        /// Moves the camera to the centre of the defined geometry
        /// </summary>
        /// <param name="gm">The associated geometry manager</param>
        public void MoveCameraToRoomCentre(GeometryManager gm)
        {
            Vector3 centre = gm.GetCentreOfGeometry();
            SetCameraPosition(new Vector3(centre.x, AllocationConstants.CAMERA_HEIGHT, centre.z));
        }

        /// <summary>
        /// Moves the camera to the min corner, with a provided offset
        /// </summary>
        /// <param name="cornerOffset">The offset amount from the minimum corner the camera will be placed</param>
        /// <param name="gm">The associated geometry managers</param>
        public void MoveCameraToCornerOfRoom(float cornerOffset, GeometryManager gm)
        {
            Vector3 corner = new Vector3(gm.XMin + cornerOffset, AllocationConstants.CAMERA_HEIGHT, gm.ZMin + cornerOffset);
            SetCameraPosition(corner);
        }

        /// <returns>The current position of the camera</returns>
        public Vector3 GetCameraPosition()
        {
            return MainCamera.transform.position;
        }

        /// <summary>
        /// Sets the current position of the camera
        /// </summary>
        /// <param name="pos">The position that the camera is being set to</param>
        public void SetCameraPosition(Vector3 pos)
        {
            MainCamera.transform.position = pos;
        }

        /// <summary>
        /// Forces the camera position to the floor, keeping its x and z positions
        /// </summary>
        public void SetCameraOnFloor()
        {
            SetCameraPosition(new Vector3(
                GetCameraPosition().x,
                AllocationConstants.FLOOR_LEVEL,
                GetCameraPosition().z)
            );
        }

        /// <summary>
        /// Sets the camera back to its normal height
        /// </summary>
        public void SetCameraAtNormalHeight()
        {
            SetCameraPosition(new Vector3(
                GetCameraPosition().x,
                AllocationConstants.CAMERA_HEIGHT,
                GetCameraPosition().z)
           );
        }

        /// <summary>
        /// Checks all archetypes provided and returns one that contains the camera,
        /// I.e. the one that the user is currently in.
        /// Only checks the currently rendered archetypes
        /// </summary>
        /// <returns>The archetype containing the camera, or null if no archetypes could be found</returns>
        public RoomArchetype GetArchetypeThatContainsCamera()
        {
            foreach (GameObject obj in UtilityHelper.GetAllRenderedArchetypes())
            {
                RoomArchetype archetype = obj.GetComponent<RoomArchetype>();
                if (archetype.IsPositionInArchetypeBounds(GetCameraPosition()))
                    return archetype;
            }
            return null;
        }

        /// <summary>
        /// Finds the closest door that is within the door open distance.
        /// </summary>
        /// <param name="archetype">The archetypes whose doors are to be tested</param>
        /// <returns>The closest door in the archetype, that is not a deadend</returns>
        public Door FindClosestDoorInArchetype(RoomArchetype archetype)
        {
            Door closestDoor = null;
            float closestDist = float.MaxValue;
            if (archetype != null)
            {
                foreach (GameObject doorObj in archetype.Doors)
                {
                    Door door = doorObj.GetComponent<Door>();
                    //Check if the camera is within the specified distance to the door to commit to generation
                    float dist = Vector3.Distance(doorObj.transform.position, GetCameraPosition());
                    if (dist <= doorOpenDistance && !door.IsDeadEnd)
                    {
                        if (dist <= closestDist)
                        {
                            closestDist = dist;
                            closestDoor = doorObj.GetComponent<Door>();
                        }
                    }
                    //Ensure all  doors are closed, will only open the viable door after returning
                    if (door.Open)
                        door.CloseDoor();
                }
            }

            if (closestDoor != null)
            {
                DrawArrow.ForDebug(GetCameraPosition(), closestDoor.transform.position - GetCameraPosition(), Color.white);

                return closestDoor;
            }
            else
                return null;
        }

        /// <summary>
        /// Performs sanity checks on the inspector values of the Camera Manager
        /// </summary>
        public void DoSanityChecks()
        {
            if (startPos == StartingPosition.CameraPosition)
            {
                Debug.LogWarning("Starting the initial generation at the camera position will almost certainly result in no viable allocations." +
                    " Use this position if you are resetting whilst already in the system.");
            }

            if (cornerOffset < 0.5)
            {
                Debug.LogWarning("Having a corner offset of less than 0.5 will likely result in no viable allocations." +
                    " Using 0.5 will place the camera in the centre of the square from the min corner.");
            }
        }
    }
}