using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
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
    public Transform roundExmp;
    private List<Platform> platforms;
    ArrayList interactableObjects = new ArrayList();
    public float sizeOfBox;
    public float sizeOfAbilityObject;
    public int aantalAbilities;
    List<String> listOfAbilities;
    public Transform deathBoxPreFab;
    public GameObject Player;
    public GameObject Spawn;
    private List<AbilityObject> spawnedAbilities;
    private List<AbilityObject> spawnedDoubleJumps;

    private int SpeedBoostAdjustments;
    public Transform Finish;
    public Transform ouder; 



    private void spawns()
    {
        listOfAbilities = new List<string> { "speed", "double" };
        spawnedAbilities = new List<AbilityObject>();
        spawnedDoubleJumps = new List<AbilityObject>();
        SpeedBoostAdjustments = 0;
        DetermineAbilityXCoordinates();
        generateLevelPlatforms();
        AbilityObject abilityObject = new AbilityObject();
        abilityObject.length = sizeOfAbilityObject;
        abilityObject.heigth = sizeOfAbilityObject;
        placeAbilities(abilityObject, roundExmp);
        Box boxtemp = new Box();
        boxtemp.heigth = sizeOfBox;
        boxtemp.length = sizeOfBox; 
        placeObjects(boxtemp, squareExmp);


        
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
        platforms = new();
        float lengthleft = maxLevelLength;

        Vector2 lastPoint = new(2f,0f);


        var random = new System.Random();


        while ( lengthleft > minSizeLastPlatform )
        {

            float length = random.Next(minPlatformSize,maxPlatformSize);

            


            //rand doet allleen ints dus we delen door 100 om een getal tot 2 na de komme te krijgen dunno Y ma werkt niet als ik dat in de toewijzing doe beste gok is Int reasons
            float xSpacing = random.Next(minPlatformXspacing,maxPlatformXSpacing);
            float ySpacing = random.Next(minPlatformYSpacing, maxPlatformYSpacing);

            //als we een platform gaan spawnen net na de  speedboost ability zetten we die verder
            if (SpeedBoostAdjustments < spawnedAbilities.FindAll(x => x.Ability is SpeedBoost).Count && lastPoint.x > spawnedAbilities.FindAll(x => x.Ability is SpeedBoost)[SpeedBoostAdjustments].startPoint.x)
            {
                SpeedBoostAdjustments += 1;
                xSpacing *= 3;
            }

            // if ySpacing is large xSpacing gets smaller so we can make the jump
            xSpacing -= ySpacing;
            //we need more precise numbers when handling the ditance between platforms and the random double is only between 0 and 1 so we just take big int that we divide by 100
            xSpacing /= 100 ;
            ySpacing /= 100 ;

            //make the jump randomly up or down
            if (random.Next(100) > 66)
            {
                ySpacing = -ySpacing;
            }


            Platform tempPlatform = new Platform(new Vector2(lastPoint.x+xSpacing, lastPoint.y + ySpacing), new Vector2(lastPoint.x +xSpacing+(length), lastPoint.y + ySpacing - 1));
            

            var tempInitiated = Instantiate(squareExmp, ouder, false);
            tempInitiated.localPosition = tempPlatform.origin;
            tempInitiated.localRotation = Quaternion.identity;
            tempInitiated.tag = "Ground";
            tempInitiated.localScale = new Vector3(length, 1, 1);
            DeathBox deathBox = new ();
            deathBox.startPoint = new(lastPoint.x - 2 , tempPlatform.endPoint.y-1);
            deathBox.endPoint = new(tempPlatform.startPoint.x + 2, tempPlatform.endPoint.y-2);
            

            var tempDeathbox = Instantiate(deathBoxPreFab, ouder, false);
            tempDeathbox.localPosition = deathBox.origin;
            tempInitiated.localRotation = Quaternion.identity;  
            tempDeathbox.tag = "Ground";
            deathBox.length = deathBox.endPoint.x- deathBox.startPoint.x;
            tempDeathbox.tag = "DeathBox";
            tempDeathbox.localScale = new(deathBox.length,1,1);
            tempDeathbox.GetComponent<DeathScript>().Player = Player;
            tempDeathbox.GetComponent<DeathScript>().Spawn = Spawn;
           

            platforms.Add(tempPlatform);


            lengthleft -= length;
            lastPoint.x = tempPlatform.endPoint.x;
            //top that matters for matching new platform
            lastPoint.y = tempPlatform.startPoint.y;
        }


        Platform lastPlatform = platforms[platforms.Count - 1];

        DeathBox lastdeathBox = new();
        lastdeathBox.startPoint = new(lastPoint.x - 2, lastPoint.y -3) ;
        lastdeathBox.endPoint = new(lastPoint.x + 10,lastPoint.y - 4);
        var tempLastDeathbox = Instantiate(deathBoxPreFab, ouder, false);
        tempLastDeathbox.localPosition = lastdeathBox.origin;
        tempLastDeathbox.rotation = Quaternion.identity;
        tempLastDeathbox.tag = "DeathBox";
        lastdeathBox.length = lastdeathBox.endPoint.x - lastdeathBox.startPoint.x;
        tempLastDeathbox.tag = "DeathBox";
        tempLastDeathbox.localScale = new(lastdeathBox.length, 1, 1);
        tempLastDeathbox.GetComponent<DeathScript>().Player = Player;
        tempLastDeathbox.GetComponent<DeathScript>().Spawn = Spawn;

        Finish.localPosition = new Vector3(lastPoint.x + 1.5f, lastPoint.y + 0.5f);
        
        



    }

    private void placeObjects(InteractableObject objectToSpawnTemplate, Transform transform)
    {
        

        System.Random random = new System.Random();

        foreach(Platform platform in platforms) 
        {

            float platformlength = platform.endPoint.x - platform.startPoint.x;
            int chanceofBoxSpawning = random.Next(minPlatformSize,maxPlatformSize);
            //if the platform has a doublejump on it not near the end always spawn obstacles or if the double jump is near the start of the platform
            bool DoubleJumpPlatform = !spawnedDoubleJumps.Find(e => (e.startPoint.x >= platform.startPoint.x && e.startPoint.x <= platform.endPoint.x - 5)).IsUnityNull() || !spawnedDoubleJumps.Find(e => (e.startPoint.x >= platform.startPoint.x -5 && e.startPoint.x <= platform.startPoint.x )).IsUnityNull();
           

            // the larger the platform the higher the chance of a box spawning or if big enough we defintetly spawn .
            if (platformlength >= maxPlatformSize / 3 || chanceofBoxSpawning < platformlength ||DoubleJumpPlatform)
            {
                // we ensure not to fill the platform with so much boxes that it is all box  => platformLength/3 = number of boxes *sizeof of a box only half of the length filled
                int maxNumberOfObstacles = (int)Math.Floor(platformlength * sizeOfBox);//size of box is kleiner dan nul dus verlagen door vermenigvuldiging

                int numberofObstacles = random.Next(maxNumberOfObstacles);

                if(DoubleJumpPlatform) { numberofObstacles*=2; }
                for (int i = 0; i < numberofObstacles; i++)
                {

                    //to not put it on the front edge so we can make the jump or off the platform we adjust the limits
                    int obstacleVectorX = random.Next((int)Math.Ceiling(platform.startPoint.x) + 1, (int)platform.endPoint.x - 1);
                    objectToSpawnTemplate.startPoint = new Vector2(obstacleVectorX, platform.startPoint.y);


                    if (interactableObjects.Count == 0 || !checkIfObjectInsideOther(interactableObjects, objectToSpawnTemplate))
                    {
                        foreach (AbilityObject ab in spawnedAbilities.FindAll(x => x.Ability is DoubleJump))
                        {
                            //if it comes after the actual doubleJump on the platform we double the height we need to do change the objects height to determine the right origin but just doubling it will double it everytime and keep it doubles because it is a refernce so we set it back after we are done
                            if (objectToSpawnTemplate.startPoint.x > ab.startPoint.x +1 && objectToSpawnTemplate.startPoint.x < 7 + ab.startPoint.x)
                            {
                                objectToSpawnTemplate.heigth  *= 2;
                                interactableObjects.Add(objectToSpawnTemplate);
                                Transform t = Instantiate(transform, ouder, false);
                                t.localPosition = objectToSpawnTemplate.origin; 
                                t.localRotation = Quaternion.identity;
                                t.tag = "Obstacle";
                                t.gameObject.GetComponent<SpriteRenderer>().color = Color.black;
                                t.tag = "Obstacle";
                                t.localScale = new Vector3(objectToSpawnTemplate.length, objectToSpawnTemplate.heigth, 0);
                                
                                objectToSpawnTemplate.heigth /= 2;
                            }
                            // if not we just spanwn
                            else{
                                interactableObjects.Add(objectToSpawnTemplate);
                                Transform t = Instantiate(transform, ouder, false);
                                t.localPosition = objectToSpawnTemplate.origin;
                                t.localRotation = Quaternion.identity;
                                t.tag = "Obstacle";
                                t.gameObject.GetComponent<SpriteRenderer>().color = Color.black;
                                t.tag = "Obstacle";
                                t.localScale = new Vector3(objectToSpawnTemplate.length, objectToSpawnTemplate.heigth, 0);
                            }

                        }


                        



                       /* //check if after a doubleJump power up
                        foreach (AbilityObject ABObj in spawnedAbilities.FindAll(e => e.Ability is DoubleJump))
                        {
                            //comes after the upgrade and before it's start + 10
                            if (objectToSpawn.startPoint.x > ABObj.startPoint.x + 1 && objectToSpawn.startPoint.x < ABObj.startPoint.x + 10)
                            {
                                t.localScale = new Vector3(objectToSpawn.length, objectToSpawn.heigth*2, 0);
                            }
                        }*/





                    }
                }
            }
        }

    }

    private bool checkIfObjectInsideOther(ArrayList alreadySpawnedObjects, InteractableObject boxTemplate)
    {
       if(alreadySpawnedObjects.Count == 0) return false;

       foreach(InteractableObject box in alreadySpawnedObjects)
       {
            //check of het startpunt van de box tussen het einde of begin van de al bestaande boxen valt of korter voor het begin dan de box die bijkomt lang is
            if(box.startPoint.x <= boxTemplate.startPoint.x - boxTemplate.length && boxTemplate.endPoint.x <= box.endPoint.x)
            {
                return true;
            }
       }
       return false;    
    }


    private void DetermineAbilityXCoordinates()
    {
        System.Random random = new System.Random();
        // 1 minder zodat als er toevallifg altijd de laatste zou worden geselcteerd zal de laatste niet op het laatste platform spawnen
        int sectionlength = maxLevelLength / aantalAbilities -1 ;
        float lastSpawnedPoint = 0;

        //zolang we niet het maximum aantal abilities overschrijden
        for (int i = 0; i < aantalAbilities; i++)
        {
            int AbilityXCoordinate = random.Next(i*sectionlength  ,(i+1)*sectionlength);
            //als die te kort staat op de huidige zetten we hem wat verder
            if (AbilityXCoordinate-lastSpawnedPoint<= sectionlength/4)
            {
                //was aan het moeilijk doen over da het decimal of double kon zijn dus heb ik het in double geforced
                AbilityXCoordinate += sectionlength / 4 ;
            }

            AbilityObject abilityObject= new ();
            abilityObject.startPoint = new Vector2(AbilityXCoordinate, 0);

            

            switch (listOfAbilities[random.Next(listOfAbilities.Count)])
            {
                case "speed":
                    abilityObject.Ability = new SpeedBoost();
                    listOfAbilities.Remove("speed");
                    break;
                /*case "time":
                    abilityObject.Ability = new SlowmotionToggle();
                    listOfAbilities.Remove("time");
                    break;*/
                case "double":
                    abilityObject.Ability = new DoubleJump();
                    listOfAbilities.Remove("double");
                    break;
            }
            spawnedAbilities.Add(abilityObject);
            lastSpawnedPoint = abilityObject.startPoint.x;

            /*
            Platform platform = (Platform) platforms[platMetAbilityXCoordinate];

            float platformlength = platform.endPoint.x - platform.startPoint.x;
            float abilityX = random.Next((int)platformlength);
            abilityObject.startPoint = new(platform.startPoint.x + abilityX, platform.startPoint.y +1);
            */


        }

    }

    public void placeAbilities(AbilityObject abilityObjectTemplate, Transform transform)
    {
        foreach (AbilityObject ability in spawnedAbilities)
        {
            Platform platNaAbility = platforms.Find(x => x.startPoint.x > ability.startPoint.x);
            Platform plat = platforms[platforms.IndexOf(platNaAbility) - 1];
            if (plat != null)
            {
                abilityObjectTemplate.startPoint = new Vector2(ability.startPoint.x, plat.startPoint.y +1);

                //will be first thing to spawn so redundant to check if it is spawned in something else
                interactableObjects.Add(abilityObjectTemplate);
                Transform abilityTransform = Instantiate(transform, ouder, false);
                abilityTransform.position = abilityObjectTemplate.origin;
                abilityTransform.rotation = Quaternion.identity;
                abilityTransform.localScale = new Vector3(abilityObjectTemplate.length, abilityObjectTemplate.heigth, 0);

                abilityTransform.gameObject.GetComponent<GiveAbilityScript>().ability = ability.Ability;

                if (ability.Ability is SpeedBoost)
                {
                    abilityTransform.gameObject.GetComponent<SpriteRenderer>().color = Color.yellow;
                    abilityTransform.tag = "SpeedBoost";
                }/*else if(ability.Ability is SlowmotionToggle)
                {
                    abilityTransform.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
                }*/
                else if (ability.Ability is DoubleJump)
                {
                    abilityTransform.gameObject.GetComponent<SpriteRenderer>().color = Color.blue;
                    abilityTransform.tag = "DoubleJump";
                }

            }
            
        }

        spawnedDoubleJumps.AddRange(spawnedAbilities.FindAll(e => e.Ability is DoubleJump));
        
    }

}