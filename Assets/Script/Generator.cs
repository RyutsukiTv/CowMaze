using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Collections;
using UnityEngine.SceneManagement;



public class Generator : MonoBehaviour
{

    public GameObject wallToPlace;
    public GameObject towerToPlace;
    public GameObject objectifGame;
    public float timerDelay = 0.01f;
    public float length = 21;
    public GameObject objectToSpawn;
    public int[,] myArray ;
    private List<Vector2Int> emptyCells = new List<Vector2Int>();
    private bool bloc = true;
    private int cpt  =1;
    public GameObject mazeParent;
    public bool canmove = false;


    void Start()
    {
        myArray = new int[(int)(length), (int)(length)];
        PlaceObjectsWithDelay();
        UpdateArrayWithNeighbors();
        CallRandomNumberOfTimes();
        borderLimit();
        PlaceAgentOnArray();
        BuildObjectsWithDelay();
        //StartCoroutine(BuildObjectsWithDelay());
        clearMap();
        ObjectifRandomCoord();

        //PrintMap();
        
    }


    void Update()
    {
        // Détectez si la touche "F" est enfoncée
        if (Input.GetKeyDown(KeyCode.F))
        {
            // Rechargez la scène actuelle
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    //placement agent posision 1,1
    void PlaceAgentOnArray(){
        myArray[1, 1]=1;
    }


    //initialisation du labyrinthe 
    void PlaceObjectsWithDelay()
    {
        //initialisation grill
        
        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < length; j++)
            {
                myArray[i, j] = -2;
                if(i%2==1){
                    if(bloc){
                        myArray[i, j] = -2;
                    }else{
                        myArray[i, j] = 0;
                    }
                    bloc = !bloc;
                    
                }
            }
            bloc = true;
        }
        

        //Set Wall
        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < length; j++)
            {
                if(myArray[i, j]==0){
                    myArray[i+1, j]=-1;
                    myArray[i, j+1]=-1;
                }
                
            }
        }

        borderLimit();

        //set  cpt
        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < length; j++)
            {
                if(myArray[i, j]==0){
                    cpt++;
                    myArray[i, j]=cpt;
                }
            }
        }

    }

    void borderLimit(){
        //Set outside Wall and corner tower
        for (int i = 0; i < length; i++)
        {
            myArray[i, 0] = -3;
            myArray[i, (int)(length-1)] = -3;
            for (int j = 0; j < length; j++)
            {
                myArray[0, j] = -3;
                myArray[(int)(length-1), j] = -3;
            }
        }
    }




    //creer la premier plan de labyrinthe
    void UpdateArrayWithNeighbors()
    {
        while (true){
                // Collectez les coordonnées des éléments -1 dans une liste
                List<Vector2Int> minusOneCoords = new List<Vector2Int>();
                for (int i = 0; i < length; i++)
                {
                    for (int j = 0; j < length; j++)
                    {
                        if (myArray[i, j] == -1)
                        {
                            minusOneCoords.Add(new Vector2Int(i, j));
                        }
                    }
                }


                // Choisissez aléatoirement une coordonnée -1
                int randomIndex = Random.Range(0, minusOneCoords.Count);
                Vector2Int randomCoord = minusOneCoords[randomIndex];

                // Obtenez les voisins
                List<int> neighborValues = new List<int>();
                Vector2Int[] neighborOffsets = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
                foreach (Vector2Int offset in neighborOffsets)
                {
                    Vector2Int neighborCoord = randomCoord + offset;
                    if (IsInsideGrid(neighborCoord))
                    {
                        int neighborValue = myArray[neighborCoord.x, neighborCoord.y];
                        if (neighborValue > 0 && !neighborValues.Contains(neighborValue))
                        {
                            neighborValues.Add(neighborValue);
                        }
                    }
                }

                if (neighborValues.Count >= 2)
                {
                    // Choisissez aléatoirement deux voisins distincts
                    int randomNeighborIndex1 = Random.Range(0, neighborValues.Count);
                    int randomNeighborIndex2 = Random.Range(0, neighborValues.Count - 1);
                    if (randomNeighborIndex2 >= randomNeighborIndex1)
                    {
                        randomNeighborIndex2++;
                    }

                    int neighbor1 = neighborValues[randomNeighborIndex1];
                    int neighbor2 = neighborValues[randomNeighborIndex2];

                    // Affectez la valeur du premier voisin à l'élément -1 et remplacez la valeur du deuxième voisin
                    myArray[randomCoord.x, randomCoord.y] = neighbor1;

                    for (int i = 0; i < length; i++)
                    {
                        for (int j = 0; j < length; j++)
                        {
                            if (myArray[i, j] == neighbor2)
                            {
                                myArray[i, j] = neighbor1;
                            }
                        }
                    }
                }

                
                if(EnsurePositiveValuesAreEqualToOneOne()){
                    break;
                }


            }
        }
    

    //verifier si tout les case on la meme valeur
    bool EnsurePositiveValuesAreEqualToOneOne()
    {
        int valueToMatch = myArray[1, 1];

        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < length; j++)
            {
                if (myArray[i, j] > 0 && myArray[i, j] != valueToMatch)
                {
                    return false;
                }
            }
        }

        return true; 
    }

    //utile tqt
    bool IsInsideGrid(Vector2Int coord)
    {
        return coord.x >= 0 && coord.x < length && coord.y >= 0 && coord.y < length;
    }

    //casser un mur aleatoire
    void ReplaceRandomValueWithZero()
    {
        // Collectez les coordonnées des éléments non nuls dans une liste
        List<Vector2Int> nonZeroCoords = new List<Vector2Int>();
        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < length; j++)
            {
                if (myArray[i, j] != 0)
                {
                    nonZeroCoords.Add(new Vector2Int(i, j));
                }
            }
        }

        // Si la liste contient des coordonnées non nulles
        if (nonZeroCoords.Count > 0)
        {
            // Choisissez aléatoirement une coordonnée non nulle
            int randomIndex = Random.Range(0, nonZeroCoords.Count);
            Vector2Int randomCoord = nonZeroCoords[randomIndex];

            // Changez la valeur de l'élément à cette coordonnée en 0
            myArray[randomCoord.x, randomCoord.y] = 0;
        }
        
        // Collectez les coordonnées des éléments -1 dans une liste
        List<Vector2Int> minusOneCoords = new List<Vector2Int>();
        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < length; j++)
            {
                if (myArray[i, j] == -1)
                {
                    minusOneCoords.Add(new Vector2Int(i, j));
                }
            }
        }


        // Si la liste contient des coordonnées -1
        if (minusOneCoords.Count > 0)
        {
            // Choisissez aléatoirement une coordonnée -1
            int randomIndex = Random.Range(0, minusOneCoords.Count);
            Vector2Int randomCoord = minusOneCoords[randomIndex];

            // Changez la valeur de l'élément à cette coordonnée en 0
            myArray[randomCoord.x, randomCoord.y] = 0;
        }
    }

    //choisir plusieur fois un mur aleatoirement
    void CallRandomNumberOfTimes()
    {
        int numberOfCalls = Random.Range(1, (int)(length/4)); 
        for (int i = 0; i < numberOfCalls; i++)
        {
            ReplaceRandomValueWithZero(); 
        }
    }

    //placement de l objectif de l agent
    void ObjectifRandomCoord()
    {

        // Créer une liste pour stocker les indices des cases contenant des zéros.
        List<Vector2Int> zeroIndices = new List<Vector2Int>();

        // Parcourir le tableau pour trouver les indices des zéros.
        for (int x = 0; x < length; x++)
        {
            for (int y = 0; y < length; y++)
            {
                if (myArray[x, y] == 0)
                {
                    zeroIndices.Add(new Vector2Int(x, y));
                }
            }
        }

        // Si des zéros ont été trouvés, choisissez un indice au hasard.
        if (zeroIndices.Count > 0)
        {
            int randomIndex = Random.Range(0, zeroIndices.Count);

            // Placez -3 à l'indice choisi au hasard.
            Vector2Int randomZeroIndex = zeroIndices[randomIndex];
            myArray[randomZeroIndex.x, randomZeroIndex.y] = -4;



            // Créez un nouvel objet de objectif à la position souhaitée
            Vector3 newPosition = new Vector3(randomZeroIndex.x, 0, randomZeroIndex.y); // Utilisez les indices du tableau pour positionner l'objet
            GameObject wall = Instantiate(objectifGame, newPosition, Quaternion.identity);


        }
    }



    //poser les blocs

    //IEnumerator 
    void BuildObjectsWithDelay()
    {
        for (int i = 0; i < length; i++)
        {
            
            for (int j = 0; j < length; j++)
            {
                if (myArray[i, j] == -1 || myArray[i, j] == -3)
                {
                    // Créez un nouvel objet de mur à la position souhaitée
                    Vector3 newPosition = new Vector3(i, 0, j); // Utilisez les indices du tableau pour positionner l'objet
                    GameObject wall = Instantiate(wallToPlace, newPosition, Quaternion.identity);

                    // Placez l'objet sous le GameObject parent
                    wall.transform.parent = mazeParent.transform;
                }
                if (myArray[i, j] == -2)
                {
                    // Créez un nouvel objet de tour à la position souhaitée
                    Vector3 newPosition = new Vector3(i, 0, j); // Utilisez les indices du tableau pour positionner l'objet
                    GameObject tower = Instantiate(towerToPlace, newPosition, Quaternion.identity);

                    // Placez l'objet sous le GameObject parent
                    tower.transform.parent = mazeParent.transform;
                }
                if (myArray[i, j] == 1)
                {
                    // Créez un nouvel objet de vache à la position souhaitée
                    Vector3 newPosition = new Vector3(i, 0.5f, j); // Utilisez les indices du tableau pour positionner l'objet
                    
                    GameObject spawnedObject = Instantiate(objectToSpawn, newPosition, Quaternion.identity);
                    Move moveScript = spawnedObject.GetComponent<Move>();
                    if (moveScript != null)
                    {
                        moveScript.myArray = myArray;
                    }
                    else
                    {
                        Debug.LogError("Le script Move.cs n'a pas été trouvé sur l'objet nouvellement créé.");
                    }

                }

                
                //yield return new WaitForSeconds(timerDelay); // Attendez 0,02 seconde
            }
        }

        canmove=true;
    }





    //remplacer les valeur aleatoir de la generation pour 0 afin d avoir une meilleur vision
    void clearMap(){
        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < length; j++)
            {
                if(myArray[i,j]>1&& myArray[i,j]!=1){
                    myArray[i,j]=0;
                }
                if(myArray[i,j]<0){
                    myArray[i,j]=-1;
                }
            }
        }
        
    }



    //print map etude 
    public void PrintMap()
{
    if (myArray != null)
    {
        int rows = myArray.GetLength(0);
        int columns = myArray.GetLength(1);

        string arrayString = "";

        for (int i = 0; i < rows; i++)
        {
            arrayString += "";
            for (int j = 0; j < columns; j++)
            {
                int value = myArray[i, j];
                switch (value)
                {
                    case -1:
                        arrayString += "🟫"; // Mur
                        break;
                    case 0:
                        arrayString += "⬜"; // Passage
                        break;
                    case 2:
                        arrayString += "🟪"; // Chemin
                        break;
                    case 1:
                        arrayString += "🟥"; // Départ
                        break;
                    case -4:
                        arrayString += "🟦"; // Fin
                        break;
                    default:
                        arrayString += value.ToString();
                        break;
                }

                if (j < columns - 1)
                {
                    arrayString += "";
                }
            }
            arrayString += "";
            if (i < rows - 1)
            {
                arrayString += "\n";
            }
        }

        Debug.Log(arrayString);
    }
    else
    {
        Debug.LogError("myArray est null. Assurez-vous qu'il est correctement initialisé.");
    }
}
}
