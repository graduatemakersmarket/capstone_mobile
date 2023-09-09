using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class Placement : MonoBehaviour
{
    public GameObject placementObject;
    public GameObject objectToDrop;

    private ARRaycastManager arRaycastManager;
    private Pose placementPose;
    private bool placementPoseIsValid = false;

    // Start is called before the first frame update
    void Start()
    {
        // Find the ARRayCastManager attached to the ARSessionOrigin
        arRaycastManager = FindObjectOfType<ARRaycastManager>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePlacementPose();
        UpdatePlacementObject();

        // Detect if a user is tapping the screen to place an object
        if (placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            PlaceObject();
        }
    }

   private void UpdatePlacementPose()
    {
        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();

        // See if the camera is fixated on any surfaces and add to hit list
        arRaycastManager.Raycast(screenCenter, hits, TrackableType.Planes);

        // Placement is valid if camera is fixated on a surface
        placementPoseIsValid = hits.Count > 0;

        if (placementPoseIsValid )
        {
            // Update placement pose
            placementPose = hits[0].pose;

            // Compute the camera bearing
            var cameraForward = Camera.current.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;

            // Make the placement object rotate as the camera rotates
            placementPose.rotation = Quaternion.LookRotation(cameraBearing);

        }
    }

    private void UpdatePlacementObject()
    {
        if (placementPoseIsValid)
        {
            // Show the placement object if it is on top of a surface
            placementObject.SetActive(true);
            placementObject.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        else
        {
            // Hide the placement object if it is not on top of a surface
            placementObject.SetActive(false);
        }
    }

    private void PlaceObject()
    {
        // Create a new object to show on the screen
        Instantiate(objectToDrop, placementPose.position, placementPose.rotation);
    }
}