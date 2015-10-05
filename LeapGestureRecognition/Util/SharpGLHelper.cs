using GlmNet;
using Leap;
using SharpGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LeapGestureRecognition.Util
{
	public class SharpGLHelper
	{
		private OpenGL _gl;
		private mat4 _modelMatrix,_viewMatrix, _projectionMatrix;

		public SharpGLHelper(OpenGL gl)
		{
			_gl = gl;
			//initializeMatrices();
		}

		// Temp, just for drawing pyramid
		private static float _rotation = 0;
		public void DrawRotatingPyramid()
		{
			//  Load the identity matrix.
			_gl.LoadIdentity();

			//  Rotate around the Y axis.
			_gl.Rotate(_rotation, 0.0f, 1.0f, 0.0f);

			//  Draw a coloured pyramid.
			_gl.Begin(OpenGL.GL_TRIANGLES);
			_gl.Color(1.0f, 0.0f, 0.0f);
			_gl.Vertex(0.0f, 1.0f, 0.0f);
			_gl.Color(0.0f, 1.0f, 0.0f);
			_gl.Vertex(-1.0f, -1.0f, 1.0f);
			_gl.Color(0.0f, 0.0f, 1.0f);
			_gl.Vertex(1.0f, -1.0f, 1.0f);
			_gl.Color(1.0f, 0.0f, 0.0f);
			_gl.Vertex(0.0f, 1.0f, 0.0f);
			_gl.Color(0.0f, 0.0f, 1.0f);
			_gl.Vertex(1.0f, -1.0f, 1.0f);
			_gl.Color(0.0f, 1.0f, 0.0f);
			_gl.Vertex(1.0f, -1.0f, -1.0f);
			_gl.Color(1.0f, 0.0f, 0.0f);
			_gl.Vertex(0.0f, 1.0f, 0.0f);
			_gl.Color(0.0f, 1.0f, 0.0f);
			_gl.Vertex(1.0f, -1.0f, -1.0f);
			_gl.Color(0.0f, 0.0f, 1.0f);
			_gl.Vertex(-1.0f, -1.0f, -1.0f);
			_gl.Color(1.0f, 0.0f, 0.0f);
			_gl.Vertex(0.0f, 1.0f, 0.0f);
			_gl.Color(0.0f, 0.0f, 1.0f);
			_gl.Vertex(-1.0f, -1.0f, -1.0f);
			_gl.Color(0.0f, 1.0f, 0.0f);
			_gl.Vertex(-1.0f, -1.0f, 1.0f);
			_gl.End();

			//  Nudge the rotation.
			_rotation += 3.0f;
		}

		public void DrawSphere()
		{
			IntPtr sphereQuadric = _gl.NewQuadric();
			_gl.LoadIdentity();
			_gl.Color(0.0, 0.0, 1.0);
			_gl.Translate(20.0, 0.0, 0.0);
			_gl.Sphere(sphereQuadric, 1.0, 25, 25);
		}

		public void DrawTwoCylinders()
		{
			IntPtr cylinderQuadric = _gl.NewQuadric();
			// Left cylinder
			_gl.LoadIdentity();
			_gl.Translate(-2, 0, 0);
			_gl.Color(0.0, 0.0, 1.0);
			_gl.Cylinder(cylinderQuadric, 1.0, 1.0, 3.0, 20, 20);

			// Right cylinder
			_gl.LoadIdentity();
			_gl.Translate(2, 0, 0);
			_gl.Color(0.0, 1.0, 0.0);
			_gl.Cylinder(cylinderQuadric, 1.5, 1.0, 3.0, 20, 20);
		}

		public void DrawFrame(Frame frame)
		{
			InteractionBox interactionBox = frame.InteractionBox;
			foreach (Hand hand in frame.Hands)
			{
				Vector palmPos = hand.PalmPosition;
				Vector normalizedPalmPos = interactionBox.NormalizePoint(palmPos);
				normalizedPalmPos.x -= 0.5f;
				normalizedPalmPos.y -= 0.5f;
				normalizedPalmPos.z -= 0.5f;
				vec3 vec3pos = new vec3(5 * normalizedPalmPos.x, 5 * normalizedPalmPos.y, 5 * normalizedPalmPos.z);
				DrawSphere(vec3pos, 0.2);

				foreach (Finger finger in hand.Fingers)
				{
					Vector tipPos = finger.TipPosition;
					Vector normalizedTipPos = interactionBox.NormalizePoint(tipPos);
					normalizedTipPos.x -= 0.5f;
					normalizedTipPos.y -= 0.5f;
					normalizedTipPos.z -= 0.5f;
					vec3pos = new vec3(5 * normalizedTipPos.x, 5 * normalizedTipPos.y, 5 * normalizedTipPos.z);
					DrawSphere(vec3pos, 0.07);
				}
			//	DrawSphere(normalizedPalmPos);
			}
		}

		public void DrawSphere(vec3 position, double radius) // vec4?
		{
			IntPtr sphereQuadric = _gl.NewQuadric();
			_gl.LoadIdentity();
			_gl.Color(0.0, 0.5, 0.5);
			_gl.Translate(position.x, position.y, position.z);
			_gl.Sphere(sphereQuadric, radius, 25, 25);
		}

		public void DrawSphere(Vector position) // Leap Vector
		{
			IntPtr sphereQuadric = _gl.NewQuadric();
			_gl.LoadIdentity();
			_gl.Color(0.0, 0.5, 0.5);
			_gl.Translate(position.x, position.y, position.z);
			_gl.Sphere(sphereQuadric, 0.2, 25, 25);
		}

		public static mat4 GetMat4(float[] values)
		{
			vec4 v1 = new vec4(values[0], values[1], values[2], values[3]);
			vec4 v2 = new vec4(values[4], values[5], values[6], values[7]);
			vec4 v3 = new vec4(values[8], values[9], values[10], values[11]);
			vec4 v4 = new vec4(values[12], values[13], values[14], values[15]);
			return new mat4(v1,v2,v3,v4);
		}

		// From http://webglfactory.blogspot.com/2011/05/how-to-convert-world-to-screen.html
		public vec3 Get3dPoint(vec2 point2D, int width, int height, mat4 viewMatrix, mat4 projectionMatrix)
		{
			float x = 2.0f * point2D.x / width - 1;
			float y = -2.0f * point2D.y / height + 1;
			mat4 viewProjectionInverse = glm.inverse(viewMatrix * projectionMatrix);

			vec4 point4D = new vec4(x, y, 0, 0);
			point4D = viewProjectionInverse * point4D;
			return new vec3(point4D);
		}

		private void initializeMatrices()
		{
			_projectionMatrix = glm.perspective(60, 16 / 9, 0.01f, 100.0f);

			vec4 viewRight = new vec4(1,0,0,0);
			vec4 viewUp = new vec4(0,1,0,0);
			vec4 viewAt = new vec4(0,0,0,0);
			vec4 viewPos = new vec4(0,5,5,1);
			_viewMatrix = new mat4(viewRight, viewUp, viewAt, viewPos);

			_modelMatrix = new mat4(1);

			mat4 MVP = _projectionMatrix * _viewMatrix * _modelMatrix;

			vec2 point2D = new vec2(1,2);
			vec3 test = Get3dPoint(point2D, 800, 600, _viewMatrix, _projectionMatrix);
		}
	}
}
