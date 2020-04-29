using MathNet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;
using System;
public class Player2 : MonoBehaviour
{
/*
    int fitness = 0;
    float leftDist = 0f;
    float middleDist = 0f;
    bool isDead = false;
    int deadCount = 0;
    float rightDist = 0f;
    float[,] temp = new float[,] { { 1f, 1f, 1f } };
    GameObject thisPlayer;
    NeuralNetwork NN;

    int startGen = 0;
    public player()
    {
        fitness = 0;
    }
    public player(GameObject thisPlayer)
    {
        this.thisPlayer = thisPlayer;

        NN = new NeuralNetwork(temp);
        startGen = Population2.getGen();

    }
    public GameObject GetObject()
    {
        return thisPlayer;
    }
    public bool itisDead()
    {
        return isDead;
    }
    public int getFitness()
    {
        return fitness;
    }

    public List<float> playerDNA()
    {
        List<float> tempDNA = NN.getDNA();
        return tempDNA;
    }

    public void setDNA(List<float> newDNA)
    {
        NN.setDNA(newDNA);
    }
    public void setDead(bool newState)
    {
        isDead = newState;
    }
    // public void Update(){
    //     Debug.Log("Updating");
    //     if (startGen != Population2.getGen()){
    //         Destroy(thisPlayer);
    //        // Destroy(this);
    //     }
    // }
    public void PlayerUpdate()
    {
        if (!isDead && thisPlayer != null)
        {
            fitness++;

            RaycastHit hit;
            Ray leftRay = new Ray(thisPlayer.transform.position, new Vector3(-0.707f, 1f, 0));
            Ray forwardRay = new Ray(thisPlayer.transform.position, new Vector3(0f, 1f, 0));
            Ray rightRay = new Ray(thisPlayer.transform.position, new Vector3(0.707f, 1f, 0));
            if (Physics.Raycast(leftRay, out hit, 10))
            {
                leftDist = hit.distance;
                Debug.DrawLine(thisPlayer.transform.position, hit.point, Color.red);
            }

            if (Physics.Raycast(forwardRay, out hit, 10))
            {
                middleDist = hit.distance;
                Debug.DrawLine(thisPlayer.transform.position, hit.point, Color.red);
            }

            if (Physics.Raycast(rightRay, out hit, 10))
            {
                rightDist = hit.distance;
                Debug.DrawLine(thisPlayer.transform.position, hit.point, Color.red);
            }

            temp = new float[,] { { leftDist, middleDist, rightDist } };
            NN.setInput(temp);
            Matrix<float> returnMatrix = NN.feedforward();

            if (returnMatrix[0, 0] > 0.5)
            {
                //if (thisPlayer.transform.position.x < 11)
                thisPlayer.transform.position = new Vector3(thisPlayer.transform.position.x + 0.05f, thisPlayer.transform.position.y, 0);
            }
            else
            {
                //if (thisPlayer.transform.position.x > -11)
                thisPlayer.transform.position = new Vector3(thisPlayer.transform.position.x - 0.05f, thisPlayer.transform.position.y, 0);
            }
            //fix the piece of crap function
            if ((leftDist < 0.1f && leftDist > 0.01) || (rightDist < 0.1f && rightDist > 0.01) || (middleDist < 0.1f && middleDist > 0.01))
            {
                isDead = true;
                //thisPlayer.GetComponent<Renderer>().material.color = Color.green;
                Destroy(thisPlayer);
            }
            if (thisPlayer.transform.position.x < -11 || thisPlayer.transform.position.x > 11)
            {
                isDead = true;
                //thisPlayer.GetComponent<Renderer>().material.color = Color.green;
                Destroy(thisPlayer);
            }
        }
    }
}
public class NeuralNetwork
{
    Matrix<float> input; //3
    Matrix<float> weights1 = Matrix<float>.Build.Random(3, 4);
    Matrix<float> biases1 = Matrix<float>.Build.Random(1, 4);
    Matrix<float> weights2 = Matrix<float>.Build.Random(4, 1);
    Matrix<float> biases2 = Matrix<float>.Build.Random(1, 1);
    Matrix<float> layer1;
    Matrix<float> layer2;


    public NeuralNetwork(float[,] newInput)
    {
        this.input = Matrix<float>.Build.DenseOfArray(newInput);
    }
    public void setInput(float[,] newInput)
    {
        this.input = Matrix<float>.Build.DenseOfArray(newInput);
    }
    public float relu(float t)
    {
        t = (System.Math.Abs(t) + t) / 2;
        return t;
    }
    public float sigmoid(float t)
    {
        t = (float)(1 / (1 + Math.Pow(Math.E, -t)));
        return t;
    }
    public Matrix<float> feedforward()
    {
        layer1 = (input * weights1 + biases1);
        for (int i = 0; i < layer1.RowCount; i++)
        {
            for (int j = 0; j < layer1.ColumnCount; j++)
            {
                layer1[i, j] = relu(layer1[i, j]);
            }
        }
        layer2 = (layer1 * weights2 + biases2);
        for (int i = 0; i < layer2.RowCount; i++)
        {
            for (int j = 0; j < layer2.ColumnCount; j++)
            {
                layer2[i, j] = sigmoid(layer2[i, j]);
            }
        }
        return layer2;


    }
    public List<float> getDNA()
    {
        List<float> DNA = new List<float>();
        for (int i = 0; i < weights1.RowCount; i++)
        {
            for (int j = 0; j < weights1.ColumnCount; j++)
            {
                DNA.Add(weights1[i, j]);
            }
        }

        for (int i = 0; i < biases1.RowCount; i++)
        {
            for (int j = 0; j < biases1.ColumnCount; j++)
            {
                DNA.Add(biases1[i, j]);
            }
        }


        for (int i = 0; i < weights2.RowCount; i++)
        {
            for (int j = 0; j < weights2.ColumnCount; j++)
            {
                DNA.Add(weights2[i, j]);
            }
        }


        for (int i = 0; i < biases2.RowCount; i++)
        {
            for (int j = 0; j < biases2.ColumnCount; j++)
            {
                DNA.Add(biases2[i, j]);
            }
        }
        return DNA;

    }

    public void setDNA(List<float> newDNA)
    {
        int startingPoint = 0;
        int tempStart = 0;
        for (int i = 0; i < weights1.RowCount; i++)
        {
            for (int j = 0; j < weights1.ColumnCount; j++)
            {
                weights1[i, j] = newDNA[i + j + tempStart];
                startingPoint++;
            }
        }
        tempStart = startingPoint;
        for (int i = 0; i < biases1.RowCount; i++)
        {
            for (int j = 0; j < biases1.ColumnCount; j++)
            {
                biases1[i, j] = newDNA[i + j + tempStart];
                startingPoint++;
            }
        }

        tempStart = startingPoint;
        for (int i = 0; i < weights2.RowCount; i++)
        {
            for (int j = 0; j < weights2.ColumnCount; j++)
            {
                weights2[i, j] = newDNA[i + j + tempStart];
                startingPoint++;
            }
        }

        tempStart = startingPoint;
        for (int i = 0; i < biases2.RowCount; i++)
        {
            for (int j = 0; j < biases2.ColumnCount; j++)
            {
                biases2[i, j] = newDNA[i + j + tempStart];
                startingPoint++;
            }
        }
    }


*/
}