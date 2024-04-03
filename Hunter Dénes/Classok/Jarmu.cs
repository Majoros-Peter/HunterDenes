using HelixToolkit.Wpf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.IO;
using System.Windows.Controls;

namespace Hunter_Dénes.Classok
{
    public class ObjImporter
    {
        public static void ImportObjWithColors(string objFilePath, string mtlFilePath, Viewport3D viewport, double scaleFactor = 1.0)
        {
            if (!File.Exists(objFilePath) || !File.Exists(mtlFilePath))
            {
                Console.WriteLine("OBJ or MTL file not found.");
                return;
            }

            var importer = new ModelImporter();
            var model = importer.Load(objFilePath);

            ScaleModel(model, scaleFactor);

            var materialsDict = ParseMaterials(mtlFilePath);

            ApplyMaterials(model, materialsDict);

            ModelVisual3D modelVisual = new ModelVisual3D { Content = model };



            // Create a Transform3DGroup to hold the rotation transformation
            Transform3DGroup transformGroup = new Transform3DGroup();

            // Create a RotateTransform3D to rotate around the Y-axis
            RotateTransform3D rotationTransform = new RotateTransform3D();
            AxisAngleRotation3D axisRotation = new AxisAngleRotation3D(new Vector3D(0, 0, 1), 180); // Rotate by -90 degrees around the Y-axis
            rotationTransform.Rotation = axisRotation;

            // Add the rotation transformation to the Transform3DGroup
            transformGroup.Children.Add(rotationTransform);

            // Apply the Transform3DGroup to the ModelVisual3D object
            modelVisual.Transform = transformGroup;



            modelVisual.SetName("tengeralattjaro");
            viewport.Children.Add(modelVisual);
        }

        private static void ScaleModel(Model3D model, double scaleFactor)
        {
            if (model is GeometryModel3D geometryModel)
            {
                geometryModel.Transform = new ScaleTransform3D(scaleFactor, scaleFactor, scaleFactor);
            }
            else if (model is Model3DGroup modelGroup)
            {
                foreach (var childModel in modelGroup.Children)
                {
                    ScaleModel(childModel, scaleFactor);
                }
            }
        }

        private static Dictionary<string, Material> ParseMaterials(string mtlFilePath)
        {
            var materialsDict = new Dictionary<string, Material>();

            string[] mtlLines = File.ReadAllLines(mtlFilePath);

            string currentMaterial = null;
            Color currentColor = Colors.White;

            foreach (string line in mtlLines)
            {
                if (line.StartsWith("newmtl"))
                {
                    currentMaterial = line.Split()[1];
                }
                else if (line.StartsWith("Kd"))
                {
                    string[] parts = line.Split(' ');
                    double r = double.Parse(parts[1], CultureInfo.InvariantCulture);
                    double g = double.Parse(parts[2], CultureInfo.InvariantCulture);
                    double b = double.Parse(parts[3], CultureInfo.InvariantCulture);
                    currentColor = Color.FromRgb((byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
                    materialsDict.Add(currentMaterial, new DiffuseMaterial(new SolidColorBrush(currentColor)));
                }
            }

            return materialsDict;
        }

        private static void ApplyMaterials(Model3D model, Dictionary<string, Material> materialsDict)
        {
            if (model is GeometryModel3D geometryModel && geometryModel.Material is MaterialGroup materialGroup)
            {
                string materialName = geometryModel.Material.GetType().Name;
                if (materialsDict.ContainsKey(materialName))
                {
                    Material material = materialsDict[materialName];
                    materialGroup.Children.Clear();
                    materialGroup.Children.Add(material);
                }
            }
            else if (model is Model3DGroup modelGroup)
            {
                foreach (var childModel in modelGroup.Children)
                {
                    ApplyMaterials(childModel, materialsDict);
                }
            }
        }
    }
}