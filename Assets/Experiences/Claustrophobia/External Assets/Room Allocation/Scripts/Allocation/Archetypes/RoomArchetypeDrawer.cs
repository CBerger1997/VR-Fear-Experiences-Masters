using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoomAllocation
{
    /// <summary>
    /// Used to visualise a particular room archetype using a Line Renderer
    /// </summary>
    public class RoomArchetypeDrawer : MonoBehaviour
    {
        public Color cornerGizmoColour = Color.green,
            spawnGizmoColour = Color.red,
            doorGizmoColour = Color.yellow;

        public LineRenderer LineRenderer { get; set; }

        public void Awake()
        {
            LineRenderer = gameObject.GetComponent<LineRenderer>();
            LineRenderer.positionCount = 0;
            LineRenderer.startWidth = 0.1f;
            LineRenderer.endWidth = 0.1f;
        }

        ///<summary>Draws the archetype using the Line Renderer, using only the corners</summary>
        ///<param name="pointObjects">The points (in order) to be used to render the archetype</param>
        public void DrawArchetype(List<GameObject> pointObjects)
        {
            //Add positions of point objects to array to set positions of line renderer
            List<Vector3> rendererPoints = new List<Vector3>();

            for (int i = 0; i < pointObjects.Count; i++)
            {
                GameObject obj = pointObjects[i];
                if (obj.CompareTag(AllocationConstants.CORNER_TAG_NAME))
                    rendererPoints.Add(obj.transform.position);
            }

            //Set up line renderer, add connection from last point to origin
            LineRenderer.positionCount = rendererPoints.Count + 1;
            LineRenderer.SetPositions(rendererPoints.ToArray());
            LineRenderer.SetPosition(rendererPoints.Count, LineRenderer.GetPosition(0));
        }

        public void OnDrawGizmos()
        {
            if (Application.isPlaying)
            {
                RoomArchetype owner = gameObject.GetComponent<RoomArchetype>();
                //Ensure archetype is rendered, and there are lines to be drawn
                if (owner != null && owner.IsRendered())
                {
                    DrawDoorArrows();
                    DrawGizmoSpheres(owner.Doors, doorGizmoColour, 0.05f);
                    DrawGizmoSpheres(owner.CornerPoints, cornerGizmoColour, 0.05f);
                    DrawGizmoSpheres(owner.SpawnPoints, spawnGizmoColour, 0.05f);
                }
            }
            else
            {
                //Draw for each item in the room points object transform
                //As in editor, need to directly find the object rather than the instance
                UtilityHelper.RenderArchetype(gameObject, spawnGizmoColour, cornerGizmoColour, doorGizmoColour);
            }
        }

        private void DrawGizmoSpheres(List<GameObject> objects, Color clr, float size)
        {
            foreach (GameObject obj in objects)

            {
                Gizmos.color = clr;
                Gizmos.DrawSphere(obj.transform.position, size);
            }
        }

        ///<summary>Draws arrows on the doors to help display their facing direction</summary>
        private void DrawDoorArrows()
        {
            foreach (GameObject doorObj in gameObject.GetComponent<RoomArchetype>().Doors)
            {
                if (doorObj.activeInHierarchy)
                    UtilityHelper.DrawDoorArrow(doorObj.transform, doorGizmoColour);
            }
        }
    }
}