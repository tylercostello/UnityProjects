using MathNet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;
using System;

public class player : MonoBehaviour
{

    float startTime;
    int fitness = 0;
    float sideLeftDist = 0f;
    float leftDist = 0f;
    float sideRightDist = 0f;
    float middleDist = 0f;
    bool isDead = false;
    float rightDist = 0f;
    float[,] temp = new float[,] { { 1f, 1f, 1f, 1f, 1f } };
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
        startTime = Time.time;

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

    /// <summary>
    /// This method has been corrected.
    /// 1. All raycast distances are now positive and normalized.
    /// 2. "No hit" rays result in a consistent max distance value.
    /// 3. Death checks are simplified and use the same clean data fed to the network.
    /// </summary>
    public void PlayerUpdate()
    {
        if (!isDead && thisPlayer != null)
        {
            // --- 1. Sensor Initialization ---
            // Use a consistent max distance value (20f ray / 10f normalization = 2f)
            float maxDistNormalized = 2f;
            float maxRayDistance = 20f;
            float deathThreshold = 0.05f; // Corresponds to 1 unit of distance (0.1 * 10)

            sideLeftDist = maxDistNormalized;
            leftDist = maxDistNormalized;
            middleDist = maxDistNormalized;
            rightDist = maxDistNormalized;
            sideRightDist = maxDistNormalized;

            RaycastHit hit;

            // --- 2. Raycasting (All distances are now positive) ---
            Ray leftRay = new Ray(thisPlayer.transform.position, new Vector3(-0.707f, 1f, 0));
            Ray forwardRay = new Ray(thisPlayer.transform.position, new Vector3(0f, 1f, 0));
            Ray rightRay = new Ray(thisPlayer.transform.position, new Vector3(0.707f, 1f, 0));
            Ray sideLeftRay = new Ray(thisPlayer.transform.position, new Vector3(-1f, 0f, 0));
            Ray sideRightRay = new Ray(thisPlayer.transform.position, new Vector3(1f, 0f, 0));

            if (Physics.Raycast(sideLeftRay, out hit, maxRayDistance))
            {
                sideLeftDist = hit.distance / 10f; // Normalized positive distance
                Debug.DrawLine(thisPlayer.transform.position, hit.point, Color.red);
            }
            if (Physics.Raycast(sideRightRay, out hit, maxRayDistance))
            {
                sideRightDist = hit.distance / 10f;
                Debug.DrawLine(thisPlayer.transform.position, hit.point, Color.red);
            }
            if (Physics.Raycast(leftRay, out hit, maxRayDistance))
            {
                leftDist = hit.distance / 10f;
                Debug.DrawLine(thisPlayer.transform.position, hit.point, Color.red);
            }
            if (Physics.Raycast(forwardRay, out hit, maxRayDistance))
            {
                middleDist = hit.distance / 10f;
                Debug.DrawLine(thisPlayer.transform.position, hit.point, Color.red);
            }
            if (Physics.Raycast(rightRay, out hit, maxRayDistance))
            {
                rightDist = hit.distance / 10f;
                Debug.DrawLine(thisPlayer.transform.position, hit.point, Color.red);
            }

            // --- 3. Neural Network Input ---
            // The network now receives clean, consistent, positive inputs
            temp = new float[,] { { sideLeftDist, leftDist, middleDist, sideRightDist, rightDist } };
            NN.setInput(temp);
            Matrix<float> returnMatrix = NN.feedforward();

            // --- 4. Movement ---
            if (returnMatrix[0, 0] > 0.5)
            {
                // Move Right
                thisPlayer.transform.position = new Vector3(thisPlayer.transform.position.x + 0.1f, thisPlayer.transform.position.y, 0);
            }
            else
            {
                // Move Left
                thisPlayer.transform.position = new Vector3(thisPlayer.transform.position.x - 0.1f, thisPlayer.transform.position.y, 0);
            }

            // --- 5. Death Checks (Simplified and Corrected) ---
            
            // Check for off-screen death
            bool isOffScreen = thisPlayer.transform.position.x < -11 || thisPlayer.transform.position.x > 11;

            // Check if any ray is too close
            // This logic is simple: if any sensor reads a value less than the threshold, it's a hit.
            bool isHit = (leftDist < deathThreshold) || 
                         (rightDist < deathThreshold) || 
                         (middleDist < deathThreshold) || 
                         (sideLeftDist < deathThreshold) || 
                         (sideRightDist < deathThreshold);

            if (isOffScreen || isHit)
            {
                isDead = true;
                fitness = (int)((10) * (Time.time - startTime)); // Calculate fitness based on survival time
                Destroy(thisPlayer);
            }
        }
    }
}

// This Neural Network class was already well-implemented and is included as-is.
public class NeuralNetwork
{
    Matrix<float> input; //5 inputs
    Matrix<float> weights1 = Matrix<float>.Build.Random(5, 10);
    Matrix<float> biases1 = Matrix<float>.Build.Random(1, 10);
    Matrix<float> weights2 = Matrix<float>.Build.Random(10, 1);
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

        // Set weights1 (5x10 = 50 values)
        for (int i = 0; i < weights1.RowCount; i++)
        {
            for (int j = 0; j < weights1.ColumnCount; j++)
            {
                weights1[i, j] = newDNA[startingPoint];
                startingPoint++;
            }
        }

        // Set biases1 (1x10 = 10 values)
        for (int i = 0; i < biases1.RowCount; i++)
        {
            for (int j = 0; j < biases1.ColumnCount; j++)
            {
                biases1[i, j] = newDNA[startingPoint];
                startingPoint++;
            }
        }

        // Set weights2 (10x1 = 10 values)
        for (int i = 0; i < weights2.RowCount; i++)
        {
            for (int j = 0; j < weights2.ColumnCount; j++)
            {
                weights2[i, j] = newDNA[startingPoint];
                startingPoint++;
            }
        }

        // Set biases2 (1x1 = 1 value)
        for (int i = 0; i < biases2.RowCount; i++)
        {
            for (int j = 0; j < biases2.ColumnCount; j++)
            {
                biases2[i, j] = newDNA[startingPoint];
                startingPoint++;
            }
        }
    }
}