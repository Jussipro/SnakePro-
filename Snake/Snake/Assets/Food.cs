using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    public float radius;
    public float eatlength;
    public TextMesh points;

    private int p = 0; 
	
	void Update ()
    {
        if ((Snake.instance.position - transform.position).magnitude > radius)
            return;

        Snake.instance.increaselength(eatlength);

        //yksi piste lisää
        p++;

        points.text = p.ToString();

        Vector3 temp = new Vector3(Random.Range(-5.5f, 5.5f), 0, Random.Range(-5.5f, 5.5f));

        transform.position = temp;
	}
}
