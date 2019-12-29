using System.Collections.Generic;

namespace MathCore.WPF.TeX
{
    /// <summary>Represents glueElement for holding together boxes</summary>
    internal class Glue
    {
        private static readonly List<Glue> glueTypes;
        private static readonly int[,,] glueRules;

        static Glue()
        {
            var parser = new GlueSettingsParser();
            glueTypes = parser.GetGlueTypes();
            glueRules = parser.GetGlueRules();
        }

        public double Space { get; }

        public double Stretch { get; }

        public double Shrink { get; }

        public string Name { get; private set; }

        public static Box CreateBox(TexAtomType leftAtomType, TexAtomType rightAtomType, TexEnvironment environment)
        {
            leftAtomType = leftAtomType > TexAtomType.Inner ? TexAtomType.Ordinary : leftAtomType;
            rightAtomType = rightAtomType > TexAtomType.Inner ? TexAtomType.Ordinary : rightAtomType;
            var glueType = glueRules[(int)leftAtomType, (int)rightAtomType, (int)environment.Style / 2];
            return glueTypes[glueType].CreateBox(environment);
        }

        public Glue(double space, double stretch, double shrink, string name)
        {
            Space = space;
            Stretch = stretch;
            Shrink = shrink;
            Name = name;
        }

        private Box CreateBox(TexEnvironment environment)
        {
            var texFont = environment.TexFont;
            var quad = texFont.GetQuad(texFont.GetMuFontId(), environment.Style);
            return new GlueBox((Space / 18.0f) * quad, (Stretch / 18.0f) * quad, (Shrink / 18.0f) * quad);
        }
    }
}
