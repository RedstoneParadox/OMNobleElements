using AtomTypes = class_175;
using Texture = class_256;
using CardinalAtomColors = class_5;
using DefaultAtomColors = class_106;

namespace OMNobleElements
{
    public class NobleElementsAtoms
    {
        public static AtomType Nobilis, Alpha, Beta, Gamma;

        public static void AddAtomTypes()
        {
            Nobilis = CreateAtomType("nobilis", 220);
            Alpha = CreateAtomType("alpha", 221);
            Beta = CreateAtomType("beta", 222);
            Gamma = CreateAtomType("gamma", 223);
        }

        private static AtomType CreateAtomType(string name, byte id)
        {
            var upperName = name[0].ToString().ToUpper() + name.Substring(1);
            
            return new AtomType()
            {
                field_2283 = 218, /*ID*/
                field_2293 = false, /*Is Cardinal*/
                field_2284 = class_134.method_254(upperName), /*Non-local Name*/
                field_2285 = class_134.method_253($"Elemental {upperName}", string.Empty), /*Atomic Name*/
                field_2286 = class_134.method_253(name, string.Empty), /*Local name*/
                field_2287 = class_235.method_615($"noble_elements/textures/atoms/{name}_symbol"), /*Symbol*/
                field_2288 = class_235.method_615($"noble_elements/textures/atoms/{name}_shadow"), /*Shadow*/
                field_2290 = new DefaultAtomColors()
                {
                    field_994 = class_235.method_615($"noble_elements/textures/atoms/{name}_diffuse"),
                    field_995 = class_235.method_615($"noble_elements/textures/atoms/{name}_shade")
                },
                field_2296 = true,
                QuintAtomType = $"NobleElements:{name}"
            };
        }
    }
}