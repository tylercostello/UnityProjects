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
    float[] sensorDistances = new float[20];
    bool isDead = false;
    float[,] temp = new float[,] { { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f } };
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

    /// <summary>
    /// Updated to use 20 sensing rays distributed in a 180-degree arc in front of the player.
    /// Neural network input layer expanded from 5 to 20 inputs.
    /// </summary>
    public void PlayerUpdate()
    {
        if (!isDead && thisPlayer != null)
        {
            // --- 1. Sensor Initialization ---
            float maxDistNormalized = 2f;
            float maxRayDistance = 20f;
            float deathThreshold = 0.05f;

            // Initialize all 20 sensors to max distance
            for (int i = 0; i < 20; i++)
            {
                sensorDistances[i] = maxDistNormalized;
            }

            RaycastHit hit;

            // --- 2. Raycasting (20 rays distributed across 180 degrees) ---
            // Rays spread from -90 degrees (left) to +90 degrees (right)
            // All rays point upward (y=1) with varying x components
            for (int i = 0; i < 20; i++)
            {
                // Calculate angle: from -90 to +90 degrees
                float angle = -90f + (i * 180f / 19f); // 19 gaps between 20 rays
                float angleRad = angle * Mathf.Deg2Rad;
                
                // Convert to direction vector (upward arc)
                Vector3 direction = new Vector3(Mathf.Sin(angleRad), Mathf.Cos(angleRad), 0f).normalized;
                
                Ray ray = new Ray(thisPlayer.transform.position, direction);
                
                if (Physics.Raycast(ray, out hit, maxRayDistance))
                {
                    sensorDistances[i] = hit.distance / 10f;
                    Debug.DrawLine(thisPlayer.transform.position, hit.point, Color.red);
                }
                else
                {
                    Debug.DrawRay(thisPlayer.transform.position, direction * maxRayDistance, Color.green);
                }
            }

            // --- 3. Neural Network Input ---
            // Build input array with all 20 sensor readings
            temp = new float[1, 20];
            for (int i = 0; i < 20; i++)
            {
                temp[0, i] = sensorDistances[i];
            }
            
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

            // --- 5. Death Checks ---
            bool isOffScreen = thisPlayer.transform.position.x < -11 || thisPlayer.transform.position.x > 11;

            // Check if any ray is too close
            bool isHit = false;
            for (int i = 0; i < 20; i++)
            {
                if (sensorDistances[i] < deathThreshold)
                {
                    isHit = true;
                    break;
                }
            }

            if (isOffScreen || isHit)
            {
                isDead = true;
                fitness = (int)((10) * (Time.time - startTime));
                Destroy(thisPlayer);
            }
        }
    }
}

public class NeuralNetwork
{
    Matrix<float> input; // Now 20 inputs
    Matrix<float> weights1 = Matrix<float>.Build.Random(20, 10); // Changed from 5x10 to 20x10
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

        // Set weights1 (20x10 = 200 values) - Changed from 5x10
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