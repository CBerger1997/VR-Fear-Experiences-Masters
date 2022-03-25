using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RoomAllocation
{
    /// <summary>
    /// Performs various utility functions, such as selecting a random object/component from a list
    /// </summary>
    public class UtilityHelper : MonoBehaviour
    {
        /// <summary>
        /// Chooses a random T value from the list
        /// </summary>
        /// <typeparam name="T">The type of component to choose</typeparam>
        /// <param name="listOfComponents">The list of the componenets to choose from</param>
        /// <returns>The chosen component</returns>
        public static T ChooseRandomComponent<T>(List<T> listOfComponents) where T : Component
        {
            if (listOfComponents.Count > 0)
            {
                //Choose a random room to spawn
                int randomIndex = Random.Range(0, listOfComponents.Count);
                return listOfComponents[randomIndex];
            }
            return null;
        }

        /// <summary>
        /// Chooses a random object
        /// </summary>
        /// <param name="listOfObjects">The list of objects to choose from</param>
        /// <param name="destroyNotSelected">If objects not selected should be destroyed or not</param>
        /// <returns>The chosen object</returns>
        public static GameObject ChooseRandomObject(List<GameObject> listOfObjects, bool destroyNotSelected)
        {
            if (listOfObjects.Count > 0)
            {
                //Choose a random room to spawn
                int randomIndex = Random.Range(0, listOfObjects.Count);
                GameObject selected = listOfObjects[randomIndex];
                //Delete all that were not selected
                if (destroyNotSelected)
                {
                    listOfObjects.RemoveAt(randomIndex);
                    foreach (GameObject obj in listOfObjects) { DestroyImmediate(obj); }
                }
                return selected;
            }
            return null;
        }

        /// <summary>
        /// Chooses an random archetype but takes into account weightings to have specific biases.
        /// Maintains a list that store x amount of references to an archetype, where x is the archetype weighting,
        ///  then randomly selects an archetype from that list.
        /// </summary>
        /// <param name="archetypes">The archetypes to choose from</param>
        /// <returns>The chosen weighted archetype</returns>
        public static RoomArchetype ChooseWeightedArchetype(List<RoomArchetype> archetypes)
        {
            if (archetypes.Count > 0)
            {
                List<RoomArchetype> weightedArchetypes = new List<RoomArchetype>();
                //Add the archetype equal to the number of its weighting
                //i.e. 2 weighting results in 2 entries
                foreach (RoomArchetype archetype in archetypes)
                {
                    int weight = archetype.selectionWeighting;
                    for (int i = 0; i < weight; i++)
                        weightedArchetypes.Add(archetype);
                }
                //Choose a random archetype, then destroy the ones not chosen
                RoomArchetype chosen = ChooseRandomComponent(weightedArchetypes);
                foreach (RoomArchetype archetype in archetypes)
                    if (archetype != chosen) { DestroyImmediate(archetype.gameObject); }
                return chosen;
            }
            return null;
        }

        /// <summary>
        ///This script was obtained from PolyContainsPoint
        ///Created by Eric Haines
        ///Which can be found here:
        ///http://wiki.unity3d.com/index.php?title=PolyContainsPoint&_ga=2.140270256.2059921158.1614448564-750362166.1608161750
        ///Determines if a point is located inside a set of objects as a polygon
        /// </summary>
        /// <param name="p">The point to test</param>
        /// <param name="points">The objects that dictate the area to be tested</param>
        /// <returns>True if the point is within the polygon, false otherwise</returns>

        public static bool IsPositiontWithinPointPolygon(Vector3 p, List<GameObject> points)
        {
            var j = points.Count - 1;
            var inside = false;
            for (int i = 0; i < points.Count; j = i++)
            {
                var pi = points[i].transform.position;
                var pj = points[j].transform.position;
                if (((pi.z <= p.z && p.z < pj.z) || (pj.z <= p.z && p.z < pi.z)) &&
                    (p.x < (pj.x - pi.x) * (p.z - pi.z) / (pj.z - pi.z) + pi.x))
                    inside = !inside;
            }
            return inside;
        }

        /// <summary>
        /// Determines if a point is located inside a set of points, derived from point polygon function
        /// </summary>
        /// <param name="p">The point to test</param>
        /// <param name="points">The points that dicatate the area to be tested</param>
        /// <returns>True if the point is within the points, false otherwise</returns>
        public static bool IsPositionWithinVectors(Vector3 p, List<Vector3> points)
        {
            var j = points.Count - 1;
            var inside = false;
            for (int i = 0; i < points.Count; j = i++)
            {
                var pi = points[i];
                var pj = points[j];
                if (((pi.z <= p.z && p.z < pj.z) || (pj.z <= p.z && p.z < pi.z)) &&
                    (p.x < (pj.x - pi.x) * (p.z - pi.z) / (pj.z - pi.z) + pi.x))
                    inside = !inside;
            }
            return inside;
        }

        /// <summary>
        /// Calculates the opposing angle of the origin angle in degrees
        /// E.g. opposing of 180 is 0.
        /// </summary>
        /// <param name="originY"></param>
        /// <returns>The opposing angle</returns>
        public static float GetOppositeAngle(float originY)
        {
            float opposite = originY + 180;
            //Keep the angle between 0-360, the angles of the door rotation

            return opposite % 360;
        }

        /// <summary>
        /// Calculates the (average) centre of a list of vectors
        /// </summary>
        /// <param name="vectors">The vectors to be used in calculating the centre</param>
        /// <returns>The centre of the list of vectors</returns>
        public static Vector3 CalculateCentroidOfVectors(List<Vector3> vectors)
        {
            float xSum = 0;
            float zSum = 0;
            //Sum all vectors in geometry
            foreach (Vector3 v in vectors)
            {
                xSum += v.x;
                zSum += v.z;
            }
            //Find averages of both sums
            float xAvg = xSum / vectors.Count;
            float zAvg = zSum / vectors.Count;
            //Calculate centre
            return new Vector3(xAvg, AllocationConstants.FLOOR_LEVEL, zAvg);
        }

        /// <summary>
        /// Finds all archetypes and tests which ones are rendered
        /// </summary>
        /// <returns>Returns a list of all the currently rendered archetypes</returns>
        public static List<GameObject> GetAllRenderedArchetypes()
        {
            List<GameObject> archetypes = new List<GameObject>();
            RoomArchetype[] allArchetypes = FindObjectsOfType<RoomArchetype>();

            foreach (RoomArchetype archetype in allArchetypes)
            {
                if (archetype != null && archetype.IsRendered())
                    archetypes.Add(archetype.gameObject);
            }

            return archetypes;
        }

        /// <summary>
        /// Editor functino to render an archetype outside of play mode
        /// </summary>
        /// <param name="archetypePrefab">The archetype to render</param>
        /// <param name="spawnGizmoColour">The colour of the spawn object</param>
        /// <param name="cornerGizmoColour">The colour of the corner object</param>
        /// <param name="doorGizmoColour">The colour of the door object</param>
        public static void RenderArchetype(GameObject archetypePrefab, Color spawnGizmoColour, Color cornerGizmoColour, Color doorGizmoColour)
        {
            foreach (Transform t in archetypePrefab.transform.GetChild(0))
            {
                if (t.gameObject.CompareTag(AllocationConstants.SPAWN_TAG_NAME))
                {
                    Gizmos.color = spawnGizmoColour;
                    Gizmos.DrawSphere(t.position, 0.05f);
                }
                else if (t.gameObject.CompareTag(AllocationConstants.CORNER_TAG_NAME))
                {
                    Gizmos.color = cornerGizmoColour;
                    Gizmos.DrawSphere(t.position, 0.05f);
                }
                else if (t.gameObject.CompareTag(AllocationConstants.DOOR_TAG_NAME))
                {
                    Gizmos.color = doorGizmoColour;
                    Gizmos.DrawSphere(t.position, 0.05f);
                    DrawDoorArrow(t, doorGizmoColour);
                }
            }
        }

        /// <summary>
        /// Draws an arrow for a particular transform
        /// </summary>
        /// <param name="t">The transform the arrow will be drawn to</param>
        /// <param name="colour">The colour of the arrow</param>
        public static void DrawDoorArrow(Transform t, Color colour)
        {
            DrawArrow.ForGizmo(
                    t.position,
                    t.forward * 0.2f,
                    colour,
                    0.1f, 20
                );
        }

        /// <summary>
        /// Splits an object name by spaces and sets it to the first 3 words
        /// </summary>
        /// <param name="obj">The object whose name will be standardised</param>
        /// <returns>The new object name</returns>
        public static string StandardiseObjectName(GameObject obj)
        {
            obj.name = GetStandardisedObjectName(obj);
            return obj.name;
        }

        /// <summary>
        /// Calculates the standardised object name
        /// </summary>
        /// <param name="obj">The object whose standardised name is being calculated</param>
        /// <returns>The standardised name of the object</returns>
        public static string GetStandardisedObjectName(GameObject obj)
        {
            string newName = "";

            //Split name by spaces
            string[] splitName = obj.name.Split(' ');

            for (int i = 0; i < splitName.Length; i++)
            {
                if (i < 3)
                    newName += splitName[i] + " ";
                else
                    break;
            }
            return newName;
        }

        /// <summary>
        /// Shuffles a list of type t using the Fisher-Yates shuffle
        /// </summary>
        /// <typeparam name="T">The type of the list items to be shuffled</typeparam>
        /// <param name="list">The list to be shuffled</param>
        /// <returns>The shuffled list</returns>
        public static List<T> ShuffleList<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                T temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }

            return list;
        }
    }
}