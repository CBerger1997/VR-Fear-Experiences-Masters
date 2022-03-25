using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoomAllocation {
    ///<summary>Manages (keyboard) input for specific interactive functions</summary>
    public class InputManager : MonoBehaviour {
        [Tooltip("The keycode for input that will reset the entire generation process")]
        public KeyCode resetGenerationInput = KeyCode.E;

        [Tooltip("The keycode for input that will replace an archetype if the user is looking at a particular door")]
        public KeyCode replaceArchetypeInput = KeyCode.R;

        public AllocationManager AllocationManager { get; set; }

        private void Start() {
            StartCoroutine(InitialiseRoom());
        }

        public void Update() {
            DoInput();
        }

        /// <summary>
        /// Input loop that tests if keys are dowwn, and calls the relevant function
        /// </summary>
        public void DoInput() {
            //Implement whatever inputs you like here and call the relevant functions
            //Default is to press 'e' to reset
            //Default is to press 'r' to replace
        }

        /// <summary>
        /// Calls the PathManager to replace an archetype with a new one
        /// </summary>
        public void ReplaceArchetype() {
            AllocationManager.RoomAllocator.PathManager.ReplaceArchetype(AllocationManager.RoomAllocator.CameraManager);
        }

        ///<summary>
        ///Destroys residual elements from the previous generation, like the area and archetypes
        ///Recreates the area using the specified values from the inspector
        ///Restart from the beginning
        ///</summary>
        public void ResetGeneration() {
            AllocationManager.SetupArea();
        }

        IEnumerator InitialiseRoom() {
            yield return new WaitForSeconds(1);
            ResetGeneration();
        }
    }
}