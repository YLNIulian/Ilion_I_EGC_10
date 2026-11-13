using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

namespace Egc;

public static class Shapes
{
    public static Mesh Grid(int halfSize, float step, Color4 color)
    {
        int lines = halfSize * 2 + 1;
        var vertices = new float[lines * 4 * Mesh.FloatsPerVertex];
        var indices = new uint[lines * 4];
        int v = 0;

        for (int i = -halfSize; i <= halfSize; i++)
        {
            float p = i * step;
            SetVertex(vertices, v++, -halfSize * step, 0, p, color.R, color.G, color.B);
            SetVertex(vertices, v++, halfSize * step, 0, p, color.R, color.G, color.B);
            SetVertex(vertices, v++, p, 0, -halfSize * step, color.R, color.G, color.B);
            SetVertex(vertices, v++, p, 0, halfSize * step, color.R, color.G, color.B);
        }

        for (uint i = 0; i < indices.Length; i++) indices[i] = i;
        return new Mesh(vertices, indices, PrimitiveType.Lines);
    }

    public static Mesh Axes(float length)
    {
        var vertices = new float[]
        {
            0,0,0, 1,0,0, length,0,0, 1,0,0,
            0,0,0, 0,1,0, 0,length,0, 0,1,0,
            0,0,0, 0,0,1, 0,0,length, 0,0,1
        };
        var indices = Enumerable.Range(0, 6).Select(i => (uint)i).ToArray();
        return new Mesh(vertices, indices, PrimitiveType.Lines);
    }

    public static Mesh Pyramid()
    {
        var vertices = new float[]
        {
            -0.5f,0,-0.5f, 1,0,0,  0.5f,0,-0.5f, 0,1,0,
             0.5f,0, 0.5f, 0,0,1, -0.5f,0, 0.5f, 1,1,0,
             0,1,0, 1,0,1
        };
        var indices = new uint[]
        {
            0,1,4, 1,2,4, 2,3,4, 3,0,4,
            0,3,2, 0,2,1
        };
        return new Mesh(vertices, indices);
    }

    public static Mesh Cube()
    {
        float h = 0.5f;
        var vertices = new List<float>();
        var indices = new List<uint>();

        AddFace(vertices, indices, new Vector3(-h,-h,h), new Vector3(h,-h,h), new Vector3(h,h,h), new Vector3(-h,h,h), new Color4(0.2f,0.4f,1,1));
        AddFace(vertices, indices, new Vector3(h,-h,-h), new Vector3(-h,-h,-h), new Vector3(-h,h,-h), new Vector3(h,h,-h), new Color4(1,0.3f,0.2f,1));
        AddFace(vertices, indices, new Vector3(-h,h,h), new Vector3(h,h,h), new Vector3(h,h,-h), new Vector3(-h,h,-h), new Color4(0.2f,1,0.3f,1));
        AddFace(vertices, indices, new Vector3(-h,-h,-h), new Vector3(h,-h,-h), new Vector3(h,-h,h), new Vector3(-h,-h,h), new Color4(1,0.8f,0.2f,1));
        AddFace(vertices, indices, new Vector3(h,-h,h), new Vector3(h,-h,-h), new Vector3(h,h,-h), new Vector3(h,h,h), new Color4(1,0.2f,0.2f,1));
        AddFace(vertices, indices, new Vector3(-h,-h,-h), new Vector3(-h,-h,h), new Vector3(-h,h,h), new Vector3(-h,h,-h), new Color4(0.8f,0.2f,1,1));

        return new Mesh(vertices.ToArray(), indices.ToArray());
    }

    public static Mesh Sphere(float radius, int slices, int stacks)
    {
        int columns = slices + 1;
        int rows = stacks + 1;
        var vertices = new float[rows * columns * Mesh.FloatsPerVertex];
        var indices = new uint[stacks * slices * 6];

        for (int i = 0; i < rows; i++)
        {
            float v = (float)i / stacks;
            float latitude = -MathF.PI / 2 + v * MathF.PI;

            for (int j = 0; j < columns; j++)
            {
                float u = (float)j / slices;
                float longitude = u * 2 * MathF.PI;
                float x = radius * MathF.Cos(latitude) * MathF.Cos(longitude);
                float y = radius * MathF.Sin(latitude);
                float z = radius * MathF.Cos(latitude) * MathF.Sin(longitude);
                SetVertex(vertices, i * columns + j, x, y, z, u, v, 0.35f);
            }
        }

        int k = 0;
        for (int i = 0; i < stacks; i++)
        {
            for (int j = 0; j < slices; j++)
            {
                uint bottomLeft = (uint)(i * columns + j);
                uint bottomRight = bottomLeft + 1;
                uint topLeft = bottomLeft + (uint)columns;
                uint topRight = topLeft + 1;
                indices[k++] = bottomLeft;
                indices[k++] = bottomRight;
                indices[k++] = topLeft;
                indices[k++] = bottomRight;
                indices[k++] = topRight;
                indices[k++] = topLeft;
            }
        }

        return new Mesh(vertices, indices);
    }

    private static void AddFace(List<float> vertices, List<uint> indices, Vector3 a, Vector3 b, Vector3 c, Vector3 d, Color4 color)
    {
        uint start = (uint)(vertices.Count / Mesh.FloatsPerVertex);
        AddVertex(vertices, a, color);
        AddVertex(vertices, b, color);
        AddVertex(vertices, c, color);
        AddVertex(vertices, d, color);
        indices.AddRange(new[] { start, start + 1, start + 2, start, start + 2, start + 3 });
    }

    private static void AddVertex(List<float> vertices, Vector3 p, Color4 color)
    {
        vertices.AddRange(new[] { p.X, p.Y, p.Z, color.R, color.G, color.B });
    }

    private static void SetVertex(float[] vertices, int index, float x, float y, float z, float r, float g, float b)
    {
        int i = index * Mesh.FloatsPerVertex;
        vertices[i] = x;
        vertices[i + 1] = y;
        vertices[i + 2] = z;
        vertices[i + 3] = r;
        vertices[i + 4] = g;
        vertices[i + 5] = b;
    }
}
