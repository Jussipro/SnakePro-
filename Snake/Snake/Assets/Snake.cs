using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Snake : MonoBehaviour
{
    public static Snake instance;
    public Vector3 position { get { return new Vector3(positions[positions.Count - 1].x, 0, positions[positions.Count - 1].y); } }

    public GameObject failPrompt;
    public float turnOffset;
    public float length;
    public float speed;

    private Vector2 direction = Vector2.up;

    private List<Vector2> positions;

    private LineRenderer line;

    private float   m_length;
    private float   m_lengthPrev;
    private bool    m_turned;
    private bool    m_ded;

    void Awake ()
    {
        instance = this;

        line = GetComponent<LineRenderer>();
        line.positionCount = 2;

        positions = new List<Vector2>();

        positions.Add(transform.position - new Vector3(0, 0, 2));
        positions.Add(transform.position);
	}
	
	void LateUpdate ()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

        if(m_ded)
        {
            if (Input.GetKeyDown(KeyCode.Space))
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

            return;
        }

        m_turned = false;

        if (Input.GetKeyDown(KeyCode.LeftArrow)) Turn(Vector2.left);
        if (Input.GetKeyDown(KeyCode.RightArrow)) Turn(Vector2.right);
        if (Input.GetKeyDown(KeyCode.UpArrow)) Turn(Vector2.up);
        if (Input.GetKeyDown(KeyCode.DownArrow)) Turn(Vector2.down);

        if (positions.Count == 0)
            return;

        CullPoints();

        //pää liikkuu
        positions[positions.Count - 1] += direction * speed * Time.deltaTime;

        line.positionCount = positions.Count;

        for (int i = 0; i < positions.Count; ++i)
            line.SetPosition(i, new Vector3(positions[i].x, 0 ,positions[i].y));

        Collision();
    }

    void Collision()
    {
        Vector2 temp = Vector2.zero;

        for (int i = positions.Count - 2; i > 0; --i)
        {
            if (!ExtensionMethods.LineIntersection(positions[positions.Count - 1], positions[positions.Count - 1] + direction * speed * Time.deltaTime, positions[i], positions[i - 1], ref temp))
                continue;

            failPrompt.SetActive(true);
            m_ded = true;
        }

        if (position.x > 6 || position.x < -6 || position.z > 6 || position.z < -6)
        {
            failPrompt.SetActive(true);
            m_ded = true;
        }
    }

    //Lenght lyhenee kunnes se kohtaa lenghtPrevin ja sitten positio nolla poistetaan ja positio ykkösestä tulee uusi nolla. 
    void CullPoints()
    {
        m_length = 0;

        for (int i = 1; i < positions.Count; i++)
            m_length += (positions[i -1] - positions[i]).magnitude;

        m_lengthPrev = m_length - (positions[0] - positions[1]).magnitude;

        if (m_length < length)
            return;

        positions[0] = Vector2.MoveTowards(positions[0], positions[1], Time.deltaTime * speed);

        if (m_lengthPrev < length)
            return;

        positions.RemoveAt(0);
    }

    void Turn(Vector2 dir)
    {
        if (m_turned)
            return;

        if (Mathf.Abs(Vector2.Dot(dir, direction)) > 0.5f)
            return;

        m_turned = true;

        direction = dir;

        positions.Add(positions[positions.Count - 1] + direction * turnOffset);
    }

    public void increaselength(float amount)
    {
        length += amount;
    }
}
public static class ExtensionMethods
{
    //Courtesy of konsta
    public static bool  LineIntersection(Vector2 a0, Vector2 a1, Vector2 b0, Vector2 b1, ref Vector2 point)
    {
        float d0 = DistanceFromPlaneDir(a0 - a1, a0, b0);
        float d1 = DistanceFromPlaneDir(a0 - a1, a0, b1);

        //both points are on either side of the line normal
        if ((d0 > 0 && d1 > 0) || (d0 < 0 && d1 < 0))
            return false;

        Vector2 x = Vector2.Lerp(b0, b1, Mathf.Abs(d0) / (Mathf.Abs(d0) + Mathf.Abs(d1)));
        Vector2 c = Vector2.Lerp(a0, a1, 0.5f);
        d1 = (a0 - a1).magnitude / 2;

        //Distance from center of line a
        d0 = DistanceFromPlaneNor(a0 - a1, c, x);
        d0 = Mathf.Abs(d0);

        //If distance from line center is greater than its half extent there was no intersection
        if (d0 > d1)
            return false;

        point = x;

        return true;
    }
    public static float DistanceFromPlaneDir(Vector2 planeDir, Vector2 planePoint, Vector2 point)
    {
        planeDir = new Vector2(-planeDir.y, planeDir.x);
        planeDir.Normalize();
        return Vector2.Dot(planeDir, point - planePoint);
    }
    public static float DistanceFromPlaneNor(Vector2 planeNormal, Vector2 planePoint, Vector2 point)
    {
        return Vector2.Dot(planeNormal.normalized, point - planePoint);
    }
}