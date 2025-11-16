using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class Population2 : MonoBehaviour
{
    static int gen = 0;
    public Text genText;
    public Text scoreText;
    private int highScore;
    public GameObject playerPrefab;
    public GameObject obstaclePrefab;
    //public static bool gameOver = false;
    List<player> pop = new List<player>();
    List<List<float>> champions = new List<List<float>>();
    player topPlayer;
    player secondPlayer;
    public float lastTime = 0f;
    public float obstacleTime = 0f;
    
    // NEW: Variables for increasing difficulty
    private float generationStartTime = 0f;
    private float baseSpawnInterval = 0.4f;
    private float minSpawnInterval = 0.15f; // Minimum time between spawns
    private float difficultyIncreaseRate = 0.02f; // How much faster spawns get per second
    
    void Start()
    {
        Time.timeScale = 1f; 
        CreateText("New Game \n");
        generationStartTime = Time.time;
        
        for (int i = 0; i < 100; i++)
        {
            pop.Add(new player(Instantiate(playerPrefab, new Vector3(Random.Range(-5f, 5f), -3f, 0), Quaternion.identity)));
        }

    }
    
    void CreateText(string content) {
        string path = Application.dataPath +"/Log.txt";
        if (!File.Exists(path)){
            File.WriteAllText(path, "");
        }
        File.AppendAllText(path, content);

    }
    
    public static int getGen()
    {
        return gen;
    }
    
    // NEW: Calculate current spawn interval based on time elapsed
    float GetCurrentSpawnInterval()
    {
        float elapsedTime = Time.time - generationStartTime;
        float currentInterval = baseSpawnInterval - (elapsedTime * difficultyIncreaseRate);
        return Mathf.Max(currentInterval, minSpawnInterval);
    }
    
    void Update()
    {
        if (!allDead())
        {
            // MODIFIED: Use dynamic spawn interval
            float currentSpawnInterval = GetCurrentSpawnInterval();
            
            if (Time.time - obstacleTime > currentSpawnInterval)
            {
                Instantiate(obstaclePrefab, new Vector3(Random.Range(-11.0f, 11.0f), 3.85f, 0), Quaternion.identity);
                obstacleTime = Time.time;
            }

            if (Time.time - lastTime > 0.01)
            {
                foreach (player players in pop)
                {
                    players.PlayerUpdate();
                }
                lastTime = Time.time;
            }
        }
        else
        {
            //they are all dead now
            this.checkFitnesses();
            
            // RESET: Reset timer for new generation
            generationStartTime = Time.time;
            
            //they are all destroyed except for best 2 now
            for (int i = 2; i < 100; i++)
            {
                pop.Add(new player(Instantiate(playerPrefab, new Vector3(Random.Range(-5f, 5f), -3f, 0), Quaternion.identity)));
                pop[i].setDNA(breed(pop[0], pop[1]));
            }
            for (int i = 100; i<100+champions.Count;i++){
                pop.Add(new player(Instantiate(playerPrefab, new Vector3(Random.Range(-5f, 5f), -3f, 0), Quaternion.identity)));
                pop[i].setDNA(champions[i-100]);
                pop[i].GetObject().GetComponent<SpriteRenderer>().color = Color.cyan;
                //Debug.Log("Champion");
            }
        }
    }

    List<float> breed(player p1, player p2)
    {
        int randInt = 0;
        int mutateInt = 0;
        List<float> babyList = new List<float>();

        List<float> p1List = p1.playerDNA();
        List<float> p2List = p2.playerDNA();
        for (int i = 0; i < p1List.Count; i++)
        {
            randInt = Random.Range(0, 2);
            mutateInt = Random.Range(0, 100);

            if (randInt == 0)
            {
                babyList.Add(p1List[i]);
            }
            else
            {
                babyList.Add(p2List[i]);
            }
            
            // IMPROVED: Smaller, more reasonable mutations
            if (mutateInt == 1)
            {
                babyList[i] = babyList[i] - Random.Range(0.1f, 0.3f);
            }
            else if (mutateInt == 2)
            {
                babyList[i] = babyList[i] + Random.Range(0.1f, 0.3f);
            }
        }
        return babyList;
    }
    
    bool allDead()
    {
        for (int i = 0; i < pop.Count; i++)
        {
            if (pop[i].itisDead() == false)
            {
                //Debug.Log(i+" alive");
                return false;
            }

        }
        return true;
    }
    
    public void checkFitnesses()
    {
        //puts top two players into spots 0 and 1 of pop
        player bestPlayer = pop[0];
        player nextBest = pop[1];
        foreach (player players in pop)
        {
            if (bestPlayer.getFitness() < players.getFitness())
            {
                nextBest = bestPlayer;
                bestPlayer = players;
            }
            else if (nextBest.getFitness() < players.getFitness())
            {
                nextBest = players;
            }
        }
        
        // NEW: Clear all obstacles when generation ends
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        foreach (GameObject obstacle in obstacles)
        {
            Destroy(obstacle);
        }
        
        // FIXED: Remove from end to beginning to avoid skipping elements
        for (int i = pop.Count - 1; i >= 0; i--)
        {
            Destroy(pop[i].GetObject());
            pop.RemoveAt(i);
        }
        
        // Create new players with same DNA as top two
        pop.Add(new player(Instantiate(playerPrefab, new Vector3(Random.Range(-5f, 5f), -3f, 0), Quaternion.identity)));
        pop[0].setDNA(bestPlayer.playerDNA());
        champions.Add(bestPlayer.playerDNA());
        pop[0].GetObject().GetComponent<SpriteRenderer>().color = Color.green;
        
        genText.text = "Gen: " + gen;
        if (bestPlayer.getFitness() > highScore)
        {
            highScore = bestPlayer.getFitness();
            scoreText.text = "High Score: " + highScore;
        }
        Debug.Log("Fitness " + bestPlayer.getFitness());
        Debug.Log("Gen " + gen);
        gen++;

        pop.Add(new player(Instantiate(playerPrefab, new Vector3(Random.Range(-5f, 5f), -3f, 0), Quaternion.identity)));
        pop[1].setDNA(nextBest.playerDNA());
        pop[1].GetObject().GetComponent<SpriteRenderer>().color = Color.yellow;
        CreateText(bestPlayer.getFitness() + "\n");
    }
}