
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace MysteriousRing.Framework.Companions
{
    interface RingServantFactory
    {
        public RingServant create(Farmer owner);
    }

    internal class ServantCreator
    {
        private static int count = 0;

        internal RingServant create(Farmer owner)
        {
            RingServantFactory[] creators = new RingServantFactory[] {
                new RingServant1Factory(),
                new RingServant2Factory()
            };
            return creators[count % creators.Length].create(owner);
        }

        internal RingServant createNext(Farmer owner)
        {
            count = count + 1;
            return create(owner);
        }
    }
}