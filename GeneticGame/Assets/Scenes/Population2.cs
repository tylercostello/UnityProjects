using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Population2 : MonoBehaviour
{
    static int gen = 0;
    public GameObject playerPrefab;
    public GameObject obstaclePrefab;
    //public static bool gameOver = false;
    List<player> pop = new List<player>();
    List<List<float>> champions = new List<List<float>>();
    player topPlayer;
    player secondPlayer;
    public float lastTime = 0f;
    public float obstacleTime = 0f;
    void Start()
    {

        CreateText("New Game \n");
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
    void Update()
    {

        if (!allDead())
        {


            if (Time.time - obstacleTime > 0.4)
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
            //they are all destroyed except for best 2 now
            for (int i = 2; i < 100; i++)
            {
                pop.Add(new player(Instantiate(playerPrefab, new Vector3(Random.Range(-5f, 5f), -3f, 0), Quaternion.identity)));
                pop[i].setDNA(breed(pop[0], pop[1]));
            }
            for (int i = 100; i<100+champions.Count;i++){
                pop.Add(new player(Instantiate(playerPrefab, new Vector3(Random.Range(-5f, 5f), -3f, 0), Quaternion.identity)));
                pop[i].setDNA(champions[i-100]);
                pop[i].GetObject().GetComponent<Renderer>().material.color = Color.green;
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
            if (mutateInt == 1)
            {
                babyList[i] = babyList[i] * -2f;
            }
            else if (mutateInt == 2)
            {
                babyList[i] = babyList[i] * 2f;
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
        for (int i = 0; i < pop.Count; i++)
        {
            Destroy(pop[i].GetObject());
            pop.RemoveAt(i);
        }
        //creates new players with same DNA as top two
        //retains genetic material and makes breeding easier
        //Debug.Log(pop.Count);
        while (pop.Count > 0)
        {
            //If somebody reads this don't ask about this
            //Its not my fault this is necessary don't blame me this is Unity's fault
            //I just did what I had to
            for (int i = 0; i < pop.Count; i++)
            {

                pop.RemoveAt(i);
            }
        }
        //Debug.Log(pop.Count);
        pop.Add(new player(Instantiate(playerPrefab, new Vector3(Random.Range(-5f, 5f), -3f, 0), Quaternion.identity)));
        pop[0].setDNA(bestPlayer.playerDNA());
        champions.Add(bestPlayer.playerDNA());

        pop[0].GetObject().GetComponent<Renderer>().material.color = Color.green;
        Debug.Log("Fitness " + bestPlayer.getFitness());
        Debug.Log("Gen " + gen);
        gen++;

        pop.Add(new player(Instantiate(playerPrefab, new Vector3(Random.Range(-5f, 5f), -3f, 0), Quaternion.identity)));
        pop[1].setDNA(nextBest.playerDNA());
        pop[1].GetObject().GetComponent<Renderer>().material.color = Color.green;
        CreateText(bestPlayer.getFitness()+"\n");
        //Debug.Log("Count "+pop.Count);
        //Debug.Log("Fitness2 " + nextBest.getFitness());


        /*
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
                */
    }


}

