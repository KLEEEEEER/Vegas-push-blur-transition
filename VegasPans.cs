using ScriptPortal.Vegas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Push_Blur_Transition
{
    class VegasPans
    {
        public static void PanFromLeftToCenter(Vegas vegas, VideoEvent videoEvent)
        {
            using (UndoBlock undo = new UndoBlock("Add pan/crop"))
            {
                VideoMotionKeyframe key1 = new VideoMotionKeyframe(Timecode.FromMilliseconds(Config.splitOffset));
                videoEvent.VideoMotion.Keyframes.Add(key1);
                VideoMotionKeyframe key0 = videoEvent.VideoMotion.Keyframes[0];
                int videoWidth = vegas.Project.Video.Width;
                key0.MoveBy(new VideoMotionVertex(videoWidth, 0));
            }
        }

        public static void PanFromCenterToRight(Vegas vegas, VideoEvent videoEvent)
        {
            using (UndoBlock undo = new UndoBlock("Add pan/crop"))
            {
                VideoMotionKeyframe key0 = videoEvent.VideoMotion.Keyframes[0];
                int videoWidth = vegas.Project.Video.Width;
                VideoMotionKeyframe key1 = new VideoMotionKeyframe(Timecode.FromMilliseconds(Config.splitOffset));
                videoEvent.VideoMotion.Keyframes.Add(key1);
                key1.MoveBy(new VideoMotionVertex(-videoWidth, 0));
            }
        }

        public static void PanFromRightToCenter(Vegas vegas, VideoEvent videoEvent)
        {
            using (UndoBlock undo = new UndoBlock("Add pan/crop"))
            {
                VideoMotionKeyframe key1 = new VideoMotionKeyframe(Timecode.FromMilliseconds(Config.splitOffset));
                videoEvent.VideoMotion.Keyframes.Add(key1);
                VideoMotionKeyframe key0 = videoEvent.VideoMotion.Keyframes[0];
                int videoWidth = vegas.Project.Video.Width;
                key0.MoveBy(new VideoMotionVertex(-videoWidth, 0));
            }
        }

        public static void PanFromCenterToLeft(Vegas vegas, VideoEvent videoEvent)
        {
            using (UndoBlock undo = new UndoBlock("Add pan/crop"))
            {
                VideoMotionKeyframe key0 = videoEvent.VideoMotion.Keyframes[0];
                int videoWidth = vegas.Project.Video.Width;
                VideoMotionKeyframe key1 = new VideoMotionKeyframe(Timecode.FromMilliseconds(Config.splitOffset));
                videoEvent.VideoMotion.Keyframes.Add(key1);
                key1.MoveBy(new VideoMotionVertex(videoWidth, 0));
            }
        }

        public static string GetPanFunction(TransitionMode transitionMode, TransitionVideo transitionVideo)
        {
            switch (transitionMode)
            {
                case TransitionMode.ToRight:
                    if (transitionVideo == TransitionVideo.First) return "PanFromCenterToRight";
                    if (transitionVideo == TransitionVideo.Last) return "PanFromLeftToCenter";
                    break;
                case TransitionMode.ToLeft:
                    if (transitionVideo == TransitionVideo.First) return "PanFromCenterToLeft";
                    if (transitionVideo == TransitionVideo.Last) return "PanFromRightToCenter";
                    break;
                default:
                    return String.Empty;
            }
            return String.Empty;
        }

        public static PlugInNode GetPlugin(Vegas vegas, String plugInName)
        {
            PlugInNode plugIn = vegas.PlugIns.GetChildByName(plugInName);
            return (plugIn != null) ? plugIn : null;
        }
    }
}
