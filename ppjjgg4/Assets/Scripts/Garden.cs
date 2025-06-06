using System;
using System.Collections.Generic;
using System.Linq;
using com.cyborgAssets.inspectorButtonPro;
using Unity.VisualScripting;
using UnityEngine;

public class Garden : MonoBehaviour
{
    public static Action<Plot, Plant> OnPlantEnter;
    public static Action<Plot, Plant> OnPlantMatured;
    public static Action<Plot, Plant> OnPlantExit;
    public static Garden Instance { get; private set; }
    [field : SerializeField] public int width { get; private set; } = 6;
    [field : SerializeField] public int height { get; private set; } = 4;

    private Plot[,] plots;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("Multiple instances of Garden detected. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        OnPlantEnter = null;
        OnPlantMatured = null;
        OnPlantExit = null;
        StoreChildrenToCellsArray();
    }

    public void SetGarden(Plot[,] plots)
    {
        this.plots = plots;
        this.width = plots.GetLength(1);
        this.height = plots.GetLength(0);
    }

    [ProButton]
    public void StoreChildrenToCellsArray()
    {
        plots = new Plot[height, width];

        int totalCells = height * width;
        int expectedChildren = Mathf.Min(transform.childCount, totalCells);

        for (int index = 0; index < expectedChildren; index++)
        {
            int i = index / width; // ligne
            int j = index % width; // colonne

            Transform child = transform.GetChild(index);
            Plot plot = child.gameObject.GetComponent<Plot>();
            plot.i = i;
            plot.j = j;
            this.plots[i, j] = plot;
        }
    }

    public Plot GetPlot(int i, int j)
    {
        if (i < 0 || i >= height || j < 0 || j >= width)
        {
            Debug.LogError($"Invalid plot coordinates: ({i}, {j})");
            return null;
        }
        return plots[i, j];
    }

    public void EndDay()
    {
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                GetPlot(i, j).EndDay();
            }
        }
    }


    public List<Plot> GetNeighbours(int i, int j)
    {
        List<Plot> n = new();
        int[] i_id = { -1, -1, -1, 0, 0, 1, 1, 1 };
        int[] j_id = { -1, 0, 1, -1, 1, -1, 0, 1 };
        for (int id = 0; id < i_id.Count(); id++)
        {
            int ni = i + i_id[id];
            int nj = j + j_id[id];
            if (ni >= 0 && ni <= height - 1 && nj >= 0 && nj <= width - 1)
            {
                n.Add(GetPlot(ni, nj));
            }
        }
        return n;
    }

    public List<Plot> GetSpecies(string species)
    {
        List<Plot> plots = new();
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                Plot plot = GetPlot(i, j);
                if (plot.plant != null && plot.plant.Species.Equals(species)) plots.Add(plot);
            }
        }
        return plots;
    }

    public void OnEnable()
    {
        GameManager.OnDayEnded += EndDay;
    }

    public void OnDisable()
    {
        GameManager.OnDayEnded -= EndDay;
    }

    public int GetScore()
    {
        int score = 0;
        foreach (Plot plot in plots)
        {
            score += plot.GetPlotScore();
        }
        return score;
    }
}
