using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class Level : MonoBehaviour
{

    // Levelgeneration
    private ArrayList Platforms = new ();
    //limits for level generation
    public int maxLevelLength;
    public int minSizeLastPlatform;
    public int maxPlatformSize;
    public int minPlatformSize;
    public int maxPlatformYSpacing;
    public int minPlatformYSpacing;
    public int maxPlatformXSpacing = 100;
    public int minPlatformXspacing;
    public Transform squareExmp;

    private void Awake()
    {

        generateLevelPlatforms();

    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void generateLevelPlatforms()
    {
        ArrayList platforms = new ArrayList ();
        float lengthleft = maxLevelLength;

        Vector2 lastPoint = new Vector2(2f,0f);


        var random = new System.Random();


        while ( lengthleft > minSizeLastPlatform )
        {
            Console.WriteLine("in while");

            float length = random.Next(minPlatformSize,maxPlatformSize);
            length /= 10;


            //rand doet allleen ints dus we delen door 100 om een getal tot 2 na de komme te krijgen dunno Y ma werkt niet als ik dat in de toewijzing doe beste gok is Int reasons
            float xSpacing = random.Next(minPlatformXspacing,maxPlatformXSpacing);
            xSpacing = (xSpacing/100)*4 ;

            float ySpacing = random.Next(minPlatformYSpacing, maxPlatformYSpacing);
            ySpacing = (ySpacing/100)*4; 

            Platform tempPlatform = new Platform(new Vector2(lastPoint.x+xSpacing, lastPoint.y + ySpacing), new Vector2(lastPoint.x +xSpacing+(length*4), lastPoint.y + ySpacing));
            
           //TODO: vertical
            lengthleft -= length;
            lastPoint.x = tempPlatform.endPoint.x;


            var tempInitiated = Instantiate(squareExmp, tempPlatform.origin , Quaternion.identity);
            tempInitiated.localScale = new Vector3(length, 0.75f, 1);


        }

       
    }
}
