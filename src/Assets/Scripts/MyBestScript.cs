using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class MyBestScript : MonoBehaviour
{
    private Mesh mesh;
    private Mesh mesh2;
    private Mesh mesh3;
    float resetTime;
    List<Vector3> posList = new List<Vector3>();
    List<Vector3> posList2 = new List<Vector3>();
    List<Vector3> bifurcation1PosList = new List<Vector3>();
    List<Vector3> bifurcation2PosList = new List<Vector3>();
    List<Vector3> bifurcation1PosList2 = new List<Vector3>();
    List<Vector3> bifurcation2PosList2 = new List<Vector3>();
    Vector3 dir2;
    Vector3 dir3;
    Vector3 pos2;
    Vector3 pos3;
    [SerializeField] GameObject thunder;
    private GameObject thunder2;
    private GameObject thunder3;
    int bifurcationCount1;
    int bifurcationCount2;
    int posCount;
    int posCount2;
    int posCount3;
    int bifurcation;
    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        resetTime = 0.0f;
        posCount = 0;
        bifurcation = 0;
    }

    // Update is called once per frame
    void Update()
    {
        resetTime -= Time.deltaTime;
        if (resetTime < 0.0f)
        {
            if (thunder2!=null) {
                Destroy(thunder2);
            }
            if (thunder3!=null) {
                Destroy(thunder3);
            }
            thunder2 = null;
            thunder3 = null;
            bifurcation = 0;
            bifurcationCount1 = 0;
            bifurcationCount2 = 0;
            resetTime = Random.Range(1.0f, 3.0f);
            posList2.Clear();
            posList.Clear();
            bifurcation1PosList.Clear();
            bifurcation2PosList.Clear();
            bifurcation1PosList2.Clear();
            bifurcation2PosList2.Clear();
            posCount = 0;
            posCount2 = 0;
            posCount3 = 0;
            generateThunder();
        }
        StartCoroutine("Coroutine", posList);
    }

    void generateThunder()
    {
        Vector3 pos = new Vector3(0.0f, 1000.0f, 0.0f);

        Vector3 dir = new Vector3(0.0f, -1.0f, 0.0f);
        dir.Normalize();

        posList.Add(pos);
        gameObject.transform.position = new Vector3(
          Random.Range(-300.0f, +300.0f), 0.0f, Random.Range(-300.0f, +300.0f));
        for (int i = 0; i < 1000; i++)
        {
            if (bifurcation<2) {
                int branch = Random.Range(0, 100);
                if (branch>95&& bifurcation==0)
                {
                    thunder2=Instantiate(thunder, gameObject.transform.position, Quaternion.identity);
                    dir2 = dir;
                    pos2 = pos;
                    mesh2 = thunder2.GetComponent<MeshFilter>().mesh;
                    bifurcationCount1 =i+1;
                    bifurcation1PosList.Add(pos);
                    bifurcation++;
                }
                else if (branch < 5 && bifurcation == 1)
                {
                    thunder3 = Instantiate(thunder, gameObject.transform.position, Quaternion.identity);
                    dir3 = dir;
                    pos3 = pos;
                    mesh3 = thunder3.GetComponent<MeshFilter>().mesh;
                    bifurcationCount2 = i + 1;
                    bifurcation2PosList.Add(pos);
                    bifurcation++;
                }
            }
            float d = Random.Range(-15.0f, +15.0f);
            Quaternion q = Quaternion.AngleAxis(d, new Vector3(0.0f, 0.0f, 1.0f));
            dir = q * dir;
            dir.Normalize();

            float distance = Random.Range(30.0f, 50.0f);
            pos += dir * distance;
            posList.Add(pos);


            if (bifurcation >= 1)
            {
                float d2 = Random.Range(-10.0f, +10.0f);
                Quaternion q2 = Quaternion.AngleAxis(d2, new Vector3(0.0f, 0.0f, 1.0f));
                dir2 = q2 * dir2;
                dir.Normalize();

                float distance2 = Random.Range(30.0f, 50.0f);
                pos2 += dir2 * distance2;
                bifurcation1PosList.Add(pos2);
            }
            if (bifurcation >= 2)
            {
                float d3 = Random.Range(-10.0f, +10.0f);
                Quaternion q3 = Quaternion.AngleAxis(d3, new Vector3(0.0f, 0.0f, 1.0f));
                dir3 = q3 * dir3;
                dir3.Normalize();

                float distance3 = Random.Range(30.0f, 50.0f);
                pos3 += dir3 * distance3;
                bifurcation2PosList.Add(pos3);
            }

            //generateMesh(posList);
            if (pos.y < 0.0f) break;
        }
    }

    void generateMesh(List<Vector3> a_pos ,Mesh mesh)
    {
        if (a_pos.Count < 2) return;

        mesh.Clear();

        int vertex_count = 2 * a_pos.Count;

        Vector3[] vertices = new Vector3[vertex_count];
        int vtx = 0;
        for (int i = 0; i < a_pos.Count; i++)
        {
            Vector3 dir;
            if (i == 0)
            {
                dir = a_pos[1] - a_pos[0];
            }
            else if (i == a_pos.Count - 1)
            {
                dir = a_pos[a_pos.Count - 1] - a_pos[a_pos.Count - 2];
            }
            else
            {
                dir = a_pos[i + 1] - a_pos[i - 1];
            }
            dir.Normalize();

            const float THUNDER_WIDTH = 5.0f;
            Vector3 t = new Vector3(-dir.y, dir.x, 0.0f) * THUNDER_WIDTH;

            vertices[vtx + 0] = a_pos[i] - t;
            vertices[vtx + 1] = a_pos[i] + t;
            vtx += 2;
        }

        Color[] colors = new Color[vertex_count];
        for (int i = 0; i < vertex_count; i++)
        {
            colors[i] = Color.white;
        }

        int triangle_count = 6 * (a_pos.Count - 1);
        int[] triangles = new int[triangle_count];
        int idx = 0;
        vtx = 0;
        while (idx < triangle_count)
        {
            triangles[idx + 0] = vtx + 0;
            triangles[idx + 1] = vtx + 1;
            triangles[idx + 2] = vtx + 2;
            triangles[idx + 3] = vtx + 2;
            triangles[idx + 4] = vtx + 1;
            triangles[idx + 5] = vtx + 3;

            idx += 6;
            vtx += 2;
        }
        mesh.vertices = vertices;
        mesh.colors = colors;
        mesh.triangles = triangles;

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

    }
   IEnumerator Coroutine(List<Vector3> posList)
    {
        if(posCount>=bifurcationCount1&&thunder2!=null&&posCount2<bifurcation1PosList.Count)
        {
            bifurcation1PosList2.Add(bifurcation1PosList[posCount2]);
            posCount2++;
        }
        if(posCount >= bifurcationCount2 && thunder3 != null&&posCount3< bifurcation2PosList.Count)
        {
            bifurcation2PosList2.Add(bifurcation2PosList[posCount3]);
           posCount3++;
        }
        if (posCount<posList.Count)
        {
            posList2.Add(posList[posCount]);
            posCount++;
        }
        if (posList2.Count>0) {
            generateMesh(posList2,mesh);
        }
        if (bifurcation1PosList2.Count > 0)
        {
            generateMesh(bifurcation1PosList2, mesh2);
        }
        if (bifurcation2PosList2.Count > 0)
        {
            generateMesh(bifurcation2PosList2, mesh3);
        }
        yield return new WaitForSeconds(0.001f);
    }
}
