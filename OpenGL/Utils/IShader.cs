using System;
using System.Collections.Generic;
using System.Text;

namespace GLSL
{
    public interface IShader
    {
        void LoadFromFile(string path);
        //void Compile();
        //void Bind();
    }
}