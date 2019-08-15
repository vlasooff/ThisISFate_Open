using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Community.Tools
{
    public class MeshBaker : EditorWindow
    {
        enum Resolving
        {
            Not,
            Remove,
            Create
        }

        private const int MAX_VERTEX_COUNT_PER_ONE_OBJECT = 65000;

        private static Resolving colorResolving;
        private static Resolving normalsResolving;
        private static Resolving tangentsResolving;
        private static Resolving uvResolving;

        private static int objectNum;

        [MenuItem("GameObject/Bake Meshes", false, 0)]
        private static void BakeMeshes(MenuCommand menuCommand)
        {
            if (Selection.objects.Length == 0)
            {
                return;
            }
            //Prevent executing multiple times
            if (Selection.objects.Length > 1)
            {
                if (menuCommand.context != Selection.objects[0])
                {
                    return;
                }
            }

            List<Vector3> vertexes = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<Vector4> tangents = new List<Vector4>();
            List<Color> colors = new List<Color>();
            List<Vector2> uvs = new List<Vector2>();
            List<int> triangles = new List<int>();

            Dictionary<Material, List<Mesh>> meshesWithMaterials = new Dictionary<Material, List<Mesh>>();
            List<Mesh> meshesWithoutMaterials = new List<Mesh>();

            objectNum = 0;
            if (!FillDataAndCheckResolving(meshesWithMaterials, meshesWithoutMaterials))
            {
                return;
            }

            bool separateObjectsWithDifferentMaterials = false;
            if (meshesWithMaterials.Count + (meshesWithoutMaterials.Count > 0 ? 1 : 0) > 1)
            {
                separateObjectsWithDifferentMaterials = EditorUtility.DisplayDialog("Simplest Mesh Baker",
                    "Do you want to separate objects with different materials?", "Yes", "No");
            }

            if (!separateObjectsWithDifferentMaterials)
            {
                Material material = null;
                foreach (var meshToBake in meshesWithMaterials)
                {
                    material = meshToBake.Key;
                    if (material != null)
                    {
                        break;
                    }
                }

                foreach (var mesh in meshesWithoutMaterials)
                {
                    Bake(mesh, vertexes, normals, tangents, colors, uvs, triangles, material);
                }
                foreach (var meshToBake in meshesWithMaterials)
                {
                    foreach (var mesh in meshToBake.Value)
                    {
                        Bake(mesh, vertexes, normals, tangents, colors, uvs, triangles, material);
                    }
                }
                if (vertexes.Count > 0)
                {
                    CreateObject(vertexes, normals, tangents, colors, uvs, triangles, material);
                    vertexes.Clear();
                    normals.Clear();
                    tangents.Clear();
                    colors.Clear();
                    uvs.Clear();
                    triangles.Clear();
                }
            }
            else
            {
                foreach (var mesh in meshesWithoutMaterials)
                {
                    Bake(mesh, vertexes, normals, tangents, colors, uvs, triangles, null);
                }
                if (vertexes.Count > 0)
                {
                    CreateObject(vertexes, normals, tangents, colors, uvs, triangles, null);
                    vertexes.Clear();
                    normals.Clear();
                    tangents.Clear();
                    colors.Clear();
                    uvs.Clear();
                    triangles.Clear();
                }
                foreach (var meshToBake in meshesWithMaterials)
                {
                    foreach (var mesh in meshToBake.Value)
                    {
                        Bake(mesh, vertexes, normals, tangents, colors, uvs, triangles, meshToBake.Key);
                    }
                    CreateObject(vertexes, normals, tangents, colors, uvs, triangles, meshToBake.Key);
                    vertexes.Clear();
                    normals.Clear();
                    tangents.Clear();
                    colors.Clear();
                    uvs.Clear();
                    triangles.Clear();
                }
            }

            if (EditorUtility.DisplayDialog("Simplest Mesh Baker",
                "Do you want to remove sources?", "Yes", "No"))
            {
                foreach (GameObject selected in Selection.gameObjects)
                {
                    if (selected != null)
                    {
                        Undo.DestroyObjectImmediate(selected);
                    }
                }
            }

            int meshesCount = meshesWithoutMaterials.Count;
            foreach (var bakedMeshes in meshesWithMaterials)
            {
                meshesCount += bakedMeshes.Value.Count;
            }
            EditorUtility.DisplayDialog("Simplest Mesh Baker",
                "Baked " + meshesCount + " meshes.", "Cool!");
        }
        [MenuItem("GameObject/Save Meshes", false, 0)]
        private static void OptMeshes(MenuCommand menuCommand)
        {
            if (Selection.objects.Length == 0)
            {
                return;
            }
            //Prevent executing multiple times
            if (Selection.objects.Length > 1)
            {
                if (menuCommand.context != Selection.objects[0])
                {
                    return;
                }
            }
            int meshesCount = 0;
            foreach (GameObject selected in Selection.gameObjects)
            {
                if (selected != null)
                {
                    meshesCount += 1;
                    MeshFilter filter = selected.GetComponent<MeshFilter>();
                    MeshToFile(filter, $"{Application.dataPath}/Test{meshesCount}.obj");
                    
                }
            }
            EditorUtility.DisplayDialog("Simplest Mesh Baker",
                "Baked " + meshesCount + " meshes.", "Cool!");
        }
        private static bool FillDataAndCheckResolving(Dictionary<Material, List<Mesh>> meshesWithMaterials, List<Mesh> meshesWithoutMaterials)
        {
            colorResolving = Resolving.Not;
            normalsResolving = Resolving.Not;
            uvResolving = Resolving.Not;

            bool anyHasColors = false;
            bool anyHasNormals = false;
            bool anyHasUVs = false;
            bool anyHasNotColors = false;
            bool anyHasNotNormals = false;
            bool anyHasNotUVs = false;

            HashSet<Transform> transforms = new HashSet<Transform>();

            foreach (GameObject selected in Selection.gameObjects)
            {
                MeshFilter[] meshFilters = selected.GetComponentsInChildren<MeshFilter>();
                foreach (var meshFilter in meshFilters)
                {
                    if (transforms.Contains(meshFilter.transform))
                    {
                        continue;
                    }
                    Material material = null;
                    var mr = meshFilter.GetComponent<MeshRenderer>();
                    if (mr != null)
                    {
                        material = mr.sharedMaterial;
                    }
                    Mesh mesh = Instantiate(meshFilter.sharedMesh);
                    HandleMesh(meshesWithMaterials, meshesWithoutMaterials, mesh, meshFilter.transform, material, transforms, ref anyHasNotNormals, ref anyHasNotColors, ref anyHasNotUVs, ref anyHasNormals, ref anyHasColors, ref anyHasUVs);
                }

                SkinnedMeshRenderer[] skinnedMeshRenderers = selected.GetComponentsInChildren<SkinnedMeshRenderer>();
                foreach (var skinnedMeshRenderer in skinnedMeshRenderers)
                {
                    if (transforms.Contains(skinnedMeshRenderer.transform))
                    {
                        continue;
                    }
                    Material material = skinnedMeshRenderer.sharedMaterial;
                   // Mesh mesh = BonesBaker.GetMeshFromSkinnedMeshRenderer(skinnedMeshRenderer);
                    //HandleMesh(meshesWithMaterials, meshesWithoutMaterials, mesh, skinnedMeshRenderer.transform, material, transforms, ref anyHasNotNormals, ref anyHasNotColors, ref anyHasNotUVs, ref anyHasNormals, ref anyHasColors, ref anyHasUVs);
                }
            }

            return SetResolving(anyHasNormals, anyHasNotNormals, ref normalsResolving, "normals") &&
                   SetResolving(anyHasColors, anyHasNotColors, ref colorResolving, "colors") &&
                   SetResolving(anyHasUVs, anyHasNotUVs, ref uvResolving, "uvs");
        }

        private static void HandleMesh(Dictionary<Material, List<Mesh>> meshesWithMaterials, List<Mesh> meshesWithoutMaterials, Mesh mesh,
            Transform transform, Material material, HashSet<Transform> transforms, ref bool anyHasNotNormals,
            ref bool anyHasNotColors, ref bool anyHasNotUVs, ref bool anyHasNormals, ref bool anyHasColors, ref bool anyHasUVs)
        {
            mesh.vertices = mesh.vertices.Select(transform.TransformPoint).ToArray();
            mesh.normals = mesh.normals.Select(transform.TransformDirection).ToArray();

            if (material == null)
            {
                meshesWithoutMaterials.Add(mesh);
            }
            else
            {
                if (meshesWithMaterials.ContainsKey(material))
                {
                    meshesWithMaterials[material].Add(mesh);
                }
                else
                {
                    meshesWithMaterials.Add(material, new List<Mesh>() { mesh });
                }
            }
            transforms.Add(transform);

            CheckMeshAttributes(mesh, ref anyHasNotNormals, ref anyHasNotColors, ref anyHasNotUVs, ref anyHasNormals,
                ref anyHasColors, ref anyHasUVs);
        }

        private static void CheckMeshAttributes(Mesh mesh, ref bool anyHasNotNormals, ref bool anyHasNotColors, ref bool anyHasNotUVs,
            ref bool anyHasNormals, ref bool anyHasColors, ref bool anyHasUVs)
        {
            bool hasNormals = mesh.vertexCount == mesh.normals.Length;
            bool hasColors = mesh.vertexCount == mesh.colors.Length;
            bool hasUVs = mesh.vertexCount == mesh.uv.Length;

            anyHasNotNormals |= !hasNormals;
            anyHasNotColors |= !hasColors;
            anyHasNotUVs |= !hasUVs;

            anyHasNormals |= hasNormals;
            anyHasColors |= hasColors;
            anyHasUVs |= hasUVs;
        }

        private static bool SetResolving(bool has, bool hasNot, ref Resolving resolving, string property)
        {
            if (has && hasNot)
            {
                var result = EditorUtility.DisplayDialogComplex("Simplest Mesh Baker",
                    "Not all objects used " + property + ".",
                    "Don't use " + property, //0
                    "Cancel", //1
                    "Create fake " + property //2
                );
                if (result == 1)
                {
                    return false;
                }
                resolving = result == 0 ? Resolving.Remove : Resolving.Create;
            }
            return true;
        }

        private static void CreateObject(List<Vector3> vertexes, List<Vector3> normals, List<Vector4> tangents, List<Color> colors,
            List<Vector2> uvs, List<int> triangles, Material material)
        {
            GameObject go = new GameObject();
            Undo.RegisterCreatedObjectUndo(go, "Create a new baked gameobject");
            objectNum++;
            go.name = "Baked Mesh" + (objectNum == 1 ? "" : " " + objectNum);
            MeshFilter mf = go.AddComponent<MeshFilter>();
            MeshRenderer mr = go.AddComponent<MeshRenderer>();
            Mesh newMesh = new Mesh();
            

            newMesh.SetVertices(vertexes);
            if (normals.Count != 0 && normalsResolving != Resolving.Remove)
            {
                newMesh.SetNormals(normals);
            }
            if (tangents.Count != 0 && tangentsResolving != Resolving.Remove)
            {
                newMesh.SetTangents(tangents);
            }
            if (colors.Count != 0 && colorResolving != Resolving.Remove)
            {
                newMesh.SetColors(colors);
            }
            if (uvs.Count != 0 || uvResolving != Resolving.Remove)
            {
                newMesh.SetUVs(0, uvs);
            }

            newMesh.SetTriangles(triangles, 0);
            mf.sharedMesh = newMesh;
            mr.material = material;
            newMesh.Optimize();
        }

        private static void Bake(Mesh mesh, List<Vector3> vertexes, List<Vector3> normals, List<Vector4> tangents,
            List<Color> colors, List<Vector2> uvs, List<int> triangles, Material material)
        {
            //mesh may not have more than 65000 vertices.
            if (vertexes.Count + mesh.vertexCount > MAX_VERTEX_COUNT_PER_ONE_OBJECT)
            {
                
                CreateObject(vertexes, normals, tangents, colors, uvs, triangles, material);
                vertexes.Clear();
                normals.Clear();
                tangents.Clear();
                colors.Clear();
                uvs.Clear();
                triangles.Clear();
            }

            int startCount = vertexes.Count;
            foreach (Vector3 vertex in mesh.vertices)
            { 
                vertexes.Add(vertex);
    
            }
            foreach (int triangle in mesh.triangles)
            {
                triangles.Add(triangle + startCount);
            }

            FillOrResolve(mesh.normals, normals, mesh.vertices.Length, normalsResolving);
            FillOrResolve(mesh.tangents, tangents, mesh.tangents.Length, tangentsResolving);
            FillOrResolve(mesh.colors, colors, mesh.vertices.Length, colorResolving);
            FillOrResolve(mesh.uv, uvs, mesh.vertices.Length, uvResolving);
        }

        private static void FillOrResolve<T>(T[] source, List<T> distanation, int expectedCount,
            Resolving resolvingLogic)
        {
            if (source.Length == 0 && resolvingLogic == Resolving.Create)
            {
                for (int i = 0; i < expectedCount; i++)
                {
                    distanation.Add(default(T));
                }
            }
            else
            {
                distanation.AddRange(source);
            }
        }
        public static string MeshToString(MeshFilter mf)
        {
            Mesh m = mf.mesh;
            Material[] mats = mf.GetComponent<MeshRenderer>().sharedMaterials;

            StringBuilder sb = new StringBuilder();

            sb.Append("g ").Append(mf.name).Append("\n");
            foreach (Vector3 v in m.vertices)
            {
                sb.Append(string.Format("v {0} {1} {2}\n", v.x, v.y, v.z));
            }
            sb.Append("\n");
            foreach (Vector3 v in m.normals)
            {
                sb.Append(string.Format("vn {0} {1} {2}\n", v.x, v.y, v.z));
            }
            sb.Append("\n");
            foreach (Vector3 v in m.uv)
            {
                sb.Append(string.Format("vt {0} {1}\n", v.x, v.y));
            }
            for (int material = 0; material < m.subMeshCount; material++)
            {
                sb.Append("\n");
                sb.Append("usemtl ").Append(mats[material].name).Append("\n");
                sb.Append("usemap ").Append(mats[material].name).Append("\n");

                int[] triangles = m.GetTriangles(material);
                for (int i = 0; i < triangles.Length; i += 3)
                {
                    sb.Append(string.Format("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}\n",
                        triangles[i] + 1, triangles[i + 1] + 1, triangles[i + 2] + 1));
                }
            }
            return sb.ToString();
        }

        public static void MeshToFile(MeshFilter mf, string filename)
        {
            using (StreamWriter sw = new StreamWriter(filename))
            {
                sw.Write(MeshToString(mf));
            }
        }
    }
}
