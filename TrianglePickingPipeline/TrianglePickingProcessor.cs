#region File Description
//-----------------------------------------------------------------------------
// TrianglePickingProcessor.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using System.IO;
using System.Text;
#endregion

namespace TrianglePickingPipeline
{
    /// <summary>
    /// Custom content pipeline processor attaches vertex position information to
    /// a model, which can be used at runtime to implement per-triangle picking.
    /// It derives from the built-in ModelProcessor, and overrides the Process
    /// method, using this to attach custom data to the model Tag property.
    /// </summary>
    [ContentProcessor]
    public class TrianglePickingProcessor : ModelProcessor
    {
        List<Vector3> vertices = new List<Vector3>();
        public const string TextureMapKey = "Texture";
        public const string NormalMapKey = "Bump0";
        public const string SpecularMapKey = "Specular0";
        public const string GlowMapKey = "Emissive0";

        static string[] fileKeys = { "Bump0", "Specular0", "Emissive0" };
        static string[] fileExt = { "_n.tga", "_s.tga", "_i.tga" };


        /// <summary>
        /// The main method in charge of processing the content.
        /// </summary>
        public override ModelContent Process(NodeContent input,
                                             ContentProcessorContext context)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }
            context.Logger.LogImportantMessage("processing: " + input.Name);
            PreprocessSceneHierarchy(input, context, input.Name);
            // Chain to the base ModelProcessor class.
            ModelContent model = base.Process(input, context);

            // Look up the input vertex positions.
            FindVertices(input);

            // You can store any type of object in the model Tag property. This
            // sample only uses built-in types such as string, Vector3, BoundingSphere,
            // dictionaries, and arrays, which the content pipeline knows how to
            // serialize by default. We could also attach custom data types here, but
            // then we would have to provide a ContentTypeWriter and ContentTypeReader
            // implementation to tell the pipeline how to serialize our custom type.
            //
            // We are setting our model Tag to a dictionary that maps strings to
            // objects, and then storing two different kinds of custom data into that
            // dictionary. This is a useful pattern because it allows processors to
            // combine many different kinds of information inside the single Tag value.

            Dictionary<string, object> tagData = new Dictionary<string, object>();

            model.Tag = tagData;

            // Store vertex information in the tag data, as an array of Vector3.
            tagData.Add("Vertices", vertices.ToArray());

            // Also store a custom bounding sphere.
            tagData.Add("BoundingSphere", BoundingSphere.CreateFromPoints(vertices));

            //tagData.Add("Name", input.Name.Replace("_geo", ""));
            string[] split = context.OutputFilename.Split('.', '\\');
            tagData.Add("Name", split[split.Length-2]);
            
            return model;
        }


        /// <summary>
        /// Helper for extracting a list of all the vertex positions in a model.
        /// </summary>
        void FindVertices(NodeContent node)
        {
            // Is this node a mesh?
            MeshContent mesh = node as MeshContent;

            if (mesh != null)
            {
                // Look up the absolute transform of the mesh.
                Matrix absoluteTransform = mesh.AbsoluteTransform;

                // Loop over all the pieces of geometry in the mesh.
                foreach (GeometryContent geometry in mesh.Geometry)
                {
                    // Loop over all the indices in this piece of geometry.
                    // Every group of three indices represents one triangle.
                    foreach (int index in geometry.Indices)
                    {
                        // Look up the position of this vertex.
                        Vector3 vertex = geometry.Vertices.Positions[index];

                        // Transform from local into world space.
                        vertex = Vector3.Transform(vertex, absoluteTransform);

                        // Store this vertex.
                        vertices.Add(vertex);
                    }
                }
            }

            // Recursively scan over the children of this node.
            foreach (NodeContent child in node.Children)
            {
                FindVertices(child);
            }
        }

        /// <summary>
        /// Recursively calls MeshHelper.CalculateTangentFrames for every MeshContent
        /// object in the NodeContent scene. This function could be changed to add 
        /// more per vertex data as needed.
        /// </summary>
        /// <param initialFileName="input">A node in the scene.  The function should be called
        /// with the root of the scene.</param>
        private void PreprocessSceneHierarchy(NodeContent input,
            ContentProcessorContext context, string inputName)
        {
            MeshContent mesh = input as MeshContent;
            if (mesh != null)
            {
                MeshHelper.CalculateTangentFrames(mesh,
                    VertexChannelNames.TextureCoordinate(0),
                    VertexChannelNames.Tangent(0),
                    VertexChannelNames.Binormal(0));

                foreach (GeometryContent geometry in mesh.Geometry)
                {
                    if (false == geometry.Material.Textures.ContainsKey(TextureMapKey))
                        geometry.Material.Textures.Add(TextureMapKey,
                                new ExternalReference<TextureContent>(
                                        "null_color.tga"));
                    else
                    {
                        context.Logger.LogImportantMessage("has: " + geometry.Material.Textures[TextureMapKey].Filename);
                        string fileName = Path.GetFileName(geometry.Material.Textures[TextureMapKey].Filename);
                        if (fileName != null && fileName.StartsWith("ship") && fileName.EndsWith("_c.tga"))
                            InsertMissedMapTextures(geometry.Material.Textures,
                                fileName.Substring(0, fileName.Length - "_c.tga".Length), context);
                    }

                    if (false == geometry.Material.Textures.ContainsKey(NormalMapKey))
                        geometry.Material.Textures.Add(NormalMapKey,
                                new ExternalReference<TextureContent>(
                                        "null_normal.tga"));
                    else
                        context.Logger.LogImportantMessage("has: " + geometry.Material.Textures[NormalMapKey].Filename);

                    if (false == geometry.Material.Textures.ContainsKey(SpecularMapKey))
                        geometry.Material.Textures.Add(SpecularMapKey,
                                new ExternalReference<TextureContent>(
                                        "null_specular.tga"));
                    else
                        context.Logger.LogImportantMessage("has: " + geometry.Material.Textures[SpecularMapKey].Filename);

                    if (false == geometry.Material.Textures.ContainsKey(GlowMapKey))
                        geometry.Material.Textures.Add(GlowMapKey,
                                new ExternalReference<TextureContent>(
                                        "null_glow.tga"));
                    else
                        context.Logger.LogImportantMessage("has: " + geometry.Material.Textures[GlowMapKey].Filename);
                }
            }

            foreach (NodeContent child in input.Children)
            {
                PreprocessSceneHierarchy(child, context, inputName);
            }
        }

        /// <summary>
        /// Ship models miss map textures. We insert them were need.
        /// </summary>
        /// <param initialFileName="textureReferenceDictionary"></param>
        /// <param initialFileName="initialFileName"></param>
        private void InsertMissedMapTextures(TextureReferenceDictionary textures, string initialFileName,
                        ContentProcessorContext context)
        {
            for (int i = 0; i < fileKeys.Length; i++)
            {
                string key = fileKeys[i];
                if (textures.ContainsKey(key))
                    continue;
                string fileName = "ships/" + initialFileName + fileExt[i];
                textures.Add(key,
                    new ExternalReference<TextureContent>(fileName));
                context.Logger.LogImportantMessage("inserted: " + fileName);
            }
        }

        // acceptableVertexChannelNames are the inputs that the normal map effect
        // expects.  The NormalMappingModelProcessor overrides ProcessVertexChannel
        // to remove all vertex channels which don't have one of these four
        // names.
        static IList<string> acceptableVertexChannelNames =
            new string[]
            {
                VertexChannelNames.TextureCoordinate(0),
                VertexChannelNames.Normal(0),
                VertexChannelNames.Binormal(0),
                VertexChannelNames.Tangent(0)
            };


        /// <summary>
        /// As an optimization, ProcessVertexChannel is overriden to remove data which
        /// is not used by the vertex shader.
        /// </summary>
        /// <param initialFileName="geometry">the geometry object which contains the 
        /// vertex channel</param>
        /// <param initialFileName="vertexChannelIndex">the index of the vertex channel
        /// to operate on</param>
        /// <param initialFileName="context">the context that the processor is operating
        /// under.  in most cases, this parameter isn't necessary; but could
        /// be used to log a warning that a channel had been removed.</param>
        protected override void ProcessVertexChannel(GeometryContent geometry,
            int vertexChannelIndex, ContentProcessorContext context)
        {
            String vertexChannelName =
                geometry.Vertices.Channels[vertexChannelIndex].Name;

            // if this vertex channel has an acceptable names, process it as normal.
            if (acceptableVertexChannelNames.Contains(vertexChannelName))
            {
                base.ProcessVertexChannel(geometry, vertexChannelIndex, context);
            }
            // otherwise, remove it from the vertex channels; it's just extra data
            // we don't need.
            else
            {
                geometry.Vertices.Channels.Remove(vertexChannelName);
            }
        }

        //protected override MaterialContent ConvertMaterial(MaterialContent material,
        //    ContentProcessorContext context)
        //{
        //    EffectMaterialContent normalMappingMaterial = new EffectMaterialContent();
        //    normalMappingMaterial.Effect = new ExternalReference<EffectContent>
        //        ("shaders/NormalMapping.fx");

        //    // copy the textures in the original material to the new normal mapping
        //    // material. this way the diffuse texture is preserved. The
        //    // PreprocessSceneHierarchy function has already added the normal map
        //    // texture to the Textures collection, so that will be copied as well.
        //    foreach (KeyValuePair<String, ExternalReference<TextureContent>> texture
        //        in material.Textures)
        //    {
        //        normalMappingMaterial.Textures.Add(texture.Key, texture.Value);
        //    }

        //    return context.Convert<MaterialContent, MaterialContent>
        //        (normalMappingMaterial, typeof(MaterialProcessor).Name);
        //}
    }
}
