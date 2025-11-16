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
    float sideLeftDist=0f;

    float leftDist = 0f;
    float sideRightDist=0f;
    float middleDist = 0f;
    bool isDead = false;
    int deadCount = 0;
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
        startTime=Time.time;

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
    
    public void PlayerUpdate()
    {
        if (!isDead && thisPlayer != null)
        {
            fitness++; // Increment fitness every frame survived
            
            RaycastHit hit;
            Ray leftRay = new Ray(thisPlayer.transform.position, new Vector3(-0.707f, 1f, 0));
            Ray forwardRay = new Ray(thisPlayer.transform.position, new Vector3(0f, 1f, 0));
            Ray rightRay = new Ray(thisPlayer.transform.position, new Vector3(0.707f, 1f, 0));
            Ray sideLeftRay = new Ray(thisPlayer.transform.position, new Vector3(-1f, 0f, 0));
            Ray sideRightRay = new Ray(thisPlayer.transform.position, new Vector3(1f, 0f, 0));
            
            // Get all distances (normalized 0-1, where 1 = max distance)
            if (Physics.Raycast(sideLeftRay, out hit, 10))
            {
                sideLeftDist = hit.distance/10f;
                Debug.DrawLine(thisPlayer.transform.position, hit.point, Color.red);
            }
            else{
                sideLeftDist = 1f; // No obstacle = max distance
            }
            
            if (Physics.Raycast(sideRightRay, out hit, 10))
            {
                sideRightDist = hit.distance/10f;
                Debug.DrawLine(thisPlayer.transform.position, hit.point, Color.red);
            }
            else{
                sideRightDist = 1f;
            }
            
            if (Physics.Raycast(leftRay, out hit, 10))
            {
                leftDist = hit.distance/10f;
                Debug.DrawLine(thisPlayer.transform.position, hit.point, Color.red);
            }
            else{
                leftDist = 1f;
            }

            if (Physics.Raycast(forwardRay, out hit, 10))
            {
                middleDist = hit.distance/10f;
                Debug.DrawLine(thisPlayer.transform.position, hit.point, Color.red);
            }
            else{
                middleDist = 1f;
            }

            if (Physics.Raycast(rightRay, out hit, 10))
            {
                rightDist = hit.distance/10f;
                Debug.DrawLine(thisPlayer.transform.position, hit.point, Color.red);
            }
            else{
                rightDist = 1f;
            }
            
            // Feed inputs to neural network (all values 0-1)
            temp = new float[,] { { sideLeftDist, leftDist, middleDist, rightDist, sideRightDist } };
            NN.setInput(temp);
            Matrix<float> returnMatrix = NN.feedforward();

            // Move based on neural network output
            if (returnMatrix[0, 0] > 0.5)
            {
                thisPlayer.transform.position = new Vector3(thisPlayer.transform.position.x + 0.05f, thisPlayer.transform.position.y, 0);
            }
            else
            {
                thisPlayer.transform.position = new Vector3(thisPlayer.transform.position.x - 0.05f, thisPlayer.transform.position.y, 0);
            }
            
            // Check out of bounds death
            if (thisPlayer.transform.position.x < -11 || thisPlayer.transform.position.x > 11)
            {
                isDead = true;
                Destroy(thisPlayer);
                return;
            }
            
            // Check collision death - if ANY obstacle is very close
            if (leftDist < 0.12f || rightDist < 0.12f || middleDist < 0.12f || 
                sideLeftDist < 0.12f || sideRightDist < 0.12f)
            {
                isDead = true;
                Destroy(thisPlayer);
            }
        }
    }
}

public class NeuralNetwork
{
    Matrix<float> input; 
    Matrix<float> weights1;
    Matrix<float> biases1;
    Matrix<float> weights2;
    Matrix<float> biases2;
    Matrix<float> layer1;
    Matrix<float> layer2;

    public NeuralNetwork(float[,] newInput)
    {
        this.input = Matrix<float>.Build.DenseOfArray(newInput);
        
        // Initialize with smaller random weights (-0.5 to 0.5)
        weights1 = Matrix<float>.Build.Random(5, 3) - Matrix<float>.Build.Dense(5, 3, 0.5);
        biases1 = Matrix<float>.Build.Random(1, 3) - Matrix<float>.Build.Dense(1, 3, 0.5);
        weights2 = Matrix<float>.Build.Random(3, 1) - Matrix<float>.Build.Dense(3, 1, 0.5);
        biases2 = Matrix<float>.Build.Random(1, 1) - Matrix<float>.Build.Dense(1, 1, 0.5);
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
        
        // Set weights1 (5x3 = 15 values)
        for (int i = 0; i < weights1.RowCount; i++)
        {
            for (int j = 0; j < weights1.ColumnCount; j++)
            {
                weights1[i, j] = newDNA[startingPoint];
                startingPoint++;
            }
        }
        
        // Set biases1 (1x3 = 3 values)
        for (int i = 0; i < biases1.RowCount; i++)
        {
            for (int j = 0; j < biases1.ColumnCount; j++)
            {
                biases1[i, j] = newDNA[startingPoint];
                startingPoint++;
            }
        }
        
        // Set weights2 (3x1 = 3 values)
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