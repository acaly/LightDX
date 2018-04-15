﻿using LightDx;
using LightDx.InputAttributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DynamicTriangle2
{
    static class Program
    {
        private struct Vertex
        {
            [Position]
            public Float4 Position;
            [Color]
            public Float4 Color;
        }

        private struct ConstantBuffer
        {
            public float Time;
        }

        static void SetCoordinate(ref Float4 position, double angle)
        {
            position.X = 0.5f * (float)Math.Cos(angle) / 4 * 3;
            position.Y = 0.5f * (float)Math.Sin(angle);
        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var form = new Form();
            form.ClientSize = new Size(800, 600);

            using (var device = LightDevice.Create(form))
            {
                var target = device.CreateDefaultTarget(false);
                target.Apply();

                Pipeline pipeline;
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("DynamicTriangle2.Shader.fx"))
                {
                    pipeline = device.CompilePipeline(ShaderSource.FromStream(stream), false, InputTopology.Triangle);
                }
                pipeline.Apply();

                var vertexData = new[] {
                    new Vertex { Position = new Float4(0, 0, 0.5f, 1), Color = Color.Green },
                    new Vertex { Position = new Float4(0, 0, 0.5f, 1), Color = Color.Red },
                    new Vertex { Position = new Float4(0, 0, 0.5f, 1), Color = Color.Blue },
                };

                var input = pipeline.CreateInputDataProcessor<Vertex>();
                var buffer = input.CreateDynamicBuffer(3);

                var indexBuffer = pipeline.CreateImmutableIndexBuffer(new uint[] { 0, 1, 2 });

                var constantBuffer = pipeline.CreateConstantBuffer<ConstantBuffer>();
                pipeline.SetConstant(ConstantUsage.VertexShader, 0, constantBuffer);

                form.Show();

                var clock = Stopwatch.StartNew();
                device.RunMultithreadLoop(delegate ()
                {
                    var angle = -clock.Elapsed.TotalSeconds * Math.PI / 3;
                    var distance = Math.PI * 2 / 3;

                    SetCoordinate(ref vertexData[0].Position, angle);
                    SetCoordinate(ref vertexData[1].Position, angle - distance);
                    SetCoordinate(ref vertexData[2].Position, angle + distance);
                    input.UpdateBufferDynamic(buffer, vertexData, 0, 3);

                    constantBuffer.Value.Time = ((float)clock.Elapsed.TotalSeconds % 2) / 2;
                    constantBuffer.Update();

                    target.ClearAll(Color.BlanchedAlmond);
                    indexBuffer.DrawAll(buffer);
                    device.Present(true);
                });
            }
        }
    }
}