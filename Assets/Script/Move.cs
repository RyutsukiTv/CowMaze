using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Video;


public class Move : MonoBehaviour
{
    public int[,] myArray;
    public int[,] mapping;
    public GameObject agent;
    public GameObject videoObject; // Faites glisser votre GameObject avec la vidéo ici dans l'inspecteur Unity.
    private VideoPlayer videoPlayer;
    public GameObject objectif;
    public int goalX;
    public int goalZ ;
    public float waitTime = 0.0f;
    public int x ;
    public int z ;
    private List<string> movements = new List<string>();


    void Start()
    {
        videoPlayer = videoObject.GetComponent<VideoPlayer>();

        if (videoPlayer != null)
        {
            // Configurez les paramètres du lecteur vidéo si nécessaire, par exemple, le volume, la vitesse, etc.
            videoPlayer.playOnAwake = false; // Pour éviter la lecture automatique.

            // Si vous voulez lire la vidéo dès le démarrage du jeu, décommentez la ligne suivante.
            // videoPlayer.Play();
        }
        else
        {
            Debug.LogError("Le GameObject ne contient pas de composant VideoPlayer.");
        }
    }

    private void Update()
    {
    if (Input.GetKeyDown(KeyCode.O))
            {

       

        x = Mathf.RoundToInt(agent.transform.position.x);
        z = Mathf.RoundToInt(agent.transform.position.z);

        int rows = myArray.GetLength(0);
        int columns = myArray.GetLength(1);
        
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                if (myArray[i, j] == -4)
                {
                    Debug.Log("Position of -4: x=" + i + ", z=" + j);
                    goalX = i;
                    goalZ = j;
                }
            }
        }

        if (myArray != null)
        {
            // Initialisation de la carte vide
            CreateEmptyMap();

            StartCoroutine(WaitAndExecute());

            // Affichage de la carte (vous devez décider où vous souhaitez l'afficher)
        
        }
        else
        {
            Debug.LogError("myArray n'a pas été correctement transmis depuis Generator.cs.");
        }
    }
}

    IEnumerator WaitAndExecute()
    {
        yield return new WaitForSeconds(waitTime);

        // Algorithme de parcours sur la carte
        ParcourMap();
    }



    private void CreateEmptyMap()
    {
        int rows = myArray.GetLength(0);
        int columns = myArray.GetLength(1);
        
        mapping = new int[rows, columns];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                mapping[i, j] = 0;
            }
        }
    }

    private void ParcourMap()
    {
        // Les valeurs autour de l'agent (Gauche, Droite, Bas, Haut)
        List<int> aroundValues = new List<int>();

        if (x - 1 >= 0)
            aroundValues.Add(myArray[x - 1, z]); // Gauche
        if (x + 1 < myArray.GetLength(0))
            aroundValues.Add(myArray[x + 1, z]); // Droite
        if (z - 1 >= 0)
            aroundValues.Add(myArray[x, z - 1]); // Bas
        if (z + 1 < myArray.GetLength(1))
            aroundValues.Add(myArray[x, z + 1]); // Haut

        Debug.Log(aroundValues[0]+"|"+aroundValues[1]+"|"+aroundValues[2]+"|"+aroundValues[3]+"\n");

        //mise en place des mur dans le mapping

        if(aroundValues[0]==-1){
            mapping[x - 1, z] = -1;  //gauche
        }
        if(aroundValues[1]==-1){
            mapping[x + 1, z] = -1;  //droite
        }
        if(aroundValues[2]==-1){
            mapping[x, z - 1] = -1;  //bas
        }
        if(aroundValues[3]==-1){
            mapping[x, z + 1] = -1;  //haut
        }

        //prendre la valeur la plus petit et superieur a 0

        int coordValue = -1; // Initialize with -1 to indicate no valid direction found yet
        int NbValue = int.MaxValue; // Initialize with the smallest possible integer value


        for (int i = 0; i < aroundValues.Count; i++)
        {
            if ((aroundValues[i] >= 0 || aroundValues[i] == -4 )&&aroundValues[i] < 500  )
            {
                if (aroundValues[i] <= NbValue)
                {
                    NbValue = aroundValues[i];
                    coordValue = i;
                }
            }
        }

        if (coordValue != -1)
        {
            Debug.Log("Direction: " + coordValue + " | Value of the Direction: " + NbValue);
        }
        else
        {
            Debug.Log("No valid direction found.");
        }
        //si il y a une seul possibiliter ajouter 1 a la case actuelle en valeur  (si cul de sac)

        int countNegativeOnes = 0;
        foreach (int value in aroundValues)
        {
            if (value == -1 || value > 500)
            {
                countNegativeOnes++;
            }
        }

        Debug.Log("Number of -1 values in around Agent: " + countNegativeOnes);

        if(countNegativeOnes>=3){
            Debug.Log("Cul de Sac");
            mapping[x,z] =999;
            myArray[x,z]  = 999;
        }


        //deplacement et ajout de 1 sur la case ou on va   Coordonne actuelle + [move]


        if(coordValue==0){
            if(mapping[x - 1, z]>=0){
                myArray[x - 1, z] +=1;
                mapping[x - 1, z] +=1;  //gauche
            }else{
                mapping[x - 1, z] =1;  
            }
            Debug.Log("Gauche|"+mapping[x - 1, z]);
            GoTo("gauche");

        }
        if(coordValue==1){
            if(mapping[x + 1, z]>=0){
                myArray[x + 1, z] +=1;
                mapping[x + 1, z] +=1;  //droite
            }else{
                mapping[x + 1, z] =1;  
            }

            Debug.Log("droite|"+mapping[x + 1, z]);
            GoTo("droite");

        }
        if(coordValue==2){
            if(mapping[x, z - 1]>=0){
                myArray[x, z - 1] +=1;
               mapping[x, z - 1] +=1;  //bas
            }else{
                mapping[x, z - 1] =1;  
            }
            
            Debug.Log("bas|"+mapping[x, z - 1]);
            GoTo("bas");


        }
        if(coordValue==3){
            if(mapping[x, z + 1]>=0){
                myArray[x, z + 1] +=1;
                mapping[x, z + 1] +=1;  //haut
            }else{
                mapping[x, z + 1] =1;  
            }
            
            Debug.Log("haut|"+mapping[x, z + 1]);
            GoTo("haut");

        }


        //faire le deplacement de movements 
        // et rappeler la fonction
        
        void GoTo(string direction)
        {
            Vector3 currentPosition = agent.transform.position;

            if (direction == "gauche")
            {
                // Move one unit to the left
                agent.transform.position = new Vector3(currentPosition.x - 1, currentPosition.y, currentPosition.z);
            }
            else if (direction == "droite")
            {
                // Move one unit to the right
                agent.transform.position = new Vector3(currentPosition.x + 1, currentPosition.y, currentPosition.z);
            }
            else if (direction == "bas")
            {
                // Move one unit down
                agent.transform.position = new Vector3(currentPosition.x, currentPosition.y, currentPosition.z - 1);
            }
            else if (direction == "haut")
            {
                // Move one unit up
                agent.transform.position = new Vector3(currentPosition.x, currentPosition.y, currentPosition.z + 1);
            }

            x = Mathf.RoundToInt(agent.transform.position.x);
            z = Mathf.RoundToInt(agent.transform.position.z);
            Debug.Log(x+"=="+goalX);
            Debug.Log(z+"=="+goalZ);
            if(x==goalX && z ==goalZ){
                Debug.Log("TERMINER");
                //play wining
                PrintMap(mapping);
            }else{
                StartCoroutine(WaitAndExecute());
            }
        
            
        }









    }







    public void PrintMap( int[,] Array)
    {
        if (Array != null)
        {
            int rows = Array.GetLength(0);
            int columns = Array.GetLength(1);

            string arrayString = "";

            for (int i = 0; i < rows; i++)
            {
                arrayString += "";
                for (int j = 0; j < columns; j++)
                {
                    int value = Array[i, j];
                    if(value==-1){
                        arrayString += "🟫"; //mur
                    }
                    if(value>0&&value<500){
                        arrayString += "🟪";//visiter
                    }
                    if(value>500){
                        arrayString += "🟥"; // Cul de sac
                    }
                    
                    if (value==0)
                    {
                        arrayString += "⬜";//non visiter
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
            Debug.LogError("Array is null. Assurez-vous qu'il est correctement initialisé.");
        }
    }

    // Autres méthodes pour gérer le déplacement et d'autres fonctionnalités ici



}
