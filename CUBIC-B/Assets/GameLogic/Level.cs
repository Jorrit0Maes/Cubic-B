using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Level : MonoBehaviour
{

    // Levelgeneration
    private List<Platform> platforms;
    private List<Transform> platformsTransforms;
    private List<Transform> deathBoxesTransforms;
    private List<DeathBox> deathBoxes;

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
    public Transform pikeExmp;
    public Transform missleExmp;
    public Transform wallJumpExmp;
    public Transform rightWallExmp;
    public Transform leftWallExmp;
    private ArrayList interactableObjects = new ArrayList();
    public float sizeOfBox;
    public int maxNumberOfPikes;
    public float sizeOfAbilityObject;
    public int aantalAbilities;
    public float respawnTimer;
    public float missleLength;
    public float missleHeight;
    public float missleSpeed;
    List<String> listOfAbilities = new List<string> { "speed", "double" };
    public Transform deathBoxPreFab;
    public GameObject MLModel;
    public GameObject Player;
    public GameObject Spawn;
    private List<AbilityObject> spawnedAbilities;
    private List<AbilityObject> spawnedDoubleJumps;

    private int SpeedBoostAdjustments;

    private List<Transform> abilityTransforms= new List<Transform>();
    public Transform Finish;

    public int kansOpJumps;
    public int wallJumpMinHeight;
    public int wallJumpMaxHeight;

    public float minPlatformLentghForMissle;
    public int chanceOfMissleSpawning;

    public Transform ouder;
    private void Awake()
    {
        platforms = new();
        platformsTransforms = new();
        deathBoxesTransforms= new();
        deathBoxes = new();
        spawnedAbilities = new();
        spawnedDoubleJumps = new();
        SpeedBoostAdjustments = 0;
        DetermineAbilityXCoordinates();
        generateLevelPlatforms();
        placeWallJumps(wallJumpExmp,rightWallExmp,leftWallExmp, squareExmp);
        AbilityObject abilityObject = new ();
        abilityObject.length = sizeOfAbilityObject;
        abilityObject.height = sizeOfAbilityObject;
        placeAbilities(abilityObject, roundExmp);
        Box boxtemp = new();
        boxtemp.height = sizeOfBox;
        boxtemp.length = sizeOfBox; 
        placeObjects(boxtemp, squareExmp);
        Pike piketemp = new();
        piketemp.length = sizeOfBox;
        piketemp.height = sizeOfBox;
        //pikeExmp.GetComponent<DeathScript>().Spawn = Spawn;
        //pikeExmp.GetComponent<DeathScript>().Player= Player;
        placePikes(piketemp, pikeExmp);
        Missle missleTemp = new();
        missleTemp.length = missleLength;
        missleTemp.height = missleHeight;
        placeMissles(missleTemp, missleExmp);
    }



    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        foreach ( Transform trans in abilityTransforms)
        {
            if (!trans.gameObject.activeSelf)
            {
                StartCoroutine(RespawnObject(trans.gameObject, respawnTimer ));

            }
        }

       


    }

    private IEnumerator RespawnObject(GameObject toRespawn, float delay)
    {
        yield return new WaitForSeconds(delay);
        toRespawn.SetActive(true);
    }

    private void generateLevelPlatforms()
    {
        
        float lengthleft = maxLevelLength;

        Vector2 lastPoint = new(2f,0f);


        var random = new System.Random();


        while ( lengthleft > minSizeLastPlatform )
        {

            float length = random.Next(minPlatformSize,maxPlatformSize);  


            //rand doet alleen ints dus we delen door 100 om een getal tot 2 na de komme te krijgen dunno Y ma werkt niet als ik dat in de toewijzing doe beste gok is Int reasons
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


            Platform tempPlatform = new Platform(new Vector2(lastPoint.x+xSpacing, lastPoint.y + ySpacing));
            tempPlatform.length = length;
            

            Transform tempInitiated = Instantiate(squareExmp, ouder, false);
            tempInitiated.localPosition = tempPlatform.origin;
            tempInitiated.localScale = new Vector3(length, 1, 1);
            DeathBox deathBox = new ();
            deathBox.startPoint = new(lastPoint.x - 2 , tempPlatform.endPoint.y-1);
            deathBox.length = tempPlatform.startPoint.x - lastPoint.x +4;

            var tempDeathbox = Instantiate(deathBoxPreFab, ouder, false);
            tempDeathbox.localPosition = deathBox.origin;
            tempDeathbox.tag = "DeathBox";
            tempDeathbox.localScale = new(deathBox.length,1,1);
          

            platforms.Add(tempPlatform);
            platformsTransforms.Add(tempInitiated);
            deathBoxesTransforms.Add(tempDeathbox);
            deathBoxes.Add(deathBox);

            lengthleft -= length;
            lastPoint.x = tempPlatform.endPoint.x;
            //top that matters for matching new platform
            lastPoint.y = tempPlatform.startPoint.y;
        }


        Platform lastPlatform = platforms[platforms.Count - 1];

        DeathBox lastdeathBox = new();
        lastdeathBox.startPoint = new(lastPoint.x - 2, lastPoint.y -3) ;
        lastdeathBox.length = 6;
        deathBoxes.Add(lastdeathBox);

        Transform tempLastDeathbox = Instantiate(deathBoxPreFab, ouder, false);
        tempLastDeathbox.localPosition = lastdeathBox.origin;
        tempLastDeathbox.tag = "DeathBox";
        tempLastDeathbox.localScale = new(lastdeathBox.length, 1, 1);
        deathBoxesTransforms.Add(tempLastDeathbox);

        Finish.localPosition = new Vector3(lastPoint.x + 1.5f, lastPoint.y + 0.5f);
        




    }

    private void placeWallJumps(Transform wallJump, Transform rightWall, Transform leftWall, Transform helpPlatformTrans)
    {
        System.Random random = new();
        foreach(Platform platform in platforms) {
            WallJump templateWallJump = new();

            if (random.Next(100) <= kansOpJumps && platforms.Count - 1 > platforms.IndexOf(platform) && platform.length >= templateWallJump.length + 2 && spawnedAbilities.Find(e => e.startPoint.x > platform.startPoint.x && e.startPoint.x < platform.endPoint.x) == null)
            {
                templateWallJump.startPoint = new(platform.endPoint.x - 5, platform.startPoint.y + 1);
                templateWallJump.height = random.Next(wallJumpMinHeight, wallJumpMaxHeight + 1);

                Transform initWallJump = Instantiate(wallJump, ouder, false);
                initWallJump.localPosition = templateWallJump.origin;
                initWallJump.localScale = new(1,templateWallJump.height);
                initWallJump.name = "WallJump";

                MiniWall rightHelp = new();
                rightHelp.startPoint = new(templateWallJump.endPoint.x - rightHelp.length, templateWallJump.startPoint.y - 1);

                Transform initRightHelp = Instantiate(rightWall, ouder, false);
                initRightHelp.localPosition= rightHelp.origin;

                MiniWall leftHelp = new();
                leftHelp.startPoint = new(templateWallJump.startPoint.x, templateWallJump.endPoint.y);

                Transform initLeftHelp = Instantiate(leftWall, ouder, false);
                initLeftHelp.localPosition = leftHelp.origin;

                Platform helpPlatform = new(templateWallJump.endPoint);
                helpPlatform.length = 0.5f;
                helpPlatform.height = 1f;
                Transform initHelpPlatform = Instantiate(helpPlatformTrans, ouder, false);
                initHelpPlatform.localPosition = helpPlatform.origin;
                initHelpPlatform.localScale = new(helpPlatform.length, helpPlatform.height);
                initHelpPlatform.GetComponent<SpriteRenderer>().color = initRightHelp.GetComponent<SpriteRenderer>().color;
                initHelpPlatform.tag = "Ground";

                interactableObjects.Add(templateWallJump);

                for (int i = platforms.IndexOf(platform) + 1; i < platforms.Count; i++)
                {
                    platforms[i].startPoint = new(platforms[i].startPoint.x, platforms[i].startPoint.y + templateWallJump.height + rightHelp.height);
                    platformsTransforms[i].localPosition = new(platformsTransforms[i].localPosition.x, platformsTransforms[i].localPosition.y + templateWallJump.height + rightHelp.height);
                    deathBoxesTransforms[i].localPosition = new(deathBoxesTransforms[i].localPosition.x, deathBoxesTransforms[i].localPosition.y + templateWallJump.height + rightHelp.height);
                }
                //eerst volgende deathbox opschuiven om deze niet in de muur te hebben
                Transform firstDBAfterWallTrans = deathBoxesTransforms[platforms.IndexOf(platform) + 1];

                //voer nieuw startpunt in maar laat lengte het zelfde maakt niet echt uit
                DeathBox firstDBAfterWall = deathBoxes[platforms.IndexOf(platform) + 1];
                firstDBAfterWall.startPoint = new(templateWallJump.endPoint.x, templateWallJump.endPoint.y - 1);

                // verplaats de deathbox naar zijn nieuwe oorsprong met het nieuwe startpunt
                firstDBAfterWallTrans.localPosition = firstDBAfterWall.origin;

                var lastDeathBox = deathBoxesTransforms.Last();
                lastDeathBox.localPosition = new(lastDeathBox.localPosition.x, lastDeathBox.localPosition.y + templateWallJump.height + rightHelp.height);
                Finish.localPosition = new(Finish.localPosition.x, Finish.localPosition.y + templateWallJump.height + rightHelp.height);
                //mini walls
            }
        }
    }

    //################## OBSTACLES ##################
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

                if(DoubleJumpPlatform) {  numberofObstacles = numberofObstacles + (int) Math.Ceiling(numberofObstacles *0.5); }
                for (int i = 0; i < numberofObstacles; i++)
                {
                    Box boxClone = new Box();
                    boxClone.length = objectToSpawnTemplate.length;
                    boxClone.height = objectToSpawnTemplate.height;

                    //to not put it on the front edge so we can make the jump or off the platform we adjust the limits
                    //generating floating point between bounds
                    double upperBound = platform.endPoint.x - 1;
                    double lowerBound = Math.Ceiling(platform.startPoint.x) + 1;
                    float obstacleVectorX = (float)(random.NextDouble() * (upperBound - lowerBound) + lowerBound);
                    boxClone.startPoint = new Vector2(obstacleVectorX, platform.startPoint.y);


                    if (interactableObjects.Count == 0 || !checkIllegalToSpawn(interactableObjects, boxClone))
                    {
                        foreach (AbilityObject ab in spawnedAbilities.FindAll(x => x.Ability is DoubleJump))
                        {
                            //if it comes after the actual doubleJump on the platform we double the height we need to do change the objects height to determine the right origin but just doubling it will double it everytime and keep it doubles because it is a refernce so we set it back after we are done
                            if (boxClone.startPoint.x > ab.startPoint.x +1 && boxClone.startPoint.x < 10 + ab.startPoint.x)
                            {
                                boxClone.height  *= 2;
                                interactableObjects.Add(boxClone);
                                Transform t = Instantiate(transform,ouder,false);
                                t.localPosition = boxClone.origin;
                                t.gameObject.GetComponent<SpriteRenderer>().color = Color.black;
                                t.tag = "Obstacle";
                                t.name = "BOX" + i.ToString();
                                t.localScale = new Vector3(boxClone.length, boxClone.height, 0);
                                boxClone.height /= 2;
                            }
                            // if not we just spawn
                            else{
                                interactableObjects.Add(boxClone);
                                Transform t = Instantiate(transform, ouder, false);
                                t.localPosition = boxClone.origin;
                                t.gameObject.GetComponent<SpriteRenderer>().color = Color.black;
                                t.tag = "Obstacle";
                                t.name = "BOX" + i.ToString();

                                t.localScale = new Vector3(boxClone.length, boxClone.height, 0);
                            }

                        }

                       /* //check if after a doubleJump power up
                        foreach (AbilityObject ABObj in spawnedAbilities.FindAll(e => e.Ability is DoubleJump))
                        {
                            //comes after the upgrade and before it's start + 10
                            if (objectToSpawn.startPoint.x > ABObj.startPoint.x + 1 && objectToSpawn.startPoint.x < ABObj.startPoint.x + 10)
                            {
                                t.localScale = new Vector3(objectToSpawn.length, objectToSpawn.height*2, 0);
                            }
                        }*/

                    }
                }
            }
        }

    }

    private void placePikes(Pike pikeTemplate, Transform transform)
    {

        System.Random random = new System.Random();

        foreach (Platform platform in platforms)
        {

            float platformlength = platform.endPoint.x - platform.startPoint.x;
            

            int chanceofBoxSpawning = random.Next(minPlatformSize , maxPlatformSize ) ;
            //if the platform has a doublejump on it not near the end always spawn obstacles or if the double jump is near the start of the platform
            bool DoubleJumpPlatform = !spawnedDoubleJumps.Find(e => (e.startPoint.x >= platform.startPoint.x && e.startPoint.x <= platform.endPoint.x - 5)).IsUnityNull() || !spawnedDoubleJumps.Find(e => (e.startPoint.x >= platform.startPoint.x - 5 && e.startPoint.x <= platform.startPoint.x)).IsUnityNull();


            // the larger the platform the higher the chance of a box spawning or if big enough we defintetly spawn .
            if (platformlength >= maxPlatformSize / 3 || chanceofBoxSpawning < platformlength || DoubleJumpPlatform)
            {
                // we ensure not to fill the platform with so much boxes that it is all box  => platformLength/3 = number of boxes *sizeof of a box only half of the length filled
                int maxNumberOfObstacles = (int)Math.Floor(platformlength * sizeOfBox);//size of box is kleiner dan nul dus verlagen door vermenigvuldiging

                int numberofObstacles = random.Next(maxNumberOfObstacles);

                if (DoubleJumpPlatform) { numberofObstacles = numberofObstacles + (int)Math.Ceiling(numberofObstacles * 0.5); }
                for (int i = 0; i < numberofObstacles; i++)
                {
                    Pike pikeClone = new Pike();
                    pikeClone.length = pikeTemplate.length;
                    pikeClone.height = pikeTemplate.height;
                    //to not put it on the front edge so we can jump on or off the platform we adjust the limits
                    //generating floating point between bounds
                    double upperBound = platform.endPoint.x - 1;
                    double lowerBound = Math.Ceiling(platform.startPoint.x) + 1;
                    float obstacleVectorX =  (float) (random.NextDouble() *(upperBound -lowerBound) + lowerBound);
                    pikeClone.startPoint = new Vector2(obstacleVectorX, platform.startPoint.y);

                    //random 1 2 of 3 pikes achter elkaar om de lengte te bepalen
                    int pikelength = random.Next(maxNumberOfPikes) +1;

                    // als er pikes kunnen worden gezet maar gewoon niet zoveel als de random heeft geselecteerd gaan we voor het max aantal dat wel gaat
                    Pike tempPike = new Pike();
                    tempPike.startPoint = pikeClone.startPoint;
                    tempPike.height = pikeClone.height; 
                    tempPike.length = pikelength * pikeClone.length;
                    int k = 0;

                    //check if after double jump if it is double the length
                    foreach (AbilityObject ab in spawnedAbilities.FindAll(x => x.Ability is DoubleJump))
                    {
                        //check if it comes after the actual doubleJump on the platform 
                        if (pikeClone.startPoint.x > ab.startPoint.x + 1 && pikeClone.startPoint.x < 10 + ab.startPoint.x)
                        {
                            //bij een doublejump maken we deze twee keer zo lang
                            pikelength *= 2;
                            tempPike.length = pikelength * pikeClone.length;

                        }


                    }
                    //als de hele lengte gespawned kan worden geen probleem en geeft de "checkIfObjectInsideOther" false en stappen we niet in de while loop en de minimum lengte moet 1 zijn dus k < pikelength
                    while (k < pikelength && checkIllegalToSpawn(interactableObjects, tempPike))
                    {
                        k++;
                        // we verlagen het aantal pikes met 1 en de while checkt of dit werkt
                        tempPike.length = (pikelength - k) * pikeClone.length;
                        pikelength -= k;
                        
                    }
                    
                    if (interactableObjects.Count == 0 || !checkIllegalToSpawn(interactableObjects, tempPike))
                    {
                       
                        interactableObjects.Add(tempPike);
                        for (int j = 0; j < pikelength; j++)
                        {

                            Transform t = Instantiate(transform, ouder, false);
                            t.localPosition = pikeClone.origin;
                            t.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
                            t.tag = "Pike";
                            t.name = "PIKE" + i.ToString();
                            t.localScale = new Vector3(pikeClone.length, pikeClone.height, 0);
                            pikeClone.startPoint = new(pikeClone.endPoint.x, pikeClone.startPoint.y);
                        }

                    }
                }
            }
        }

    }

    private void placeMissles(Missle missleTemplate, Transform transform)
    {
        System.Random random= new ();
        foreach (Platform platform in platforms.FindAll(e => e.length > minPlatformLentghForMissle))
        {
            if(random.Next(100) <= chanceOfMissleSpawning)
            {
                Missle missleClone = new ();
                missleClone.startPoint = new(platform.endPoint.x, platform.startPoint.y);
                missleClone.height = missleTemplate.height;
                missleClone.length = missleTemplate.length;

                Transform newMissle = Instantiate(transform, ouder, false);
                newMissle.localPosition = missleClone.origin;
                newMissle.tag = "Missle";
                newMissle.GetComponent<MissleMovement>().speed = missleSpeed;
                newMissle.GetComponent<MissleMovement>().Players.AddRange(new List<GameObject> { Player, MLModel });
                //newMissle.GetComponent<DeathScript>().Player = Player;
                //newMissle.GetComponent<DeathScript>().Spawn = Spawn;


            }
        }

    }

    //################## ABILITIES ##################
    private void DetermineAbilityXCoordinates()
    {
        System.Random random = new System.Random();
        // 1 minder zodat als er toevallifg altijd de laatste zou worden geselcteerd zal de laatste niet op het laatste platform spawnen
        int sectionlength = maxLevelLength / aantalAbilities -1 ;
        float lastSpawnedPoint = 0;

        //zolang we niet het maximum aantal abilities overschrijden
        for (int i = 0; i < aantalAbilities; i++)
        {
            int AbilityXCoordinate = random.Next(i*sectionlength, (i+1)*sectionlength);
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
            Platform platNaAbility = platforms.Find(x => x.startPoint.x > ability.startPoint.x) ?? platforms.Last();
            int platNaAbilityIndex = platforms.IndexOf(platNaAbility);
            Platform plat;
            if (platNaAbilityIndex <= 0)
            {
                plat = platforms[0];
            }
            else
            {
                plat = platforms[platNaAbilityIndex - 1];
            }
            if (plat != null)
            {
                abilityObjectTemplate.startPoint = new Vector2(ability.startPoint.x, plat.startPoint.y +1);

                //will be first thing to spawn so redundant to check if it is spawned in something else
                interactableObjects.Add(abilityObjectTemplate);
                Transform abilityTransform = Instantiate(transform, ouder, false);
                abilityTransform.localPosition = abilityObjectTemplate.origin;
                abilityTransform.localScale = new Vector3(abilityObjectTemplate.length, abilityObjectTemplate.height, 0);

                abilityTransform.gameObject.GetComponent<GiveAbilityScript>().ability = ability.Ability;
                abilityTransforms.Add(abilityTransform);

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

    //##################  CHECKS ##################
    private bool checkIllegalToSpawn(ArrayList alreadySpawnedObjects, InteractableObject newObject)
    {
        if (alreadySpawnedObjects.Count == 0) return false;
        if (isNotOnPlatform(newObject)) return true;

        foreach (InteractableObject alreadyOnMapObject in alreadySpawnedObjects)
        {
            //check of het startpunt van de box tussen het einde of begin van de al bestaande boxen valt of korter voor het begin dan de box die bijkomt lang is
            if (startPointInObject(alreadyOnMapObject, newObject) || endPointInObject(alreadyOnMapObject, newObject) || startPointInObject(newObject, alreadyOnMapObject) || endPointInObject(newObject, alreadyOnMapObject) || TooClose(newObject, alreadyOnMapObject))
            {
                return true;
            }
        }
        return false;
    }
    public bool startPointInObject(InteractableObject albestaand, InteractableObject nieuw)
    {
        return (albestaand.startPoint.x <= nieuw.startPoint.x && nieuw.startPoint.x <= albestaand.endPoint.x);
    }
    public bool endPointInObject(InteractableObject albestaand, InteractableObject nieuw)
    {
        return (albestaand.startPoint.x <= nieuw.endPoint.x && nieuw.endPoint.x <= albestaand.endPoint.x);
    }
    public bool TooClose(InteractableObject albestaand, InteractableObject nieuw)
    {
        if(albestaand is Pike && nieuw is Pike)
        {
            return Math.Abs(albestaand.startPoint.x - nieuw.endPoint.x) <= 1 || Math.Abs(albestaand.endPoint.x - nieuw.startPoint.x) <= 1;
        }
        return false;
    }
    public bool isNotOnPlatform(InteractableObject nieuwObject)
    {
        Platform platformVanObject = platforms.Find(e=>e.startPoint.x -1<nieuwObject.startPoint.x && e.endPoint.x -1> nieuwObject.endPoint.x);

        return platformVanObject == null;
    }
}
