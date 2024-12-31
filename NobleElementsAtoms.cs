using Quintessential;
using AtomTypes = class_175;
using Texture = class_256;
using AtomGraphics = class_106;

namespace OMNobleElements
{
    public class NobleElementsAtoms
    {
        public static AtomType Nobilis, Alpha, Beta, Gamma;

        public static void AddAtomTypes()
        {
            Nobilis = CreateAtomType("nobilis", 220, prime: true);
            Alpha = CreateAtomType("alpha", 221);
            Beta = CreateAtomType("beta", 222);
            Gamma = CreateAtomType("gamma", 223);
            
            QApi.AddAtomType(Nobilis);
            QApi.AddAtomType(Alpha);
            QApi.AddAtomType(Beta);
            QApi.AddAtomType(Gamma);
        }

        private static AtomType CreateAtomType(string name, byte id, bool prime = false)
        {
            var upperName = name[0].ToString().ToUpper() + name.Substring(1);
            var graphics = new AtomGraphics()
            {
                field_994 = class_235.method_615($"noble_elements/textures/atoms/{name}_diffuse"),
                field_995 = class_235.method_615($"textures/atoms/salt_shade")
            };

            if (prime)
            {
                graphics = new AtomGraphics()
                {
                    field_994 = class_235.method_615($"textures/atoms/salt_diffuse"),
                    field_995 = class_235.method_615($"textures/atoms/salt_shade")
                };
            }
            
            return new AtomType()
            {
                field_2283 = id, /*ID*/
                field_2284 = class_134.method_254(upperName), /*Non-local Name*/
                field_2285 = class_134.method_253($"Elemental {upperName}", string.Empty), /*Atomic Name*/
                field_2286 = class_134.method_253(name, string.Empty), /*Local name*/
                field_2287 = class_235.method_615($"noble_elements/textures/atoms/{name}_symbol"), /*Symbol*/
                field_2288 = class_238.field_1989.field_81.field_599,                 /*Shadow*/
                field_2290 = graphics,
                field_2293 = false, /*Is Cardinal*/
                field_2294 = false, /*Is Metal(?)*/
                field_2295 = false, /*Is projector fuel*/
                field_2296 = false, /*Is quintessence*/
                QuintAtomType = $"NobleElements:{name}"
            };
        }
    }
}