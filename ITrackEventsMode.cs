using ScriptPortal.Vegas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Push_Blur_Transition
{
    interface ITrackEventsMode
    {
        void applyEffects();
        void CutFirstClip();
        void AddEffectsFirstClip();
        void CutSecondClip();
        void AddEffectsSecondClip();
        void MoveSecondClip();
        void ChangeTrackSecondClip();

    }
}
