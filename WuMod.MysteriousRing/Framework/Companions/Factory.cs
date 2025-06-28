
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace MysteriousRing.Framework.Companions
{
    interface RingServantFactory
    {
        public NPC create(Farmer owner);
    }

    internal class ServantCreator
    {
        private static int count = 0;

        internal NPC create(Farmer owner)
        {
            RingServantFactory[] creators = new RingServantFactory[] {
                new RingServant1Factory(),
                new RingServant2Factory(),
                new RingServant3Factory(),
                new RingServant4Factory()
            };
            return creators[count % creators.Length].create(owner);
        }

        internal NPC createNext(Farmer owner)
        {
            count = count + 1;
            return create(owner);
        }
    }
}