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
    List<player> pop = new List<player>();
    List<List<float>> champions = new List<List<float>>();
    public float lastTime = 0f;
    public float obstacleTime = 0f;
    public int populationSize = 1000;

    // Variables for increasing difficulty
    private float generationStartTime = 0f;
    private float baseSpawnInterval = 0.4f;
    private float minSpawnInterval = 0.15f; // Minimum time between spawns
    private float difficultyIncreaseRate = 0.00f; // How much faster spawns get per second

    void Start()
    {
        Time.timeScale = 1f;
        CreateText("New Game \n");
        generationStartTime = Time.time;

        for (int i = 0; i < populationSize; i++)
        {
            pop.Add(new player(Instantiate(playerPrefab, new Vector3(Random.Range(-5f, 5f), -3f, 0), Quaternion.identity)));
        }

    }

    void CreateText(string content)
    {
        string path = Application.dataPath + "/Log.txt";
        if (!File.Exists(path))
        {
            File.WriteAllText(path, "");
        }
        File.AppendAllText(path, content);

    }

    public static int getGen()
    {
        return gen;
    }

    // Calculate current spawn interval based on time elapsed
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
            // Use dynamic spawn interval
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
            // --- MODIFIED BREEDING LOGIC ---

            // 1. Check fitnesses and get a copy of the old population
            List<player> oldPop = this.checkFitnesses();

            // 2. Reset timer for new generation
            generationStartTime = Time.time;

            // 3. Breed 98 new players using Tournament Selection
            //    (pop[0] and pop[1] already exist as the elites)
            for (int i = 2; i < populationSize; i++)
            {
                // Select two parents from the *previous* generation
                player parent1 = SelectParent(oldPop);
                player parent2 = SelectParent(oldPop);

                pop.Add(new player(Instantiate(playerPrefab, new Vector3(Random.Range(-5f, 5f), -3f, 0), Quaternion.identity)));
                pop[i].setDNA(breed(parent1, parent2));
            }

            // 4. Add back champions from previous generations
            for (int i = populationSize; i < populationSize + champions.Count; i++)
            {
                pop.Add(new player(Instantiate(playerPrefab, new Vector3(Random.Range(-5f, 5f), -3f, 0), Quaternion.identity)));
                pop[i].setDNA(champions[i - populationSize]);
                pop[i].GetObject().GetComponent<SpriteRenderer>().color = Color.cyan;
            }
        }
    }

    /// <summary>
    /// Selects a parent using tournament selection.
    /// Randomly picks N players from the old population and returns the best one.
    /// </summary>
    player SelectParent(List<player> oldPopulation)
    {
        int tournamentSize = 3; // You can tune this value
        player bestInTournament = null;

        for (int i = 0; i < tournamentSize; i++)
        {
            // Pick a random player from the *previous* generation
            player randomPlayer = oldPopulation[Random.Range(0, oldPopulation.Count)];

            if (bestInTournament == null || randomPlayer.getFitness() > bestInTournament.getFitness())
            {
                bestInTournament = randomPlayer;
            }
        }
        return bestInTournament;
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

            // Smaller, more reasonable mutations
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
                return false;
            }

        }
        return true;
    }

    /// <summary>
    /// Finds best players, clears the board, and returns a copy of the
    /// just-deceased population for breeding purposes.
    /// </summary>
    public List<player> checkFitnesses()
    {
        // 1. Find top two players
        player bestPlayer = pop[0];
        player nextBest = pop[1];
        foreach (player players in pop)
        {
            if (bestPlayer.getFitness() < players.getFitness())
            {
                nextBest = bestPlayer;
                bestPlayer = players;
            }
            else if (nextBest.getFitness() < players.getFitness() && players != bestPlayer)
            {
                nextBest = players;
            }
        }

        // --- MODIFICATION: Save a copy of the old population for breeding ---
        List<player> oldPopulation = new List<player>(pop);


        // 2. Clear all obstacles
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        foreach (GameObject obstacle in obstacles)
        {
            Destroy(obstacle);
        }

        // 3. Destroy all old player objects and clear the list
        for (int i = pop.Count - 1; i >= 0; i--)
        {
            Destroy(pop[i].GetObject());
            pop.RemoveAt(i);
        }

        // 4. Add the two elites back to the (now empty) pop list
        pop.Add(new player(Instantiate(playerPrefab, new Vector3(Random.Range(-5f, 5f), -3f, 0), Quaternion.identity)));
        pop[0].setDNA(bestPlayer.playerDNA());
        champions.Add(bestPlayer.playerDNA()); // Add best to champions list
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

        // --- MODIFICATION: Return the saved old population ---
        return oldPopulation;
    }
}