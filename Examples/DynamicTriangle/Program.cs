﻿using LightDx;
using LightDx.InputAttributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DynamicTriangle
{
    static class Program
    {
        private struct VertexP
        {
            [Position]
            public Vector4 Position;
        }
        private struct VertexC
        {
            [Color]
            public Vector4 Color;
        }

        private struct ConstantBuffer
        {
            public Vector4 GlobalAlpha;
            public float Time;
        }

        static void SetCoordinate(LightDevice device, ref Vector4 position, double angle)
        {
            position.X = 0.5f * (float)Math.Cos(angle) * 600 / device.ScreenWidth;
            position.Y = 0.5f * (float)Math.Sin(angle) * 600 / device.ScreenHeight;
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
                var target = new RenderTargetList(device.GetDefaultTarget());
                target.Apply();

                Pipeline pipeline = device.CompilePipeline(InputTopology.Triangle,
                    ShaderSource.FromResource("Shader.fx", ShaderType.Vertex | ShaderType.Pixel));
                pipeline.Apply();

                var inputGroup = pipeline.CreateVertexDataProcessors(new[] {
                    typeof(VertexP),
                    typeof(VertexC),
                });
                var input1 = inputGroup.GetVertexDataProcessor<VertexP>();
                var input2 = inputGroup.GetVertexDataProcessor<VertexC>();
                
                var buffer1 = input1.CreateDynamicBuffer(3);
                var vertexPosData = new[] {
                    new VertexP { Position = new Vector4(0, 0, 0.5f, 1) },
                    new VertexP { Position = new Vector4(0, 0, 0.5f, 1) },
                    new VertexP { Position = new Vector4(0, 0, 0.5f, 1) },
                };
                var buffer2 = input2.CreateImmutableBuffer(new[] {
                    new VertexC { Color = Color.Green.WithAlpha(1) },
                    new VertexC { Color = Color.Red.WithAlpha(1) },
                    new VertexC { Color = Color.Blue.WithAlpha(1) },
                });
                var bufferGroup = new[] { buffer1, buffer2 };

                var indexBuffer = pipeline.CreateImmutableIndexBuffer(new uint[] { 0, 1, 2 });

                var constantBuffer = pipeline.CreateConstantBuffer<ConstantBuffer>();
                pipeline.SetConstant(ShaderType.Vertex, 0, constantBuffer);
                pipeline.SetConstant(ShaderType.Pixel, 0, constantBuffer);

                constantBuffer.Value.GlobalAlpha = new Vector4(1, 1, 1, 1);

                form.Show();
                
                var i = 0;
                var rand = new Random();

                var clock = Stopwatch.StartNew();
                device.RunMultithreadLoop(delegate ()
                {
                    var angle = -clock.Elapsed.TotalSeconds * Math.PI / 3;
                    var distance = Math.PI * 2 / 3;

                    SetCoordinate(device, ref vertexPosData[0].Position, angle);
                    SetCoordinate(device, ref vertexPosData[1].Position, angle - distance);
                    SetCoordinate(device, ref vertexPosData[2].Position, angle + distance);
                    buffer1.Update(vertexPosData);

                    constantBuffer.Value.Time = ((float)clock.Elapsed.TotalSeconds % 2) / 2;

                    if (++i == 60)
                    {
                        i = 0;
                        constantBuffer.Value.GlobalAlpha.X = (float)rand.NextDouble() * 0.5f + 0.5f;
                        constantBuffer.Value.GlobalAlpha.Y = (float)rand.NextDouble() * 0.5f + 0.5f;
                        constantBuffer.Value.GlobalAlpha.Z = (float)rand.NextDouble() * 0.5f + 0.5f;
                    }
                    constantBuffer.Update();

                    target.ClearAll();
                    indexBuffer.DrawAll(inputGroup, bufferGroup);

                    device.Present(true);
                });
            }
        }
    }
}
