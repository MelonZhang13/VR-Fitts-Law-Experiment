using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Order : MonoBehaviour
{

    private float[] A;
    private float[] W;
    private int ParticipantID;
    public SceneGeneration sceneGeneration;
    public TrialCondition[] currentTrialCondition;

    void Start()
    {
        ParticipantID = ParticipantData.ParticipantID;
        A = sceneGeneration.A;
        W = sceneGeneration.W;
        currentTrialCondition = GetParticipantOrder();
    }

    // Internal structure to define a single trial condition (A x W combination)
    public struct TrialCondition
    {
        public int A_Index;
        public int W_Index;
        public float A_Value; // Amplitude
        public float W_Value; // Width

        public override string ToString()
        {
            // Improved string representation for debugging
            return $"[Idx:({A_Index},{W_Index}) Val:({A_Value:F2},{W_Value:F2})]";
        }
    }

    /// <summary>
    /// Generates a complete list of all A x W combinations (the 'T' treatments).
    /// </summary>
    private List<TrialCondition> GetAllCombinations()
    {
        List<TrialCondition> allCombinations = new List<TrialCondition>();

        // Iterate through all A and W levels to form the Cartesian product (A x W)
        for (int i = 0; i < A.Length; i++)
        {
            for (int j = 0; j < W.Length; j++)
            {
                allCombinations.Add(new TrialCondition {
                    A_Index = i,
                    W_Index = j,
                    A_Value = A[i],
                    W_Value = W[j] });
            }
        }
        return allCombinations;
    }
    
    /// <summary>
    /// Retrieves the balanced experimental sequence for the given participant ID.
    /// Uses a Universal Complete Latin Square (2T sequences: T base + T reverse) design 
    /// to counterbalance the order of A x W combinations, ensuring first-order carry-over balancing 
    /// for ALL T values (odd or even).
    /// </summary>
    public TrialCondition[] GetParticipantOrder()
    {
        List<TrialCondition> baseCombinations = GetAllCombinations();
        int T = baseCombinations.Count; // Total number of treatments (e.g., 3 A * 3 W = 9)
        int TotalSequences = 2 * T;     // Total sequences for Complete Latin Square (e.g., 2 * 9 = 18)

        if (T <= 0)
        {
            Debug.LogError("The number of A x W combinations <= zero.");
            return new TrialCondition[0];
        }

        // 1. Determine the participant's sequence group (Index ranges from 0 to T - 1)
        // This ensures participants loop through the T available sequences.
        int sequenceGroupIndex = (ParticipantID - 1) % TotalSequences;

        // 2. Determine the base Cyclic Shift index (0 to T - 1)
        // This decides which of the T base Latin Square sequences is used.
        int baseShiftIndex = sequenceGroupIndex % T;
        
        // 3. Determine if the sequence should be reversed
        // The first T sequences are normal, the next T sequences are reversed.
        bool isReversed = sequenceGroupIndex >= T;

        // Start generating the T-length sequence based on the Cyclic Shift
        TrialCondition[] balancedOrder = new TrialCondition[T];

        for (int i = 0; i < T; i++)
        {
            // The standard Cyclic Shift logic (Standard Latin Square generation)
            // The shifted index determines which combination goes into the current position 'i'
            int shiftedIndex = (baseShiftIndex + i) % T;
            balancedOrder[i] = baseCombinations[shiftedIndex];
        }

        // 4. Apply the reversal if required (to complete the 2T balancing)
        if (isReversed)
        {
            // Use System.Array.Reverse to reverse the elements in place.
            // This reversal ensures first-order carry-over balance for ALL T values.
            System.Array.Reverse(balancedOrder);
        }

        return balancedOrder;
    }

    // =========================================================
    // Example and Debugging Code
    // =========================================================

    // public void Update()
    // {
    //     // Output the sequence when the space key is pressed (for testing)
    //     if (Input.GetKeyDown(KeyCode.A))
    //     {
    //         PrintOrder(1);
    //         PrintOrder(2);
    //         PrintOrder(3);
    //         PrintOrder(4);
    //         PrintOrder(5);
    //         PrintOrder(6);
    //         PrintOrder(7);
    //         PrintOrder(8);
    //     }
    // }
    
    // private void PrintOrder(int id)
    // {
    //     TrialCondition[] order = GetParticipantOrder(id);
    //     if (order == null || order.Length == 0) return;

    //     // Calculate the sequence group for printing clarity
    //     int sequenceGroup = ((id - 1) % order.Length) + 1;

    //     string result = $"Participant ID {id} (Sequence Group {sequenceGroup}) Order:\n";
        
    //     for (int i = 0; i < order.Length; i++)
    //     {
    //         result += order[i].ToString();
            
    //         // Add arrow unless it's the last element
    //         if (i < order.Length - 1)
    //         {
    //             result += " -> ";
    //         }
    //     }
    //     Debug.Log(result);
    // }



    // public int order_x;
    // public int[,] order;
    // public int[,] new_order;
    // public int[,] Latin;

    // // Start is called before the first frame update
    // void Start()
    // {
    //     order = new int[9, 2] { { 0, 0 }, { 0, 1 }, { 0, 2 }, { 1, 0 }, { 1, 1 }, { 1, 2 }, { 2, 0 }, { 2, 1 }, { 2, 2 } };
    //     //                         1         2         3         4         5         6         7         8         9
    //     Debug.Log(order_x);
    //     Latin = new int[9, 9];
    //     LatinGeneration(9);
    //     int n = order_x % 9 - 1;
    //     new_order = new int[9, 2];
    //     for (int i = 0; i < 9; i++) 
    //     {
    //         new_order[i, 0] = order[Latin[n, i] - 1, 0];
    //         new_order[i, 1] = order[Latin[n, i] - 1, 1];
    //     }
    // }

    // /// <summary>
    // /// ��������������
    // /// </summary>
    // public void LatinGeneration(int N)
    // {
    //     for (int i = 0; i < N; i++)
    //     {
    //         if (i == 0)
    //         {
    //             for (int j = 0; j < N; j++)
    //             {
    //                 if (j == 0) 
    //                 {
    //                     Latin[i, j] = 1;
    //                 }
    //                 if (j > 0 && j % 2 == 1)
    //                 {
    //                     Latin[i, j] = j / 2 + 2;
    //                 }
    //                 if (j > 0 && j % 2 == 0)
    //                 {
    //                     Latin[i, j] = N - j / 2 + 1;
    //                 }
    //             }
    //         }
    //         if (i > 0) 
    //         {
    //             for (int j = 0; j < N; j++) 
    //             {
    //                 Latin[i, j] = Latin[i - 1, j] + 1;
    //                 if (Latin[i, j] > N) Latin[i, j] = 1;
    //             }
    //         }
    //     }
    //     int[,] Latin_copy = new int[N, N];
    //     for (int i = 0; i < N; i++)
    //     {
    //         for (int j = 0; j < N; j++)
    //         {
    //             Latin_copy[i, j] = Latin[i,N - j - 1];
    //         }
    //     }
    //     Latin = Latin_copy;
    // }
}
