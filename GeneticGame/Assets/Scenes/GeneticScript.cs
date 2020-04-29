

using MathNet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;
using System;

public class GeneticScript : MonoBehaviour
{
    /*
        float leftDist = 0f;
        float middleDist = 0f;

        float rightDist = 0f;
        float[,] temp = new float[,] { { 1f, 1f, 1f } };

        NeuralNetwork NN;

        // Start is called before the first frame update
        void Start()
        {
            NN = new NeuralNetwork(temp);
            List<float>DNA = NN.getDNA();
            Debug.Log("DNA "+DNA.Count);
            NN.setDNA(DNA);
            Debug.Log("NewDNA");
            DNA = NN.getDNA();
            Debug.Log("DNA "+DNA.Count);
            //Debug.Log(NN.feedforward());
            //temp = new float[,] { { 0f, 0f, 1f } };
            // NN.setInput(temp);
            // Debug.Log(NN.feedforward());

            //Debug.Log("hey");
        }

        // Update is called once per frame
        void Update()
        {

            RaycastHit hit;
            Ray leftRay = new Ray(transform.position, new Vector3(-0.707f, 1f, 0));
            Ray forwardRay = new Ray(transform.position, new Vector3(0f, 1f, 0));
            Ray rightRay = new Ray(transform.position, new Vector3(0.707f, 1f, 0));
            // Cast a ray straight downwards.
            // Debug.DrawRay(forwardRay);
            Debug.DrawRay(transform.position, new Vector3(transform.position.x, transform.position.y + 1f, 0), Color.green);
            Debug.DrawRay(transform.position, new Vector3(transform.position.x - 0.707f, transform.position.y + 1f, 0), Color.green);
            Debug.DrawRay(transform.position, new Vector3(transform.position.x + 0.707f, transform.position.y + 1f, 0), Color.green);
            if (Physics.Raycast(leftRay, out hit, 10))
            {
                // Debug.Log("Left " + hit.distance);
                leftDist = hit.distance;
            }
            if (Physics.Raycast(forwardRay, out hit, 10))
            {
                //Debug.Log("Middle " + hit.distance);
                middleDist = hit.distance;
            }
            if (Physics.Raycast(rightRay, out hit, 10))
            {
                //Debug.Log("Right " + hit.distance);
                rightDist = hit.distance;
            }
            temp = new float[,] { { leftDist, middleDist, rightDist } };
            NN.setInput(temp);
            Matrix<float> returnMatrix = NN.feedforward();
            Debug.Log(returnMatrix[0, 0]);
            if (returnMatrix[0, 0] > 0.5)
            {
                Debug.Log(1);
            }
            else
            {
                Debug.Log(0);
            }



            //Debug.Log("test");
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

        public void setDNA(List<float> newDNA){
            int startingPoint=0;
            int tempStart=0;
            for (int i = 0; i < weights1.RowCount; i++)
            {
                for (int j = 0; j < weights1.ColumnCount; j++)
                {
                    weights1[i, j]=newDNA[i+j+tempStart];
                    startingPoint++;
                }
            }
            tempStart=startingPoint;
            for (int i = 0; i < biases1.RowCount; i++)
            {
                for (int j = 0; j < biases1.ColumnCount; j++)
                {
                    biases1[i,j]=newDNA[i+j+tempStart];
                    startingPoint++;
                }
            }

            tempStart=startingPoint;
            for (int i = 0; i < weights2.RowCount; i++)
            {
                for (int j = 0; j < weights2.ColumnCount; j++)
                {
                    weights2[i,j]=newDNA[i+j+tempStart];
                    startingPoint++;
                }
            }

            tempStart=startingPoint;
            for (int i = 0; i < biases2.RowCount; i++)
            {
                for (int j = 0; j < biases2.ColumnCount; j++)
                {
                    biases2[i,j]=newDNA[i+j+tempStart];
                    startingPoint++;
                }
            }
        }


    */
}

