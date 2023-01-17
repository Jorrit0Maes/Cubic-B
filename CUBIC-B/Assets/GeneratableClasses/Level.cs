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
    private ArrayList platforms;
    public float sizeOfBox;

    private void Awake()
    {

        generateLevelPlatforms();
        placeBoxes();

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
        platforms = new ArrayList ();
        float lengthleft = maxLevelLength;

        Vector2 lastPoint = new Vector2(2f,0f);


        var random = new System.Random();


        while ( lengthleft > minSizeLastPlatform )
        {
            Console.WriteLine("in while");

            float length = random.Next(minPlatformSize,maxPlatformSize);
            


            //rand doet allleen ints dus we delen door 100 om een getal tot 2 na de komme te krijgen dunno Y ma werkt niet als ik dat in de toewijzing doe beste gok is Int reasons
            float xSpacing = random.Next(minPlatformXspacing,maxPlatformXSpacing);

            float ySpacing = random.Next(minPlatformYSpacing, maxPlatformYSpacing);

            // if ySpacing is large xSpacing gets smaller so we can make the jump
            xSpacing -= ySpacing;

            xSpacing = (xSpacing / 100) * 4;
            ySpacing = (ySpacing / 100) * 4;

            //make it rand up or down
            if (random.Next(100) > 66)
            {
                ySpacing = -ySpacing;
            }

            Platform tempPlatform = new Platform(new Vector2(lastPoint.x+xSpacing, lastPoint.y + ySpacing), new Vector2(lastPoint.x +xSpacing+(length*4), lastPoint.y + ySpacing  - 3f));
            
            lengthleft -= length;
            lastPoint.x = tempPlatform.endPoint.x;
            lastPoint.y = tempPlatform.startPoint.y;  


            var tempInitiated = Instantiate(squareExmp, tempPlatform.origin , Quaternion.identity);
            tempInitiated.localScale = new Vector3(length, 0.75f, 1);

            platforms.Add(tempPlatform);
        }

       
    }

    private void placeBoxes()
    {

        System.Random random = new System.Random();

        foreach(Platform platform in platforms) 
        {

            float platformlength = platform.endPoint.x - platform.startPoint.x;
            int chanceofBoxSpawning = random.Next(minPlatformSize,maxPlatformSize);
            // the larger the platform the higher the chance of a box spawning or if big enough we defintetly spawn .
            if (platformlength >= maxPlatformSize/2 || chanceofBoxSpawning < platformlength)
            {
                // we ensure not to fill the platform with so much boxes that it is all box  => platformLength/2 = number of boxes *sizeof of a box only half of the length filled

                int maxNumberOfboxes = (int)Math.Floor(platformlength / 2 * sizeOfBox);//size of box is kleiner dan nul dus verlagen door vermenigvuldiging

                random.Next(maxNumberOfboxes);

                ArrayList boxes = new ArrayList();
                for (int i = 0; i < maxNumberOfboxes; i++)
                {
                    //to not put it on the front edge so we can make the jump or off the platform we adjust the limits
                    int boxVectorX = random.Next((int) Math.Ceiling(platform.startPoint.x) +1 , (int) platform.endPoint.x -1);
                    Box boxTemplate = new Box(new Vector2(boxVectorX, platform.startPoint.y),sizeOfBox,sizeOfBox);

                    if(boxes.Count == 0 || !checkIfBoxInOtherBox(boxes, boxTemplate))
                    {
                        boxes.Add(boxTemplate);
                        Transform t = Instantiate(squareExmp, boxTemplate.origin,Quaternion.identity);
                        t.localScale= new Vector3(boxTemplate.length/4,boxTemplate.heigth/4, 0);
                        t.gameObject.GetComponent<SpriteRenderer>().color = Color.black;
                        
                    }

                }

            }
            
        }
    }

    private bool checkIfBoxInOtherBox(ArrayList boxes, Box boxTemplate)
    {
       if(boxes.Count == 0) return false;

       foreach(Box box in boxes)
       {
            //check of het startpunt van de box tussen het einde of begin van de al bestaande boxen valt of korter voor het begin dan de box die bijkomt lang is
            if(box.startPoint.x < boxTemplate.startPoint.x - boxTemplate.length && boxTemplate.endPoint.x < box.endPoint.x)
            {
                return true;
            }
       }
       return false;    
    }
}
