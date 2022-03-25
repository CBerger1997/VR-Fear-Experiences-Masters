using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoomAllocation
{
    /// <summary>Maintains and manages the current path the user is on </summary>
    public class PathManager : MonoBehaviour
    {
        /// <summary>The root node of the tree </summary>
        public AllocationTree Path { get; set; }

        /// <summary>The archetype the user is currently in </summary>
        public RoomArchetype CurrentArchetype { get; set; }

        /// <summary>The archetype the user was previously in</summary>
        public RoomArchetype PreviousArchetype { get; set; }

        ///<summary>The archetpye the system has predicted the user will enter next (the closest non-current)</summary>
        public RoomArchetype NextArchetype { get; set; }

        ///<summary>Initialises/resets the path with the provided objects creating a new root node</summary>
        ///<param name="archetypeObject"> The archetype object to be associated with the new root node</param>
        ///<param name="roomObject">The room object to be associated with the new root node</param>
        public void InitialisePath(GameObject archetypeObject, GameObject roomObject)
        {
            Path = new AllocationTree(archetypeObject, roomObject, null);
        }

        /// <summary>
        /// If the camera is looking at a doorpoint, this will replace the associated archetype
        /// and will also update the path to put the current archetype as its root.
        /// Will delete all subsequent archetypes and rooms that are subtrees of the deleted node.
        /// </summary>
        /// <param name="cm">The current instance of the camera manager to obtain the main camera transform</param>
        public void ReplaceArchetype(CameraManager cm)
        {
            //Check if ray hits a door
            RaycastHit rayHit;
            Ray camRay = new Ray(cm.GetCameraPosition(), cm.MainCamera.transform.forward);
            if (Physics.Raycast(camRay, out rayHit, 500))
            {
                Debug.DrawRay(cm.GetCameraPosition(), cm.MainCamera.transform.forward);
                GameObject hitObj = rayHit.collider.gameObject;
                DoorPoint dp = hitObj.transform.GetComponentInParent<DoorPoint>();
                //Find the corresponding door point
                if (dp != null)
                {
                    Door associatedDoor = dp.AssociatedDoor.GetComponent<Door>();
                    //Ensure there is an archetype to be replaced, and that the door is not open
                    if (associatedDoor.ArchetypeAssignedTo != null
                        && !associatedDoor.Open)
                    {
                        //Ensure the current is accurate
                        RoomArchetype current = cm.GetArchetypeThatContainsCamera();
                        if (current != null)
                        {
                            //Rework the path to make the current node the root
                            AllocationTree currentNode = Path.FindNodeByArchetype(current.gameObject);
                            currentNode.MakeNodeIntoRoot();
                            Path = currentNode;
                            //Remove the to delete node and all of its children
                            AllocationTree deleteNode = Path.FindNodeByArchetype(associatedDoor.ArchetypeAssignedTo.gameObject);
                            currentNode.RemoveChildrenFromNode(deleteNode);
                            currentNode.Children.Remove(deleteNode);

                            //Find all archetypes that need to be destroyed
                            List<GameObject> toDestroy = new List<GameObject>();
                            foreach (RoomArchetype archetype in FindObjectsOfType<RoomArchetype>())
                            {
                                if (archetype.gameObject.transform.parent == GetComponent<RoomAllocator>().GeneratedParent
                                    && currentNode.FindNodeByArchetype(archetype.gameObject) == null)
                                    toDestroy.Add(archetype.gameObject);
                            }

                            //Destroy them
                            foreach (GameObject obj in toDestroy)
                            {
                                DestroyImmediate(obj);
                            }
                        }
                    }
                }
            }
        }
    }
}