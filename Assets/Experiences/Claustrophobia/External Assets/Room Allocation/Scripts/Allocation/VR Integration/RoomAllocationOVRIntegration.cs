using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoomAllocation {
    //    UNCOMMENT IF YOU ARE USING THE OCULUS INTEGRATION PACKAGE
    //    DELETE OR LEAVE COMMENTED OTHERWISE
    //    (Ctrl + k + c to comment all, Ctrl + k + u to uncomment all selected)

    public class RoomAllocationOVRIntegration : MonoBehaviour {
    public OVRInput.Button resetGenerationInput = OVRInput.Button.One;
    public OVRInput.Button replaceArchetypeInput = OVRInput.Button.Two;
    private InputManager InputManager { get; set; }

    private GeometryManager GeometryManager { get; set; }

    public List<Vector3> VRGeometry { get; set; }

    public void Awake() {
        InputManager = GetComponent<InputManager>();
        GeometryManager = GetComponent<GeometryManager>();
        GeometryManager.isVR = true;
        GeometryManager.shape = GeometryManager.AreaShape.VR;
        //Obtain the VR geometry...
        VRGeometry = GetOVRGeometry();
        if (VRGeometry != null) {
            Debug.Log("Geometry obtained from OVR Manager successfully");
        }
        GeometryManager.Geometry = VRGeometry;
        GeometryManager.SetMinMaxPositions();

        GeometryManager.isVR = true;
        GeometryManager.shape = GeometryManager.AreaShape.VR;
        GeometryManager.Geometry = VRGeometry;
    }

        private void Start() {
            InputManager.ResetGeneration();
        }

        public void Update() {
            //Input for resetting the generation
            if (OVRInput.GetDown(resetGenerationInput)) {
                    if (InputManager != null) {
                        InputManager.ResetGeneration();
                    }
                }

            //Input for replacing archetype
            if (OVRInput.GetDown(replaceArchetypeInput)) {
                if (InputManager != null) {
                    InputManager.ReplaceArchetype();
                }
            }
        }

    public List<Vector3> GetOVRGeometry() {
        //Ensure that the boundary is defined, and is actually configured
        if (OVRManager.boundary != null && OVRManager.boundary.GetConfigured()) {
            Vector3[] playAreaGeometry = OVRManager.boundary.GetGeometry(OVRBoundary.BoundaryType.PlayArea);
            if (playAreaGeometry != null && playAreaGeometry.Length > 0) {
                if (GeometryManager.renderGeometryPoints) {
                    GameObject vrGeometryParent = new GameObject("VR Geometry Area") {
                        tag = AllocationConstants.TESTAREA_TAG_NAME
                    };
                    foreach (Vector3 v in playAreaGeometry) {
                        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        sphere.transform.position = v;
                    }
                }

                return new List<Vector3>(playAreaGeometry);
            }
        }

        Debug.LogError("OVR geometry not configured");
        return null;
    }
}
}