using System.Collections.Generic;
using System.Reflection;
using Quintessential;

using PartType = class_139;
using PartTypes = class_191;
using Texture = class_256;

namespace OMNobleElements
{
    public class NobleElementsParts
    {
        // Part Types
        public static PartType Reactivity;
        // Puzzle Options
        public static PuzzleOption ReactivityOption;
        // Hex order for reactivity
        public static HexIndex[] reactivityHexes = new HexIndex[3]
        {
            new HexIndex(0, 0), // Alpha
            new HexIndex(0, -1), // Beta
            new HexIndex(1, -1) // Gamma
        };
        //define a convenient helper
        public static MethodInfo PrivateMethod<T>(string method) => typeof(T).GetMethod(method, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
        
        public static void AddGlyphs()
        {
            string path, iconpath, selectpath;
            path = "noble_elements/textures/";
            iconpath = path + "parts/icons/";
            selectpath = path + "select/";
            
            Reactivity = makeGlyph(
                    "noble-elements-reactivity",
                    "Glyph of Reactivity",
                    "the glyph of reactivity transmutes one nobilis and one noble element into the other two",
                    30,
                    reactivityHexes,
                    class_235.method_615(iconpath + "reactivity"),
                    class_235.method_615(iconpath + "reactivity_hover"),
                    class_238.field_1989.field_97.field_374, // double_glow
                    class_238.field_1989.field_97.field_375, // double_stroke
                    false
                );
            
            var reactivityPath = path + "parts/reactivity/";
            Texture reactivityBase = class_235.method_615( "textures/parts/prismabonder_base");
            Texture reactivityBowl = class_235.method_615( "textures/parts/calcinator_bowl");
            Texture alphaSymbol = class_235.method_615(reactivityPath + "alpha_symbol");
            Texture betaSymbol = class_235.method_615(reactivityPath + "beta_symbol");
            Texture gammaSymbol = class_235.method_615(reactivityPath + "gamma_symbol");
            Texture[] symbols = new[] { alphaSymbol, betaSymbol, gammaSymbol };
            
            QApi.AddPartType(Reactivity, (part, pos, editor, renderer) =>
            {
                PartSimState partSimState = editor.method_507().method_481(part);
                var simTime = editor.method_504();
                
                float partAngle = renderer.field_1798;
                Vector2 base_offset = new Vector2(82f, 120f);

                var originHex = new HexIndex(0, 0);
                
                drawPartGraphic(
                    renderer,
                    reactivityBase, 
                    base_offset, 
                    0.0f, 
                    Vector2.Zero, 
                    new Vector2(-1f, -1f)
                    );

                int i = 0;
                foreach (HexIndex idx in part.method_1159().field_1540)
                {
                    renderer.method_528(reactivityBowl, idx, Vector2.Zero);
                    renderer.method_528(symbols[i], idx, Vector2.Zero);
                    i++;
                }
            });
            QApi.AddPartTypeToPanel(Reactivity, false);
        }

        public static void RunGlyphs(Sim sim, bool isConsumptionHalfStep)
        {
            var SEB = sim.field_3818;
            var solution = SEB.method_502();
            var partList = solution.field_3919;
            var partSimStates = sim.field_3821;
            var struct122List = sim.field_3826;
            var moleculeList = sim.field_3823;
            var gripperList = sim.HeldGrippers;
            
            //define some helpers
            Maybe<AtomReference> maybeFindAtom(Part part, HexIndex hex, List<Part> list, bool checkWheels = false)
            {
                return (Maybe<AtomReference>)PrivateMethod<Sim>("method_1850").Invoke(sim, new object[] { part, hex, list, checkWheels });
            }

            void spawnAtomAtHex(Part part, HexIndex hex, AtomType atom)
            {
                Molecule molecule = new Molecule();
                molecule.method_1105(new Atom(atom), part.method_1184(hex));
                moleculeList.Add(molecule);
            }

            void consumeAtomReference(AtomReference atomRef)
            {
                // delete the input atom
                atomRef.field_2277.method_1107(atomRef.field_2278);
                // draw input getting consumed
                SEB.field_3937.Add(new class_286(SEB, atomRef.field_2278, atomRef.field_2280));
            }

            bool doesBondExist(Part part, Molecule molecule, enum_126 bond, HexIndex index1, HexIndex index2)
            {
                var index1True = part.method_1184(index1);
                var index2True = part.method_1184(index2);

                return molecule.method_1113(index1True, index2True) == bond;
            }

            foreach (var part in partList)
            {
                PartSimState partSimState = partSimStates[part];
                var partType = part.method_1159();

                if (partType == Reactivity)
                {
                    var types = new AtomType[] { NobleElementsAtoms.Alpha, NobleElementsAtoms.Beta, NobleElementsAtoms.Gamma };
                    var canTransmuate = true;
                    AtomReference nobilis = null;
                    AtomReference other = null;
                    AtomType nobilisTransumation = null;
                    AtomType otherTransumation = null;
                    int nobilisIndex = -1;
                    int otherIndex = -1;

                    // Count the number of occupied hexes
                    for (var i = 0; i < 3; i++)
                    {
                        var atom = default(AtomReference);
                        var hex = reactivityHexes[i];
                        maybeFindAtom(part, hex, partList).method_99(out atom);

                        if (atom == null)
                        {
                            continue;
                        }

                        if (atom.field_2280 == types[i])
                        {
                            canTransmuate = false;
                        }
                        else if (atom.field_2280 == NobleElementsAtoms.Nobilis)
                        {
                            // More than one atom of nobilis is on the glyph
                            if (nobilis != null)
                            {
                                canTransmuate = false;
                            }
                            
                            nobilis = atom;
                            nobilisTransumation = types[i];
                            nobilisIndex = i;
                        }
                        else
                        {
                            // More than one of alpha, beta, and/or gamma is on the glyph
                            if (other != null)
                            {
                                canTransmuate = false;
                            }
                            
                            other = atom;
                            otherTransumation = types[i];
                            otherIndex = i;
                        }
                    }
                    
                    if (nobilis != null && other != null && canTransmuate)
                    {
                        // Transmute Nobilis
                        consumeAtomReference(nobilis);
                        spawnAtomAtHex(part, reactivityHexes[nobilisIndex], nobilisTransumation);
                        
                        // Transmute Alpha, Beta, or Gamma
                        consumeAtomReference(other);
                        spawnAtomAtHex(part, reactivityHexes[otherIndex], otherTransumation);
                    }
                }
            }
        }
        
        // Theft
        private static PartType makeGlyph(
            string id,
            string name,
            string desc,
            int cost,
            HexIndex[] footprint,
            Texture icon,
            Texture hover,
            Texture glow,
            Texture stroke,
            bool onlyOne = false)
        {
            QApi.AddPuzzlePermission(id, name);
            var partType = new PartType()
            {
                /*ID*/
                field_1528 = id,
                /*Name*/
                field_1529 = class_134.method_253(name, string.Empty),
                /*Desc*/
                field_1530 = class_134.method_253(desc, string.Empty),
                /*Cost*/
                field_1531 = cost,
                /*Is a Glyph?*/
                field_1539 = true,
                /*Hex Footprint*/
                field_1540 = footprint,
                /*Icon*/
                field_1547 = icon,
                /*Hover Icon*/
                field_1548 = hover,
                /*Glow (Shadow)*/
                field_1549 = glow,
                /*Stroke (Outline)*/
                field_1550 = stroke,
                /*Only One Allowed?*/
                field_1552 = onlyOne,
                CustomPermissionCheck = perms => true
            };
            return partType;
        }
        
        #region drawingHelpers
        private static Vector2 hexGraphicalOffset(HexIndex hex) => NobleElementsMod.hexGraphicalOffset(hex);
        private static Vector2 textureDimensions(Texture tex) => tex.field_2056.ToVector2();
        private static Vector2 textureCenter(Texture tex) => (textureDimensions(tex) / 2).Rounded();
        private static void drawPartGraphic(class_195 renderer, Texture tex, Vector2 graphicPivot, float graphicAngle, Vector2 graphicTranslation, Vector2 screenTranslation)
        {
            drawPartGraphicScaled(renderer, tex, graphicPivot, graphicAngle, graphicTranslation, screenTranslation, new Vector2(1f, 1f));
        }

        private static void drawPartGraphicScaled(class_195 renderer, Texture tex, Vector2 graphicPivot, float graphicAngle, Vector2 graphicTranslation, Vector2 screenTranslation, Vector2 scaling)
        {
            //for graphicPivot and graphicTranslation, rightwards is the positive-x direction and upwards is the positive-y direction
            //graphicPivot is an absolute position, with (0,0) denoting the bottom-left corner of the texture
            //graphicTranslation is a translation, so (5,-3) means "translate 5 pixels right and 3 pixels down"
            //graphicAngle is measured in radians, and counterclockwise is the positive-angle direction
            //screenTranslation is the final translation applied, so it is not affected by rotations
            Matrix4 matrixScreenPosition = Matrix4.method_1070(renderer.field_1797.ToVector3(0f));
            Matrix4 matrixTranslateOnScreen = Matrix4.method_1070(screenTranslation.ToVector3(0f));
            Matrix4 matrixRotatePart = Matrix4.method_1073(renderer.field_1798);
            Matrix4 matrixTranslateGraphic = Matrix4.method_1070(graphicTranslation.ToVector3(0f));
            Matrix4 matrixRotateGraphic = Matrix4.method_1073(graphicAngle);
            Matrix4 matrixPivotOffset = Matrix4.method_1070(-graphicPivot.ToVector3(0f));
            Matrix4 matrixScaling = Matrix4.method_1074(scaling.ToVector3(0f));
            Matrix4 matrixTextureSize = Matrix4.method_1074(tex.field_2056.ToVector3(0f));

            Matrix4 matrix4 = matrixScreenPosition * matrixTranslateOnScreen * matrixRotatePart * matrixTranslateGraphic * matrixRotateGraphic * matrixPivotOffset * matrixScaling * matrixTextureSize;
            class_135.method_262(tex, Color.White, matrix4);
        }

        private static void drawPartGraphicSpecular(class_195 renderer, Texture tex, Vector2 graphicPivot, float graphicAngle, Vector2 graphicTranslation, Vector2 screenTranslation)
        {
            float specularAngle = (renderer.field_1799 - (renderer.field_1797 + graphicTranslation.Rotated(renderer.field_1798))).Angle() - 1.570796f - renderer.field_1798;
            drawPartGraphic(renderer, tex, graphicPivot, graphicAngle + specularAngle, graphicTranslation, screenTranslation);
        }

        private static void drawPartGloss(class_195 renderer, Texture gloss, Texture glossMask, Vector2 offset)
        {
            drawPartGloss(renderer, gloss, glossMask, offset, new HexIndex(0, 0), 0f);
        }
        private static void drawPartGloss(class_195 renderer, Texture gloss, Texture glossMask, Vector2 offset, HexIndex hexOffset, float angle)
        {
            class_135.method_257().field_1692 = class_238.field_1995.field_1757; // MaskedGlossPS shader
            class_135.method_257().field_1693[1] = gloss;
            var hex = new HexIndex(0, 0);
            Vector2 method2001 = 0.0001f * (renderer.field_1797 + hexGraphicalOffset(hex).Rotated(renderer.field_1798) - 0.5f * class_115.field_1433);
            class_135.method_257().field_1695 = method2001;
            drawPartGraphic(renderer, glossMask, offset, angle, hexGraphicalOffset(hexOffset), Vector2.Zero);
            class_135.method_257().field_1692 = class_135.method_257().field_1696; // previous shader
            class_135.method_257().field_1693[1] = class_238.field_1989.field_71;
            class_135.method_257().field_1695 = Vector2.Zero;
        }
        private static void drawAtomIO(class_195 renderer, AtomType atomType, HexIndex hex, float num)
        {
            Molecule molecule = Molecule.method_1121(atomType);
            Vector2 method1999 = renderer.field_1797 + hexGraphicalOffset(hex).Rotated(renderer.field_1798);
            Editor.method_925(molecule, method1999, new HexIndex(0, 0), 0f, 1f, num, 1f, false, null);
        }
        #endregion
    }
}