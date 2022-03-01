using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SmokeScreen : MonoBehaviour
{
    const int VERTICES_PER_CELL = 4;
    const int TRIANGLE_INDICES_PER_CELL = 6;
    const int UVS_PER_CELL = 4;

    public static SmokeScreen instance;

    private List<SmokeCloud> smokeClouds = new List<SmokeCloud>();
    private List<SmokeCell> cells = new List<SmokeCell>();

    private MeshFilter meshFilter;

    private void Awake()
    {
        instance = this;

        meshFilter = transform.GetChild(0).GetComponent<MeshFilter>();
        transform.position = Vector3.zero;
    }

    private void RenderCell(SmokeCell targetCell, List<Vector3> vertices, List<int> triangles, List<Vector2> uvs)
    {
        int currVertexCount = vertices.Count;

        for (int i = 0; i < VERTICES_PER_CELL; i++)
        {
            vertices.Add(targetCell.linkedVertices[i]);
        }
        for (int i = 0; i < TRIANGLE_INDICES_PER_CELL; i++)
        {
            triangles.Add(currVertexCount + targetCell.linkedTriangles[i]);
        }
        for (int i = 0; i < UVS_PER_CELL; i++)
        {
            uvs.Add(targetCell.linkedUvs[i]);
        }
    }

    private void UpdateMeshWithGroups()
    {
        List<Vector3> currVertices = new List<Vector3>();
        List<int> currTriangles = new List<int>();
        List<Vector2> currUvs = new List<Vector2>();

        foreach (SmokeCell cell in cells)
        {
            if (cell.isVisible)
                RenderCell(cell, currVertices, currTriangles, currUvs);
        }

        Mesh mesh = new Mesh
        {
            vertices = currVertices.ToArray(),
            triangles = currTriangles.ToArray(),
            uv = currUvs.ToArray()
        };
        mesh.Optimize();
        meshFilter.mesh = mesh;
    }

    public SmokeCell GetSmokeCellAtPosition(Vector3 targetPoint)
    {
        foreach (SmokeCloud cloud in smokeClouds)
        {
            if (cloud.fillArea.Contains(targetPoint))
            {
                foreach (SmokeCell c in cloud.containedCells)
                {
                    if (c.fillArea.Contains(targetPoint))
                        return c;
                }
            }
        }

        return null;
    }

    #region Mesh Creation helpers
    private Vector3[] CellVertices(Vector2 minCorner, Vector2 maxCorner)
    {
        Vector2 size = maxCorner - minCorner;

        Vector3[] vertices = new Vector3[]
        {
            minCorner,
            minCorner + new Vector2(size.x, 0),
            minCorner + size,
            minCorner + new Vector2(0, size.y)
        };
        return vertices;
    }

    private int[] CellTriangles()
    {
        int[] triangles = new int[] 
        {
            0, 2, 1,
            0, 3, 2
        };

        return triangles;
    }
    private Vector2[] CellUvs()
    {
        Vector2[] uvs = new Vector2[UVS_PER_CELL] 
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(1, 1),
            new Vector2(0, 1)
        };
        return uvs;
    }

    private SmokeCell GetDefaultCell()
    {
        SmokeCell newCell = new SmokeCell
        {
            isVisible = true
        };
        return newCell;
    }

    private SmokeCell GetCellAtPosition(Vector2 position)
    {
        return GetCellWithDimensions(position, position + new Vector2(1, 1));
    }

    private SmokeCell GetCellWithDimensions(Vector2 minPoint, Vector2 maxPoint)
    {
        SmokeCell newCell = GetDefaultCell();

        newCell.linkedVertices = CellVertices(minPoint, maxPoint);
        newCell.linkedTriangles = CellTriangles();
        newCell.linkedUvs = CellUvs();

        newCell.fillArea = GetCellRect(newCell);

        return newCell;
    }

    private Rect GetCellRect(SmokeCell cell)
    {
        return Rect.MinMaxRect(cell.linkedVertices[0].x, cell.linkedVertices[0].y,
                               cell.linkedVertices[2].x, cell.linkedVertices[2].y);
    }
    #endregion

    #region Clearing Smoke
    public SmokeCloud AddSmokeCloud(Vector2 minPoint, Vector2 maxPoint)
    {
        List<SmokeCell> containedCells = new List<SmokeCell>();

        for (int y = (int)minPoint.y; y < (int)maxPoint.y; y++)
        {
            containedCells.Add(GetCellAtPosition(new Vector2((int)minPoint.x, y)));
            containedCells.Add(GetCellAtPosition(new Vector2((int)maxPoint.x - 1, y)));
        }

        for (int x = (int)minPoint.x + 1; x < (int)maxPoint.x - 1; x++)
        {
            containedCells.Add(GetCellAtPosition(new Vector2(x, (int)minPoint.y)));
            containedCells.Add(GetCellAtPosition(new Vector2(x, (int)maxPoint.y - 1)));
        }

        // Fill center of room
        containedCells.Add(GetCellWithDimensions(minPoint + new Vector2(1, 1),
                                                 maxPoint - new Vector2(1, 1)));

        for (int i = 0; i < containedCells.Count; i++)
            cells.Add(containedCells[i]);

        SmokeCloud newCloud = new SmokeCloud
        {
            containedCells = containedCells.ToArray(),

            fillArea = Rect.MinMaxRect(minPoint.x, minPoint.y,
                                            maxPoint.x, maxPoint.y)
        };

        MergeSmokeCloudIntoExisting(newCloud);

        smokeClouds.Add(newCloud);
        UpdateMeshWithGroups();
        return newCloud;
    }

    private void MergeSmokeCloudIntoExisting(SmokeCloud targetCloud)
    {
        foreach (SmokeCloud cloud in smokeClouds)
        {
            if (!cloud.fillArea.Overlaps(targetCloud.fillArea))
                continue;

            for (int i = 0; i < cloud.containedCells.Length; i++)
            {
                for (int j = 0; j < targetCloud.containedCells.Length; j++)
                {
                    if (cloud.containedCells[i].fillArea.Overlaps(targetCloud.containedCells[j].fillArea))
                    {
                        if (cloud.containedCells[i].Equals(targetCloud.containedCells[j]))
                            continue;

                        cells.Remove(targetCloud.containedCells[j]);
                        targetCloud.containedCells[j] = cloud.containedCells[i];
                    }
                }
            }
        }
    }

    public void RevealSmokeCloud(SmokeCloud targetCloud)
    {
        for (int i = 0; i < targetCloud.containedCells.Length; i++)
        {
            targetCloud.containedCells[i].isVisible = false;
        }

        UpdateMeshWithGroups();
    }
    #endregion
}

public class SmokeCell
{
    public bool isVisible;
    public Vector3[] linkedVertices;
    public int[] linkedTriangles;
    public Vector2[] linkedUvs;
    public Rect fillArea;
}

public struct SmokeCloud
{
    public SmokeCell[] containedCells;
    public Rect fillArea;
}
