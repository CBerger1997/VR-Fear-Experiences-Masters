using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoomAllocation
{
    /// <summary>
    /// Handles the overarching functionality of the allocation system
    /// </summary>
    public class AllocationManager : MonoBehaviour
    {
        public int targetFPS = 60;

        public RoomAllocator RoomAllocator { get; set; }

        public void Awake()
        {
            RoomAllocator = gameObject.GetComponent<RoomAllocator>();
            //Obtain the input manager component and set a reference to this in it
            gameObject.GetComponent<InputManager>().AllocationManager = this;
            //Lock the target framerate
            Application.targetFrameRate = targetFPS;
        }

        private void Start()
        {
            //Update the floor and camera level to reflect the desired y position
            if(!RoomAllocator.GeometryManager.isVR)
            {
                AllocationConstants.FLOOR_LEVEL += RoomAllocator.GeometryManager.areaOrigin.y;
                AllocationConstants.CAMERA_HEIGHT += RoomAllocator.GeometryManager.areaOrigin.y;
            }

        }

        /// <summary>
        /// Performs the setup of the area geometry, and packs the initial room
        /// </summary>
        /// <returns>The initial packed room</returns>
        public RoomArchetype SetupArea()
        {
            //Check if the geometry manager has been set to vr mode
            if (RoomAllocator.GeometryManager.isVR)
            {
                //Force that the starting position is always the position of the camera in VR
                RoomAllocator.CameraManager.startPos = CameraManager.StartingPosition.CameraPosition;
                if (RoomAllocator.GeometryManager.Geometry != null && RoomAllocator.GeometryManager.Geometry.Count > 0)
                    Debug.Log("VR Geometry successfully obtained, with " + RoomAllocator.GeometryManager.Geometry.Count + " points");
                else
                {
                    Debug.LogError("VR Geometry not defined, aborting...");
                    return null;
                }
            }
            else
            {
                //Ensure input is valid
                DoSanityChecks();
                //Clear the scene of any residuals
                ResetArea();
                //Create the area
                RoomAllocator.GeometryManager.CreateArea();
                //Set the min/max positions of the area
                RoomAllocator.GeometryManager.SetMinMaxPositions();
                //Move camera to its initial starting position for the generated room
                MoveCameraToInitialPosition();
            }

            //Pack the initial room and return the result
            return RoomAllocator.PackInitialRoom();
        }

        /// <summary>
        /// Moves the camera to an initial position, as specified in the camera manager
        /// </summary>
        private void MoveCameraToInitialPosition()
        {
            CameraManager cm = RoomAllocator.CameraManager;

            //Default to centre if undefined
            switch (cm.startPos)
            {
                case (CameraManager.StartingPosition.Centre): cm.MoveCameraToRoomCentre(RoomAllocator.GeometryManager); break;
                case (CameraManager.StartingPosition.Corner): cm.MoveCameraToCornerOfRoom(cm.cornerOffset, RoomAllocator.GeometryManager); break;
                case (CameraManager.StartingPosition.CameraPosition): break;
                default: cm.MoveCameraToRoomCentre(RoomAllocator.GeometryManager); break;
            }
        }

        /// <summary>
        /// Clean up any residual objects left from a previous generation.
        /// Destroys the test area, and its subsequent sphere child objects.
        /// Destroys all room archetypes.
        /// </summary>
        public void ResetArea()
        {
            //Including the previos test area and all archetypes
            RoomAllocator.InitialAllocator.UnviableArchetypeNames.Clear();
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag(AllocationConstants.TESTAREA_TAG_NAME))
                DestroyImmediate(obj);

            foreach (RoomArchetype archetype in FindObjectsOfType<RoomArchetype>())
            {
                DestroyImmediate(archetype.gameObject);
            }

            RoomAllocator.Awake();
        }

        /// <summary>
        /// Runs sanity checks on relevant managers and their assigned values
        /// </summary>
        private void DoSanityChecks()
        {
            RoomAllocator.GeometryManager.DoSanityChecks();
            RoomAllocator.CameraManager.DoSanityChecks();
        }
    }
}