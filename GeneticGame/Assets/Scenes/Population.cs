using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Population : MonoBehaviour
{

    public GameObject playerPrefab;
    public GameObject obstaclePrefab;
    public static bool gameOver = false;
    List<player> pop = new List<player>();
    player topPlayer;
    player secondPlayer;
    public float lastTime = 0f;
    public float obstacleTime = 0f;
    void Start()
    {

        for (int i = 0; i < 20; i++)
        {
            pop.Add(new player(Instantiate(playerPrefab, new Vector3(0, -3f, 0), Quaternion.identity)));
        }

    }
    void Update()
    {

        if (!allDead())
        {


            if (Time.time - obstacleTime > 0.2)
            {

                Instantiate(obstaclePrefab, new Vector3(Random.Range(-11.0f, 11.0f), 3.85f, 0), Quaternion.identity);
                obstacleTime = Time.time;
            }

            if (Time.time - lastTime > 0.01)
            {
                int updateCounter = 0;
                foreach (player players in pop)
                {

                    players.PlayerUpdate();

                    updateCounter++;
                }
                lastTime = Time.time;
            }
        }
        else
        {
            this.checkFitnesses();
            gameOver = true;
            for (int i = 2; i < 20; i++)
            {
                pop.Add(new player(Instantiate(playerPrefab, new Vector3(Random.Range(-10f, 10f), -3f, 0), Quaternion.identity)));
                pop[i].setDNA(breed(pop[0], pop[1]));
            }
            gameOver = false;

        }
    }

    List<float> breed(player p1, player p2)
    {
        int randInt = 0;
        List<float> babyList = new List<float>();

        List<float> p1List = p1.playerDNA();
        List<float> p2List = p2.playerDNA();
        for (int i = 0; i < p1List.Count; i++)
        {
            randInt = Random.Range(0, 2);
            if (randInt == 0)
            {
                babyList.Add(p1List[i]);
            }
            else
            {
                babyList.Add(p2List[i]);
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
            else
            {
                if (pop.Count > 2)
                {
                    Destroy(pop[i].GetObject());
                    pop.RemoveAt(i);
                }
            }
        }
        return true;
    }

    public void checkFitnesses()
    {
        player bestPlayer = pop[0];

        foreach (player players in pop)
        {
            if (bestPlayer.getFitness() < players.getFitness())
            {
                bestPlayer = players;
            }
        }
        player nextBest = pop[0];
        foreach (player players in pop)
        {
            if (nextBest.getFitness() < players.getFitness() && players != bestPlayer)
            {
                nextBest = players;
            }
        }
        pop[0] = bestPlayer;
        Debug.Log("Fitness " + bestPlayer.getFitness());

        pop[0].setDead(false);
        pop[1] = nextBest;
        pop[1].setDead(false);
    }


}

