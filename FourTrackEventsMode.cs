using ScriptPortal.Vegas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Push_Blur_Transition
{
    class FourTrackEventsMode : ITrackEventsMode
    {
        private List<TrackEvent> selectedMedias;
        private Vegas myVegas;
        private TransitionMode transitionMode;

        private TrackEvent firstCut1 = null;
        private TrackEvent firstCut2 = null;

        private TrackEvent secondCut1 = null;
        private TrackEvent secondCut2 = null;

        private TrackEvent firstClip1 = null;
        private TrackEvent firstClip2 = null;
        private TrackEvent secondClip1 = null;
        private TrackEvent secondClip2 = null;



        public FourTrackEventsMode(List<TrackEvent> SelectedMedias, Vegas vegas, TransitionMode TransitionMode)
        {
            selectedMedias = SelectedMedias;
            myVegas = vegas;
            transitionMode = TransitionMode;

            firstClip1 = selectedMedias[0];
            firstClip2 = selectedMedias[1];
            secondClip1 = selectedMedias[2];
            secondClip2 = selectedMedias[3];

            MessageBox.Show($"firstClip1.Start = {firstClip1.Start} MediaType = {firstClip1.MediaType}");
            MessageBox.Show($"firstClip2.Start = {firstClip2.Start} MediaType = {firstClip2.MediaType}");
            MessageBox.Show($"secondClip1.Start = {secondClip1.Start} MediaType = {secondClip1.MediaType}");
            MessageBox.Show($"secondClip2.Start = {secondClip2.Start} MediaType = {secondClip2.MediaType}");

        }

        public void applyEffects()
        {
            /* *
                 * 1. Обрезать первый клип
                 * 1.1 Добавить эффекты и пан на первый обрезанный клип
                 * 2. Переместить по возможности второй клип на другую дорожку
                 * 3. Переместить второй клип на точку начала обрезанного фрагмента из первого клипа
                 * 4. Обрезать начало второго клипа
                 * 4.2 Добавить эффекты и пан на второй обрезанный клип
                 */
            CutFirstClip(); // 
            AddEffectsFirstClip(); //
            ChangeTrackSecondClip();  //
            MoveSecondClip(); //
            CutSecondClip(); //
            AddEffectsSecondClip(); //
        }

        public void AddEffectsFirstClip()
        {
            VideoEvent videoEvent = null;

            if (firstCut1 != null)
            {
                videoEvent = firstCut1 as VideoEvent;
            }
            else if (firstCut2 != null)
            {
                videoEvent = firstCut2 as VideoEvent;
            }

            if (videoEvent != null)
            {
                PlugInNode linearBlur = VegasPans.GetPlugin(myVegas, Config.pluginName);
                using (UndoBlock undo = new UndoBlock("Add plugin to first clip"))
                {
                    Effect effect = videoEvent.Effects.AddEffect(linearBlur);
                    effect.Preset = Config.pluginPreset;
                }

                var method = typeof(VegasPans).GetMethod(VegasPans.GetPanFunction(transitionMode, TransitionVideo.First));
                var result = method.Invoke(null, new object[] { myVegas, videoEvent });
            }
        }

        public void AddEffectsSecondClip()
        {
            if (secondClip1.MediaType == MediaType.Video)
            {
                VideoEvent videoEvent = secondClip1 as VideoEvent;
                PlugInNode linearBlur = VegasPans.GetPlugin(myVegas, Config.pluginName);
                using (UndoBlock undo = new UndoBlock("Add plugin to second video"))
                {
                    Effect effect = videoEvent.Effects.AddEffect(linearBlur);
                    effect.Preset = Config.pluginPreset;
                }

                //PanFromLeftToCenter(vegas, videoEvent);
                var method = typeof(VegasPans).GetMethod(VegasPans.GetPanFunction(transitionMode, TransitionVideo.Last));
                var result = method.Invoke(null, new object[] { myVegas, videoEvent });
            }
            if (secondClip2.MediaType == MediaType.Video)
            {
                VideoEvent videoEvent = secondClip2 as VideoEvent;
                PlugInNode linearBlur = VegasPans.GetPlugin(myVegas, Config.pluginName);
                using (UndoBlock undo = new UndoBlock("Add plugin to second video"))
                {
                    Effect effect = videoEvent.Effects.AddEffect(linearBlur);
                    effect.Preset = Config.pluginPreset;
                }

                //PanFromLeftToCenter(vegas, videoEvent);
                var method = typeof(VegasPans).GetMethod(VegasPans.GetPanFunction(transitionMode, TransitionVideo.Last));
                var result = method.Invoke(null, new object[] { myVegas, videoEvent });
            }
        }

        public void ChangeTrackSecondClip()
        {
             if (secondClip1.MediaType == MediaType.Audio)
             {
                 List<Track> audioTracks = VegasHelper.FindAudioTracks(myVegas);
                 foreach (Track track in audioTracks)
                 {
                     if (secondClip1.Track == track) continue;
                     if (track.MediaType == MediaType.Audio)
                         using (UndoBlock undo = new UndoBlock("Change track of audio"))
                         {
                            secondClip1.Track = track;
                         }
                     break;
                 }
             }
             else if (secondClip1.MediaType == MediaType.Video)
             {
                 List<Track> videoTracks = VegasHelper.FindVideoTracks(myVegas);
                 foreach (Track track in videoTracks)
                 {
                     if (secondClip1.Track == track) continue;
                     if (track.MediaType == MediaType.Video)
                         using (UndoBlock undo = new UndoBlock("Change track of video"))
                         {
                            secondClip1.Track = track;
                         }
                     break;
                 }
             }


             if (secondClip2.MediaType == MediaType.Audio)
             {
                 List<Track> audioTracks = VegasHelper.FindAudioTracks(myVegas);
                 foreach (Track track in audioTracks)
                 {
                     if (secondClip2.Track == track) continue;
                     if (track.MediaType == MediaType.Audio)
                         using (UndoBlock undo = new UndoBlock("Change track of audio"))
                         {
                            secondClip2.Track = track;
                         }
                     break;
                 }
             }
             else if (secondClip2.MediaType == MediaType.Video)
             {
                 List<Track> videoTracks = VegasHelper.FindVideoTracks(myVegas);
                 foreach (Track track in videoTracks)
                 {
                     if (secondClip2.Track == track) continue;
                     if (track.MediaType == MediaType.Video)
                         using (UndoBlock undo = new UndoBlock("Change track of video"))
                         {
                            secondClip2.Track = track;
                         }
                     break;
                 }
             }
        }

        public void CutFirstClip()
        {
            Timecode firstOffset = selectedMedias[0].Length - new Timecode(Config.splitOffset);
            using (UndoBlock undo = new UndoBlock("Cut first clip"))
            {
                firstCut1 = firstClip1.Split(firstOffset);
            }
            using (UndoBlock undo = new UndoBlock("Cut first clip"))
            {
                firstCut2 = firstClip2.Split(firstOffset);
            }
        }

        public void CutSecondClip()
        {
            Timecode secondOffset = new Timecode(Config.splitOffset);
            using (UndoBlock undo = new UndoBlock("Cut second clip"))
            {
                TrackEvent secondCut = secondClip1.Split(secondOffset);
            }
            using (UndoBlock undo = new UndoBlock("Cut second clip"))
            {
                TrackEvent secondCut = secondClip2.Split(secondOffset);
            }
        }

        public void MoveSecondClip()
        {
             if (firstCut1 != null)
             {
                 using (UndoBlock undo = new UndoBlock("Move second clip"))
                 {
                    secondClip1.Start = firstCut1.Start;
                 }
                 using (UndoBlock undo = new UndoBlock("Move second clip"))
                 {
                    secondClip2.Start = firstCut1.Start;
                 }
             }
             else
             {
                 MessageBox.Show($"FirstCut is empty!");
             }
        }
    }
}
