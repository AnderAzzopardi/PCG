using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class Track : MonoBehaviour
{

    [SerializeField]
    private int submeshCount = 3;

    [SerializeField]
    private float radius = 50f;

    [SerializeField]
    private float roadMarkerWidth = 0.2f;

    [SerializeField]
    private float roadWidth = 5.0f;

    [SerializeField]
    private float barrierWidth = 0.6f;


    [SerializeField]
    private int quadCount = 300;


    //Variance Fields required for the Perlin Noise function
    [SerializeField]
    private float variance = 5.0f;

    [SerializeField]
    private float varianceScale = 0.1f;

    [SerializeField]
    private Vector2 varianceOffset;

    [SerializeField]
    private Vector2 varianceStep = new Vector2(0.01f, 0.01f);
    

    private MeshGenerator meshGenerator;


  

    // Start is called before the first frame update
    void Start()
    {   
        radius = Random.Range(40f, 50f);
        variance = Random.Range(5f, 15f);
        float random = Random.Range(0.1f, 0.07f);
            varianceOffset = new Vector2(Mathf.Cos(random), Mathf.Sin(random)); 
        
        RenderTrack();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void RenderTrack(){
        MeshFilter meshFilter = this.GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = this.GetComponent<MeshRenderer>();
        MeshCollider meshCollider = this.GetComponent<MeshCollider>();

        meshFilter.mesh = GenerateTrack();
        meshRenderer.materials = MaterialsList().ToArray();
        meshCollider.sharedMesh = meshFilter.mesh;
    }

    private Mesh GenerateTrack(){
        meshGenerator = new MeshGenerator(submeshCount);

        // generate track code
        float quadDistance = 360f / quadCount;

        List<Vector3> pointRefList = new List<Vector3>();

        for(float degrees = 0; degrees < 360f; degrees+=quadDistance){

            Vector3 point = Quaternion.AngleAxis(degrees, Vector3.up) * Vector3.forward * radius;
            pointRefList.Add(point);
        }

        //Add Noise to our points to create randomness/curves to our track
        Vector2 curve = varianceOffset;
        for(int i = 0; i < pointRefList.Count; i++){
            curve += varianceStep;

            Vector3 pointRef = pointRefList[i].normalized;

            float perlinNoise = Mathf.PerlinNoise(curve.x * varianceScale, curve.y * varianceScale);
            perlinNoise *= variance;

            pointRefList[i] += pointRef * perlinNoise;
        }


        for(int i = 1; i <= pointRefList.Count; i++){
            Vector3 prevQuad = pointRefList[i - 1];
            Vector3 currQuad = pointRefList[i % pointRefList.Count];
            Vector3 nextQuad = pointRefList[(i + 1) % pointRefList.Count];

            CreateTrack(prevQuad, currQuad, nextQuad);
        }

        return meshGenerator.CreateMesh();
    }

    private void CreateTrack(Vector3 prevQuad, Vector3 currQuad, Vector3 nextQuad){

        //create the road marker
        Vector3 offset = Vector3.zero;
        Vector3 targetOffset = Vector3.forward * roadMarkerWidth;
        CreateQuad(prevQuad, currQuad, nextQuad, 0, offset, targetOffset);

        //create the road
        offset += targetOffset;
        targetOffset = Vector3.forward * roadWidth;
        CreateQuad(prevQuad, currQuad, nextQuad, 1, offset, targetOffset);

        //create the barrier
        offset += targetOffset;
        targetOffset = Vector3.forward * barrierWidth;
        CreateQuad(prevQuad, currQuad, nextQuad, 2, offset, targetOffset);

    }

    private void CreateQuad(Vector3 prevQuad, Vector3 currQuad, Vector3 nextQuad, 
                            int submesh, Vector3 offset, Vector3 targetOffset){
        //right Side
        Vector3 nextDirection = (nextQuad - currQuad).normalized;
        Vector3 prevDirection = (currQuad - prevQuad).normalized;

        Quaternion nextQuaternion = Quaternion.LookRotation(Vector3.Cross(nextDirection, Vector3.up));
        Quaternion prevQuaternion = Quaternion.LookRotation(Vector3.Cross(prevDirection, Vector3.up));

        Vector3 topLeftPoint = currQuad + (prevQuaternion * offset);
        Vector3 topRightPoint = currQuad + (prevQuaternion * (offset + targetOffset));

        Vector3 bottomLeftPoint = nextQuad + (nextQuaternion * offset);
        Vector3 bottomRightPoint = nextQuad + (nextQuaternion * (offset + targetOffset));

        meshGenerator.CreateTriangle(topLeftPoint, topRightPoint, bottomLeftPoint, submesh);
        meshGenerator.CreateTriangle(topRightPoint, bottomRightPoint, bottomLeftPoint, submesh);

        //left Side
        nextQuaternion = Quaternion.LookRotation(Vector3.Cross(-nextDirection, Vector3.up));
        prevQuaternion = Quaternion.LookRotation(Vector3.Cross(-prevDirection, Vector3.up));

        topLeftPoint = currQuad + (prevQuaternion * offset);
        topRightPoint = currQuad + (prevQuaternion * (offset + targetOffset));

        bottomLeftPoint = nextQuad + (nextQuaternion * offset);
        bottomRightPoint = nextQuad + (nextQuaternion * (offset + targetOffset));

        meshGenerator.CreateTriangle(bottomLeftPoint, bottomRightPoint, topLeftPoint, submesh);
        meshGenerator.CreateTriangle(bottomRightPoint, topRightPoint, topLeftPoint, submesh);

    }

    private List<Material> MaterialsList(){
        List<Material> materialsList = new List<Material>();

        Material whiteMat = new Material(Shader.Find("Specular"));
        whiteMat.color = Color.white;

        Material blackMat = new Material(Shader.Find("Specular"));
        blackMat.color = Color.black;

        Material redMat = new Material(Shader.Find("Specular"));
        redMat.color = Color.red;

        materialsList.Add(whiteMat);
        materialsList.Add(blackMat);
        materialsList.Add(redMat);

        return materialsList;

    }
}
