﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Xml;
using SharpDX.Direct3D11;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace PhysX.Samples.Engine
{
	/// <summary>A very basic COLLADA loader.</summary>
	/// <remarks>
	/// This basically loads the torus DAE file for use as a convex mesh.
	/// </remarks>
	public class ColladaLoader
	{
		public Model Load(string file, Device device)
		{
			XmlDocument document = new XmlDocument();
			document.Load(file);

			XmlNamespaceManager ns = new XmlNamespaceManager(document.NameTable);
			ns.AddNamespace("c", "http://www.collada.org/2005/11/COLLADASchema");

			// Read vertex positions
			var positionsNode = document.SelectSingleNode("/c:COLLADA/c:library_geometries/c:geometry/c:mesh/c:source/c:float_array", ns);
			var indicesNode = document.SelectSingleNode("/c:COLLADA/c:library_geometries/c:geometry/c:mesh/c:triangles/c:p", ns);

			var positions = ParseVector3s(positionsNode.InnerText).ToArray();
			var indices = ParseInts(indicesNode.InnerText).ToArray();

			// Mixing concerns here, but oh well
			var model = new Model()
			{
				VertexBuffer = Buffer.Create(device, BindFlags.VertexBuffer, positions),
				IndexBuffer = Buffer.Create(device, BindFlags.IndexBuffer, indices),
				IndexCount = indices.Length,
				VertexPositions = positions,
				Indices = indices
			};

			return model;
		}

		private IEnumerable<Vector3> ParseVector3s(string positionsText)
		{
			var positions = positionsText.Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

			for (int i = 0; i < positions.Length; i += 3)
			{
				float x = float.Parse(positions[i + 0], CultureInfo.InvariantCulture);
				float y = float.Parse(positions[i + 1], CultureInfo.InvariantCulture);
				float z = float.Parse(positions[i + 2], CultureInfo.InvariantCulture);

				yield return new Vector3(x, y, z);
			}
		}
		private IEnumerable<int> ParseInts(string indicesText)
		{
			var indices = indicesText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

			for (int i = 0; i < indices.Length; i++)
			{
				yield return int.Parse(indices[i], CultureInfo.InvariantCulture);
			}
		}
	}
}