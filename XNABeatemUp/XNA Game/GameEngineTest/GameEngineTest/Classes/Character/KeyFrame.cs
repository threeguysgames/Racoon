using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngineTest
{
    public class KeyFrame
    {
        public int frameRef;
        public int duration;
        ScriptLine[] scripts;

        public KeyFrame()
        {
            frameRef = -1;
            duration = 0;

            scripts = new ScriptLine[4];
            for (int i = 0; i < scripts.Length; i++)
                scripts[i] = null;
        }

        public ScriptLine[] Scripts
        {
            get { return scripts; }
        }

        public void SetScript(int idx, String val)
        {
            scripts[idx] = new ScriptLine(val);
        }

        public ScriptLine GetScript(int idx)
        {
            return scripts[idx];
        }

        public ScriptLine[] GetScriptArray()
        {
            return scripts;
        }
    }
}
