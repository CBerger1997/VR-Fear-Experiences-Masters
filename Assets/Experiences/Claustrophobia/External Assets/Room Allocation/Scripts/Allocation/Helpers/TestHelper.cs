using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RoomAllocation;

namespace RoomAllocation
{
    /// <summary>
    /// Simulates user interaction with the system, to obtain data about generation distribution and to see how the system works
    /// </summary>
    public class TestHelper : MonoBehaviour
    {
        private Vector3 nullTargetPosition = new Vector3(-1, -1, -1);

        [Tooltip("x: minimum value \ny: maximum value \nz: size increase after each test")]
        public Vector3 xProperties = new Vector3(2, 5, 1), zProperties = new Vector3(2, 5, 1);

        [Tooltip("How long a particular room size is tested before moving onto the next one")]
        public float timePerRoomSize = 60;

        [Tooltip("How fast the generation test function is invoked, minimum value of 0.05")]
        public float testCycleTime = 0.2f;

        [Tooltip("Move to the next room size if there are no more possible moves (i.e. stuck in the initial room")]
        public bool moveOnIfStuck = false;

        [Tooltip("Instead of cycling through all sizes, test the minimum size repeatedly")]
        public bool repeatMinSize = false;

        [Tooltip("If repeatMinSize enabled, how many times the minimum room size will be tested before the test ends")]
        public int numberOfMinRepetitions = 5;

        /// <summary>
        /// The door being moved towards
        /// </summary>
        public Door TargetDoor { get; set; }

        /// <summary>
        /// If testing is currently active, not regenerating room etc.
        /// </summary>
        public bool Testing { get; set; }

        /// <summary>
        /// Whether the tester is moving backwards through the system or not
        /// </summary>
        public bool Backtracking { get; set; }

        public AllocationManager AllocationManager { get; set; }
        public RoomAllocator RoomAllocator { get; set; }
        public CameraManager CameraManager { get; set; }
        public PathManager PathManager { get; set; }

        /// <summary>
        /// Contains all size combinations of the rooms to be tested
        /// </summary>
        public Queue<Vector2> TestAreaSizeVectors { get; set; }

        /// <summary>
        /// The maximum time for the whole test
        /// </summary>
        public float TotalTestTime { get; set; }

        /// <summary>
        /// The maximum time for a particular room size test
        /// </summary>
        public float RelativeMaxTime { get; set; }

        /// <summary>
        /// The number of total room size combinations to be tested
        /// </summary>
        public int TotalRoomSizes { get; set; }

        public void Start()
        {
            AllocationManager = gameObject.GetComponent<AllocationManager>();
            RoomAllocator = gameObject.GetComponent<RoomAllocator>();
            CameraManager = gameObject.GetComponent<CameraManager>();
            PathManager = gameObject.GetComponent<PathManager>();
            DoSanityChecks();
            SetupSizeVectors();
            TotalRoomSizes = TestAreaSizeVectors.Count;

            //Calculate the approximate test time
            TotalTestTime = TotalRoomSizes * timePerRoomSize;
            Debug.Log("Approximate test time = " + TotalTestTime);
            //Initiate the repeating test
            InvokeRepeating(nameof(DoGenerationTest), 0, testCycleTime);
        }

        //This test will more accuractly simulate the ability of the user
        //Mainly, the system will allow for backtracking to previously generated rooms
        //In order to go through missed doors at a particular room
        //This is more representative of how the system will work in practice
        private void DoGenerationTest()
        {
            int currentTime = Mathf.FloorToInt(Time.time);
            if (currentTime <= TotalTestTime)
            {
                //Reset and create the next test area
                if (!Testing && TestAreaSizeVectors.Count > 0)
                {
                    if (SetupNextTestArea() != null)
                    {
                        Testing = true;
                        Backtracking = false;
                        //Calculate finish time for the new area
                        RelativeMaxTime = currentTime + timePerRoomSize;
                        return;
                    }
                    else
                    {
                        Debug.LogError("Testing could not find an initial room, moving on");
                        SetupNextTestArea();
                    }
                }
                //Check that test time per area is measured
                //Testing can either end if no further moves can be made
                //Or time for a particular area is up
                if (currentTime >= RelativeMaxTime)
                {
                    if (TestAreaSizeVectors.Count <= 0)
                    {
                        //Test for last room
                        Debug.Log("No more test areas in the size vector");
                        return;
                    }
                    else
                    {
                        Debug.Log("Ending test phase, allocated time for room is up");
                        Testing = false;
                        return;
                    }
                }

                MoveToDoor();
            }
            else
            {
                //Cancel repeating function
                CancelInvoke();
                Debug.Log("Test has ended");
            }
        }

        /// <summary>
        /// Performs the required simulation steps to move the camera to a transition door.
        /// Finds a particular transition door.
        /// Initiates backtracking if necessary
        /// </summary>
        private void MoveToDoor()
        {
            if (!Backtracking)
            {
                if (TargetDoor != null)
                {
                    MoveToDoorPosition();
                }
                else
                {
                    //Find the target door
                    TargetDoor = FindTransitionDoor();
                    //If null then no possible transitions can be made from the current archetype
                    //So initiate backtracking
                    if (TargetDoor == null)
                    {
                        Backtracking = true;
                        RoomArchetype current = CameraManager.GetArchetypeThatContainsCamera();
                        //Find the parent door and move towards it
                        if (current.ParentDoor != null)
                        {
                            current.ParentDoor.GetComponent<Door>().IsDeadEnd = true;
                            TargetDoor = current.ParentDoor.GetComponent<Door>();
                            //Force the parent to be rendered, and also disable the current rendering
                            current.ParentDoor.GetComponent<Door>().ArchetypeAssignedTo.EnableRendering();
                            current.DisableRendering();
                            if (TargetDoor.DoorMirror != null)
                                TargetDoor.DoorMirror.GetComponent<Door>().IsDeadEnd = true;
                        }
                    }
                }
            }
            else
            {
                if (TargetDoor == null)
                {
                    //Find the parent door of the current archetype
                    GameObject parent = CameraManager.GetArchetypeThatContainsCamera().ParentDoor;
                    //If not null then we can backtrack
                    if (parent != null)
                    {
                        TargetDoor = parent.GetComponent<Door>();
                    }
                    else
                    {
                        //A null parent means that the user is in the initial room
                        TargetDoor = FindTransitionDoor();
                        if (TargetDoor == null)
                        {
                            Debug.Log("No more possible moves can be made from the initial room");
                            Testing = false;
                        }
                        else
                            Backtracking = false;
                        return;
                    }
                }
                else
                    MoveToDoorPosition();
            }
        }

        /// <summary>
        /// Simulates user movement by gradually moving the camera towards the position of the target door
        /// </summary>
        private void MoveToDoorPosition()
        {
            MoveCamera(TargetDoor.gameObject.transform.position);
            //If so, then ensure the archetype has been rendered to avoid anything weird happening
            if (TargetDoor.ArchetypeAssignedTo != null)
            {
                if (TargetDoor.ArchetypeAssignedTo.IsRendered())
                {
                    //Move to a spawn point of the assigned archetype
                    CameraManager.MoveCameraToRandomSpawnPoint(TargetDoor.ArchetypeAssignedTo);
                    //Force dead end on target doors so they are not selected
                    RoomArchetype current = CameraManager.GetArchetypeThatContainsCamera();
                    if (current.ParentDoor != null)
                        current.ParentDoor.GetComponent<Door>().IsDeadEnd = true;
                    //Set target to null so a new one is generated on the next pass
                    TargetDoor = null;
                    Backtracking = false;
                }
            }
        }

        /// <summary>
        /// Finds a door to move towards
        /// </summary>
        /// <returns></returns>
        private Door FindTransitionDoor()
        {
            RoomArchetype current = CameraManager.GetArchetypeThatContainsCamera();
            if (current != null)
            {
                List<GameObject> possible = new List<GameObject>();
                foreach (GameObject doorObj in current.Doors)
                {
                    Door door = doorObj.GetComponent<Door>();
                    //Check that it is not a dead end or doesn't lead to a dead end
                    //Also check that it is not the door we just entered from
                    //Only want to return if no other options available
                    if (!door.IsDeadEnd)
                    {
                        possible.Add(doorObj);
                    }
                }
                GameObject randomDoor = UtilityHelper.ChooseRandomObject(possible, false);
                if (randomDoor != null)
                {
                    return randomDoor.GetComponent<Door>();
                }
            }
            return null;
        }

        /// <summary>
        /// Creates the next area for testing
        /// </summary>
        /// <returns></returns>
        private RoomArchetype SetupNextTestArea()
        {
            Vector2 size = TestAreaSizeVectors.Dequeue();
            RoomAllocator.GeometryManager.AreaX = size.x;
            RoomAllocator.GeometryManager.AreaZ = size.y;
            Debug.Log("Starting new test area with width " + size.x + " and height " + size.y);
            return AllocationManager.SetupArea();
        }

        /// <summary>
        /// Sets up the queue with different room sizes based on provided limits to be used in the testing
        /// </summary>
        private void SetupSizeVectors()
        {
            TestAreaSizeVectors = new Queue<Vector2>();
            //Add minimum repeats for testing specific area sizes
            if (repeatMinSize && numberOfMinRepetitions > 0)
            {
                for (int i = 0; i < numberOfMinRepetitions; i++)
                {
                    TestAreaSizeVectors.Enqueue(new Vector2(xProperties.x, zProperties.x));
                }
            }
            else //Otherwise iterate through each size individually
            {
                for (float i = xProperties.x; i <= xProperties.y; i += xProperties.z)
                {
                    for (float j = zProperties.x; j <= zProperties.y; j += zProperties.z)
                        TestAreaSizeVectors.Enqueue(new Vector2(i, j));
                }
            }
        }

        /// <summary>
        /// Moves the camera towards the target vector
        /// </summary>
        /// <param name="target">The position the camera is to move towards</param>
        private void MoveCamera(Vector3 target)
        {
            CameraManager.MainCamera.transform.position = Vector3.MoveTowards(
                CameraManager.GetCameraPosition(),
                target,
                0.5f
            );
        }

        /// <summary>
        /// Performs test value sanity checks
        /// </summary>
        private void DoSanityChecks()
        {
            //Enforce that testing and backtracking are both false at the start
            Backtracking = false;
            Testing = false;

            //Check if min and max are defined correctly
            if (xProperties.x > xProperties.y)
            {
                Debug.LogError("Oops, you got your min and max x values the wrong way round! " +
                    "Remember - x represents the minimum, y the maximum and z the size increase after each iteration.");
                float tempX = xProperties.x;
                xProperties.x = xProperties.y;
                xProperties.y = tempX;
            }

            if (zProperties.x > zProperties.y)
            {
                Debug.LogError("Oops, you got your min and max z values the wrong way round! " +
                    "Remember - x represents the minimum, y the maximum and z the size increase after each iteration.");
                float tempX = zProperties.x;
                zProperties.x = zProperties.y;
                zProperties.y = tempX;
            }

            //Check that the cycle time doesn't result in a freeze of the test, as invoking by 0 seconds doesn't invoke at all
            if (testCycleTime <= 0)
            {
                Debug.LogError("Setting the cycle time to 0 or less will make this test not run. That's bad. Setting to 0.2s");
                testCycleTime = 0.2f;
            }

            //Check that the cycle time is appropriate
            //A difference of about 0.05s seems to stop weird things from happening
            if (testCycleTime <= RoomAllocator.generationLoopRate)
            {
                testCycleTime = RoomAllocator.generationLoopRate + 0.08f;
                Debug.LogError("Setting the cycle too close to or less than the generation loop rate is too fast" +
                    " and will lead to consequences. Changing the cycle time to " + testCycleTime);
            }

            //Check that the minimum/maximum values don't lead to an empty area
            if (xProperties.x <= 0)
            {
                Debug.LogError("Your minimum x value cannot be 0 or less. " +
                    "Automatically adjusting to 2.");
                xProperties.x = 2;
            }

            if (xProperties.y <= 0)
            {
                Debug.LogError("Your maximum x value cannot be 0 or less. " +
                    "Automatically adjusting to " + (xProperties.x + 2) + ".");
                xProperties.y = xProperties.x + 2;
            }

            if (zProperties.x <= 0)
            {
                Debug.LogError("Your minimum z value cannot be 0 or less. " +
                    "Automatically adjusting to 2.");
                zProperties.x = 2;
            }

            if (zProperties.y <= 0)
            {
                Debug.LogError("Your maximum z value cannot be 0 or less. " +
                    "Automatically adjusting to " + (zProperties.x + 2) + ".");
                zProperties.y = zProperties.x + 2;
            }
        }
    }
}