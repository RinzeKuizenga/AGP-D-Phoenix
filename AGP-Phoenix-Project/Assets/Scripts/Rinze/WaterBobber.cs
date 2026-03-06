using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

[RequireComponent(typeof(Rigidbody))]
public class SimpleBoatBuoyancy : MonoBehaviour
{
    [Header("References")]
    public WaterSurface waterSurface;

    [Header("Buoyancy Settings")]
    public Transform[] floaters; 
    public float buoyancyForce = 15f;
    public float waterDrag = 0.99f;
    public float waterAngularDrag = 0.5f;

    private Rigidbody rb;
    private WaterSearchParameters searchParameters = new WaterSearchParameters();
    private WaterSearchResult searchResult = new WaterSearchResult();

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.linearDamping = waterDrag;
        rb.angularDamping = waterAngularDrag;

        if (waterSurface == null)
        {
            waterSurface = FindObjectOfType<WaterSurface>();
        }
    }

    void FixedUpdate()
    {
        if (waterSurface == null || floaters.Length == 0) return;

        foreach (Transform floater in floaters)
        {
            searchParameters.startPositionWS = searchResult.candidateLocationWS;
            searchParameters.targetPositionWS = floater.position;
            searchParameters.error = 0.01f;
            searchParameters.maxIterations = 8;

            if (waterSurface.ProjectPointOnWaterSurface(searchParameters, out searchResult))
            {
                float waterHeight = searchResult.projectedPositionWS.y;
                float floaterHeight = floater.position.y;

                if (floaterHeight < waterHeight)
                {
                    float submersionDepth = waterHeight - floaterHeight;
                    Vector3 buoyancyVector = Vector3.up * buoyancyForce * submersionDepth;
                    rb.AddForceAtPosition(buoyancyVector, floater.position, ForceMode.Force);
                }
            }
        }
    }
}