using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGridDraw : MonoBehaviour {
    public int gridSizeX;
    public int gridSizeY;
    public int gridSizeZ;

    public float gridStep;

    public Color gridColor = Color.gray;

    public Material lineMaterial;

    private bool _enable = true;

    private void Start() {
        if (lineMaterial != null) {
            Color tmp = lineMaterial.color;
            tmp.a = 0f;
            lineMaterial.color = tmp;
        }
    }

    void OnPostRender() {
        if (!_enable)
            return;

        // Make sure have line material
        if (lineMaterial == null)
            return;

        // Check valid value
        if (gridSizeX < 0 || gridSizeY < 0 || gridSizeZ < 0 || gridStep <= 0)
            return;
        // Start to draw
        lineMaterial.SetPass(0);
        GL.Begin(GL.LINES);

        // GL.Color(gridColor);
        // get start point
        Vector3 startPoint = getMostClosePos();
        Vector3 delta = new Vector3(gridSizeX, gridSizeY, gridSizeZ);
        delta *= (gridStep / 2);
        startPoint -= delta;

        for(float y = 0; y <= gridSizeY * gridStep; y += gridStep) {
            // z-axis lines
            for(float x = 0; x <= gridSizeX * gridStep; x += gridStep) {
                GL.Vertex3(startPoint.x + x, startPoint.y + y, startPoint.z + x);
                GL.Vertex3(startPoint.x + x, startPoint.y + y, startPoint.z + gridSizeZ * gridStep);
            }

            // x-axis lines
            for (float z = 0; z <= gridSizeZ * gridStep; z += gridStep) {
                GL.Vertex3(startPoint.x, startPoint.y + y, startPoint.z + z);
                GL.Vertex3(startPoint.x + gridSizeX * gridStep, startPoint.y + y, startPoint.z + z);
            }
        }

        // y-axis lines
        for(float x = 0; x <= gridSizeX * gridStep; x += gridStep) {
            for(float z = 0; z <= gridSizeZ * gridStep; z += gridStep) {
                GL.Vertex3(startPoint.x + x, startPoint.y, startPoint.z + z);
                GL.Vertex3(startPoint.x + x, startPoint.y + gridSizeY * gridStep, startPoint.z + z);
            }
        }
        GL.End();
    }

    private Vector3 getMostClosePos() {
        float x, y, z;
        x = Mathf.Floor(transform.position.x / gridStep) * gridStep;
        y = Mathf.Floor(transform.position.y / gridStep) * gridStep;
        z = Mathf.Floor(transform.position.z / gridStep) * gridStep;
        return new Vector3(x, y, z);
    }

    public void SetLineDrawEnable(bool flag) {
        _enable = flag;
    }
}
