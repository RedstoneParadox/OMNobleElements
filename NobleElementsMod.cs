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
    }
}