using UnityEngine;

public class TexturedSphere : MonoBehaviour
{
    public float radius = 5f;
    public int segments = 64;
    public Material sphereMaterial;
    public float rotationSpeed = 10f;
    // linewidth for the wireframe
    public float lineWidth = 0.01f;
    private LineRenderer lineRenderer;
    private bool wireframeVisible = false;

    void Start()
    {
        // earth visible at startup
        CreateTexturedSphere();
        // toggle wireframe visibility with Left Ctrl key
        CreateWireframe();
    }

    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        // toggle wireframe visibility with Left Ctrl key
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            wireframeVisible = !wireframeVisible;
            lineRenderer.enabled = wireframeVisible;
        }
    }

    void CreateTexturedSphere()
    {
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        // assigning the earth texture
        meshRenderer.material = sphereMaterial;

        Mesh mesh = new Mesh();
        mesh.name = "TexturedSphere";

        int latitudeSegments = segments;
        int longitudeSegments = segments * 2;

        Vector3[] vertices = new Vector3[(latitudeSegments + 1) * (longitudeSegments + 1)];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[latitudeSegments * longitudeSegments * 6];

        // vertices and UVs
        // latitude: horizontal lines
        for (int lat = 0; lat <= latitudeSegments; lat++)
        {
            float theta = Mathf.PI * lat / latitudeSegments; 
            float sinTheta = Mathf.Sin(theta);
            float cosTheta = Mathf.Cos(theta);

            // longitude: vertical lines
            for (int lon = 0; lon <= longitudeSegments; lon++)
            {
                float phi = 2 * Mathf.PI * lon / longitudeSegments;
                float sinPhi = Mathf.Sin(phi);
                float cosPhi = Mathf.Cos(phi);

                int index = lon + lat * (longitudeSegments + 1);

                // spherical to cartesian coordinates
                vertices[index] = new Vector3(
                    radius * sinTheta * cosPhi,
                    radius * cosTheta,
                    radius * sinTheta * sinPhi
                );

                // UV mapping (inverted V to flip the texture. In unity, it seems to be inverted)
                uv[index] = new Vector2((float)lon / longitudeSegments, 1 - (float)lat / latitudeSegments);
            }
        }

        // create two triangles per quad
int triIndex = 0;
for (int lat = 0; lat < latitudeSegments; lat++)
{
    for (int lon = 0; lon < longitudeSegments; lon++)
    {
        int current = lon + lat * (longitudeSegments + 1);
        int next = current + longitudeSegments + 1;

        // reversing order of these vertices to correct the winding order
        // first triangle
        triangles[triIndex++] = current;
        triangles[triIndex++] = current + 1;
        triangles[triIndex++] = next;

        // second triangle
        triangles[triIndex++] = current + 1;
        triangles[triIndex++] = next + 1;
        triangles[triIndex++] = next;
    }
}
        // mesh data
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
    }

    void CreateWireframe()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.material = new Material(Shader.Find("Unlit/Texture"));
        // linerenderer.color = Color."replace with color";
        // use local space for the line renderer
        lineRenderer.useWorldSpace = false;

        // calculate total positions: latitude + longitude
        int totalPositions = (segments + 1) * (segments + 1) + segments * (segments + 1);
        lineRenderer.positionCount = totalPositions;

        int index = 0;

        // draw latitude lines
        for (int i = 0; i <= segments; i++)
        {
            float theta = Mathf.PI * i / segments;
            for (int j = 0; j <= segments; j++)
            {
                float phi = 2 * Mathf.PI * j / segments;
                lineRenderer.SetPosition(index++, new Vector3(
                    radius * Mathf.Sin(theta) * Mathf.Cos(phi),
                    radius * Mathf.Cos(theta),
                    radius * Mathf.Sin(theta) * Mathf.Sin(phi)
                ));
            }
        }

        // draw longitude lines
        for (int i = 0; i < segments; i++)
        {
            float phi = 2 * Mathf.PI * i / segments;
            for (int j = 0; j <= segments; j++)
            {
                float theta = Mathf.PI * j / segments;
                lineRenderer.SetPosition(index++, new Vector3(
                    radius * Mathf.Sin(theta) * Mathf.Cos(phi),
                    radius * Mathf.Cos(theta),
                    radius * Mathf.Sin(theta) * Mathf.Sin(phi)
                ));
            }
        }

        lineRenderer.enabled = wireframeVisible;
    }
}
