using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

namespace RoomAllocation
{
    public class Logger : MonoBehaviour
    {
        private string eventFilename;
        public bool loggingActive = true;
        private FPSCounter FPSCounter { get; set; }

        // Use this for initialization
        private void Awake()
        {
            FPSCounter = gameObject.AddComponent<FPSCounter>();
            //Don't want to log if using VR
            if (!FindObjectOfType<GeometryManager>().isVR && loggingActive)
            {
                //Create a file, even if logging is disabled
                Directory.CreateDirectory(Application.dataPath + "/Room Allocation/Documentation/GenerationLogs/");

                eventFilename = Application.dataPath + "/Room Allocation/Documentation/GenerationLogs/eventlog-" + System.DateTime.Now.Ticks + ".csv";
                FileStream f = File.Create(eventFilename);
                f.Close();
                //Structure the columns of the log
                string s = "TIME(s), FPS, ROOM SIZE, GENERATED ROOM, ARCHETYPE, GENERATION BIAS,PARENT,TREE LAYER\n";
                File.WriteAllText(eventFilename, s);
            }
        }

        /// <summary>
        /// Adds an entry to the opened file
        /// </summary>
        /// <param name="eventData">The data to be added</param>
        public void LogEvent(string eventData)
        {
            if (loggingActive
                && eventData != null
                && eventFilename != null)
            {
                if (eventData != "")
                {
                    //Obtain time and fps for current event, and append event data
                    string s = Time.time + "," + eventData + "\n";
                    File.AppendAllText(eventFilename, s);
                }
            }
        }

        /// <summary>
        /// Parses the required log data from the provided parameter values
        /// </summary>
        /// <param name="path">The root node of the allocation path</param>
        /// <param name="archetypeNode">The node to be logged</param>
        /// <param name="roomSize">The size of the room</param>
        /// <returns></returns>
        public string ParseEventData(AllocationTree path, AllocationTree archetypeNode, Vector2 roomSize)
        {
            if (loggingActive)
            {
                //Add details of archetype
                if (archetypeNode != null)
                {
                    //Structures an event entry for the logger
                    string data = "";

                    //Add FPS
                    data += FPSCounter.m_CurrentFps.ToString() + ",";

                    //Add room dimensions
                    if (roomSize != null)
                        data += roomSize.x + "x" + roomSize.y + ",";
                    else
                        data += "null,";

                    //Add associatred room name
                    if (archetypeNode.RoomObject != null)
                        data += archetypeNode.RoomObject.name + ",";
                    else
                        data += "null,";

                    //Add archetype name and its weighting
                    if (archetypeNode.ArchetypeObject != null)
                        data += archetypeNode.ArchetypeObject.name + ","
                            + archetypeNode.ArchetypeObject.GetComponent<RoomArchetype>().selectionWeighting
                            + ",";
                    else
                        data += "null, null,";

                    //Add the parent node connecting to the passed archetype
                    if (archetypeNode.Parent != null)
                        data += archetypeNode.Parent.ArchetypeObject.name + ",";
                    else
                        data += "null,";

                    //Calculate the number of nodes between the origin node and the current one
                    //Represents the layer of the tree
                    data += archetypeNode.GetNumberOfNodesBetween(path, archetypeNode);
                    return data;
                }

                return "";
            }
            return null;
        }
    }

    public class FPSCounter : MonoBehaviour
    {
        private const float fpsMeasurePeriod = 0.5f;
        private int m_FpsAccumulator = 0;
        private float m_FpsNextPeriod = 0;
        public int m_CurrentFps;

        private void Start()
        {
            m_FpsNextPeriod = Time.realtimeSinceStartup + fpsMeasurePeriod;
        }

        private void Update()
        {
            // measure average frames per second
            m_FpsAccumulator++;
            if (Time.realtimeSinceStartup > m_FpsNextPeriod)
            {
                m_CurrentFps = (int)(m_FpsAccumulator / fpsMeasurePeriod);
                m_FpsAccumulator = 0;
                m_FpsNextPeriod += fpsMeasurePeriod;
            }
        }
    }
}