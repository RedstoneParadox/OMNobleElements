using Quintessential;

namespace OMNobleElements
{
    public class NobleElementsMod : QuintessentialMod
    {
        public override void Load()
        {
            
        }

        public override void PostLoad()
        {

        }

        public override void Unload()
        {

        }

        public override void LoadPuzzleContent()
        {
            NobleElementsAtoms.AddAtomTypes();
        }
        
        public static Vector2 hexGraphicalOffset(HexIndex hex) => class_187.field_1742.method_492(hex);
    }
}