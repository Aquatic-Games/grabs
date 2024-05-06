using System.Drawing;
using grabs.Graphics;
using grabs.Tests;
using grabs.Tests.Tests;

using TestBase test = new CubeTest();
test.Run(GraphicsApi.D3D11, new Size(1280, 720));